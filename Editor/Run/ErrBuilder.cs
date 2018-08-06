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
	FileNode _f;
	string _code, _link;

	public void SetFile(FileNode f, string code)
	{
		_f = f;
		_code = code;
	}

	/// <summary>
	/// Adds compiler error message with a link that opens the C# file and goes to that position.
	/// </summary>
	public void Add(Diagnostic d) => Add(d.ToString());

	/// <summary>
	/// Adds compiler-generated or other error message.
	/// Adds link to open the C# file. If s starts with "(line,column)", adds link to go to that place in code.
	/// </summary>
	/// <param name="s">
	/// Error message.
	/// If not compiler-generated, should be like "error AU0001: error message". Use other overloads if want to add position info to the link.
	/// </param>
	public void Add(string s)
	{
		_StartAdd();
		if(0 != s.RegexReplace_(@"^\((\d+),(\d+)\)", _link, out s, 1)) _b.Append(s); //compiler-generated
		else _b.AppendFormat("<_open {0}>{1}<>: {2}", _f.GUID, _f.Name, s);
	}

	/// <summary>
	/// Adds error message with a link that opens the C# file and goes to the specified position.
	/// </summary>
	/// <param name="pos">Position in code.</param>
	/// <param name="message">Text to append. Should be like "error AU0001: error message".</param>
	/// <param name="formatArgs">If not null/empty, calls StringBuilder.AppendFormat(message, formatArgs).</param>
	public void Add(int pos, string message, params object[] formatArgs)
	{
		_StartAdd();
		int line = 0, lineStart;
		for(int i = 0; ; i++) {
			lineStart = i;
			i = _code.IndexOf('\n', i); if(i >= pos || i < 0) break;
			line++;
		}
		//Print(line, lineStart, pos-lineStart);
		_Append(line, pos - lineStart, message, formatArgs);
	}

	/// <summary>
	/// Adds error message with a link that opens the C# file and goes to the specified position.
	/// </summary>
	/// <param name="pos">Position in code.</param>
	/// <param name="message">Text to append. Should be like "error AU0001: error message".</param>
	/// <param name="formatArgs">If not null/empty, calls StringBuilder.AppendFormat(message, formatArgs).</param>
	public void Add(Microsoft.CodeAnalysis.Text.LinePosition pos, string message, params object[] formatArgs)
	{
		_StartAdd();
		_Append(pos.Line, pos.Character, message, formatArgs);
	}

	void _StartAdd()
	{
		if(_b == null) {
			_b = new StringBuilder("<>");
			_link = "<_open " + _f.GUID + "|$1|$2>$0<>";
		} else _b.AppendLine();
	}

	void _Append(int line, int col, string message, params object[] formatArgs)
	{
		_b.AppendFormat("<_open {0}|{1}|{2}>({1},{2})<>: ", _f.GUID, ++line, ++col);
		if((formatArgs?.Length ?? 0) != 0) _b.AppendFormat(message, formatArgs); else _b.Append(message);
	}

	public bool IsEmpty => _b == null;

	public override string ToString() => _b?.ToString();
}
