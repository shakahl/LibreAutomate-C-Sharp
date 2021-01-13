﻿using System;
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
using Au.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;

class PanelInfo : Grid
{
	FlowDocumentControl _xaml;
	_Scintilla _sci;

	public PanelInfo() {
		_sci = new _Scintilla { ZInitReadOnlyAlways = true, ZInitTagsStyle = KScintilla.ZTagsStyle.AutoAlways, Visibility = Visibility.Hidden };
		_sci.ZHandleCreated += _sci_ZHandleCreated;
		this.Children.Add(_sci);

		_xaml = CiXaml.CreateControl();
		AutomationProperties.SetName(_xaml.Document, "Info_code");
		_xaml.LinkClicked += _Xaml_LinkClicked;
		this.Children.Add(_xaml);
	}

	class _Scintilla : KScintilla
	{
		protected override string ZAccessibleName => "Info_mouse";
	}

	private void _sci_ZHandleCreated() {
		var z = _sci.Z;
		z.StyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		z.StyleFont(Sci.STYLE_DEFAULT, App.Wmain);
		z.MarginWidth(1, 4);
		z.StyleClearAll();
		//_sci.Call(Sci.SCI_SETHSCROLLBAR);
		_sci.Call(Sci.SCI_SETVSCROLLBAR);

		App.MousePosChangedWhenProgramVisible += _MouseInfo;
		ATimer.After(50, _ => ZSetAboutInfo());
	}

	void _SetVisibleControl(UIElement e) {
		if (e == _sci) {
			_xaml.Visibility = Visibility.Hidden;
			_sci.Visibility = Visibility.Visible;
		} else {
			_sci.Visibility = Visibility.Hidden;
			_xaml.Visibility = Visibility.Visible;
		}
	}

	void _Xaml_LinkClicked(FlowDocumentControl sender, string s) {
		if (s == "?") {
			ZSetAboutInfo(About.Panel);
		} else {
			AFile.TryRun(s);
		}
	}

	public void ZSetAboutInfo(About about = About.Minimal) {
		var x = new CiXaml();
		x.StartParagraph();
		switch (about) {
		case About.Minimal:
			x.StartHyperlink("?", false);
			x.Append(" Foreground=\"#999\">?");
			x.EndHyperlink();
			break;
		case About.Panel:
			x.Append(@"This panel displays quick info about object from mouse position.
In code editor - function, class, etc.
In other windows - x, y, window, control.");
			break;
		//case About.Metacomments: //rejected
		//	x.Append("File properties. Use the Properties dialog to edit.");
		//	break;
		}
		x.EndParagraph();
		ZSetXaml(x.End());
	}

	public enum About {
		Minimal,
		Panel,
		//Metacomments
	}

	public void ZSetXaml(string xaml) {
		if (xaml == _prevXaml && !_isSci) return;
		_prevXaml = xaml;
		_xaml.Document.Blocks.Clear();
		if (!xaml.NE()) _xaml.Document.Blocks.Add(CiXaml.Parse(xaml));

		if (_isSci) {
			_isSci = false;
			_SetVisibleControl(_xaml);
		}
	}
	string _prevXaml;
	bool _isSci;

	//public void ZSetMouseInfoText(string text)
	//{
	//	if(this.InvokeRequired) Dispatcher.InvokeAsync(() => _SetMouseInfoText(text));
	//	else _SetText(text);
	//}

	//void _SetMouseInfoText(string text)
	//{
	//	_sci.Z.SetText(text);
	//}

	void _MouseInfo(POINT p) {
		if (!this.IsVisible) return;
		var c = AWnd.FromXY(p);
		var w = c.Window;
		if (!_isSci) {
			if (w.IsOfThisProcess) return;
			_isSci = true;
			_SetVisibleControl(_sci);
		}
		using (new StringBuilder_(out var b, 1000)) {
			var cn = w.ClassName;
			if (cn != null) {
				var pc = p; w.MapScreenToClient(ref pc);
				b.AppendFormat("<b>xy</b> {0,5} {1,5},   <b>client</b> {2,5} {3,5}\r\n<b>Window</b>  {4}\r\n\t<b>cn</b>  {5}\r\n\t<b>program</b>  {6}",
					p.x, p.y, pc.x, pc.y,
					w.Name?.Escape(200), cn.Escape(), w.ProgramName?.Escape());
				if (c != w) {
					b.AppendFormat("\r\n<b>Control   id</b>  {0}\r\n\t<b>cn</b>  {1}",
						c.ControlId, c.ClassName?.Escape());
					var ct = c.Name;
					if (!ct.NE()) b.Append("\r\n\t<b>name</b>  ").Append(ct.Escape(200));
				} else if (cn == "#32768") {
					var m = AMenuItemInfo.FromXY(p, w, 50);
					if (m != null) {
						b.AppendFormat("\r\n<b>Menu   id</b>  {0}", m.ItemId);
						if (m.IsSystem) b.Append(" (system)");
						//AOutput.Write(m.GetText(true, true));
					}
				}
			}

			_sci.Z.SetText(b.ToString());
		}
	}
}
