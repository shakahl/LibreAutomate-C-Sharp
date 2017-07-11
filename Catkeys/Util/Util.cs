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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Reflection.Emit;
using Microsoft.Win32.SafeHandles;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys.Util
{
	/// <summary>
	/// Miscellaneous classes and functions used in this library. Can be used outside it too.
	/// </summary>
	internal class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	/// <summary>
	/// Creates and manages native bitmap handle and memory DC (GDI device context).
	/// The bitmap is selected in the DC.
	/// </summary>
	public class MemoryBitmap :IDisposable
	{
		IntPtr _dc, _bm, _oldbm;

		/// <summary>
		/// DC handle.
		/// </summary>
		public IntPtr Hdc { get => _dc; }

		/// <summary>
		/// Bitmap handle.
		/// </summary>
		public IntPtr Hbitmap { get => _bm; }

		///
		public MemoryBitmap() { }

		/// <summary>
		/// Calls <see cref="Create(int, int)"/>.
		/// </summary>
		/// <exception cref="CatException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		public MemoryBitmap(int width, int height)
		{
			if(!Create(width, height)) throw new CatException("*create memory bitmap of specified size");
		}

		/// <summary>
		/// Deletes the bitmap and DC and calls GC.SuppressFinalize.
		/// </summary>
		public void Dispose()
		{
			Delete();
			GC.SuppressFinalize(this);
		}

		///
		~MemoryBitmap() { Delete(); }

		/// <summary>
		/// Deletes the bitmap and DC.
		/// Unlike Dispose, does not call GC.SuppressFinalize.
		/// </summary>
		public void Delete()
		{
			if(_dc == Zero) return;
			if(_bm != Zero) {
				Api.SelectObject(_dc, _oldbm);
				Api.DeleteObject(_bm);
				_bm = Zero;
			}
			Api.DeleteDC(_dc);
			_dc = Zero;
		}

		/// <summary>
		/// Creates new memory DC and bitmap of specified size and selects it into the DC.
		/// Returns false if failed.
		/// In any case deletes previous bitmap and DC.
		/// </summary>
		/// <param name="width">Width, pixels.</param>
		/// <param name="height">Height, pixels.</param>
		public bool Create(int width, int height)
		{
			IntPtr dcs = Api.GetDC(Wnd0);
			Attach(Api.CreateCompatibleBitmap(dcs, width, height));
			Api.ReleaseDC(Wnd0, dcs);
			return _bm != Zero;
		}

		/// <summary>
		/// Sets this variable to manage an existing bitmap.
		/// Selects the bitmap into a memory DC.
		/// Deletes previous bitmap and DC.
		/// </summary>
		/// <param name="hBitmap">Native bitmap handle.</param>
		public void Attach(IntPtr hBitmap)
		{
			Delete();
			if(hBitmap != Zero) {
				_dc = Api.CreateCompatibleDC(Zero);
				_oldbm = Api.SelectObject(_dc, _bm = hBitmap);
			}
		}

		/// <summary>
		/// Deletes memory DC, clears this variable and returns its bitmap (native bitmap handle).
		/// The returned bitmap is not selected into a DC. Will need to delete it with API DeleteObject.
		/// </summary>
		public IntPtr Detach()
		{
			IntPtr bret = _bm;
			if(_bm != Zero) {
				Api.SelectObject(_dc, _oldbm);
				Api.DeleteDC(_dc);
				_dc = Zero; _bm = Zero;
			}
			return bret;
		}
	}

	/// <summary>
	/// Creates and manages native font handle.
	/// </summary>
	internal class LibNativeFont :IDisposable
	{
		public IntPtr Handle { get; private set; }
		public int HeightOnScreen { get; private set; }

		public LibNativeFont(IntPtr handle) { Handle = handle; }

		public static implicit operator IntPtr(LibNativeFont f) { return (f == null) ? Zero : f.Handle; }

		~LibNativeFont() { Dispose(); }
		public void Dispose()
		{
			if(Handle != Zero) { Api.DeleteObject(Handle); Handle = Zero; }
		}

		public LibNativeFont(string name, int height, bool calculateHeightOnScreen = false)
		{
			var dcScreen = Api.GetDC(Wnd0);
			int h2 = -Calc.MulDiv(height, Api.GetDeviceCaps(dcScreen, 90), 72);
			Handle = Api.CreateFont(h2, iCharSet: 1, pszFaceName: name); //LOGPIXELSY=90
			if(calculateHeightOnScreen) {
				var dcMem = Api.CreateCompatibleDC(dcScreen);
				var of = Api.SelectObject(dcMem, Handle);
				Api.GetTextExtentPoint32(dcMem, "A", 1, out var z);
				HeightOnScreen = z.cy;
				Api.SelectObject(dcMem, of);
				Api.DeleteDC(dcMem);
			}
			Api.ReleaseDC(Wnd0, dcScreen);
		}
	}

	/// <summary>
	/// Gets native module handle.
	/// </summary>
	public static class ModuleHandle
	{
		/// <summary>
		/// Gets native module handle of type's assembly.
		/// </summary>
		public static IntPtr OfType(Type t)
		{
			return t == null ? Zero : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		/// <summary>
		/// Gets native module handle of an assembly.
		/// If the assembly consists of multiple modules, gets its first loaded module.
		/// </summary>
		public static IntPtr OfAssembly(Assembly asm)
		{
			return asm == null ? Zero : Marshal.GetHINSTANCE(asm.GetLoadedModules()[0]);
		}

		/// <summary>
		/// Gets native module handle of current app domain entry assembly.
		/// If the assembly consists of multiple modules, gets its first loaded module.
		/// </summary>
		public static IntPtr OfAppDomainEntryAssembly()
		{
			return OfAssembly(AppDomain_.EntryAssembly);
		}

		/// <summary>
		/// Gets native module handle of Catkeys.dll.
		/// </summary>
		public static IntPtr OfCatkeysDll()
		{
			return Marshal.GetHINSTANCE(typeof(Misc).Module);
		}

		/// <summary>
		/// Gets native module handle of the program file of this process.
		/// </summary>
		public static IntPtr OfProcessExe()
		{
			return Api.GetModuleHandle(null);
		}
	}

	/// <summary>
	/// Miscellaneous functions.
	/// </summary>
	[DebuggerStepThrough]
	public static class Misc
	{
		/// <summary>
		/// Returns true if Catkeys.dll is installed in the global assembly cache.
		/// </summary>
		public static bool IsCatkeysInGAC { get => typeof(Misc).Assembly.GlobalAssemblyCache; }

		/// <summary>
		/// Returns true if Catkeys.dll is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process or app domain.
		/// </summary>
		public static bool IsCatkeysNgened { get => IsAssemblyNgened(typeof(Misc).Assembly); }
		//tested: Module.GetPEKind always gets ILOnly.

		/// <summary>
		/// Returns true if assembly asm is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process or app domain.
		/// </summary>
		public static bool IsAssemblyNgened(Assembly asm)
		{
			var s = asm.CodeBase;
			if(asm.GlobalAssemblyCache) return s.Contains("/GAC_MSIL/"); //faster and maybe more reliable, but works only with GAC assemblies
			s = Path.GetFileName(s);
			s = s.Insert(s.LastIndexOf('.') + 1, "ni.");
			return Zero != Api.GetModuleHandle(s);
		}
		/// <summary>
		/// Returns true if assembly of type is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process or app domain.
		/// </summary>
		public static bool IsAssemblyNgened(Type type) { return IsAssemblyNgened(type.Assembly); }

		/// <summary>
		/// Frees as much as possible physical memory used by this process.
		/// Calls <see cref="GC.Collect()"/> and API <msdn>SetProcessWorkingSetSize</msdn>.
		/// </summary>
		public static void MinimizeMemory()
		{
			//return;
			GC.Collect();
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), (UIntPtr)(~0U), (UIntPtr)(~0U));
		}

		/// <summary>
		/// Do not call. Use class TypeSize, which caches the type size.
		/// This is used by TypeSize, not in it, because it is a generic type...
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int LibGetTypeSize(Type t)
		{
			var dm = new DynamicMethod("SizeOfType", typeof(int), Type.EmptyTypes);
			ILGenerator il = dm.GetILGenerator();
			il.Emit(OpCodes.Sizeof, t);
			il.Emit(OpCodes.Ret);
			return (int)dm.Invoke(null, null);
			//Print(dm.MethodImplementationFlags);
		}
	}

	/// <summary>
	/// Gets managed run-time size of type T. Works with any type.
	/// Unlike sizeof, can be used in generic classes too.
	/// Unlike Marshal.SizeOf, gets managed type size (eg 1 for bool), not native type size (eg 4 for bool).
	/// Example: <c>Print(Catkeys.Util.TypeSize&lt;T&gt;.Size);</c>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class TypeSize<T>
	{
		/// <summary>
		/// Gets T type size.
		/// </summary>
		public readonly static int Size;
		static TypeSize() { Size = Misc.LibGetTypeSize(typeof(T)); }

		//speed: quite fast, especially when ngened. When using this generic class, LibGetTypeSize is called once for each type.
	}

#if DEBUG
	/// <summary>
	/// Functions useful when testing and debugging.
	/// </summary>
	[DebuggerStepThrough]
	internal static class LibDebug
	{

		internal static void PrintLoadedAssemblies()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Assembly[] assems = currentDomain.GetAssemblies();
			foreach(Assembly assem in assems) {
				PrintList(assem.ToString(), assem.CodeBase, assem.Location);
			}
		}

		internal static int GetComObjRefCount(IntPtr obj)
		{
			Marshal.AddRef(obj);
			return Marshal.Release(obj);
		}

		internal static bool IsScrollLock { get => (Api.GetKeyState(Api.VK_SCROLL) & 1) != 0; }
	}
#endif

	/// <summary>
	/// Functions for high-DPI screen support.
	/// High DPI means when in Control Panel is set screen text size other than 100%.
	/// </summary>
	public static class Dpi
	{
		/// <summary>
		/// Gets DPI of the primary screen.
		/// On newer Windows versions, users can change DPI without logoff-logon. This function gets the setting that was after logon.
		/// </summary>
		public static int BaseDPI
		{
			get
			{
				if(_baseDPI == 0) {
					var dc = Api.GetDC(Wnd0);
					_baseDPI = Api.GetDeviceCaps(dc, 90); //LOGPIXELSY
					Api.ReleaseDC(Wnd0, dc);
				}
				return _baseDPI;
			}
		}
		static int _baseDPI;

		/// <summary>
		/// Gets small icon size that depends on DPI of the primary screen.
		/// Width and Height are <see cref="BaseDPI"/>/6, which is 16 if DPI is 96 (100%).
		/// </summary>
		public static Size SmallIconSize { get { var t = BaseDPI / 6; return new Size(t, t); } }

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns stretched i.
		/// Else returns i.
		/// </summary>
		/// <param name="i"></param>
		public static int ScaleInt(int i)
		{
			long dpi = BaseDPI;
			if(dpi > 96) i = (int)(i * dpi / 96);
			return i;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns scaled (stretched) z.
		/// Else returns z.
		/// Note: for images use <see cref="ImageSize"/>.
		/// </summary>
		/// <param name="z"></param>
		public static Size ScaleSize(Size z)
		{
			int dpi = BaseDPI;
			if(dpi > 96) {
				z.Width = (int)((long)z.Width * dpi / 96);
				z.Height = (int)((long)z.Height * dpi / 96);
			}
			return z;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96 and image resolution is different, returns scaled (stretched) image.Size.
		/// Else returns image.Size.
		/// </summary>
		/// <param name="image"></param>
		public static Size ImageSize(Image image)
		{
			if(image == null) return Size.Empty;
			var r = image.Size;
			int dpi = BaseDPI;
			if(dpi > 96) {
				r.Width = (int)((long)r.Width * dpi / (int)Math.Round(image.HorizontalResolution));
				r.Height = (int)((long)r.Height * dpi / (int)Math.Round(image.VerticalResolution));
			}
			return r;
		}
	}

	/// <summary>
	/// Wraps a waitable timer handle. Allows to create, open, set and wait.
	/// More info: API <msdn>CreateWaitableTimer</msdn>.
	/// Note: will need to dispose.
	/// </summary>
	public class WaitableTimer :WaitHandle
	{
		[DllImport("kernel32.dll", EntryPoint = "CreateWaitableTimerW", SetLastError = true)]
		static extern SafeWaitHandle CreateWaitableTimer(Api.SECURITY_ATTRIBUTES lpTimerAttributes, bool bManualReset, string lpTimerName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetWaitableTimer(SafeWaitHandle hTimer, ref long lpDueTime, int lPeriod = 0, IntPtr pfnCompletionRoutine = default(IntPtr), IntPtr lpArgToCompletionRoutine = default(IntPtr), bool fResume = false);

		[DllImport("kernel32.dll", EntryPoint = "OpenWaitableTimerW", SetLastError = true)]
		static extern SafeWaitHandle OpenWaitableTimer(uint dwDesiredAccess, bool bInheritHandle, string lpTimerName);

		WaitableTimer() { }

		/// <summary>
		/// Calls API <msdn>CreateWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="manualReset"></param>
		/// <param name="timerName">Timer name. If a timer with this name already exists, opens it if possible. If null, creates unnamed timer.</param>
		/// <exception cref="CatException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		public static WaitableTimer Create(bool manualReset = false, string timerName = null)
		{
			var h = CreateWaitableTimer(Api.SECURITY_ATTRIBUTES.Common, manualReset, timerName);
			if(h.IsInvalid) {
				var ex = new CatException(0, "*create timer");
				h.SetHandleAsInvalid();
				throw ex;
			}
			return new WaitableTimer() { SafeWaitHandle = h };
		}

		/// <summary>
		/// Calls API <msdn>OpenWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="timerName">Timer name. Fails if it does not exist; to open-or-create use <see cref="Create"/>.</param>
		/// <param name="access">.See <msdn>Synchronization Object Security and Access Rights</msdn>. The default value TIMER_MODIFY_STATE|SYNCHRONIZE allows to set and wait.</param>
		/// <exception cref="CatException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		/// <param name="inheritHandle"></param>
		/// <param name="noException">If fails, return null, don't throw exception. Supports <see cref="Native.GetError"/>.</param>
		/// <exception cref="CatException">Failed. For example, the timer does not exist.</exception>
		public static WaitableTimer Open(string timerName, uint access = Api.TIMER_MODIFY_STATE | Api.SYNCHRONIZE, bool inheritHandle = false, bool noException = false)
		{
			var h = OpenWaitableTimer(access, inheritHandle, timerName);
			if(h.IsInvalid) {
				var e = Native.GetError();
				h.SetHandleAsInvalid();
				if(noException) {
					Native.SetError(e);
					return null;
				}
				throw new CatException(e, "*open timer");
			}
			return new WaitableTimer() { SafeWaitHandle = h };
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="dueTime">
		/// The time after which the state of the timer is to be set to signaled. It is relative time (from now).
		/// If positive, in milliseconds. If negative, in 100 nanosecond intervals (microseconds*10), see <msdn>FILETIME</msdn>.
		/// Also can be 0, to set minimal time.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		/// <exception cref="OverflowException">dueTime*10000 is greater than long.MaxValue.</exception>
		public bool Set(long dueTime, int period = 0)
		{
			if(dueTime > 0) dueTime = -checked(dueTime * 10000);
			return SetWaitableTimer(this.SafeWaitHandle, ref dueTime, period, Zero, Zero, false);
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="dueTime">The UTC date/time at which the state of the timer is to be set to signaled.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		public bool SetAbsolute(DateTime dueTime, int period = 0)
		{
			var t = dueTime.ToFileTimeUtc();
			return SetWaitableTimer(this.SafeWaitHandle, ref t, period, Zero, Zero, false);
		}
	}
}
