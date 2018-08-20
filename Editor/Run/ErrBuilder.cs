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

/// <summary>Code errors text builder.</summary>
class ErrBuilder
{
	StringBuilder _b;
	int _nErrors, _nWarnings;

	/// <summary>
	/// Adds compiler error message with a link that opens the C# file and goes to that position.
	/// </summary>
	public void AddErrorOrWarning(Diagnostic d, FileNode fMain)
	{
		var s = d.ToString();
		if(s.RegexMatch_(@"^(\S{22}) (.+)\((\d+),(\d+)\)(:.+)", out var m)) {
			_StartAdd(isWarning: d.Severity != DiagnosticSeverity.Error);
			var guid = m[1].Value;
			//var name = guid == fMain.Guid ? null : m[2].Value.Limit_(20);
			var name = m[2].Value.Limit_(20);
			_b.AppendFormat("<_open {0}|{2}|{3}>{1}({2},{3})<>{4}", guid, name, m[3].Value, m[4].Value, m[5].Value);
		} else {
			AddError(fMain, s);
		}
	}

	/// <summary>
	/// Adds error message with a link that opens the C# file but does not go to a position.
	/// </summary>
	public void AddError(FileNode f, string s)
	{
		_StartAdd();
		_b.AppendFormat("<_open {0}>{1}<>: {2}", f.Guid, f.Name, s);
	}

	/// <summary>
	/// Adds error message with a link that opens the C# file and goes to the specified position.
	/// </summary>
	/// <param name="f">C# file.</param>
	/// <param name="code">f code.</param>
	/// <param name="pos">Position in code.</param>
	/// <param name="message">Text to append. Example: "error AU0001: error message".</param>
	/// <param name="formatArgs">If not null/empty, calls StringBuilder.AppendFormat(message, formatArgs).</param>
	public void AddError(FileNode f, string code, int pos, string message, params object[] formatArgs)
	{
		_StartAdd();
		int line = 0, lineStart;
		for(int i = 0; ; i++) {
			lineStart = i;
			i = code.IndexOf('\n', i); if(i >= pos || i < 0) break;
			line++;
		}
		_Append(f, line, pos - lineStart, message, formatArgs);
	}

	/// <summary>
	/// Adds error message with a link that opens the C# file and goes to the specified position.
	/// </summary>
	/// <param name="pos">Position in code.</param>
	/// <param name="message">Text to append. Should be like "error AU0001: error message".</param>
	/// <param name="formatArgs">If not null/empty, calls StringBuilder.AppendFormat(message, formatArgs).</param>
	public void AddError(FileNode f, Microsoft.CodeAnalysis.Text.LinePosition pos, string message, params object[] formatArgs) //FUTURE: delete if unused
	{
		_StartAdd();
		_Append(f, pos.Line, pos.Character, message, formatArgs);
	}

	void _StartAdd(bool isWarning = false)
	{
		if(_b == null) _b = new StringBuilder("<>");
		else _b.AppendLine();

		if(isWarning) _nWarnings++; else _nErrors++;
	}

	void _Append(FileNode f, int line, int col, string message, params object[] formatArgs)
	{
		_Append(f.Guid, f.Name, line, col, message, formatArgs);
	}

	void _Append(string guid, string name, int line, int col, string message, params object[] formatArgs)
	{
		_b.AppendFormat("<_open {0}|{2}|{3}>{1}({2},{3})<>: ", guid, name, ++line, ++col);
		if((formatArgs?.Length ?? 0) != 0) _b.AppendFormat(message, formatArgs); else _b.Append(message);
	}

	public bool IsEmpty => _b == null;

	public override string ToString() => _b?.ToString();

	public void PrintAll()
	{
		if(IsEmpty) return;
		var b = new StringBuilder("<><Z ");
		b.Append(_nErrors != 0 ? "#F0E080>" : "#A0E0A0>");
		b.Append("Compilation: ");
		if(_nErrors != 0) b.Append(_nErrors).Append(" errors").Append(_nWarnings != 0 ? ", " : "");
		if(_nWarnings != 0) b.Append(_nWarnings).Append(" warnings");
		b.Append("<>");
		Print(b.ToString());
		Print(ToString());
	}
}
