using Au.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;

class PanelInfo : Grid
{
	FlowDocumentControl _flowDoc;
	KScintilla _sci;

	public PanelInfo() {
		_sci = new KScintilla { ZInitReadOnlyAlways = true, ZInitTagsStyle = KScintilla.ZTagsStyle.AutoAlways, Visibility = Visibility.Hidden, Name = "Info_mouse" };
		_sci.ZHandleCreated += _sci_ZHandleCreated;
		this.Children.Add(_sci);

		_flowDoc = CiText.CreateControl();
		AutomationProperties.SetName(_flowDoc.Document, "Info_code");
		_flowDoc.LinkClicked += _flowDoc_LinkClicked;
		this.Children.Add(_flowDoc);
	}

	private void _sci_ZHandleCreated() {
		_sci.zStyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		_sci.zStyleFont(Sci.STYLE_DEFAULT, App.Wmain);
		_sci.zSetMarginWidth(1, 4);
		_sci.zStyleClearAll();
		_sci.Call(Sci.SCI_SETHSCROLLBAR);
		_sci.Call(Sci.SCI_SETVSCROLLBAR);
		_sci.Call(Sci.SCI_SETWRAPMODE, Sci.SC_WRAP_WORD);

		App.MousePosChangedWhenProgramVisible += _MouseInfo;
		timerm.after(50, _ => ZSetAboutInfo());
	}

	void _SetVisibleControl(UIElement e) {
		if (e == _sci) {
			_flowDoc.Visibility = Visibility.Hidden;
			_sci.Visibility = Visibility.Visible;
		} else {
			_sci.Visibility = Visibility.Hidden;
			_flowDoc.Visibility = Visibility.Visible;
		}
	}

	void _flowDoc_LinkClicked(FlowDocumentControl sender, string s) {
		if (s == "?") {
			ZSetAboutInfo(About.Panel);
		} else {
			run.itSafe(s);
		}
	}

	public void ZSetAboutInfo(About about = About.Minimal) {
		var x = new CiText();
		x.StartParagraph();
		switch (about) {
		case About.Minimal:
			x.Hyperlink("?", "?").Foreground = ColorInt.WpfBrush_(0x999999);
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
		ZSetText(x.Result);
	}

	public enum About {
		Minimal,
		Panel,
		//Metacomments
	}

	public void ZSetText(System.Windows.Documents.Section text) {
		if (!_isSci && text == _prevText) return;
		_prevText = text;
		_flowDoc.Clear();
		if (text!=null) _flowDoc.Document.Blocks.Add(text);

		if (_isSci) {
			_isSci = false;
			_SetVisibleControl(_flowDoc);
		}
	}
	System.Windows.Documents.Section _prevText;
	bool _isSci;

	//public void ZSetMouseInfoText(string text)
	//{
	//	if(this.InvokeRequired) Dispatcher.InvokeAsync(() => _SetMouseInfoText(text));
	//	else _SetText(text);
	//}

	//void _SetMouseInfoText(string text)
	//{
	//	_sci.zSetText(text);
	//}

	void _MouseInfo(POINT p) {
		if (!this.IsVisible) return;
		var c = wnd.fromXY(p);
		var w = c.Window;
		if (!_isSci) {
			if (w.IsOfThisProcess) return;
			_isSci = true;
			_SetVisibleControl(_sci);
		}
		using (new StringBuilder_(out var b, 1000)) {
			var cn = w.ClassName;
			if (cn != null) {
#if true
				var pc = p; w.MapScreenToClient(ref pc);
				b.AppendFormat("<b>xy</b> {0,5} {1,5}  .  <b>window xy</b> {2,5} {3,5}  .  <b>program</b>  {4}\r\n<b>Window   ",
					p.x, p.y, pc.x, pc.y, w.ProgramName?.Escape());
				var name = w.Name?.Escape(200); if (!name.NE()) b.AppendFormat("name</b>  {0}  .  <b>", name);
				b.Append("cn</b>  ").Append(cn.Escape());
				if (c != w) {
					b.AppendFormat("\r\n<b>Control   id</b>  {0}  .  <b>cn</b>  {1}",
						c.ControlId, c.ClassName?.Escape());
					var ct = c.Name;
					if (!ct.NE()) b.Append("  .  <b>name</b>  ").Append(ct.Escape(200));
				} else if (cn == "#32768") {
					var m = MenuItemInfo.FromXY(p, w, 50);
					if (m != null) {
						b.AppendFormat("\r\n<b>Menu   id</b>  {0}", m.ItemId);
						if (m.IsSystem) b.Append(" (system)");
						//print.it(m.GetText(true, true));
					}
				}
#else //old version. Too many lines; part invisible if control height is smaller.
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
					var m = MenuItemInfo.FromXY(p, w, 50);
					if (m != null) {
						b.AppendFormat("\r\n<b>Menu   id</b>  {0}", m.ItemId);
						if (m.IsSystem) b.Append(" (system)");
						//print.it(m.GetText(true, true));
					}
				}
#endif
			}

			_sci.zSetText(b.ToString());
		}
	}
}
