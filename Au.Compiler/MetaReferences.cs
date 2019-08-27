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
using System.Globalization;

namespace Au.Compiler
{
	/// <summary>
	/// Resolves assembly metadata references (string to PortableExecutableReference).
	/// For each compilation create a MetaReferences variable, call Resolve if need non-default references, then use Refs list.
	/// </summary>
	/// <remarks>
	/// Temporarily keeps PortableExecutableReference objects in a cache. Single static cache is used by all MetaReferences variables.
	/// Cache requires many MB of unmanaged memory, therefore PortableExecutableReference objects are removed and GC-collected when not used anywhere for some time. Reloading is quite fast.
	///		A reference to a PortableExecutableReference variable prevents removing it from cache and GC-collecting.
	///		A reference to a MetaReferences variable prevents removing/disposing is Refs items.
	///		CodeAnalysis Project etc objects also have references, therefore need to manage their lifetimes too.
	/// MetaReferences variables can be created in different threads, but a variable must be used in a single thread.
	/// </remarks>
	public class MetaReferences
	{
		/// <summary>
		/// Contains info used to load a PortableExecutableReference. Loads when need. Supports XML documentation.
		/// </summary>
		class _MR
		{
			public string name, path;
			WeakReference<PortableExecutableReference> _wr;
			PortableExecutableReference _refKeeper;
			long _timeout;

			const int c_timerPeriod = 30_000;

			public _MR(string name, string path)
			{
				//Print(name);
				this.name = name;
				this.path = path;
				_wr = new WeakReference<PortableExecutableReference>(null);
			}

			public PortableExecutableReference Ref {
				get {
					if(!_wr.TryGetTarget(out var r)) {
						//for(int i=0;i<10;i++) //tested: process memory does not grow when loading same file several times
						//APerf.First();
						DocumentationProvider docProv = null;
						if(s_docDB.HaveRef(name)) {
							docProv = s_docDB;
						} else {
							var xmlPath = Path.ChangeExtension(path, "xml");
							if(AFile.ExistsAsFile(xmlPath, true)) docProv = XmlDocumentationProvider.CreateFromFile(xmlPath);
						}
						r = MetadataReference.CreateFromFile(path, (this as _MR2)?.Prop ?? default, docProv);
						//APerf.NW();

						_wr.SetTarget(r);
						//Print("LOADED", name, this is _MR2);

						//prevent GC too early, eg in the middle of compiling many files
						if(s_timer == null) {
							s_timer = new Timer(_ => {
								int nKeep = 0;
								lock(s_cache) {
									long timeNow = ATime.WinMilliseconds;
									foreach(var v in s_cache) {
										if(v._refKeeper == null) continue;
										long t = v._timeout;
										if(timeNow >= t) v._refKeeper = null;
										else nKeep++;
									}
									//Print("timer", nRemoved);
									s_isTimer = nKeep > 0 ? s_timer.Change(c_timerPeriod, -1) : false;
								}
								if(nKeep == 0) GC.Collect();
							});
						}
					} //else Print("cached", name, this is _MR2);

					if(!s_isTimer) s_isTimer = s_timer.Change(c_timerPeriod, -1);
					_timeout = ATime.WinMilliseconds + c_timerPeriod - 1000;
					_refKeeper = r;
					return r;
				}
			}

			//public static void CompactCache()
			//{
			//	lock(s_cache) {
			//		foreach(var v in s_cache) v._refKeeper = null;
			//		s_isTimer = s_timer.Change(1000, -1);
			//	}
			//}

#if DEBUG
			public bool IsCached => _wr.TryGetTarget(out _);
#endif
		}

		class _MR2 : _MR
		{
			string _alias;
			bool _isCOM;

			public _MR2(string name, string path, string alias, bool isCOM) : base(name, path) { _alias = alias; _isCOM = isCOM; }

			public MetadataReferenceProperties Prop => new MetadataReferenceProperties(aliases: _alias == null ? default : ImmutableArray.Create(_alias), embedInteropTypes: _isCOM);

			public bool PropEq(string alias, bool isCOM) => isCOM == _isCOM && alias == _alias;
		}

		static List<_MR> s_cache = new List<_MR>(16);
		static Timer s_timer;
		static bool s_isTimer;

		/// <summary>
		/// List of default references and references for which was called Resolve of this MetaReferences variable.
		/// Before calling any Resolve, contains only default references.
		/// </summary>
		public List<PortableExecutableReference> Refs { get; }

		public MetaReferences()
		{
			lock(s_cache) {
				//var p1 = APerf.Create();
				var def = Compiler.DefaultReferences;
				if(s_cache.Count == 0) {
					foreach(var x in def) {
						s_cache.Add(new _MR(x.Key, x.Value));
					}
				}
				int nDef = def.Count;
				Refs = new List<PortableExecutableReference>(nDef);
				for(int i = 0; i < nDef; i++) _AddRef(i);
				//p1.NW('R');
			}
		}

		void _AddRef(int iCache) => Refs.Add(s_cache[iCache].Ref);

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

			var a = s_cache;
			lock(a) {
				int searchFrom = isCOM ? Compiler.DefaultReferences.Count : 0;
				for(i = searchFrom; i < a.Count; i++) {
					if(a[i].name.Eqi(reference) && _PropEq(a[i], alias, isCOM)) {
						_AddRef(i);
						return true;
					}
				}

				var path = _ResolvePath(reference, isCOM);
				if(path == null) return false;
				path = APath.LibNormalize(path);

				for(i = searchFrom; i < a.Count; i++) {
					if(a[i].path.Eqi(path) && _PropEq(a[i], alias, isCOM)) goto g1;
				}
				_MR k;
				if(isCOM || alias != null) k = new _MR2(reference, path, alias, isCOM);
				else k = new _MR(reference, path);
				a.Add(k);
				g1:
				_AddRef(i);
			}

			return true;

			bool _PropEq(_MR u, string alias, bool isCOM)
			{
				if(u is _MR2 m2) return m2.PropEq(alias, isCOM);
				return !isCOM && alias == null;
			}
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
		public static void RemoveBadReference(string errorMessage)
		{
			if(errorMessage.RegexMatch(@"'(.+?)'", 1, out string path))
				lock(s_cache) { s_cache.RemoveAll(v => v.path.Eqi(path)); }
		}

		//public static void CompactCache() => _MR.CompactCache();

#if DEBUG
		internal static void DebugPrintCachedRefs()
		{
			foreach(var v in s_cache) if(v.IsCached) Print(v.name);
		}
#endif

		/// <summary>
		/// Gets XML documentation for .NET framework 4.7.2 assemblies and Au.dll.
		/// Uses a 2-column SQLite database created from XML files using script BuildDocumentationProviderDatabase.
		/// Not XML files directly because it uses many MB of process memory, eg 210 MB instead of 70 MB.
		/// </summary>
		class _DocumentationDatabase : DocumentationProvider
		{
			ASqlite _db;
			ASqliteStatement _stat;
			HashSet<string> _refs;

			public _DocumentationDatabase()
			{
				try {
					_db = new ASqlite(AFolders.ThisAppBS + "CiDoc.db", SLFlags.SQLITE_OPEN_READONLY);
					if(_db.Get(out string s, "SELECT xml FROM data WHERE name='.'")) _refs = new HashSet<string>(s.SegSplit("\n"));
				}
				catch(SLException ex) { ADebug.Print(ex.Message); }
			}

			/// <summary>
			/// Returns true if the database contains XML doc of the reference assembly.
			/// </summary>
			/// <param name="refName">Like "mscorlib" or "System.Core" or "Au.dll".</param>
			public bool HaveRef(string refName) => _refs?.Contains(refName) ?? false;

			protected override string GetDocumentationForSymbol(string documentationMemberID, CultureInfo preferredCulture, CancellationToken cancellationToken = default)
			{
				if(_db != null) {
					lock(_db) { //sometimes not in main thread
						try {
							_stat ??= _db.Statement("SELECT xml FROM data WHERE name=?");
							if(_stat.Bind(1, documentationMemberID).Step()) return _stat.GetText(0);
							//ADebug.Print(documentationMemberID);
						}
						catch(SLException ex) { ADebug.Print(ex.Message); }
						finally { _stat?.Reset(); }
					}
				}
				return null;
			}

			public override bool Equals(object obj)
			{
				ADebug.PrintIf(obj != this, "Equals");
				return obj == this;
			}

			public override int GetHashCode()
			{
				ADebug.Print("GetHashCode");
				return 1;
			}

			public string GetNamespaceDoc(string namespaceName)
			{
				var s = GetDocumentationForSymbol("N:" + namespaceName, null, default);
				if(s != null) s = s.Replace("&lt;", "<").Replace("&gt;", ">");
				return s;
				//info: the xml->db script removed <cref> and <summary> tags in namespaces.xml.
			}
		}
		static _DocumentationDatabase s_docDB = new _DocumentationDatabase();

		public static string GetNamespaceDoc(string namespaceName) => s_docDB.GetNamespaceDoc(namespaceName);
	}
}
