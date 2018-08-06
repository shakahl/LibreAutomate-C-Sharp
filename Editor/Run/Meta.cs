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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

/// <summary>
/// Extracts meta (settings, references, etc) from comments in C# code.
/// </summary>
class Meta
{
	public string Name { get; private set; }

	public string Code { get; private set; }

	public bool IsScript { get; private set; }

	public bool IsDebug { get; private set; }

	public static bool DefaultIsDebug { get; set; } = true;

	public List<string> Defines { get; private set; }

	public static List<string> DefaultDefines { get; set; } = new List<string> { "TRACE" };

	public int WarningLevel { get; private set; }

	public static int DefaultWarningLevel { get; set; } = 4;

	public List<string> DisableWarnings { get; private set; }

	public static List<string> DefaultDisableWarnings { get; set; }

	public string ConfigFile { get; private set; }

	public static string DefaultConfigFile { get; set; }

	public ErrBuilder Errors { get; private set; }

	public MetaReferences References { get; private set; }

	public List<SyntaxTree> SyntaxTrees { get; private set; }

	public List<FileNode> ClassFiles { get; private set; }

	public List<string> ClassFileCodes { get; private set; }

	public List<string> ResourceFiles { get; private set; }

	//public bool CreatePdbFile { get; private set; } //rejected, create always

	public string PreScript { get; private set; }

	public string PostScript { get; private set; }

	public EIsolation Isolation { get; private set; }

	public EUac Uac { get; private set; }

	public bool Prefer32Bit { get; private set; }

	public ERunAlone RunAlone { get; private set; }

	public int MaxInstances { get; private set; }

	public string NativeIconFile { get; private set; }

	public string ManifestFile { get; private set; }

	public string ResFile { get; private set; }

	public string OutputPath { get; private set; }

	public OutputKind OutputType { get; private set; }


	public enum EIsolation { appDomain, process, thread, hostThread }

	public enum EUac { host, user, admin, uiAccess, low }

	public enum ERunAlone { yes, no, wait }


	public bool Parse(FileNode f)
	{
		Errors = new ErrBuilder();
		if(!_ParseFile(f, true, 0)) return false;
		return Errors.IsEmpty;
	}

	/// <summary>
	/// Extracts meta from a single C# file.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="isMain">If false, it is a file added through meta 'c'.</param>
	public bool _ParseFile(FileNode f, bool isMain, int errPos)
	{
		string code = File.ReadAllText(f.FilePath, Encoding.UTF8);
		if(Empty(code)) return false;

		bool isScript = false;
		string name = f.Name;
		if(name.EndsWith_(".cs", true)) name = name.Remove(name.Length - 3);
		else if(isMain) isScript = true;
		else return _Error(errPos, "must be .cs file, not script");

		if(_isMain = isMain) {
			Name = name;
			Code = code;
			IsScript = isScript;

			IsDebug = DefaultIsDebug;
			Defines = DefaultDefines;
			WarningLevel = DefaultWarningLevel;
			DisableWarnings = DefaultDisableWarnings;
			ConfigFile = DefaultConfigFile;
			OutputType = OutputKind.WindowsApplication;
			References = new MetaReferences();
		} else {
			if(ClassFiles == null) {
				ClassFiles = new List<FileNode>();
				ClassFileCodes = new List<string>();
			}
			ClassFiles.Add(f);
			ClassFileCodes.Add(code);
		}

		_curDir = null;
		_fn = f;
		Errors.SetFile(f, code);

		if(code.Length < 10 || !code.StartsWith_("/* meta")) return true;
		int i = _FindToken(code, 7); if(i == 7) return true;
		int iTo = code.IndexOf_("*/", i);
		if(iTo < 0 || code[iTo - 1] != '\n') return _Error(errPos, "must end with */ at new line");

		while(i < iTo) {
			_ParseLine(code, ref i);
		}

		if(IsDebug && (Defines == null || !Defines.Contains("DEBUG"))) {
			if(Defines == null) Defines = new List<string>();
			Defines.Add("DEBUG");
		}

		return true;
	}

	bool _isMain;
	FileNode _fn;
	string _curDir;

	void _ParseLine(string code, ref int iStart)
	{
		//simply split the line to get key and value
		int i = iStart, iKey = i;
		if(i <= code.Length - 2 && code[i] == '/' && code[i + 1] == '/') { //comments
			iStart = _FindToken(code, _FindNewline(code, i));
			return;
		}
		while(i < code.Length && code[i] > ' ') i++;
		string key = code.Substring(iStart, i - iStart);
		for(; i < code.Length; i++) { char c = code[i]; if(c > ' ' || c == '\r' || c == '\n') break; }
		int iValue = i;
		i = _FindNewline(code, iValue);
		string value = code.Substring(iValue, i - iValue);
		iStart = _FindToken(code, i);
		if(key.Length == 0) return;
		//Print($"'{key}'  '{value}'");

		if(!_isMain) {
			switch(key) {
			case "r": case "c": case "resource": break;
			default:
				_Error(iKey, "files added through 'c' can use only 'r', 'c' and 'resource'");
				return;
			}
		}

		switch(key) {
		case "r":
			try {
				if(!References.Resolve(value)) {
					_Error(iValue, "reference assembly not found: " + value); //FUTURE: need more info, or link to Help
				}
			}
			catch(Exception e) {
				_Error(iValue, "exception: " + e.Message); //unlikely. If bad format, will be error later, without position info.
			}
			break;
		case "c":
			break;
		case "resource":
			string resPath = _GetFullPath(value, iValue);
			if(ResourceFiles == null) ResourceFiles = new List<string>();
			ResourceFiles.Add(resPath);
			break;
		case "debug":
			if(_TrueFalse(out bool isDebug, value, iValue)) IsDebug = isDebug;
			break;
		case "warningLevel":
			int wl = value.ToInt_();
			if(wl >= 0 && wl <= 4) WarningLevel = wl;
			else _Error(iValue, "must be 0 - 4");
			break;
		case "disableWarnings":
			if(DisableWarnings == null) DisableWarnings = new List<string>();
			else if(DisableWarnings == DefaultDisableWarnings) DisableWarnings = new List<string>(DefaultDisableWarnings);
			DisableWarnings.AddRange(value.Split_(", ", SegFlags.NoEmpty));
			break;
		case "define":
			if(Defines == null) Defines = new List<string>();
			else if(Defines == DefaultDefines) Defines = new List<string>(DefaultDefines);
			Defines.AddRange(value.Split_(", ", SegFlags.NoEmpty));
			break;
		case "preScript":
			PreScript = value;
			break;
		case "postScript":
			PostScript = value;
			break;
		case "isolation":
			if(Enum.TryParse(value, out EIsolation isolation)) Isolation = isolation;
			else _Error(iValue, "must be appDomain, process, thread or hostThread");
			break;
		case "uac":
			if(Enum.TryParse(value, out EUac uac)) Uac = uac;
			else _Error(iValue, "must be host, user, admin, uiAccess or low");
			break;
		case "32bit":
			if(_TrueFalse(out bool is32, value, iValue)) Prefer32Bit = is32;
			break;
		case "runAlone":
			if(Enum.TryParse(value, out ERunAlone runAlone)) RunAlone = runAlone;
			else _Error(iValue, "must be yes, no or wait");
			break;
		case "maxInstances":
			MaxInstances = value.ToInt_();
			break;
		case "config":
			ConfigFile = _GetFullPath(value, iValue);
			break;
		case "options":
			break;
		case "icon":
			if(ResFile != null) _Error(iKey, "cannot add both res file and icon");
			else if(NativeIconFile != null) _Error(iKey, "cannot add multiple icons");
			else NativeIconFile = _GetFullPath(value, iValue);
			break;
		case "manifest":
			if(ResFile != null) _Error(iKey, "cannot add both res file and manifest");
			else if(ManifestFile != null) _Error(iKey, "cannot add multiple manifests");
			else ManifestFile = _GetFullPath(value, iValue);
			break;
		case "resFile":
			if(NativeIconFile != null) _Error(iKey, "cannot add both res file and icon");
			else if(ManifestFile != null) _Error(iKey, "cannot add both res file and manifest");
			else if(ResFile != null) _Error(iKey, "cannot add multiple res files");
			else ResFile = _GetFullPath(value, iValue);
			break;
		case "outputType":
			switch(value) {
			case "app": OutputType = OutputKind.WindowsApplication;break;
			case "console": OutputType = OutputKind.ConsoleApplication; break;
			case "dll": OutputType = OutputKind.DynamicallyLinkedLibrary; break;
			default: _Error(iValue, "must be app, console or dll"); break;
			}
			break;
		case "outputPath":
			OutputPath = _GetFullPath(value, iValue, true); //and creates directory if need
			break;
		//case "targetFramework": //cannot set it easily. Need to use references of that framework. But a typical PC has single version installed.
		//	break;
		//case "version": //how to add it to resources? Maybe use [assembly: attributes]?
		//	break;
		case "sign":
			break;
		case "xmlDoc":
			break;
		default:
			_Error(iKey, "unknown meta");
			break;
		}
	}

	static int _FindToken(string s, int i)
	{
		for(; i < s.Length; i++) if(s[i] > ' ') break;
		return i;
	}

	static int _FindNewline(string s, int i)
	{
		for(; i < s.Length; i++) { char c = s[i]; if(c == '\r' || c == '\n') break; }
		return i;
	}

	bool _Error(int pos, string s)
	{
		Errors.Add(pos, "error in meta: " + s);
		return false;
	}

	bool _TrueFalse(out bool b, string s, int errPos)
	{
		b = false;
		switch(s) {
		case "true": b = true; break;
		case "false": break;
		default: return _Error(errPos, "must be true or false");
		}
		return true;
	}

	string _GetFullPath(string s, int errPos, bool isDirectory = false)
	{
		if(Empty(s)) return s;
		if(!Path_.IsFullPathExpandEnvVar(ref s)) {
			if(s[0] == '\\') s = _fn.Model.FilesDirectory + s;
			else s = (_curDir ?? (_curDir = Path_.GetDirectoryPath(_fn.FilePath, true))) + s;
		}
		s = Path_.LibNormalize(s, noExpandEV: true);

		if(isDirectory) Files.CreateDirectory(s);
		else if(!Files.ExistsAsFile(s, true)) { _Error(errPos, "file does not exist: " + s); return null; }
		return s;
	}
}
