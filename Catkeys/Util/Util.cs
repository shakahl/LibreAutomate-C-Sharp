using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Util
{
	[DebuggerStepThrough]
	public static class NoClass
	{
		public static void ResetLastError() { Api.SetLastError(0); }
    }

	[DebuggerStepThrough]
	public static class Window
	{
		/// <summary>
		/// Registers native window class that can be used for simple hidden windows.
		/// Returns class atom.
		/// Sets class name and procedure address. All other properties 0 (including hInstance).
		/// </summary>
		public static ushort RegWinClassHidden(string name, Api.WNDPROC wndProc)
		{
			var w = new Api.WNDCLASSEX(name, wndProc);
			return Api.RegisterClassEx(ref w);
		}
	}

	//TODO: test SafeMemoryMappedFileHandle, SafeMemoryMappedViewHandle.
	[DebuggerStepThrough]
	public class SharedMemoryFast
	{
		protected IntPtr _hmap, _mem;

		/// <summary>
		/// Pointer to the base of the shared memory.
		/// </summary>
		public IntPtr mem { get { return _mem; } }

		~SharedMemoryFast() { Close(); }

		public bool Create(string name, uint size)
		{
			Close();
			_hmap = Api.CreateFileMapping((IntPtr)(~0), Zero, 4, 0, size, name);
			if(_hmap==Zero) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
			//if(_mem && nbZero) Zero(_mem, nbZero); //don't zero all, because it maps memory for all possibly unused pages. Tested: although MSDN says that the memory is zero, it is not true.
			return _mem!=Zero;
		}

		public bool Open(string name)
		{
			Close();
			_hmap = Api.OpenFileMapping(0x000F001F, false, name);
			if(_hmap==Zero) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
			return _mem!=Zero;
		}

		public void Close()
		{
			if(_mem!=Zero) { Api.UnmapViewOfFile(_mem); _mem=Zero; }
			if(_hmap!=Zero) { Api.CloseHandle(_hmap); _hmap=Zero; }
		}
	}

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
				if(_appdomainAssembly==null) {
					var asm = Assembly.GetEntryAssembly(); //fails if this domain launched through DoCallBack
					if(asm==null) asm=AppDomain.CurrentDomain.GetAssemblies()[1]; //[0] is mscorlib, 1 should be our assembly
					_appdomainAssembly=asm;
				}
				return _appdomainAssembly;
			}
		}
		static Assembly _appdomainAssembly;

		public static IntPtr GetModuleHandleOf(Type t)
		{
			return t==null ? Zero : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		public static IntPtr GetModuleHandleOf(Assembly asm)
		{
			return asm==null ? Zero : Marshal.GetHINSTANCE(asm.GetLoadedModules()[0]);
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
				hicon = Api.LoadImageRes(hinst, 32512, Api.IMAGE_ICON, size, size, Api.LR_SHARED);
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
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), (UIntPtr)(~0U), (UIntPtr)(~0U));
		}

		public static unsafe int CharPtrLength(char* p)
		{
			if(p==null) return 0;
			for(int i = 0; ; i++) if(*p=='\0') return i;
		}

		public static unsafe int CharPtrLength(char* p, int nMax)
		{
			if(p==null) return 0;
			for(int i = 0; i<nMax; i++) if(*p=='\0') return i;
			return nMax;
		}

		/// <summary>
		/// Removes '&' characters from string.
		/// Replaces "&&" to "&".
		/// Returns true if s had '&' characters.
		/// </summary>
		/// <remarks>
		/// Character '&' is used to underline next character in displayed text of controls. Two '&' are used to display single '&'.
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
	}





	//TEST


	//public static class Path //cannot be Path, because hides System.IO.Path
	//{
	//	public static string Expand(string path) { return "rrrr"; }
	//}

}
