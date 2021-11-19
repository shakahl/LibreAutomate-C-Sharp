using Au.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;

class PanelMouse : Grid
{
	KScintilla _sci;

	public PanelMouse() {
		//this.UiaSetName("Mouse panel"); //no UIA element for Panel. Use this in the future if this panel will be : UserControl.

		_sci = new KScintilla {
			ZInitReadOnlyAlways = true,
			ZInitTagsStyle = KScintilla.ZTagsStyle.AutoAlways,
			Name = "Mouse_info"
		};
		_sci.ZHandleCreated += _sci_ZHandleCreated;
		this.Children.Add(_sci);
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
	}

	//public void ZSetMouseInfoText(string text)
	//{
	//	if(Dispatcher.Thread == Thread.CurrentThread) _SetMouseInfoText(text);
	//	else Dispatcher.InvokeAsync(() => _SetMouseInfoText(text));
	//
	//	void _SetMouseInfoText(string text) { _sci.zSetText(text); }
	//}

	void _MouseInfo(POINT p) {
		if (!this.IsVisible) return;
		var c = wnd.fromXY(p);
		var w = c.Window;
		using (new StringBuilder_(out var b, 1000)) {
			var cn = w.ClassName;
			if (cn != null) {
				var pc = p; w.MapScreenToClient(ref pc);
				b.AppendFormat("<b>xy</b> {0,5} {1,5}  .  <b>window xy</b> {2,5} {3,5}  .  <b>program</b>  {4}",
					p.x, p.y, pc.x, pc.y, w.ProgramName?.Escape());
				if (c.UacAccessDenied) b.Append(" <c red>(admin)<>");
				b.Append("\r\n<b>Window   ");
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

				//rejected. Makes this func 5 times slower.
				//var color = uiimage.getPixel(p);
			}
			_sci.zSetText(b.ToString());
		}
	}
}
