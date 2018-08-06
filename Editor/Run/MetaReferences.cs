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
using static Au.NoClass;

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

/// <summary>
/// Resolves assembly metadata references and adds to a temporary cache - a list of PortableExecutableReference objects.
/// Single static cache is used by all CompReferences variables. Not thread-safe.
/// Cache requires many MB of memory, therefore is cleared (all unloaded) by GC. A reference to a CompReferences variable prevents unloading.
/// </summary>
public class MetaReferences
{
	static WeakReference<List<PortableExecutableReference>> s_refsWR = new WeakReference<List<PortableExecutableReference>>(null);
	List<PortableExecutableReference> _cache; //all references resolved by any variables, + default. In s_refsWR.
	Dictionary<string, string> _dict; //name/path of references resolved by this variable

	/// <summary>
	/// Returns list of references default and unique references for which was called Resolve of this CompReferences variable.
	/// Before calling any Resolve, contains only default references.
	/// </summary>
	public List<PortableExecutableReference> Refs { get; }

	/// <summary>
	/// Returns list of default references and all resolved/loaded/cached references by any CompReferences variables.
	/// </summary>
	public List<PortableExecutableReference> Cache => _cache;

	public MetaReferences()
	{
		var defRef = Compiler.DefaultReferences;
		if(!s_refsWR.TryGetTarget(out _cache)) s_refsWR.SetTarget(_cache = _CreateDefaultRefs());

		int nDefault = defRef.Count;
		Refs = new List<PortableExecutableReference>(nDefault);
		for(int i = 0; i < nDefault; i++) Refs.Add(_cache[i]);

		List<PortableExecutableReference> _CreateDefaultRefs()
		{
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
	/// If used filename without full path, searches in <see cref="Folders.ThisApp"/>, subfolder "Libraries", <paramref name="dirs"/>, <see cref="Folders.NetFrameworkRuntime"/> and GAC.
	/// If used assembly name without ".dll", ".exe", '\\' or '/', searches only in <see cref="Folders.NetFrameworkRuntime"/> and GAC.
	/// Loads the file but does not parse. If bad format, error later when compiling.
	/// </remarks>
	public bool Resolve(string reference)
	{
		var defRef = Compiler.DefaultReferences;
		if(defRef.ContainsKey(reference)) return true;
		if(_dict?.ContainsKey(reference) ?? false) return true;

		var path = _ResolvePath(reference);
		if(path == null) return false;
		path = Path_.LibNormalize(path);

		//foreach(var v in _refs) Print(v.FilePath);
		var mr = _cache.Find(v => v.FilePath.Equals_(path, true));
		if(mr == null) _cache.Add(mr = MetadataReference.CreateFromFile(path));

		Refs.Add(mr);

		if(_dict == null) _dict = new Dictionary<string, string>(defRef.Count * 2);
		_dict.Add(reference, path);

		return true;
	}

	static string _ResolvePath(string re)
	{
		if(Empty(re)) return null;
		if(Path_.IsFullPathExpandEnvVar(ref re)) {
			return Files.ExistsAsFile(re) ? re : null;
		}
		string path, ext;
		if(0 != re.EndsWith_(true, s_asmExt) || re.IndexOfAny(String_.Lib.pathSep) >= 0) {
			foreach(var v in s_dirs) {
				path = Path_.Combine(v, re);
				if(Files.ExistsAsFile(path)) return path;
			}
			ext = null;
		} else ext = ".dll";

		var d = RuntimeEnvironment.GetRuntimeDirectory();
		path = d + re + ext;
		if(Files.ExistsAsFile(path)) return path;
		path = d + @"WPF\" + re + ext;
		if(Files.ExistsAsFile(path)) return path;

		return GAC.FindAssembly(re);

		//note: we don't use Microsoft.CodeAnalysis.Scripting.ScriptMetadataResolver. It is very slow, makes compiling many times slower.

		//TODO: also look in DEVPATH env var, if config file contains <developmentMode developerInstallation="true"/>
	}
	static string[] s_asmExt = new string[] { ".dll", ".exe" };
	static string[] s_dirs = new string[] { Folders.ThisApp, Folders.ThisApp + "Libraries" };

	/// <summary>
	/// Extracts path from compiler error message CS0009 and removes the reference from cache.
	/// </summary>
	/// <param name="errorMessage">"Metadata file '....dll' could not be opened ..."</param>
	public void RemoveBadReference(string errorMessage)
	{
		if((errorMessage.RegexMatch_(@"'(.+?)'", 1, out var path)))
			_cache.RemoveAll(v => v.FilePath.Equals_(path, true));
	}
}
/*
//TODO: create MetadataReference list only when need.

		public List<string> ReferencePaths { get; private set; }

	public List<PortableExecutableReference> ReferenceMetadata => _refs?.
*/

static class GAC
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

	static void _CheckHR(int hr)
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

		IAssemblyCache ac = null;
		_CheckHR(CreateAssemblyCache(out ac, 0));
		_CheckHR(ac.QueryAssemblyInfo(0, assemblyName, ref x));

		return new string(b);
	}

	class _AssemblyEnum
	{
		IAssemblyEnum _e;
		bool _done;

		public _AssemblyEnum(string sAsmName)
		{
			_CheckHR(CreateAssemblyNameObject(out var asmName, sAsmName, CREATE_ASM_NAME_OBJ_FLAGS.PARSE_DISPLAY_NAME, default));
			_CheckHR(CreateAssemblyEnum(out _e, default, asmName, ASM_CACHE_FLAGS.GAC, default));
		}

		public unsafe string GetNextAssembly(ASM_DISPLAY_FLAGS flags)
		{
			string R = null;
			if(!_done) {
				_CheckHR(_e.GetNextAssembly(default, out var asmName, 0));

				if(asmName != null) {
					int n = 300;
					var b = stackalloc char[n + 1];
					_CheckHR(asmName.GetDisplayName(b, ref n, flags));
					R = new string(b); //n is with null, undocumented
				}

				_done = (R == null);
			}
			return R;
		}
	}

	public static string FindAssembly(string asmName)
	{
		var asmEnum = new _AssemblyEnum(asmName);

		string highestVersion = null;
		for(; ; ) {
			var s = asmEnum.GetNextAssembly(ASM_DISPLAY_FLAGS.VERSION); if(s == null) break;
			//Print(s);
			if(highestVersion == null || string.Compare(s, highestVersion) > 0) highestVersion = s;
		}

		if(highestVersion != null) return _GetAssemblyPath(highestVersion);
		return null;
	}
}
