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
		/// Finds unmanaged '\0'-terminated string length.
		/// Scans the string until '\0' character found.
		/// </summary>
		public static unsafe int CharPtrLength(char* p)
		{
			if(p == null) return 0;
			for(int i = 0; ; i++) if(p[i] == '\0') return i;
		}

		/// <summary>
		/// Finds unmanaged '\0'-terminated string length.
		/// Scans the string until '\0' character found, but not exceeding the specified length.
		/// </summary>
		/// <param name="p">Unmanaged string.</param>
		/// <param name="nMax">Max allowed string length. The function returns nMax if does not find '\0' character within first nMax characters.</param>
		public static unsafe int CharPtrLength(char* p, int nMax)
		{
			if(p == null) return 0;
			for(int i = 0; i < nMax; i++) if(p[i] == '\0') return i;
			return nMax;
		}
	}

#if DEBUG
	/// <summary>
	/// Functions useful when testing and debugging.
	/// </summary>
	[DebuggerStepThrough]
	public static class LibDebug_
	{
#pragma warning disable 1591 //XML doc

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
#pragma warning restore 1591 //XML doc
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
	}
}
