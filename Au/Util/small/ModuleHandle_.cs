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
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Gets native module handle, or path from handle.
	/// </summary>
	public static class ModuleHandle_
	{
		/// <summary>
		/// Gets native module handle of type's assembly.
		/// </summary>
		public static IntPtr OfType(Type t)
		{
			return t == null ? default : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		/// <summary>
		/// Gets native module handle of an assembly.
		/// Returns default(IntPtr) if <i>asm</i> is null or if the assembly is in-memory (loaded from byte[]) or dynamic.
		/// </summary>
		public static IntPtr OfAssembly(Assembly asm)
		{
			if(asm == null || asm.IsDynamic || asm.Location.Length == 0) return default; //Location.Length == 0 if in-memory; it would throw if dynamic.
			var h = Marshal.GetHINSTANCE(asm.ManifestModule);
			if(h == (IntPtr)(-1)) h = default; //MSDN lies that it returns -1 for in-memory. It returns some invalid value.
			return h;
		}

		/// <summary>
		/// Gets native module handle of the entry assembly of this appdomain.
		/// Returns default(IntPtr) if the assembly is in-memory (loaded from byte[]) or dynamic.
		/// </summary>
		public static IntPtr OfAppDomainEntryAssembly()
		{
			return OfAssembly(Assembly_.EntryAssembly);
		}

		/// <summary>
		/// Gets native module handle of Au.dll.
		/// </summary>
		public static IntPtr OfAuDll()
		{
			return Marshal.GetHINSTANCE(typeof(ModuleHandle_).Module);
		}

		/// <summary>
		/// Gets native module handle of the program file of this process.
		/// </summary>
		public static IntPtr OfProcessExe()
		{
			return Api.GetModuleHandle(null);
		}

		//rejected. For script appdomains we use in-memory assemblies. They don't have a module handle (Marshal.GetHINSTANCE returns an invalid value). Use OfProcessExe.
		///// <summary>
		///// Gets native module handle of the assembly containing native icon that can be displayed as icon of this app.
		///// Some API functions need it when you use <msdn>IDI_APPLICATION</msdn>.
		///// If the entry assembly of this appdomain is dll with icon, gets dll handle; else gets exe handle.
		///// Returns default(IntPtr) if there are no native icons.
		///// </summary>
		//public static IntPtr OfAppIcon()
		//{
		//	if(s_hmodAppIcon == default) {
		//		IntPtr h = OfAppDomainEntryAssembly();
		//		if(h == default || default == Api.FindResource(h, Api.IDI_APPLICATION, 14)) { //RT_GROUP_ICON
		//			h = OfProcessExe();
		//			if(default == Api.FindResource(h, Api.IDI_APPLICATION, 14)) h = (IntPtr)1;
		//		}
		//		s_hmodAppIcon = h;
		//	}
		//	return s_hmodAppIcon == (IntPtr)1 ? default : s_hmodAppIcon;
		//}
		//static IntPtr s_hmodAppIcon;

		/// <summary>
		/// Gets full path of dll or exe file from its native handle.
		/// Returns null if fails. Supports <see cref="WinError"/>.
		/// Calls API <msdn>GetModuleFileName</msdn>.
		/// </summary>
		public static string GetFilePath(IntPtr hModule)
		{
			for(int na = 300; ; na *= 2) {
				var b = Buffers.LibChar(ref na);
				int n = Api.GetModuleFileName(default, b, na);
				if(n < na) return n == 0 ? null : b.ToString(n);
			}
		}
	}
}
