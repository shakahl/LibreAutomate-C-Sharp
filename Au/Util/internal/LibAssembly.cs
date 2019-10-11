using System;
using System.Collections.Generic;
using System.Text;
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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// Assembly functions.
	/// </summary>
	static class LibAssembly
	{
		//not used. Don't add Au to GAC, because then process may start slowly, don't know why.
		///// <summary>
		///// Returns true if Au.dll is installed in the global assembly cache.
		///// </summary>
		//internal static bool LibIsAuInGAC => typeof(LibAssembly).Assembly.GlobalAssemblyCache;

		/// <summary>
		/// Returns true if Au.dll is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process.
		/// </summary>
		internal static bool LibIsAuNgened => s_auNgened ??= IsNgened(typeof(LibAssembly).Assembly);
		static bool? s_auNgened;
		//tested: Module.GetPEKind always gets ILOnly.

		/// <summary>
		/// Returns true if assembly asm is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		public static bool IsNgened(Assembly asm)
		{
			var s = asm.CodeBase;
			//if(asm.GlobalAssemblyCache) return s.Contains("/GAC_MSIL/"); //faster and maybe more reliable, but works only with GAC assemblies
			s = s.Substring(s.LastIndexOf('/') + 1);
			s = s.Insert(s.LastIndexOf('.') + 1, "ni.");
			return default != Api.GetModuleHandle(s);
		}

		//much slower first time when ngened. Also it is undocumented that GetModuleFileName returns 0 if non-ngened (LOAD_LIBRARY_AS_DATAFILE?).
		//[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		//public static unsafe bool IsAssemblyNgened2(Assembly asm)
		//{
		//	var module =asm.GetLoadedModules()[0];
		//	var h = Marshal.GetHINSTANCE(module); //slow first time, especially when ngened
		//	var b = stackalloc char[4];
		//	return 0 != Api.GetModuleFileName(h, b, 4);
		//}

		/// <summary>
		/// Returns flags for loaded assemblies: 1 System.Windows.Forms, 2 WindowsBase (WPF).
		/// </summary>
		internal static int LibIsLoadedFormsWpf()
		{
			if(s_isLoadedFormsWpf == 0) {
				lock("zjm5R47f7UOmgyHUVZaf1w") {
					if(s_isLoadedFormsWpf == 0) {
						var ad = AppDomain.CurrentDomain;
						var a = ad.GetAssemblies();
						foreach(var v in a) {
							_FlagFromName(v);
							if(s_isLoadedFormsWpf == 3) return 3;
						}
						ad.AssemblyLoad += (_, x) => _FlagFromName(x.LoadedAssembly);
						s_isLoadedFormsWpf |= 0x100;
					}
				}
			}

			return s_isLoadedFormsWpf & 3;

			void _FlagFromName(Assembly a)
			{
				string s = a.FullName; //fast, cached. GetName can be slow because not cached.
				if(0 == (s_isLoadedFormsWpf & 1) && s.Starts("System.Windows.Forms,")) s_isLoadedFormsWpf |= 1;
				else if(0 == (s_isLoadedFormsWpf & 2) && s.Starts("WindowsBase,")) s_isLoadedFormsWpf |= 2;
			}
		}
		static volatile int s_isLoadedFormsWpf;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void LibEnsureLoaded(bool systemCore, bool formsDrawing = false)
		{
			if(systemCore) _ = typeof(System.Linq.Enumerable).Assembly; //System.Core, System
			if(formsDrawing) _ = typeof(System.Windows.Forms.Control).Assembly; //System.Windows.Forms, System.Drawing

			//It seems, after some Windows or .NET update, assembly loading became much faster. Now loads Forms+Drawing in 20-30 ms when cold CPU.
		}
	}
}
