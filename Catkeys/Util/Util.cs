using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Util
{
	[DebuggerStepThrough]
	public static class NoClass
	{
	}

	/// <summary>
	/// Manages a named kernel handle (mutex, event, memory mapping, etc).
	/// Normally calls CloseHandle when dies or is called Close.
	/// But does not call CloseHandle for the variable that uses the name first time in current process.
	/// Therefore the kernel object survives, even when the first appdomain ends.
	/// It ensures that all variables in all appdomains will use the same kernel object (although different handle to it) if they use the same name.
	/// Most CreateX API work in "create or open" way. Pass such a created-or-opened object handle to the constructor.
	/// </summary>
	[DebuggerStepThrough]
	class LibInterDomainHandle
	{
		IntPtr _h;
		bool _noClose;

		public IntPtr Handle { get { return _h; } }

		/// <summary>
		/// Initializes new object.
		/// </summary>
		/// <param name="handle">Kernel object handle</param>
		/// <param name="name">Kernel object name. Note: this function adds local atom with that name.</param>
		public LibInterDomainHandle(IntPtr handle, string name)
		{
			_h = handle;

			if(_h != Zero && 0 == Api.FindAtom(name)) {
				Api.AddAtom(name);
				_noClose = true;
			}
		}

		~LibInterDomainHandle() { Close(); }

		public void Close()
		{
			if(_h != Zero && !_noClose) { Api.CloseHandle(_h); _h = Zero; }
		}
	}

	/// <summary>
	/// Allocates or opens memory that can be used by multiple processes.
	/// Wraps Api.CreateFileMapping(), Api.MapViewOfFile().
	/// Faster and more "unsafe" than System.IO.MemoryMappedFiles.MemoryMappedFile.
	/// </summary>
	[DebuggerStepThrough]
	public unsafe class SharedMemory
	{
		IntPtr _mem;
		LibInterDomainHandle _hmap;

		/// <summary>
		/// Pointer to the base of the shared memory.
		/// </summary>
		public IntPtr mem { get { return _mem; } }

		/// <summary>
		/// Creates shared memory of specified size. Opens if already exists.
		/// Calls Api.CreateFileMapping() and Api.MapViewOfFile().
		/// </summary>
		/// <param name="name"></param>
		/// <param name="size"></param>
		/// <exception cref="Win32Exception">When fails.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive until this process (not variable or appdomain) dies.
		/// All variables in all appdomains will get the same physical memory for the same name, but they will get different virtual address.
		/// </remarks>
		public SharedMemory(string name, uint size)
		{
			IntPtr hm = Api.CreateFileMapping((IntPtr)(~0), Zero, 4, 0, size, name);
			if(hm != Zero) {
				_mem = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
				if(_mem == Zero) Api.CloseHandle(hm); else _hmap = new LibInterDomainHandle(hm, name);
			}
			if(_mem == Zero) throw new Win32Exception();
			//todo: option to use SECURITY_ATTRIBUTES to allow low IL processes open the memory.
			//todo: use single handle/address for all appdomains.
			//OutList(_hmap, _mem);
		}

		//This works but not useful.
		///// <summary>
		///// Opens shared memory.
		///// Calls Api.OpenFileMapping() and Api.MapViewOfFile().
		///// </summary>
		///// <param name="name"></param>
		///// <exception cref="Win32Exception">When fails, eg the memory does not exist.</exception>
		//public SharedMemory(string name)
		//{
		//	_hmap = Api.OpenFileMapping(0x000F001F, false, name);
		//	if(_hmap != Zero) {
		//		_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
		//	}
		//	if(_mem == Zero) throw new Win32Exception();
		//}

		~SharedMemory() { if(_mem != Zero) Api.UnmapViewOfFile(_mem); }

		public void Close()
		{
			if(_mem != Zero) { Api.UnmapViewOfFile(_mem); _mem = Zero; }
			if(_hmap != null) { _hmap.Close(); _hmap = null; }
		}
	}

	/// <summary>
	/// Memory shared by all appdomains and by other related processes.
	/// </summary>
	[DebuggerStepThrough]
	unsafe struct LibSharedMemory
	{
		#region variables used by our library classes
		//Declare variables used by our library classes.
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		internal Perf.Inst perf;

		#endregion

		/// <summary>
		/// Shared memory size.
		/// </summary>
		internal const int Size = 0x10000;

		/// <summary>
		/// Creates or opens shared memory on demand in a thread-safe and process-safe way.
		/// </summary>
		static SharedMemory _sm = new SharedMemory("Catkeys_SM_0x10000", Size);

		/// <summary>
		/// Gets pointer to the shared memory.
		/// </summary>
		public static LibSharedMemory* Ptr { get { return (LibSharedMemory*)_sm.mem; } }
	}

	/// <summary>
	/// Memory shared by all appdomains of current process.
	/// Size 0x10000 (64 KB). Initially zero.
	/// </summary>
	/// <remarks>
	/// When need to prevent simultaneous access of the memory by multiple threads, use <c>lock("uniqueString"){...}</c>.
	/// It locks in all appdomains, because literal strings are interned, ie shared by all appdomains.
	/// Using some other object with 'lock' would lock only in that appdomain.
	/// However use this only in single module, because ngened modules have own interned strings.
	/// </remarks>
	[DebuggerStepThrough]
	unsafe struct LibProcessMemory
	{
		//Api.RTL_CRITICAL_SECTION _cs; //slower than SRW but not much. Initialization speed not tested.
		//Api.RTL_SRWLOCK _lock; //2 times slower than C# lock, but we need this because C# lock is appdomain-local

		#region variables used by our library classes
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		//public int test;
		internal LibWorkarounds.ProcessVariables workarounds;
		internal Files.Icons.ProcessVariables icons;
		//internal TaskSTA.TP_CALLBACK_ENVIRON_V3 threadPool;

		#endregion

		/// <summary>
		/// Gets pointer to the memory.
		/// </summary>
		public static LibProcessMemory* Ptr { get; }

		/// <summary>
		/// Memory size.
		/// </summary>
		public const int Size = 0x10000;

		[DebuggerStepThrough]
#if true
		static LibProcessMemory()
		{
			Ptr = (LibProcessMemory*)InterDomain.GetVariable("Catkeys_LibProcessMemory", () => Api.VirtualAlloc(Zero, Size));
		}
		//This is slower (especially if using InterDomain first time in domain) but not so bizarre as with window class. And less code.
#else
		static LibProcessMemory()
		{
			string name = "Catkeys_LibMem";

			var x = new Api.WNDCLASSEX(); x.cbSize = Api.SizeOf(x);
			if(0 == Api.GetClassInfoEx(Zero, name, ref x)) {
				x.lpfnWndProc = Api.VirtualAlloc(Zero, Size); //much faster when need to zero memory
				if(x.lpfnWndProc == Zero) throw new OutOfMemoryException(name);

				x.style = Api.CS_GLOBALCLASS;
				x.lpszClassName = Marshal.StringToHGlobalUni(name);
				bool ok = 0 != Api.RegisterClassEx(ref x);

				if(ok) {
					//Api.InitializeSRWLock(&((LibProcessMemory*)x.lpfnWndProc)->_lock);
					//Api.InitializeCriticalSection(&((LibProcessMemory*)x.lpfnWndProc)->_cs);
				} else {
					if(0 == Api.GetClassInfoEx(Zero, name, ref x)) throw new OutOfMemoryException(name);
				}

				Marshal.FreeHGlobal(x.lpszClassName);
			}
			Ptr = (LibProcessMemory*)x.lpfnWndProc;
		}
#endif
	}

	/// <summary>
	/// A message loop, alternative to Application.Run which does not support nested loops.
	/// </summary>
	public class MessageLoop
	{
		IntPtr _loopEndEvent;

		/// <summary>
		/// Runs a message loop.
		/// </summary>
		public void Loop()
		{
			using(new Util.LibEnsureWindowsFormsSynchronizationContext(true)) {
				_loopEndEvent = Api.CreateEvent(Zero, true, false, null);
				try {
					Application.DoEvents();
					//Perf.NW(); //TODO

					//TODO: GetQueueStatus

					for(;;) {
						uint k = Api.MsgWaitForMultipleObjects(1, ref _loopEndEvent, false, Api.INFINITE, Api.QS_ALLINPUT);
						Application.DoEvents();
						if(k == Api.WAIT_OBJECT_0 || k == Api.WAIT_FAILED) break; //note: this is after DoEvents because may be posted messages when stopping loop. Although it seems that MsgWaitForMultipleObjects returns events after all messages.

						Api.MSG u;
						if(Api.PeekMessage(out u, Wnd0, Api.WM_QUIT, Api.WM_QUIT, Api.PM_NOREMOVE)) break; //DoEvents() reposts it. If we don't break, MsgWaitForMultipleObjects retrieves it before (instead) the event, causing endless loop.
					}
				}
				finally {
					Api.CloseHandle(_loopEndEvent);
					_loopEndEvent = Zero;
				}
				//Out("leave loop");
			}
		}

		/// <summary>
		/// Ends the message loop, causing Loop() to return.
		/// </summary>
		public void Stop()
		{
			if(_loopEndEvent != Zero) {
				Api.SetEvent(_loopEndEvent);
			}
		}
	}

	/// <summary>
	/// Constructor ensures that current SynchronizationContext of this thread is WindowsFormsSynchronizationContext.
	/// Also sets WindowsFormsSynchronizationContext.AutoInstall=false to prevent Application.DoEvents etc setting wrong context.
	/// Dispose() restores both if need. Does not restore context if was null.
	/// Example: using(new Util.LibEnsureWindowsFormsSynchronizationContext()) { ... }
	/// </summary>
	[DebuggerStepThrough]
	class LibEnsureWindowsFormsSynchronizationContext :IDisposable
	{
		[ThreadStatic]
		static WindowsFormsSynchronizationContext _wfContext;
		SynchronizationContext _prevContext;
		bool _restoreContext, _prevAutoInstall;

		/// <summary>
		/// See class help.
		/// </summary>
		/// <param name="onlyIfAutoInstall">Do nothing if WindowsFormsSynchronizationContext.AutoInstall==false. Normally WindowsFormsSynchronizationContext.AutoInstall is true (default).</param>
		public LibEnsureWindowsFormsSynchronizationContext(bool onlyIfAutoInstall=false)
		{
			if(onlyIfAutoInstall && !WindowsFormsSynchronizationContext.AutoInstall) return;

			//Ensure WindowsFormsSynchronizationContext for this thread.
			_prevContext = SynchronizationContext.Current;
			if(!(_prevContext is WindowsFormsSynchronizationContext)) {
				if(_wfContext == null) _wfContext = new WindowsFormsSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(_wfContext);
				_restoreContext = _prevContext != null;
			}

			//Workaround for Application.DoEvents/Run bug:
			//	When returning, they set a SynchronizationContext of wrong type, even if previously was WindowsFormsSynchronizationContext.
			//	They don't do it if WindowsFormsSynchronizationContext.AutoInstall == false.
			//	Info: AutoInstall is [ThreadStatic], as well as SynchronizationContext.Current.
			_prevAutoInstall = WindowsFormsSynchronizationContext.AutoInstall;
			if(_prevAutoInstall) WindowsFormsSynchronizationContext.AutoInstall = false;
		}

		public void Dispose()
		{
			if(_restoreContext) {
				_restoreContext = false;
				if(SynchronizationContext.Current == _wfContext)
					SynchronizationContext.SetSynchronizationContext(_prevContext);
			}
			if(_prevAutoInstall) WindowsFormsSynchronizationContext.AutoInstall = true;
		}
	}

	/// <summary>
	/// Miscellaneous functions.
	/// </summary>
	[DebuggerStepThrough]
	public static class Misc
	{
		/// <summary>
		/// Gets the entry assembly of current appdomain.
		/// Normally instead can be used Assembly.GetEntryAssembly(), but it fails if appdomain launched through DoCallBack.
		/// </summary>
		public static Assembly AppdomainAssembly
		{
			get
			{
				if(_appdomainAssembly == null) {
					var asm = Assembly.GetEntryAssembly(); //fails if this domain launched through DoCallBack
					if(asm == null) asm = AppDomain.CurrentDomain.GetAssemblies()[1]; //[0] is mscorlib, 1 should be our assembly
					_appdomainAssembly = asm;
				}
				return _appdomainAssembly;
			}
		}
		static Assembly _appdomainAssembly;

		public static IntPtr GetModuleHandleOf(Type t)
		{
			return t == null ? Zero : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		public static IntPtr GetModuleHandleOf(Assembly asm)
		{
			return asm == null ? Zero : Marshal.GetHINSTANCE(asm.GetLoadedModules()[0]);
		}

		public static IntPtr GetModuleHandleOfAppdomainEntryAssembly()
		{
			return GetModuleHandleOf(AppdomainAssembly);
		}

		public static IntPtr GetModuleHandleOfCatkeysDll()
		{
			return Marshal.GetHINSTANCE(typeof(Misc).Module);
		}

		public static IntPtr GetModuleHandleOfExe()
		{
			return Api.GetModuleHandle(null);
		}

		public static bool IsCatkeysInGAC { get { return Assembly.GetExecutingAssembly().GlobalAssemblyCache; } }

		public static bool IsCatkeysInNgened { get { return Assembly.GetExecutingAssembly().CodeBase.Contains("/GAC_MSIL/"); } }
		//tested: Module.GetPEKind always gets ILOnly.

		/// <summary>
		/// Gets native icon handle of the entry assembly of current appdomain.
		/// It is the assembly icon, not an icon from managed resources.
		/// Returns Zero if the assembly is without icon.
		/// The icon is extracted first time and then cached; don't destroy it.
		/// </summary>
		/// <param name="size">Icon size, 16 or 32.</param>
		public static IntPtr GetAppIconHandle(int size)
		{
			if(size < 24) return _GetAppIconHandle(ref _AppIcon16, true);
			return _GetAppIconHandle(ref _AppIcon32, false);
		}

		static IntPtr _AppIcon32, _AppIcon16;

		static IntPtr _GetAppIconHandle(ref IntPtr hicon, bool small = false)
		{
			if(hicon == Zero) {
				var asm = Misc.AppdomainAssembly; if(asm == null) return Zero;
				IntPtr hinst = Misc.GetModuleHandleOf(asm);
				int size = small ? 16 : 32;
				hicon = Api.LoadImage(hinst, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED);
				//note:
				//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
				//I could not find a .NET method to get icon directly from native resources of assembly.
				//Could use Icon.ExtractAssociatedIcon(asm.Location), but it always gets 32 icon and is several times slower.
				//Also could use PrivateExtractIcons. But it uses file path, not module handle.
				//Also could use the resource emumeration API...
				//Never mind. Anyway, we use hInstance/resId with MessageBoxIndirect (which does not support handles) etc.
				//info: MSDN says that LR_SHARED gets cached icon regardless of size, but it is not true. Caches each size separately. Tested on Win 10, 7, XP.
			}
			return hicon;
		}

		public static void MinimizeMemory()
		{
			//return;
			GC.Collect();
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), (UIntPtr)(~0U), (UIntPtr)(~0U));
		}

		public static unsafe int CharPtrLength(char* p)
		{
			if(p == null) return 0;
			for(int i = 0; ; i++) if(*p == '\0') return i;
		}

		public static unsafe int CharPtrLength(char* p, int nMax)
		{
			if(p == null) return 0;
			for(int i = 0; i < nMax; i++) if(*p == '\0') return i;
			return nMax;
		}

		/// <summary>
		/// Removes '&amp;' characters from string.
		/// Replaces "&amp;&amp;" to "&amp;".
		/// Returns true if s had '&amp;' characters.
		/// </summary>
		/// <remarks>
		/// Character '&amp;' is used to underline next character in displayed text of controls. Two '&amp;' are used to display single '&amp;'.
		/// Normally the underline is displayed only when using the keyboard to select dialog controls.
		/// </remarks>
		public static bool StringRemoveMnemonicUnderlineAmpersand(ref string s)
		{
			if(!Empty(s)) {
				int i = s.IndexOf('&');
				if(i >= 0) {
					i = s.IndexOf_("&&");
					if(i >= 0) s = s.Replace("&&", "\0");
					s = s.Replace("&", "");
					if(i >= 0) s = s.Replace("\0", "&");
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets default app domain.
		/// </summary>
		/// <param name="isCurrentDomain">Receives true if called from default app domain.</param>
		public static AppDomain GetDefaultAppDomain(out bool isCurrentDomain)
		{
			if(_defaultAppDomain == null) {
				var d = AppDomain.CurrentDomain;
				if(d.IsDefaultAppDomain()) {
					_defaultAppDomain = d;
					_defaultAppDomainIsCurrent = true;
				} else {
					d = AppDomain.CurrentDomain.GetData("Catkeys_DefaultDomain") as AppDomain;
					if(d != null) {
						_defaultAppDomain = d;
					} else { //current domain created not by Catkeys
						ICorRuntimeHost host = new CorRuntimeHost() as ICorRuntimeHost;
						//Perf.Next(); //speed:  289  3251  (3542) ngened, else 4ms. Why GetDefaultDomain so slow?
						object o = null; host.GetDefaultDomain(out o);
						_defaultAppDomain = o as AppDomain;

						//this is slower
						//IntPtr hEnum;
						//if(0 != host.EnumDomains(out hEnum)) return null;
						//if(0 != host.NextDomain(hEnum, out defaultAppDomain)) return null;
						//host.CloseEnum(hEnum);
						//_defaultAppDomain = defaultAppDomain as AppDomain;
					}
				}
			}
			isCurrentDomain = _defaultAppDomainIsCurrent;
			return _defaultAppDomain;
		}
		static AppDomain _defaultAppDomain; static bool _defaultAppDomainIsCurrent;

		/// <summary>
		/// Gets default app domain.
		/// </summary>
		public static AppDomain GetDefaultAppDomain()
		{
			bool isThisDef;
			return GetDefaultAppDomain(out isThisDef);
		}

		[ComImport, Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		unsafe interface ICorRuntimeHost
		{
			[PreserveSig]
			int CreateLogicalThreadState();
			[PreserveSig]
			int DeleteLogicalThreadState();
			[PreserveSig]
			int SwitchInLogicalThreadState(ref uint pFiberCookie);
			[PreserveSig]
			int SwitchOutLogicalThreadState(out uint* pFiberCookie);
			[PreserveSig]
			int LocksHeldByLogicalThread(out uint pCount);
			[PreserveSig]
			int MapFile(IntPtr hFile, out IntPtr hMapAddress);
			[PreserveSig]
			//int GetConfiguration(out ICorConfiguration pConfiguration);
			int GetConfiguration(IntPtr pConfiguration);
			[PreserveSig]
			int Start();
			[PreserveSig]
			int Stop();
			[PreserveSig]
			int CreateDomain([MarshalAs(UnmanagedType.LPWStr)] string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] Object pIdentityArray, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig]
			int GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig]
			int EnumDomains(out IntPtr hEnum);
			[PreserveSig]
			int NextDomain(IntPtr hEnum, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig]
			int CloseEnum(IntPtr hEnum);
			[PreserveSig]
			int CreateDomainEx([MarshalAs(UnmanagedType.LPWStr)] string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] Object pSetup, [MarshalAs(UnmanagedType.IUnknown)] Object pEvidence, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig]
			int CreateDomainSetup([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomainSetup);
			[PreserveSig]
			int CreateEvidence([MarshalAs(UnmanagedType.IUnknown)] out Object pEvidence);
			[PreserveSig]
			int UnloadDomain([MarshalAs(UnmanagedType.IUnknown)] Object pAppDomain);
			[PreserveSig]
			int CurrentDomain([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
		}

		[ComImport, Guid("CB2F6723-AB3A-11d2-9C40-00C04FA30A3E"), ClassInterface(ClassInterfaceType.None)]
		class CorRuntimeHost { }
	}


	[DebuggerStepThrough]
	public static class Debug_
	{
		[Conditional("DEBUG")]
		public static void OutMsg(ref Message m, params uint[] ignore)
		{
			uint msg = (uint)m.Msg;
			foreach(uint t in ignore) { if(t == msg) return; }

			Wnd w = (Wnd)m.HWnd;
			uint counter = w.GetProp("OutMsg"); w.SetProp("OutMsg", ++counter);
			OutList(counter, m);
		}

		[Conditional("DEBUG")]
		public static void OutMsg(Wnd w, uint msg, LPARAM wParam, LPARAM lParam, params uint[] ignore)
		{
			foreach(uint t in ignore) { if(t == msg) return; }
			var m = Message.Create(w.Handle, (int)msg, wParam, lParam);
			OutMsg(ref m);
		}

		[Conditional("DEBUG")]
		public static void OutLoadedAssemblies()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Assembly[] assems = currentDomain.GetAssemblies();
			foreach(Assembly assem in assems) {
				OutList(assem.ToString(), assem.CodeBase, assem.Location);
			}
		}
	}

}
