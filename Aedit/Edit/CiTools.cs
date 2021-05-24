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
//using System.Linq;

using Au;
using Au.Types;
using Au.Util;
using Au.Tools;
using Microsoft.CodeAnalysis.Text;
using System.Windows.Controls;

class CiTools
{
	//#if DEBUG
	//	public static void RegexTest(int position)
	//	{
	//		var node = NodeAt(position);
	//		AOutput.Write(IsInString(ref node, position));
	//	}
	//#endif

	public bool HideTempWindows() {
		bool v1 = _regexWindow?.IsVisible ?? false, v2 = _keysWindow?.IsVisible ?? false;
		if(v1) _regexWindow.Close();
		if(v2) _keysWindow.Close();
		return v1 || v2;
	}

	#region regex

	RegexWindow _regexWindow;
	string _regexTopic;

	public void RegexWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, bool replace, AWnd dontCover = default) {
		int j = stringSpan.Start, vi = _StringPrefixLength(code, j);

		if (!replace && (vi == 0 || !(code[j] == '@' || code[j + 1] == '@')))
			ADialog.ShowInfo(null, "Regular expression string should be like @\"text\", not like \"text\". The Regex tool will not escape \\ when inserting text.");

		_regexWindow ??= new RegexWindow();

		_ShowWindow(_regexWindow, doc, pos16, dontCover);
		var s = _regexWindow.CurrentTopic;
		if (s == "replace") {
			if (!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if (replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _regexWindow.Close());
	}

	//public bool RegexWindowIsVisible => _regexWindow?.Window.Visible ?? false;

	#endregion

	#region keys

	KeysWindow _keysWindow;

	public void KeysWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, AWnd dontCover = default) {
		_keysWindow ??= new KeysWindow();
		_ShowWindow(_keysWindow, doc, pos16, dontCover);
		int vi = _StringPrefixLength(code, stringSpan.Start);
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _keysWindow.Close());
	}

	#endregion

	static int _StringPrefixLength(string s, int j) {
		int R = 0;
		if (s[j] == '@') R = s[j + 1] == '$' ? 2 : 1; else if (s[j] == '$') R = s[j + 1] == '@' ? 2 : 1;
		return R;
	}

	static void _ShowWindow(InfoWindow w, SciCode doc, int position, AWnd dontCover) {
		if (w.IsVisible) w.Hwnd.ZorderTop();
		var r = CiUtil.GetCaretRectFromPos(doc, position, inScreen: true);
		r.left -= ADpi.Scale(80, doc);
		bool above = !dontCover.Is0;
		if (above) r = RECT.Union(r, dontCover.Rect);
		w.ShowByRect(doc, above ? Dock.Top : Dock.Bottom, r, exactSize: true);
		w.InsertInControl = doc;
	}

	public static void CmdShowRegexWindow() => _ShowRegexOrKeysWindow(true);
	public static void CmdShowKeysWindow() => _ShowRegexOrKeysWindow(false);

	static void _ShowRegexOrKeysWindow(bool regex) {
		//bool retry = false;
		//g1:
		if (!CodeInfo.GetDocumentAndFindNode(out var cd, out var node)) return;
		var pos16 = cd.pos16;
		if (!CiUtil.IsInString(ref node, pos16)) {
			//if(regex || retry) {
			ADialog.ShowInfo("The text cursor must be in a string.");
			return;
			//}
			//InsertCode.Statements("AKeys.Key(\"%\");", goToPercent: true); //rejected. Eg could be AKeys.Key("", here).
			//retry = true;
			//goto g1;
		}
		var doc = cd.sciDoc;
		var stringSpan = node.Span;

		var t = CodeInfo._tools;
		if (regex) t.RegexWindowShow(doc, cd.code, pos16, stringSpan, replace: false);
		else t.KeysWindowShow(doc, cd.code, pos16, stringSpan);
	}

	public void ShowForStringParameter(PSFormat stringFormat, CodeInfo.Context cd, TextSpan stringSpan, AWnd dontCover = default) {
		switch (stringFormat) {
		case PSFormat.ARegex:
		case PSFormat.ARegexReplacement:
			RegexWindowShow(cd.sciDoc, cd.code, cd.pos16, stringSpan, replace: stringFormat == PSFormat.ARegexReplacement, dontCover);
			break;
		case PSFormat.AKeys:
			KeysWindowShow(cd.sciDoc, cd.code, cd.pos16, stringSpan, dontCover);
			break;
		}
	}
}
