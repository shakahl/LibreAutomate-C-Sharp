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

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	public static partial class Compiler
	{
		/// <summary>
		/// Resolves whether need to [re]compile or can run previously compiled assembly.
		/// </summary>
		unsafe class XCompiled
		{
			ICollectionFiles _coll;
			string _file;
			Dictionary<string, string> _data;

			public string CacheDirectory { get; }

			public static XCompiled OfCollection(ICollectionFiles coll)
			{
				var cc = coll.IcfCompilerContext;
				if(cc == null) coll.IcfCompilerContext = cc = new XCompiled(coll);
				return cc as XCompiled;
			}

			public XCompiled(ICollectionFiles coll)
			{
				_coll = coll;
				CacheDirectory = _coll.IcfCollectionDirectory + @"\.compiled";
				_file = CacheDirectory + @"\compiled.log";
			}

			static XCompiled()
			{
				bool ok = Registry_.GetInt(out s_frameworkVersion, "Release", @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
				Debug.Assert(ok);
				s_auVersion = typeof(Wnd).Assembly.GetName().Version.ToString();
			}
			static int s_frameworkVersion;
			static string s_auVersion;

			/// <summary>
			/// Called before executing script/app f. If returns true, don't need to compile.
			/// </summary>
			/// <param name="f"></param>
			/// <param name="r">Receives file path and execution options.</param>
			/// <param name="projFolder">Project folder or null. If not null, f must be its main file.</param>
			public bool IsCompiled(ICollectionFile f, out CompResults r, ICollectionFile projFolder)
			{
				r = default;

				if(_data == null && !_Open()) return false;

				if(!_data.TryGetValue(f.Guid, out string value)) return false;
				//Debug_.Print(value);
				int iPipe = 0;

				bool isScript = f.IsScript;
				r.maxInstances = MetaComments.DefaultMaxInstances(isScript);
				r.runAlone = MetaComments.DefaultRunAlone(isScript);
				r.outputType = MetaComments.DefaultOutputType(isScript);

				string asmFile;
				if(value != null && value.StartsWith_("|=")) {
					iPipe = value.IndexOf('|', 2); if(iPipe < 0) iPipe = value.Length;
					asmFile = value.Substring(2, iPipe - 2);
				} else asmFile = CacheDirectory + "\\" + f.Guid;
				//Print(asmFile);

				if(!File_.GetProperties(asmFile, out var asmProp, FAFlags.UseRawPath)) return false;
				DateTime asmDate = asmProp.LastWriteTimeUtc;

				if(_IsFileModified(f)) return false;

				bool isProject = false;
				if(value != null && iPipe < value.Length) {
					iPipe++;
					foreach(var s in value.Segments_(iPipe, value.Length - iPipe, "|", SegFlags.NoEmpty)) {
						//Print(s);
						int offs = s.Offset + 1;
						switch(s[0]) {
						case 't':
							r.outputType = (EOutputType)value.ToInt_(offs);
							break;
						case 'i':
							r.isolation = (EIsolation)value.ToInt_(offs);
							break;
						case 'u':
							r.uac = (EUac)value.ToInt_(offs);
							break;
						case 'a':
							r.runAlone = (ERunAlone)value.ToInt_(offs);
							break;
						case 'n':
							r.maxInstances = value.ToInt_(offs);
							break;
						case 'z':
							r.mtaThread = true;
							break;
						case 'p':
							isProject = true;
							if(projFolder != null) {
								if(!Convert_.MD5HashResult.FromString(value, offs, s.EndOffset - offs, out var md5)) return false;
								Convert_.MD5Hash md = default;
								foreach(var f1 in projFolder.IcfEnumProjectFiles(f)) {
									if(_IsFileModified(f1)) return false;
									var g = Convert_.GuidFromHex(f1.Guid);
									md.Add(&g, sizeof(Guid));
								}
								if(md.Hash != md5) return false;
							}
							break;
						case '*':
							var dll = value.Substring(offs, s.EndOffset - offs);
							if(!Path_.IsFullPath(dll)) dll = Folders.ThisApp + dll;
							if(_IsFileModified2(dll)) return false;
							break;
						default:
							switch(s[0]) { case 'c': case 'd': case 'k': case 'm': case 'x': case 's': case 'o': break; default: return false; }
							var f2 = _coll.IcfFindByGUID(value.Substring(offs, s.EndOffset - offs));
							if(f2 == null) return false;
							switch(s[0]) {
							case 'o': r.config = f2.FilePath; break;
							default: if(_IsFileModified(f2)) return false; break;
							}
							break;
						}
					}
				}
				if(isProject != (projFolder != null)) return false;
				//Debug_.Print("compiled");

				r.file = asmFile;
				r.name = f.Name; if(!isScript) r.name = r.name.Remove(r.name.Length - 3);
				return true;

				bool _IsFileModified(ICollectionFile f_) => _IsFileModified2(f_.FilePath);

				bool _IsFileModified2(string path_)
				{
					if(!File_.GetProperties(path_, out var prop_, FAFlags.UseRawPath)) return true;
					//Print(prop_.LastWriteTimeUtc, asmDate);
					if(prop_.LastWriteTimeUtc > asmDate) return true;
					return false;
				}
			}

			/// <summary>
			/// Called when successfully compiled script/app f. Saves data that next time will be used by <see cref="IsCompiled"/>.
			/// </summary>
			/// <param name="f"></param>
			/// <param name="outFile">The output assembly.</param>
			/// <param name="m"></param>
			public void AddCompiled(ICollectionFile f, string outFile, MetaComments m, bool mtaThread)
			{
				if(_data == null) {
					_data = new Dictionary<string, string>();
				}

				/*
	GUIDmain|=path.exe|t2|i2|u2|a2|n2|z|pMD5project|cGUIDcode|dGUIDresource|kGUIDicon|mGUIDmanifest|xGUIDres|sGUIDsign|oGUIDconfig|*ref
	= - outFile
	t - outputType
	i - isolation
	u - uac
	a - runAlone
	n - maxInstances
	z - mtaThread
	p - MD5 of guid of all project files except main
	c - c
	d - resource
	k - icon
	m - manifest
	x - res
	s - sign
	o - config
	* - r
				*/

				string value = null;
				using(new Au.Util.LibStringBuilder(out var b)) {
					if(m.OutputPath != null) b.Append("|=").Append(outFile); //else f.Guid in cache
					if(m.OutputType != MetaComments.DefaultOutputType(m.IsScript)) b.Append("|t").Append((int)m.OutputType);
					if(m.Isolation != EIsolation.appDomain) b.Append("|i").Append((int)m.Isolation);
					if(m.Uac != EUac.host) b.Append("|u").Append((int)m.Uac);
					if(m.RunAlone != MetaComments.DefaultRunAlone(m.IsScript)) b.Append("|a").Append((int)m.RunAlone);
					if(m.MaxInstances != MetaComments.DefaultMaxInstances(m.IsScript)) b.Append("|n").Append(m.MaxInstances);
					if(mtaThread) b.Append("|z");

					int nAll = m.Files.Count, nNoC = nAll - m.CountC;
					if(nNoC > 1) { //add MD5 hash of project files, except main
						Convert_.MD5Hash md = default;
						for(int i = 1; i < nNoC; i++) {
							var g = Convert_.GuidFromHex(m.Files[i].f.Guid);
							md.Add(&g, sizeof(Guid));
						}
						b.Append("|p").Append(md.Hash.ToString());
					}
					for(int i = nNoC; i < nAll; i++) _AppendFile("|c", m.Files[i].f); //guids of C# files added through meta 'c'
					if(m.ResourceFiles != null) foreach(var v in m.ResourceFiles) _AppendFile("|d", v.f); //guids of 'resource' files
					_AppendFile("|k", m.IconFile);
					_AppendFile("|m", m.ManifestFile);
					_AppendFile("|x", m.ResFile);
					_AppendFile("|s", m.SignFile);
					_AppendFile("|o", m.ConfigFile);

					//references
					var refs = m.References.Refs;
					int j = DefaultReferences.Count;
					if(refs.Count > j) {
						//string netDir = Folders.Windows + @"Microsoft.NET\";
						string netDir = Folders.NetFrameworkRuntime; //no GAC
						var appDir = (string)Folders.ThisApp + @"\";
						for(; j < refs.Count; j++) {
							var s1 = refs[j].FilePath;
							if(s1.StartsWith_(netDir, true)) continue;
							if(s1.StartsWith_(appDir, true)) s1 = s1.Substring(appDir.Length);
							b.Append("|*").Append(s1);
						}
					}

					if(b.Length != 0) value = b.ToString();

					void _AppendFile(string opt, ICollectionFile f_)
					{
						if(f_ != null) b.Append(opt).Append(f_.Guid);
					}
				}

				var guid = f.Guid;
				if(_data.TryGetValue(guid, out var oldValue) && value == oldValue) { /*Debug_.Print("same");*/ return; }
				//Debug_.Print("different");
				_data[guid] = value;
				_Save();
			}

			/// <summary>
			/// Removes saved f data, so that next time <see cref="IsCompiled"/> will return false.
			/// </summary>
			/// <param name="f"></param>
			public void Remove(ICollectionFile f)
			{
				if(_data == null && !_Open()) return;
				if(_data.Remove(f.Guid)) _Save();
			}

			bool _Open()
			{
				if(_data != null) return true;
				if(!File_.ExistsAsFile(_file)) return false;
				string sData = File.ReadAllText(_file);
				foreach(var s in sData.Segments_("\n\r", SegFlags.NoEmpty)) {
					if(_data == null) {
						//first line contains .NET framework version and Au.dll version, like 12345|1.2.3.4
						var versions = s.Value.Split_("|"); if(versions.Length != 2) goto g1;
						int frameworkVersion = versions[0].ToInt_(); string auVersion = versions[1];
						if(frameworkVersion != s_frameworkVersion || auVersion != s_auVersion) { //the s_ are inited by the static ctor
																								 //Print(frameworkVersion, s_frameworkVersion, auVersion, s_auVersion);
							goto g1;
						}
						_data = new Dictionary<string, string>(sData.LineCount_());
						continue;
					}
					const int guidLength = 32; //hex GUID length
					if(s.Length < guidLength) goto g1;
					string s1, s2 = null;
					if(s.Length == guidLength) s1 = s.Value; else { s1 = s.Substring(0, guidLength); s2 = s.Substring(guidLength); }
					_data[s1] = s2;
				}
				if(_data == null) return false; //empty file
				return true;
				g1:
				_ClearCache();
				return false;
			}

			void _Save()
			{
				File_.CreateDirectory(CacheDirectory);
				using(var b = File.CreateText(_file)) {
					b.WriteLine(s_frameworkVersion + "|" + s_auVersion);
					foreach(var v in _data) {
						if(v.Value == null) b.WriteLine(v.Key); else { b.Write(v.Key); b.WriteLine(v.Value); }
					}
					//tested: fast, same speed as StringBuilder+WriteAllText. With b.WriteLine(v.Key+v.Value) same speed or slower.
				}
			}

			void _ClearCache()
			{
				_data = null;
				try { File_.Delete(CacheDirectory); }
				catch(AuException e) { PrintWarning(e.ToString(), -1); }
			}
		}
	}
}
