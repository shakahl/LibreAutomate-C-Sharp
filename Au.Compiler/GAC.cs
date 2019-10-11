using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Compiler
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class GAC
	{
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
		interface IAssemblyCache
		{
			[PreserveSig] int _U1();
			[PreserveSig] int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref ASSEMBLY_INFO assemblyInfo);
			//...
		}

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
		unsafe interface IAssemblyName
		{
			[PreserveSig] int _U1();
			[PreserveSig] int _U2();
			[PreserveSig] int _U3();
			[PreserveSig] int GetDisplayName(char* pDisplayName, ref int pccDisplayName, ASM_DISPLAY_FLAGS displayFlags);
			[PreserveSig] int _U4();
			[PreserveSig] int GetName(ref int lpcwBuffer, char* pwzName);
			//...
		}

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
		interface IAssemblyEnum
		{
			[PreserveSig] int GetNextAssembly(IntPtr pvReserved, out IAssemblyName ppName, int flags);
			//[PreserveSig] int Reset();
			//...
		}

		[Flags]
		enum ASM_CACHE_FLAGS
		{
			GAC = 0x02,
		}

		[Flags]
		enum CREATE_ASM_NAME_OBJ_FLAGS
		{
			PARSE_DISPLAY_NAME = 0x01,
		}

		[Flags]
		enum ASM_DISPLAY_FLAGS
		{
			VERSION = 0x01,
			CULTURE = 0x02,
			PUBLIC_KEY_TOKEN = 0x04,
			PROCESSORARCHITECTURE = 0x20,
			RETARGETABLE = 0x80,
			ALL = VERSION | CULTURE | PROCESSORARCHITECTURE | PUBLIC_KEY_TOKEN | RETARGETABLE
		}

#pragma warning disable 169, 649 //field never used
		unsafe struct ASSEMBLY_INFO
		{
			public int cbAssemblyInfo;
			public int assemblyFlags;
			public long assemblySizeInKB;
			public char* currentAssemblyPath;
			public int cchBuf;
		}
#pragma warning restore 169, 649

		[DllImport("fusion.dll")]
		static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

		[DllImport("fusion.dll")]
		static extern int CreateAssemblyNameObject(out IAssemblyName ppAssemblyNameObj, string szAssemblyName, CREATE_ASM_NAME_OBJ_FLAGS flags, IntPtr pvReserved);

		[DllImport("fusion.dll")]
		static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum, IntPtr pUnkReserved, IAssemblyName pName, ASM_CACHE_FLAGS flags, IntPtr pvReserved);

		static void _HR(int hr)
		{
			if(hr < 0) Marshal.ThrowExceptionForHR(hr);
		}

		static unsafe string _GetAssemblyPath(string assemblyName)
		{
			ASSEMBLY_INFO x = default;
			x.cbAssemblyInfo = Api.SizeOf<ASSEMBLY_INFO>();
			x.cchBuf = 1024;
			var b = stackalloc char[x.cchBuf];
			x.currentAssemblyPath = b;
			IAssemblyCache ac = null; //tested: static var does not make faster. The slow part is QueryAssemblyInfo.
			_HR(CreateAssemblyCache(out ac, 0));
			_HR(ac.QueryAssemblyInfo(0, assemblyName, ref x));
			Marshal.ReleaseComObject(ac);

			return new string(b);
		}

		class _AssemblyEnum
		{
			IAssemblyEnum _e;
			bool _done;

			public _AssemblyEnum()
			{
				_HR(CreateAssemblyEnum(out _e, default, null, ASM_CACHE_FLAGS.GAC, default));
				//tested: fails with ASM_CACHE_ROOT_EX. It is only for GetCachePath, it is documented.
			}

			public _AssemblyEnum(string sAsmName)
			{
				_HR(CreateAssemblyNameObject(out var an, sAsmName, CREATE_ASM_NAME_OBJ_FLAGS.PARSE_DISPLAY_NAME, default));
				_HR(CreateAssemblyEnum(out _e, default, an, ASM_CACHE_FLAGS.GAC, default));
				Marshal.ReleaseComObject(an);
			}

			public unsafe string GetNextAssembly(ASM_DISPLAY_FLAGS flags)
			{
				string R = null;
				if(!_done) {
					_HR(_e.GetNextAssembly(default, out var asmName, 0));

					if(asmName != null) {
						int n = 300;
						var b = stackalloc char[n + 1];
						_HR(flags == 0 ? asmName.GetName(ref n, b) : asmName.GetDisplayName(b, ref n, flags));
						R = new string(b); //n is with null, undocumented
					}

					_done = (R == null);
				}
				return R;
			}
		}

		/// <summary>
		/// Finds assembly by name or name/version in GAC and returns file path. Returns null if not found.
		/// </summary>
		/// <param name="asmName">Assembly name without version.</param>
		/// <param name="withVersion">If null, gets highest assembly version. Else gets assembly of exactly this version; must be like "Name, Version=1.2.3" and not like "Name,  version = 1.2.3".</param>
		public static string FindAssembly(string asmName, string withVersion = null)
		{
			var asmEnum = new _AssemblyEnum(asmName);

			string highestVersion = null;
			for(; ; ) {
				var s = asmEnum.GetNextAssembly(ASM_DISPLAY_FLAGS.VERSION); if(s == null) break;
				if(withVersion != null) {
					if(s == withVersion) return _GetAssemblyPath(s);
					continue;
				}
				if(highestVersion == null || string.Compare(s, highestVersion) > 0) highestVersion = s;
			}

			if(highestVersion != null) return _GetAssemblyPath(highestVersion);
			return null;
		}

		public static IEnumerable<string> EnumAssemblies(bool withVersion)
		{
			var e = new _AssemblyEnum();
			while(true) {
				var s = e.GetNextAssembly(withVersion ? ASM_DISPLAY_FLAGS.VERSION : 0);
				if(s == null) yield break;

				//SHOULDDO: skip assemblies of old CLR versions.
				//	For it can use _GetAssemblyPath. Old versions are in other folder. But makes slower 200 ms -> 700 ms.
				//	I could not find other ways.
				//var path = _GetAssemblyPath(s);

				yield return s;
			}
		}

		//public static unsafe void TestGetCachePath()
		//{
		//	var cp = stackalloc char[1001]; int n = 1000;
		//	int hr = GetCachePath((ASM_CACHE_FLAGS)0x80, cp, ref n); //ASM_CACHE_ROOT_EX
		//	if(hr != 0) { Print(hr); return; }
		//	var s = new string(cp);
		//	Print(s); //OK
		//}
		//[DllImport("fusion.dll")]
		//static extern unsafe int GetCachePath(ASM_CACHE_FLAGS flags, char* path, ref int ccPath);
	}
}
