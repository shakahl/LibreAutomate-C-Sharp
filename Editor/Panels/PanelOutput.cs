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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;
using static Au.Controls.Sci;

class PanelOutput : Control
{
	SciOutput _c;
	Queue<Au.Util.OutputServer.Message> _history;
	StringBuilder _sb;

	//public SciControl Output => _c;

	public PanelOutput()
	{
		_c = new SciOutput();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Output";
		this.Controls.Add(_c);

		_history = new Queue<Au.Util.OutputServer.Message>();
		OutputServer.SetNotifications(_GetServerMessages, this);

		_c.HandleCreated += _c_HandleCreated;
	}

	void _GetServerMessages()
	{
		_c.Tags.OutputServerProcessMessages(OutputServer, m => {
			if(m.Type != Au.Util.OutputServer.MessageType.Write) return;

			//create links in compilation errors/warnings or run-time stack trace
			var s = m.Text; int i;
			if(s.Length >= 22) {
				if(s.StartsWith_("<><Z #") && s.EqualsAt_(12, ">Compilation: ")) { //compilation
					if(s_rx1 == null) s_rx1 = new Regex_(@"(?m)^\[(.+?)(\((\d+),(\d+)\))?\]: ");
					m.Text = s_rx1.Replace(s, x => {
						var f = Model.FindByFilePath(x[1].Value);
						if(f == null) return x[0].Value;
						return $"<open \"{f.IdStringWithWorkspace}|{x[3].Value}|{x[4].Value}\">{f.Name}{x[2].Value}<>: ";
					});
				} else if((i = s.IndexOf("\n   at ") + 1) > 0 && s.IndexOf(":line ", i) > 0) { //stack trace with source file info
					int j = s.LastIndexOf("\r\n   at Script.Main(String[] args) in ");
					if(j >= 0) j = s.IndexOf_("\r\n", j + 30);
					if(j < 0) { j = s.Length; if(s.EndsWith_("\r\n")) j -= 2; } //include the Main line, because _Main may be missing
					int stackLen = j - i;
					if(_sb == null) _sb = new StringBuilder(s.Length + 2000); else _sb.Clear();
					var b = _sb;
					//Output.LibWriteQM2("'" + s + "'");
					if(!s.StartsWith_("<>")) b.Append("<>");
					b.Append(s, 0, i);
					var rx = s_rx2; if(rx == null) s_rx2 = rx = new Regex_(@" in (.+?):line (?=\d+$)");
					var rxm = new RXMore();
					bool replaced = false;
					foreach(var k in s.Segments_(i, stackLen, "\r\n", SegFlags.NoEmpty)) {
						//Output.LibWriteQM2("'"+k+"'");
						rxm.start = k.Offset + 6; rxm.end = k.EndOffset;
						if(k.StartsWith("   at ") && rx.MatchG(s, out var g, 1, rxm)) { //note: no "   at " if this is an inner exception marker
							var f = Model.FindByFilePath(g.Value);
							if(f != null) {
								int i1 = g.EndIndex + 6, len1 = k.EndOffset - i1;
								b.Append("   at ").
								Append("<open \"").Append(f.IdStringWithWorkspace).Append('|').Append(s, i1, len1).Append("\">")
								.Append("line ").Append(s, i1, len1).Append("<> in <z 0xFAFAD2>").Append(f.Name).Append("<>");

								bool isMain = k.StartsWith("   at Script._Main(String[] args) in ");
								if(!isMain || !f.IsScript) b.Append(", <_>").Append(s, k.Offset + 6, g.Index - k.Offset - 10).Append("</_>");
								b.AppendLine();

								replaced = true;
								if(isMain) break;
							}
						}
					}
					if(replaced) {
						b.Append("   <fold>   --- Raw stack trace ---\r\n<_>").Append(s, i, stackLen).Append("</_></fold>");
						m.Text = b.ToString();
					}
					if(_sb.Capacity > 10_000) _sb = null; //let GC free it. Usually < 4000.
				}
			}

			if(s.Length <= 10_000) { //* 50 = 1 MB
				if(!ReferenceEquals(s, m.Text)) m = new Au.Util.OutputServer.Message(Au.Util.OutputServer.MessageType.Write, s, m.TimeUtc, m.Caller);
				_history.Enqueue(m);
				if(_history.Count > 50) _history.Dequeue();
			}
		});
	}
	static Regex_ s_rx1, s_rx2;

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void Clear() { _c.ST.ClearText(); }

	public void Copy() { _c.Call(SCI_COPY); }

	public void History()
	{
		var dd = new PopupList { Items = _history.ToArray() };
		dd.SelectedAction = o => Print((o.ResultItem as Au.Util.OutputServer.Message).Text);
		dd.Show(new Rectangle(Mouse.XY, default));
	}

	//not override void OnHandleCreated, because then _c handle still not created, and we need to Call //TODO
	private void _c_HandleCreated(object sender, EventArgs e)
	{
		var h = _c.Handle;
		_inInitSettings = true;
		if(WrapLines) WrapLines = true;
		if(WhiteSpace) WhiteSpace = true;
		if(Topmost) Strips.CheckCmd("Tools_Output_Topmost", true); //see also OnParentChanged, below
		_inInitSettings = false;
	}
	bool _inInitSettings;

	public bool WrapLines {
		get => Settings.Get("Tools_Output_WrapLines", false);
		set {
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Settings.Set("Tools_Output_WrapLines", value);
			//_c.Call(SCI_SETWRAPVISUALFLAGS, SC_WRAPVISUALFLAG_START | SC_WRAPVISUALFLAG_END); //in SciControl.OnHandleCreated
			//_c.Call(SCI_SETWRAPINDENTMODE, SC_WRAPINDENT_INDENT); //in SciControl.OnHandleCreated
			_c.Call(SCI_SETWRAPMODE, value ? SC_WRAP_WORD : 0);
			Strips.CheckCmd("Tools_Output_WrapLines", value);
		}
	}

	public bool WhiteSpace {
		get => Settings.Get("Tools_Output_WhiteSpace", false);
		set {
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Settings.Set("Tools_Output_WhiteSpace", value);
			_c.Call(SCI_SETWHITESPACEFORE, 1, 0xFF0080);
			_c.Call(SCI_SETVIEWWS, value);
			Strips.CheckCmd("Tools_Output_WhiteSpace", value);
		}
	}

	public bool Topmost {
		get => Settings.Get("Tools_Output_Topmost", false);
		set {
			var p = Panels.PanelManager.GetPanel(this);
			//if(value) p.Floating = true;
			if(p.Floating) _SetTopmost(value);
			Settings.Set("Tools_Output_Topmost", value);
			Strips.CheckCmd("Tools_Output_Topmost", value);
		}
	}

	void _SetTopmost(bool on)
	{
		var w = ((Wnd)this).Window;
		if(on) {
			w.Owner = default;
			w.ZorderTopmost();
			//w.SetExStyle(WS_EX.APPWINDOW, SetAddRemove.Add);
			//Wnd.GetWnd.Root.ActivateLL(); w.ActivateLL(); //let taskbar add button
		} else {
			w.ZorderNoTopmost();
			w.Owner = (Wnd)MainForm;
		}
	}

	protected override void OnParentChanged(EventArgs e)
	{
		if(Parent is Form && Topmost) Timer_.After(1, () => _SetTopmost(true));

		base.OnParentChanged(e);
	}

	class SciOutput : AuScintilla
	{
		public SciOutput()
		{
			InitReadOnlyAlways = true;
			InitTagsStyle = TagsStyle.AutoWithPrefix;
			InitImagesStyle = ImagesStyle.ImageTag;

			SciTags.AddCommonLinkTag("open", s => _OpenLink(s));
			SciTags.AddCommonLinkTag("script", s => _RunScript(s));
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			ST.MarginWidth(1, 3);
			ST.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
			ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			ST.StyleClearAll();
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_MBUTTONDOWN:
				ST.ClearText();
				return;
			case Api.WM_CONTEXTMENU:
				Strips.ddOutput.ShowAsContextMenu_((int)m.LParam == -1);
				return;
			}
			base.WndProc(ref m);
		}

		void _OpenLink(string s)
		{
			//Print(s);
			var a = s.Split('|');
			var fn = Model.Find(a[0], false);
			if(fn == null || !Model.SetCurrentFile(fn)) return;
			var doc = Panels.Editor.ActiveDoc;
			doc.Focus();
			if(a.Length == 1) return;
			int line = a[1].ToInt_(0) - 1; if(line < 0) return;
			int column = a.Length == 2 ? -1 : a[2].ToInt_() - 1;

			var t = doc.ST;
			int i = t.LineStart(line);
			if(column > 0) i = t.Call(SCI_POSITIONRELATIVE, i, column); //not SCI_FINDCOLUMN, it calculates tabs
			t.GoToPos(i);
		}

		void _RunScript(string s)
		{
			var a = s.Split('|');
			var f = Model.Find(a[0], false); if(f == null) return;
			Run.CompileAndRun(true, f, a.Length == 1 ? null : a.RemoveAt_(0));
		}
	}
}
