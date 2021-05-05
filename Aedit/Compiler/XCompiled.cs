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

using Au.Types;
using Au.Util;

namespace Au.Compiler
{
	static partial class Compiler
	{
		/// <summary>
		/// Resolves whether need to [re]compile or can run previously compiled assembly.
		/// </summary>
		unsafe class XCompiled
		{
			readonly FilesModel _model;
			readonly string _file;
			Dictionary<uint, string> _data;

			public string CacheDirectory { get; }

			public static XCompiled OfWorkspace(FilesModel m) {
				var cc = m.CompilerContext;
				if (cc == null) m.CompilerContext = cc = new XCompiled(m);
				return cc as XCompiled;
			}

			public XCompiled(FilesModel m) {
				_model = m;
				CacheDirectory = _model.WorkspaceDirectory + @"\.compiled";
				_file = CacheDirectory + @"\compiled.log";
			}

			/// <summary>
			/// Called before executing script f. If returns true, don't need to compile.
			/// </summary>
			/// <param name="f"></param>
			/// <param name="r">Receives file path and execution options.</param>
			/// <param name="projFolder">Project folder or null. If not null, f must be its main file.</param>
			public bool IsCompiled(FileNode f, out CompResults r, FileNode projFolder) {
				r = new CompResults();

				if (_data == null && !_Open()) return false;

				if (!_data.TryGetValue(f.Id, out string value)) return false;
				//ADebug.Print(value);
				int iPipe = 0;

				bool isScript = f.IsScript;
				r.role = MetaComments.DefaultRole(isScript);

				string asmFile;
				if (r.notInCache = value?.Starts("|=") ?? false) {
					iPipe = value.IndexOf('|', 2); if (iPipe < 0) iPipe = value.Length;
					asmFile = value[2..iPipe];
				} else asmFile = CacheDirectory + "\\" + f.IdString + ".dll";
				//AOutput.Write(asmFile);

				if (!AFile.GetProperties(asmFile, out var asmProp, FAFlags.UseRawPath)) return false;
				DateTime asmTime = asmProp.LastWriteTimeUtc;

				if (_IsFileModified(f)) return false;

				bool isMultiFileProject = false;
				if (value != null && iPipe < value.Length) {
					iPipe++;
					foreach (var v in value.Segments("|", SegFlags.NoEmpty, iPipe..)) {
						int offs = v.start + 1;
						char ch = value[v.start];
						switch (ch) {
						case 't':
							r.role = (ERole)value.ToInt(offs);
							break;
						case 'a':
							r.runSingle = true;
							break;
						case 'n':
							r.ifRunning = (EIfRunning)value.ToInt(offs);
							break;
						case 'N':
							r.ifRunning2 = (EIfRunning2)value.ToInt(offs);
							break;
						case 'u':
							r.uac = (EUac)value.ToInt(offs);
							break;
						case 'b':
							r.bit32 = true;
							break;
						case 'f':
							r.flags = (MiniProgram_.EFlags)value.ToInt(offs);
							break;
						case 'p':
							isMultiFileProject = true;
							if (projFolder != null) {
								if (!AHash.MD5Result.FromString(value.AsSpan(offs, v.end - offs), out var md5)) return false;
								AHash.MD5 md = default;
								foreach (var f1 in projFolder.EnumProjectClassFiles(f)) {
									if (_IsFileModified(f1)) return false;
									md.Add(f1.Id);
								}
								if (md.IsEmpty || md.Hash != md5) return false;
							}
							break;
						case '*':
							var dll = value[offs..v.end];
							if (!APath.IsFullPath(dll)) dll = AFolders.ThisApp + dll;
							if (_IsFileModified2(dll)) return false;
							break;
						case 'l':
						case 'c':
						case 'x':
						case 'k':
						case 'm':
						//case 'y':
						case 's':
							//case 'o':
							value.ToInt(out uint u1, offs);
							var f2 = _model.FindById(u1);
							if (f2 == null) return false;
							if (ch == 'l') {
								if (f2.FindProject(out var projFolder2, out var projMain2)) f2 = projMain2;
								if (f2 == f) return false; //will be compiler error "circular reference"
														   //AOutput.Write(f2, projFolder2);
								if (!IsCompiled(f2, out _, projFolder2)) return false;
								//AOutput.Write("library is compiled");
							} else {
								if (_IsFileModified(f2)) return false;
								//switch(ch) {
								//case 'o': //f2 is the source config file
								//	r.hasConfig = true;
								//	break;
								//}
							}
							break;
						default: return false;
						}
					}
				}
				if (isMultiFileProject != (projFolder != null)) {
					if (projFolder == null) return false;
					foreach (var f1 in projFolder.EnumProjectClassFiles(f)) return false; //project with single file?
				}
				//ADebug.Print("compiled");

				r.file = asmFile;
				r.name = APath.GetNameNoExt(f.Name);
				return true;

				bool _IsFileModified(FileNode f_) => _IsFileModified2(f_.FilePath);

				bool _IsFileModified2(string path_) {
					if (!AFile.GetProperties(path_, out var prop_, FAFlags.UseRawPath)) return true;
					Debug.Assert(!prop_.Attributes.Has(FileAttributes.Directory));
					//AOutput.Write(prop_.LastWriteTimeUtc, asmDate);
					if (prop_.LastWriteTimeUtc > asmTime) return true;
					return false;
				}
			}

			/// <summary>
			/// Called when successfully compiled script f. Saves data that next time will be used by <see cref="IsCompiled"/>.
			/// </summary>
			/// <param name="f"></param>
			/// <param name="outFile">The output assembly.</param>
			/// <param name="m"></param>
			/// <param name="mtaThread">No [STAThread].</param>
			public void AddCompiled(FileNode f, string outFile, MetaComments m, MiniProgram_.EFlags miniFlags) {
				if (_data == null && !_Open()) _data = new();

				/*
	IDmain|=path.exe|tN|aN|nN|NN|uN|fN|b|pMD5project|cIDcode|lIDlibrary|xIDresource|kIDicon|mIDmanifest|yIDres|sIDsign|oIDconfig|*ref
	= - outFile
	t - role
	a - runSingle
	n - ifRunning
	N - ifRunning2
	u - uac
	f - miniFlags
	b - bit32
	p - MD5 of Id of all project files except main
	c - c
	l - pr
	x - resource
	k - icon
	m - manifest
	s - sign
	o - config (now removed)
	* - r

	rejected:
	y - res
				*/

				string value = null;
				using (new StringBuilder_(out var b)) {
					if (m.OutputPath != null) b.Append("|=").Append(outFile); //else f.Id in cache
					if (m.Role != MetaComments.DefaultRole(m.IsScript)) b.Append("|t").Append((int)m.Role);
					if (m.RunSingle) b.Append("|a");
					if (m.IfRunning != default) b.Append("|n").Append((int)m.IfRunning);
					if (m.IfRunning2 != default) b.Append("|N").Append((int)m.IfRunning2);
					if (m.Uac != default) b.Append("|u").Append((int)m.Uac);
					if (miniFlags != default) b.Append("|f").Append((int)miniFlags);
					if (m.Bit32) b.Append("|b");

					int nAll = m.CodeFiles.Count, nNoC = nAll - m.CountC;
					if (nNoC > 1) { //add MD5 hash of project files, except main
						AHash.MD5 md = default;
						for (int i = 1; i < nNoC; i++) md.Add(m.CodeFiles[i].f.Id);
						b.Append("|p").Append(md.Hash.ToString());
					}
					for (int i = nNoC; i < nAll; i++) _AppendFile("|c", m.CodeFiles[i].f); //ids of C# files added through meta 'c'
					if (m.ProjectReferences != null) foreach (var v in m.ProjectReferences) _AppendFile("|l", v); //ids of meta 'pr' files
					if (m.Resources != null) foreach (var v in m.Resources) _AppendFile("|x", v.f); //ids of meta 'resource' files
					_AppendFile("|k", m.IconFile);
					_AppendFile("|m", m.ManifestFile);
					//_AppendFile("|y", m.ResFile);
					_AppendFile("|s", m.SignFile);
					//_AppendFile("|o", m.ConfigFile);

					//references
					var refs = m.References.Refs;
					int j = MetaReferences.DefaultReferences.Count;
					if (refs.Count > j) {
						string appDir = AFolders.ThisAppBS;
						for (; j < refs.Count; j++) {
							var s1 = refs[j].FilePath;
							if (s1.Starts(appDir, true)) s1 = s1[appDir.Length..];
							b.Append("|*").Append(s1);
						}
					}

					if (b.Length != 0) value = b.ToString();

					void _AppendFile(string opt, FileNode f_) {
						if (f_ == null) return;
						if (f_.IsFolder) {
							Debug.Assert(opt is "|x" or "|k");
							foreach (var des in f_.Descendants()) if (!des.IsFolder) b.Append(opt).Append(des.IdString);
						} else {
							b.Append(opt).Append(f_.IdString);
						}
					}
				}

				uint id = f.Id;
				if (_data.TryGetValue(id, out var oldValue) && value == oldValue) { /*ADebug.Print("same");*/ return; }
				//ADebug.Print("different");
				_data[id] = value;
				_Save();
			}

			/// <summary>
			/// Removes saved f data, so that next time <see cref="IsCompiled"/> will return false.
			/// </summary>
			public void Remove(FileNode f, bool deleteAsmFile) {
				if (_data == null && !_Open()) return;
				if (_data.Remove(f.Id)) {
					_Save();
					if (deleteAsmFile) {
						try { AFile.Delete(CacheDirectory + "\\" + f.IdString + ".dll"); }
						catch (Exception ex) { ADebug.Print(ex); }
					}
				}
			}

			bool _Open() {
				if (_data != null) return true;
				if (!AFile.ExistsAsFile(_file)) return false;
				string sData = AFile.LoadText(_file);
				foreach (var v in sData.Segments(SegSep.Line, SegFlags.NoEmpty)) {
					if (_data == null) {
						//first line contains .NET version and Au.dll version, like 5.0.4|1.2.3.4
						if (sData[v.Range] != s_versions) goto g1;
						_data = new(sData.LineCount());
						continue;
					}
					sData.ToInt(out uint id, v.start, out int idEnd);
					Debug.Assert(null != _model.FindById(id));
					_data[id] = v.end > idEnd ? sData[idEnd..v.end] : null;
				}
				if (_data == null) return false; //empty file

				//delete temp files
				foreach (var f in AFile.Enumerate(CacheDirectory, FEFlags.UseRawPath | FEFlags.IgnoreInaccessible)) {
					if (f.Name.Like("*'*")) Api.DeleteFile(f.FullPath);
				}

				return true;
				g1:
				_ClearCache();
				return false;
			}

			static readonly string s_versions = Environment.Version.ToString() + "|" + typeof(AWnd).Assembly.GetName().Version.ToString();

			void _Save() {
				AFile.CreateDirectory(CacheDirectory);
				using var b = AFile.WaitIfLocked(() => File.CreateText(_file));
				b.WriteLine(s_versions);
				foreach (var v in _data) {
					if (v.Value == null) b.WriteLine(v.Key); else { b.Write(v.Key); b.WriteLine(v.Value); }
				}
				//tested: fast, same speed as StringBuilder+WriteAllText. With b.WriteLine(v.Key+v.Value) same speed or slower.
			}

			void _ClearCache() {
				_data = null;
				try { AFile.Delete(CacheDirectory); }
				catch (AuException e) { AWarning.Write(e.ToString(), -1); }
			}
		}

		public static void OnFileDeleted(FilesModel m, FileNode f) {
			XCompiled.OfWorkspace(m).Remove(f, true);
		}
	}
}
