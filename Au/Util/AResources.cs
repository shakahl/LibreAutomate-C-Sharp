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
using System.Linq;
using System.Resources;
using System.Globalization;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// Functions to work with managed resources.
	/// </summary>
	public static class AResources
	{
		/// <summary>
		/// Gets an Image or other object from managed resources of the app assembly (exe file).
		/// Returns null if not found.
		/// </summary>
		/// <param name="name">Resource name, like "example", not like "Project.Properties.Resources.example".</param>
		/// <remarks>
		/// Uses <see cref="ResourceManager.GetObject(string, CultureInfo)"/>.
		/// The Image is not cached. Will need to Dispose.
		/// </remarks>
		public static object GetAppResource(string name)
		{
			try {
				var rm = LibGetAppResourceManager(out var culture);
				return rm?.GetObject(name, culture);
			}
			catch { return null; }

			//info: why need culture? Because much much faster if culture is set to invariant.
		}

		/// <summary>
		/// Full name of embedded resources file, like "Project.Properties.Resources.resources".
		/// Set this property once before calling <see cref="GetAppResource"/>.
		/// If not set, it will look for resource where name ends with ".Resources.resources". If there is no such resources, uses the first.
		/// To see embedded resource file names you can use this code: <c>Print(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames()</c>.
		/// </summary>
		public static string AppResourcesName {
			get => s_appResourcesName;
			set { if(value != s_appResourcesName) { s_appResourcesName = value; _appResourceManager = null; } }
		}
		static string s_appResourcesName;

		/// <summary>
		/// Gets ResourceManager of the process entry assembly.
		/// Returns null if fails or if the assembly does not have resources.
		/// Note: if the assembly contains multiple embedded .resource files, may need to set <see cref="AppResourcesName"/> before.
		/// </summary>
		internal static ResourceManager LibGetAppResourceManager(out CultureInfo culture)
		{
			if(_appResourceManager == null) {
				culture = null;
				var asm = Assembly.GetEntryAssembly();
				var a = asm.GetManifestResourceNames(); if(a == null || a.Length == 0) return null; //no resources
				string s;
				if(s_appResourcesName != null) {
					s = a.FirstOrDefault(k => k == s_appResourcesName);
				} else {
					if(a.Length == 1 && a[0].Ends(".resources")) s = a[0];
					else {
						s = a.FirstOrDefault(k => k.Ends(".Resources.resources")); //eg "Project.Properties.Resources.resources". Skip those like "Form1.resources".
						if(s == null) s = a.FirstOrDefault(k => k.Ends(".resources"));
					}
				}
				if(s == null) return null;
				s = s.RemoveSuffix(10); //remove ".resources"
				var t = asm.GetType(s);
				if(t != null) {
					var fl = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; //need NonPublic because default access is internal
					if(t.GetProperty("ResourceManager", fl)?.GetValue(null) is ResourceManager rm) {
						_appResourceCulture = t.GetProperty("Culture")?.GetValue(null) as CultureInfo;
						_appResourceManager = rm;
					}
				}
				if(_appResourceManager == null) {
					//ADebug.Print("failed to get ResourceManager property"); //eg Au-compiled scripts don't have the type
					_appResourceManager = new ResourceManager(s, asm);
				}
			}
			culture = _appResourceCulture;
			return _appResourceManager;
		}
		static ResourceManager _appResourceManager;
		static CultureInfo _appResourceCulture;

		//rejected: now in generic methods we can use sizeof(T) if with 'unmanaged' constraint. Or Unsafe.SizeOf<T>().
		///// <summary>
		///// Do not call. Use class TypeSize, which caches the type size.
		///// This is used by TypeSize, not in it, because it is a generic type...
		///// </summary>
		///// <param name="t"></param>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//internal static int LibGetTypeSize(Type t)
		//{
		//	var dm = new DynamicMethod("SizeOfType", typeof(int), Type.EmptyTypes);
		//	ILGenerator il = dm.GetILGenerator();
		//	il.Emit(OpCodes.Sizeof, t);
		//	il.Emit(OpCodes.Ret);
		//	return (int)dm.Invoke(null, null);
		//	//Print(dm.MethodImplementationFlags);
		//}
	}
}
