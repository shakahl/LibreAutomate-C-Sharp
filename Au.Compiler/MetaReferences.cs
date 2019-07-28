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
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Au.Compiler
{
	/// <summary>
	/// Resolves assembly metadata references and adds to a temporary cache - a list of PortableExecutableReference objects.
	/// Single static cache is used by all MetaReferences variables.
	/// Cache requires many MB of unmanaged memory, therefore is cleared (all unloaded) by GC. A reference to a MetaReferences variable prevents unloading.
	/// For each compilation create a MetaReferences variable. Compilations can be in different threads, but a variable must be used in same thread as created.
	/// </summary>
	public class MetaReferences
	{
		static WeakReference<List<PortableExecutableReference>> s_refsWR = new WeakReference<List<PortableExecutableReference>>(null);
		List<PortableExecutableReference> _cache; //all references resolved by any variables, + default. In s_refsWR.
		Dictionary<string, string> _dict; //name/path of references resolved by this variable

		static List<PortableExecutableReference> s_cacheKeeper;
		static Timer s_timerCK = new Timer(_ => s_cacheKeeper = null);

		/// <summary>
		/// Returns list of references default and unique references for which was called Resolve of this MetaReferences variable.
		/// Before calling any Resolve, contains only default references.
		/// </summary>
		public List<PortableExecutableReference> Refs { get; }

		//unused. Also, would be not thread-safe.
		///// <summary>
		///// Returns list of default references and all resolved/loaded/cached references by any MetaReferences variables.
		///// </summary>
		//public List<PortableExecutableReference> Cache => _cache;

		public MetaReferences()
		{
			lock(s_refsWR) {
				if(!s_refsWR.TryGetTarget(out _cache)) s_refsWR.SetTarget(_cache = _CreateDefaultRefs());

				//prevent GC the cache too early, eg in the middle of compiling hundreds of files.
				//	Not important. Reloading references is quite fast. But I like such speed improvements.
				//	Also tested MemoryCache, but it is too slow, ~30 mcs when cold, and need System.Runtime.Caching.dll.
				s_cacheKeeper = _cache;
				s_timerCK.Change(3000, -1); //fast. Note: must be < than timer in Compiler.LibSetTimerGC().

				int nDefault = Compiler.DefaultReferences.Count;
				Refs = new List<PortableExecutableReference>(nDefault);
				for(int i = 0; i < nDefault; i++) Refs.Add(_cache[i]);
			}

			List<PortableExecutableReference> _CreateDefaultRefs()
			{
				var defRef = Compiler.DefaultReferences;
				var refs = new List<PortableExecutableReference>(defRef.Count);
				foreach(var s in defRef.Values) {
					refs.Add(MetadataReference.CreateFromFile(s));
				}
				return refs;
			}
		}

		/// <summary>
		/// Finds reference assembly file, creates PortableExecutableReference and adds to the cache.
		/// Returns false if file not found.
		/// </summary>
		/// <param name="reference">Assembly name (like "System.Something") or filename (like "My.dll") or path.</param>
		/// <exception cref="Exception">Some unexpected exception, eg failed to load the found file.</exception>
		/// <remarks>
		/// If used filename without full path, searches in <see cref="AFolders.ThisApp"/>, subfolder "Libraries", <see cref="AFolders.NetFrameworkRuntime"/> and GAC.
		/// If used assembly name without ".dll", ".exe" or ", Version=", searches only in <see cref="AFolders.NetFrameworkRuntime"/> and GAC.
		/// If contains", Version=", searches only in GAC.
		/// Can be "Alias=X.dll" - assembly reference that can be used with C# keyword 'extern alias'.
		/// Loads the file but does not parse. If bad format, error later when compiling.
		/// </remarks>
		public bool Resolve(string reference, bool isCOM)
		{
			string alias = null;
			int i = reference.IndexOf('=');
			if(i >= 0 && !(i > 9 && reference.Eq(i - 9, ", Version"))) {
				alias = reference.Remove(i);
				reference = reference.Substring(i + 1);
			}

			var defRef = Compiler.DefaultReferences;
			if(!isCOM && defRef.ContainsKey(reference)) return true;
			if(_dict?.ContainsKey(reference) ?? false) return true;

			var path = _ResolvePath(reference, isCOM);
			if(path == null) return false;
			path = APath.LibNormalize(path);

			//foreach(var v in _refs) Print(v.FilePath);
			PortableExecutableReference mr;
			lock(s_refsWR) {
				mr = _cache.Find(v => v.FilePath.Eqi(path));
				if(mr == null) {
					MetadataReferenceProperties prop = alias == null && !isCOM
						? default
						: new MetadataReferenceProperties(aliases: alias == null ? default : ImmutableArray.Create(alias), embedInteropTypes: isCOM);

					mr = MetadataReference.CreateFromFile(path, prop);
					_cache.Add(mr);
				}
			}

			Refs.Add(mr);

			if(_dict == null) _dict = new Dictionary<string, string>(defRef.Count * 2);
			_dict.Add(reference, path);

			return true;
		}

		static string _ResolvePath(string re, bool isCOM)
		{
			if(Empty(re)) return null;
			bool isFull = APath.IsFullPathExpandEnvVar(ref re);
			if(!isFull && isCOM) { isFull = true; re = AFolders.Workspace + @".interop\" + re; }
			if(isFull) return AFile.ExistsAsFile(re) ? re : null;

			string path, ext; int i;
			if(0 != re.Ends(true, s_asmExt)) {
				foreach(var v in s_dirs) {
					path = APath.Combine(v, re);
					if(AFile.ExistsAsFile(path)) return path;
				}
				ext = null;
			} else if((i = re.Find(", Version=")) > 0) {
				return GAC.FindAssembly(re.Remove(i), re);
			} else ext = ".dll";

			var d = RuntimeEnvironment.GetRuntimeDirectory();
			path = d + re + ext;
			if(AFile.ExistsAsFile(path)) return path;

			bool isRelPath = re.FindChars(@"\/") >= 0;
			if(!isRelPath) {
				path = d + @"WPF\" + re + ext;
				if(AFile.ExistsAsFile(path)) return path;
			}

			if(ext == null || isRelPath) path = null;
			else path = GAC.FindAssembly(re);

			//FUTURE: also look in DEVPATH env var, if config file contains <developmentMode developerInstallation="true"/>

			return path;

			//note: we don't use Microsoft.CodeAnalysis.Scripting.ScriptMetadataResolver. It is very slow, makes compiling many times slower.
		}
		static readonly string[] s_asmExt = { ".dll", ".exe" };
		static readonly string[] s_dirs = { AFolders.ThisApp, AFolders.ThisApp + "Libraries" };

		/// <summary>
		/// Extracts path from compiler error message CS0009 and removes the reference from cache.
		/// </summary>
		/// <param name="errorMessage">"Metadata file '....dll' could not be opened ..."</param>
		public void RemoveBadReference(string errorMessage)
		{
			if(errorMessage.RegexMatch(@"'(.+?)'", 1, out string path))
				lock(s_refsWR) { _cache.RemoveAll(v => v.FilePath.Eqi(path)); }
		}

#if DEBUG
		internal static void DebugPrintCachedRefs()
		{
			if(!s_refsWR.TryGetTarget(out var c)) return;
			foreach(var v in c) Print(v.Display);
		}
#endif
	}

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
			var p = APerf.StartNew();
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
