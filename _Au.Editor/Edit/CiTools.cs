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
using System.Drawing;
using System.Windows.Forms;

using Au;
using Au.Types;
using Au.Util;
using Au.Controls;
using Au.Tools;
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

	public void RegexWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, bool replace, Form dontCover = null)
	{
		int j = stringSpan.Start, vi = _StringPrefixLength(code, j);

		if(!replace && (vi == 0 || !(code[j] == '@' || code[j + 1] == '@')))
			ADialog.ShowInfo(null, "Regular expression string should be like @\"text\", not like \"text\". The Regex tool will not escape \\ when inserting text.");

		if(_regexWindow == null) {
			_regexWindow = new RegexWindow(doc);
			_regexWindow.Window.Name = "Ci.Regex"; //prevent hiding when activated
		}

		_ShowWindow(_regexWindow, doc, pos16, dontCover);
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

	#region keys

	KeysWindow _keysWindow;

	public void KeysWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, Form dontCover = null)
	{
		if(_keysWindow == null) {
			_keysWindow = new KeysWindow(doc);
			_keysWindow.Window.Name = "Ci.Keys"; //prevent hiding when activated
		}
		_ShowWindow(_keysWindow, doc, pos16, dontCover);
		int vi = _StringPrefixLength(code, stringSpan.Start);
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _keysWindow.Hide());
	}

	#endregion

	static int _StringPrefixLength(string s, int j)
	{
		int R = 0;
		if(s[j] == '@') R = s[j + 1] == '$' ? 2 : 1; else if(s[j] == '$') R = s[j + 1] == '@' ? 2 : 1;
		return R;
	}

	static void _ShowWindow(InfoWindowF w, SciCode doc, int position, Form dontCover) {
		bool above = dontCover != null;
		if (w.Window.Visible) w.Window.Hwnd().ZorderTop();
		var r = CiUtil.GetCaretRectFromPos(doc, position, inScreen: true);
		int i = ADpi.Scale(100);
		r.Width = i;
		r.Inflate(i, 0);
		if(dontCover != null) {
			r = Rectangle.Union(r, dontCover.Bounds);
		}
		var align = above ? PopupAlignment.TPM_BOTTOMALIGN : 0;
		var anchor = new POINT(r.Left, above ? r.Top : r.Bottom);
		w.Show(doc, r, true, align | PopupAlignment.TPM_VERTICAL, anchor);
		w.InsertInControl = doc;
	}

	public static void CmdShowRegexWindow() => _ShowRegexOrKeysWindow(true);
	public static void CmdShowKeysWindow() => _ShowRegexOrKeysWindow(false);

	static void _ShowRegexOrKeysWindow(bool regex)
	{
		//bool retry = false;
		//g1:
		if(!CodeInfo.GetDocumentAndFindNode(out var cd, out var node)) return;
		var pos16 = cd.pos16;
		if(!CiUtil.IsInString(ref node, pos16)) {
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
		if(regex) t.RegexWindowShow(doc, cd.code, pos16, stringSpan, replace: false);
		else t.KeysWindowShow(doc, cd.code, pos16, stringSpan);
	}

	public void ShowForStringParameter(PSFormat stringFormat, in CodeInfo.Context cd, TextSpan stringSpan, Form dontCover = null) {
		//var alignment = above ? c_popupAlignmentAbove : c_popupAlignmentNormal;
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
