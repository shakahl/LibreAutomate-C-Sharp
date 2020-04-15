//Show/hide code info tool windows such as Regex and Keys.

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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Tools;
using Au.Controls;
using Microsoft.CodeAnalysis.Text;

class CiTools
{
	//#if DEBUG
	//	public static void RegexTest(int position)
	//	{
	//		var node = NodeAt(position);
	//		AOutput.Write(IsInString(ref node, position));
	//	}
	//#endif

	public void HideTempWindows()
	{
		_regexWindow?.Hide();
		_keysWindow?.Hide();
	}

	#region regex

	RegexWindow _regexWindow;
	string _regexTopic;

	public void RegexWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, bool replace)
	{
		int j = stringSpan.Start, vi = _StringPrefixLength(code, j);

		if(!replace && (vi == 0 || !(code[j] == '@' || code[j + 1] == '@')))
			ADialog.ShowInfo(null, "Regular expression string should be like @\"text\", not like \"text\". The Regex tool will not escape \\ when inserting text.");

		if(_regexWindow == null) {
			_regexWindow = new RegexWindow();
			_regexWindow.Window.Name = "Ci.Regex"; //prevent hiding when activated
		}

		var r = CiUtil.GetCaretRectFromPos(doc, pos16);
		int i = Au.Util.ADpi.ScaleInt(100);
		r.Width = i;
		r.Inflate(i, 0);
		_regexWindow.Show(doc, r, false, PopupAlignment.TPM_CENTERALIGN | PopupAlignment.TPM_VERTICAL);
		_regexWindow.InsertInControl = doc;
		var s = _regexWindow.CurrentTopic;
		if(s == "replace") {
			if(!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if(replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _regexWindow.Hide());
	}

	//public bool RegexWindowIsVisible => _regexWindow?.Window.Visible ?? false;

	#endregion

	static int _StringPrefixLength(string s, int j)
	{
		int R = 0;
		if(s[j] == '@') R = s[j + 1] == '$' ? 2 : 1; else if(s[j] == '$') R = s[j + 1] == '@' ? 2 : 1;
		return R;
	}

	#region keys

	KeysWindow _keysWindow;

	public void KeysWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan)
	{
		if(_keysWindow == null) {
			_keysWindow = new KeysWindow();
			_keysWindow.Window.Name = "Ci.Keys"; //prevent hiding when activated
		}
		var r = CiUtil.GetCaretRectFromPos(doc, pos16);
		int i = Au.Util.ADpi.ScaleInt(100);
		r.Width = i;
		r.Inflate(i, 0);
		_keysWindow.Show(doc, r, false, PopupAlignment.TPM_CENTERALIGN | PopupAlignment.TPM_VERTICAL);
		_keysWindow.InsertInControl = doc;
		int vi = _StringPrefixLength(code, stringSpan.Start);
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _keysWindow.Hide());
	}

	#endregion

	public static void CmdShowRegexWindow() => _ShowRegexOrKeysWindow(true);
	public static void CmdShowKeysWindow() => _ShowRegexOrKeysWindow(false);

	static void _ShowRegexOrKeysWindow(bool regex)
	{
		bool retry = false;
		g1:
		if(!CodeInfo.GetDocumentAndFindNode(out var cd, out var node)) return;
		var pos16 = cd.pos16;
		if(!CiUtil.IsInString(ref node, pos16)) {
			if(regex || retry) {
				ADialog.ShowInfo("The text cursor must be in a string.");
				return;
			}
			InsertCode.Statements("AKeys.Key(\"%\");", goToPercent: true);
			retry = true;
			goto g1;
		}
		var doc = cd.sciDoc;
		var stringSpan = node.Span;

		var t = CodeInfo._tools;
		if(regex) t.RegexWindowShow(doc, cd.code, pos16, stringSpan, replace: false);
		else t.KeysWindowShow(doc, cd.code, pos16, stringSpan);
	}
}
