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
using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Compiler;

class EdMetaCommentsParser
{
	FileNode _fn;
	public string role, runMode, ifRunning, uac, prefer32bit, config,
		optimize, warningLevel, noWarnings, define, preBuild, postBuild,
		outputPath, console, icon, manifest, resFile, sign, xmlDoc;
	List<string> _r, _pr, _c, _resource, _com;

	public List<string> r => _r ?? (_r = new List<string>());
	public List<string> pr => _pr ?? (_pr = new List<string>());
	public List<string> c => _c ?? (_c = new List<string>());
	public List<string> resource => _resource ?? (_resource = new List<string>());
	public List<string> com => _com ?? (_com = new List<string>());

	public EdMetaCommentsParser(FileNode f) : this(f.GetText()) { _fn = f; }

	public EdMetaCommentsParser(string code)
	{
		if(!MetaComments.FindMetaComments(code, out int endOf)) return;
		EndOfMetaComments = endOf;
		foreach(var t in MetaComments.EnumOptions(code, endOf)) _ParseOption(t.Name(), t.Value());
	}

	public int EndOfMetaComments { get; private set; }

	void _ParseOption(string name, string value)
	{
		switch(name) {
		case "role": role = value; break;
		case "outputPath": outputPath = value; break;
		case "runMode": runMode = value; break;
		case "ifRunning": ifRunning = value; break;
		case "uac": uac = value; break;
		case "prefer32bit": prefer32bit = value; break;
		case "config": config = value; break;
		case "optimize": optimize = value; break;
		case "warningLevel": warningLevel = value; break;
		case "noWarnings": noWarnings = value; break;
		case "define": define = value; break;
		case "preBuild": preBuild = value; break;
		case "postBuild": postBuild = value; break;
		case "console": console = value; break;
		case "icon": icon = value; break;
		case "manifest": manifest = value; break;
		case "resFile": resFile = value; break;
		case "sign": sign = value; break;
		case "xmlDoc": xmlDoc = value; break;
		case "r": r.Add(value); break;
		case "com": com.Add(value); break;
		case "pr": pr.Add(value); break;
		case "c": c.Add(value); break;
		case "resource": resource.Add(value); break;
		}
	}

	/// <summary>
	/// Formats metacomments string "/*/ ... */".
	/// Returns "" if there are no options.
	/// </summary>
	public string Format(string append)
	{
		//prepare to make relative paths
		string dir = null;
		if(_fn != null) {
			dir = _fn.ItemPath;
			int i = dir.LastIndexOf('\\') + 1;
			if(i > 1) dir = dir.Remove(i); else dir = null;
		}

		var b = new StringBuilder("/*/ ");
		_Append("role", role);
		_Append("outputPath", outputPath);
		_Append("runMode", runMode);
		_Append("ifRunning", ifRunning);
		_Append("uac", uac);
		_Append("prefer32bit", prefer32bit);
		_Append("config", config, true);
		_Append("optimize", optimize);
		_Append("warningLevel", warningLevel);
		_Append("noWarnings", noWarnings);
		_Append("define", define);
		_Append("preBuild", preBuild, true);
		_Append("postBuild", postBuild, true);
		_Append("console", console);
		_Append("icon", icon, true);
		_Append("manifest", manifest, true);
		_Append("resFile", resFile, true);
		_Append("sign", sign, true);
		_Append("xmlDoc", xmlDoc);
		_AppendList("r", _r);
		_AppendList("com", _com, true);
		_AppendList("pr", _pr);
		_AppendList("c", _c, true);
		_AppendList("resource", _resource, true);
		if(b.Length == 4) return "";
		b.Append("*/");
		if(append != null) b.Append(append);
		return b.ToString();

		void _Append(string name, string value, bool relativePath = false)
		{
			if(value != null) {
				if(relativePath && dir != null && value.StartsWithI_(dir)) value = value.Substring(dir.Length);
				b.Append(name).Append(' ').Append(value).Append("; ");
			}
		}

		void _AppendList(string name, List<string> a, bool relativePath = false)
		{
			if(a != null) foreach(var v in a.Distinct()) _Append(name, v, relativePath);
		}
	}
}
