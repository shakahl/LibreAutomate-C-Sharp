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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;
using static Au.Controls.Sci;

class PanelOutput : AuUserControlBase
{
	_SciOutput _c;
	Queue<OutServMessage> _history;

	public AuScintilla ZOutput => _c;

	public PanelOutput()
	{
		this.AccessibleName = this.Name = "Output";
		_c = new _SciOutput(this);
		_c.AccessibleName = _c.Name = "Output_text";
		_c.Dock = DockStyle.Fill;
		this.Controls.Add(_c);

		_history = new Queue<OutServMessage>();
	}

	//protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void ZClear() { _c.Z.ClearText(); }

	public void ZCopy() { _c.Call(SCI_COPY); }

	public void ZFind() { Panels.Find.ZCtrlF(_c); }

	public void ZHistory()
	{
		var dd = new PopupList { Items = _history.ToArray() };
		dd.SelectedAction = o => AOutput.Write((o.ResultItem as OutServMessage).Text);
		dd.Show(new Rectangle(AMouse.XY, default));
	}

	private void _c_HandleCreated()
	{
		_inInitSettings = true;
		if(ZWrapLines) ZWrapLines = true;
		if(ZWhiteSpace) ZWhiteSpace = true;
		if(ZTopmost) Strips.CheckCmd("Tools_Output_Topmost", true); //see also OnParentChanged, below
		_inInitSettings = false;
	}
	bool _inInitSettings;

	public bool ZWrapLines {
		get => Program.Settings.output_wrap;
		set {
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Program.Settings.output_wrap = value;
			//_c.Call(SCI_SETWRAPVISUALFLAGS, SC_WRAPVISUALFLAG_START | SC_WRAPVISUALFLAG_END); //in SciControl.OnHandleCreated
			//_c.Call(SCI_SETWRAPINDENTMODE, SC_WRAPINDENT_INDENT); //in SciControl.OnHandleCreated
			_c.Call(SCI_SETWRAPMODE, value ? SC_WRAP_WORD : 0);
			Strips.CheckCmd("Tools_Output_WrapLines", value);
		}
	}

	public bool ZWhiteSpace {
		get => Program.Settings.output_white;
		set {
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Program.Settings.output_white = value;
			_c.Call(SCI_SETWHITESPACEFORE, 1, 0xFF0080);
			_c.Call(SCI_SETVIEWWS, value);
			Strips.CheckCmd("Tools_Output_WhiteSpace", value);
		}
	}

	public bool ZTopmost {
		get => Program.Settings.output_topmost;
		set {
			var p = Panels.PanelManager.ZGetPanel(this);
			//if(value) p.Floating = true;
			if(p.Floating) _SetTopmost(value);
			Program.Settings.output_topmost = value;
			Strips.CheckCmd("Tools_Output_Topmost", value);
		}
	}

	void _SetTopmost(bool on)
	{
		var w = this.Hwnd().Window;
		if(on) {
			w.OwnerWindow = default;
			w.ZorderTopmost();
			//w.SetExStyle(WS2.APPWINDOW, SetAddRemove.Add);
			//AWnd.GetWnd.Root.ActivateLL(); w.ActivateLL(); //let taskbar add button
		} else {
			w.ZorderNoTopmost();
			w.OwnerWindow = Program.MainForm.Hwnd();
		}
	}

	protected override void OnParentChanged(EventArgs e)
	{
		if(Parent is Form && ZTopmost) ATimer.After(1, _ => _SetTopmost(true));

		base.OnParentChanged(e);
	}

	class _SciOutput : AuScintilla
	{
		PanelOutput _p;
		StringBuilder _sb;

		public _SciOutput(PanelOutput panel)
		{
			_p = panel;

			ZInitReadOnlyAlways = true;
			ZInitTagsStyle = ZTagsStyle.AutoWithPrefix;
			ZInitImagesStyle = ZImagesStyle.ImageTag;

			SciTagsF.AddCommonLinkTag("open", s => _OpenLink(s));
			SciTagsF.AddCommonLinkTag("script", s => _RunScript(s));
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			_p._c_HandleCreated();
			Z.MarginWidth(1, 3);
			Z.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
			//Z.StyleFont(STYLE_DEFAULT, "Courier New", 8); //maybe better, except <b>
			Z.StyleFont(STYLE_DEFAULT, "Consolas", 9);
			Z.StyleClearAll();

			ZTags.AddLinkTag("+properties", fid => {
				var f = Program.Model.FindScript(fid);
				if(f == null || !Program.Model.SetCurrentFile(f)) return;
				Strips.Cmd.File_Properties();
			});

			Program.OutputServer.SetNotifications(this.Hwnd(), Api.WM_APP);
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_MBUTTONDOWN:
				Z.ClearText();
				return;
			case Api.WM_CONTEXTMENU:
				Strips.ddOutput.ZShowAsContextMenu((int)m.LParam == -1);
				return;
			case Api.WM_APP:
				ZTags.OutputServerProcessMessages(Program.OutputServer, _onServerMessage ??= _OnServerMessage);
				break;
			}
			base.WndProc(ref m);
		}

		Action<OutServMessage> _onServerMessage;
		void _OnServerMessage(OutServMessage m) {
			if (m.Type != OutServMessageType.Write) return;

			//create links in compilation errors/warnings or run-time stack trace
			var s = m.Text; int i;
			if (s.Length >= 22) {
				if (s.Starts("<><Z #") && s.Eq(12, ">Compilation: ")) { //compilation
					s_rx1 ??= new ARegex(@"(?m)^\[(.+?)(\((\d+),(\d+)\))?\]: ");
					m.Text = s_rx1.Replace(s, x => {
						var f = Program.Model.FindByFilePath(x[1].Value);
						if (f == null) return x[0].Value;
						return $"<open \"{f.IdStringWithWorkspace}|{x[3].Value}|{x[4].Value}\">{f.Name}{x[2].Value}<>: ";
					});
				} else if ((i = s.Find("\n   at ") + 1) > 0 && s.Find(":line ", i) > 0) { //stack trace with source file info
					var b = _sb ??= new StringBuilder(s.Length + 2000);
					b.Clear();
					//AOutput.QM2.Write("'" + s + "'");
					int iLiteral = 0;
					if (!s.Starts("<>")) b.Append("<>");
					else {
						iLiteral = i - 1; if (s[iLiteral - 1] == '\r') iLiteral--;
						if (0 == s.Eq(iLiteral -= 3, false, "<_>", "<\a>")) iLiteral = 0;
					}
					if (iLiteral > 0) b.Append(s, 0, iLiteral).AppendLine(); else b.Append(s, 0, i);
					s_rx2 ??= new ARegex(@" in (.+?):line (?=\d+$)");
					bool replaced = false, isMain = false;
					int stackEnd = s.Length/*, stackEnd2 = 0*/;
					foreach (var k in s.Segments(SegSep.Line, range: i..)) {
						//AOutput.QM2.Write("'"+k+"'");
						if (s.Eq(k.start, "   at ")) {
							if (isMain) {
								//if(stackEnd2 == 0 && s.Eq(k.start, "   at Script.Main(String[] args) in ")) stackEnd2 = k.start; //rejected. In some cases may cut something important.
								continue;
							}
							if (!s_rx2.MatchG(s, out var g, 1, (k.start + 6)..k.end)) continue; //note: no "   at " if this is an inner exception marker. Also in aggregate exception stack trace.
							var f = Program.Model.FindByFilePath(g.Value); if (f == null) continue;
							int i1 = g.End + 6, len1 = k.end - i1;
							b.Append("   at ")
							.Append("<open \"").Append(f.IdStringWithWorkspace).Append('|').Append(s, i1, len1).Append("\">")
							.Append("line ").Append(s, i1, len1).Append("<> in <z 0xFAFAD2>").Append(f.Name).Append("<>");

							isMain = s.Eq(k.start, "   at Script..ctor(String[] args) in ");
							if (!isMain || !f.IsScript) b.Append(", <\a>").Append(s, k.start + 6, g.Start - k.start - 10).Append("</\a>");
							b.AppendLine();

							replaced = true;
						} else if (!(s.Eq(k.start, "   ---") || s.Eq(k.start, "---"))) {
							stackEnd = k.start;
							break;
						}
					}
					if (replaced) {
						int j = stackEnd; //int j = stackEnd2 > 0 ? stackEnd2 : stackEnd;
						if (s[j - 1] == '\n') { if (s[--j - 1] == '\r') j--; }
						b.Append("   <fold><\a>   --- Raw stack trace ---\r\n").Append(s, i, j - i).Append("</\a></fold>");
						if (iLiteral > 0 && 0 != s.Eq(stackEnd, false, "</_>", "</\a")) stackEnd += 4;
						int more = s.Length - stackEnd;
						if (more > 0) {
							if (!s.Eq(stackEnd, "</fold>")) b.AppendLine();
							b.Append(s, stackEnd, more);
						}
						m.Text = b.ToString();
						//AOutput.QM2.Write("'" + m.Text + "'");
					}
					if (_sb.Capacity > 10_000) _sb = null; //let GC free it. Usually < 4000.
				}
			}

			if (s.Length <= 10_000) { //* 50 = 1 MB
				if (!ReferenceEquals(s, m.Text)) m = new OutServMessage(OutServMessageType.Write, s, m.TimeUtc, m.Caller);
				var h = _p._history;
				h.Enqueue(m);
				if (h.Count > 50) h.Dequeue();
			}

			(_iPanel ??= Panels.PanelManager.ZGetPanel(_p)).Visible = true;
		}
		static ARegex s_rx1, s_rx2;
		AuDockPanel.IPanel _iPanel;

		void _OpenLink(string s)
		{
			//AOutput.Write(s);
			var a = s.Split('|');
			Program.Model.OpenAndGoTo2(a[0], a.Length > 1 ? a[1] : null, a.Length > 2 ? a[2] : null);
		}

		void _RunScript(string s)
		{
			var a = s.Split('|');
			var f = Program.Model.FindScript(a[0]); if(f == null) return;
			CompileRun.CompileAndRun(true, f, a.Length == 1 ? null : a.RemoveAt(0));
		}
	}
}
