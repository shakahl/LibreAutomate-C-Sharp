#define TV_NATIVE

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;

using ScintillaNET;
using G.Controls;

using Catkeys;
using static Catkeys.NoClass;

namespace Editor
{
	public partial class MainForm :Form
	{
		GDockPanel _dock;
		CatFilesPane _tvPane;
		CatOpenPane _openPane;
		CatRunningPane _runningPane;
		CatRecentPane _recentPane;
		CatOutputPane _outputPane;
		CatFindPane _findPane;
		Scintilla _statusBar;
		Scintilla _code;
		ToolStripSpringTextBox _helpFind;

		static MainForm _form;
		static List<CatItem> _items;
		static string _collectionCsvFile = @"q:\test\ok\List.csv";
		//static string _collectionCsvFile = @"q:\test\Main\Main.csv";
		static string _collectionDir;

		public MainForm()
		{
			//InitializeComponent();
			_form = this;

			this.SuspendLayout();
			this.ClientSize = new Size(1100, 700);
			this.AutoScaleMode = AutoScaleMode.Dpi;
			this.Font = new Font("Segoe UI", 9F);
			this.Name = "Form1";
			//this.StartPosition = FormStartPosition.CenterScreen;
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(0, 100);
			this.Text = "Catkeys";

			var contentPanel = this;

			//code
			_code = new Scintilla();
			_code.Name = _code.AccessibleName = "Code";
			_code.BorderStyle = BorderStyle.None;

			_tvPane = new CatFilesPane();
			_openPane = new CatOpenPane();
			_runningPane = new CatRunningPane();
			_recentPane = new CatRecentPane();
			_outputPane = new CatOutputPane();
			_findPane = new CatFindPane();

			//status bar
			_statusBar = new Scintilla();
			_statusBar.BorderStyle = BorderStyle.None;
			_statusBar.Size = new Size(0, 40);
			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.AccessibleRole = AccessibleRole.StatusBar;
			_statusBar.AccessibleName = "Status bar";

			Perf.Next();
			InitStrips();
			_helpFind = _tsHelp.Items["Help_Find"] as ToolStripSpringTextBox;
			//_tsHelp.Padding = new Padding(); //removes 1-pixel right margin that causes a visual artefact because of gradient, but then not good when no margin when the edit is at the very right edge of the form

			_tsTools.Items[0].Click += (unu, sed) =>
			{
				Print("click");
				//_tsTools.Parent.Hide();
				//Time.SetTimer(1000, true, t=>_tsTools.Parent.Show());
			};

			Perf.Next();

			var c = new RichTextBox();
			c.Name = c.Text = "Results";

			_dock = new GDockPanel();
			_dock.Create(Folders.ThisAppData + "Panels.xml", EImageList.Strips,
				_code, _tvPane, _outputPane, _findPane, _openPane, _runningPane, _recentPane, c,
				_tsMenu, _tsFile, _tsEdit, _tsRun, _tsTools, _tsHelp, _tsCustom1, _tsCustom2
				);

			//_dock.AddPanel(c, _outputPane, GDockPanel.DockSide.TabAfter, "<panel text='Results' tooltip='Find results' image='15' />");
			//_dock.AddPanel(c, _tvPane, GDockPanel.DockSide.SplitBelow, "<panel text='Results' tooltip='Find results' image='15' hide='' />");

			contentPanel.Controls.Add(_dock);
			contentPanel.Controls.Add(_statusBar);

			this.ResumeLayout(false);
			Perf.Next();
			_LoadCollection();
			Perf.Next();

			//Perf.Next();
			Time.SetTimer(1, true, t =>
			{
				//Perf.NW();
				Perf.Next();
				//TaskDialog.Show("", Perf.Times);
				if(_dock != null) _outputPane.Write(Perf.Times); else Print(Perf.Times);
				//TaskDialog.Show(Perf.Times, IsWinEventHookInstalled(EVENT_OBJECT_CREATE).ToString()); //IsWinEventHookInstalled always true (false positive, as documented)
				//GC.Collect();

				//Close();
			});
		}

		void _LoadCollection()
		{
			var createTvHandle = (Wnd)_tvPane;

			var p1 = new Perf.Inst(true);

			//_tv.Show(false);

			//Api.SetParent(_tv, Wnd.Misc.SpecHwnd.Message);
			//_tv.SetWindowLong(Api.GWL_HWNDPARENT, 0);
			//Api.SetWindowsHookEx(Api.WH_CALLWNDPROC, _hookProc, Zero, Api.GetCurrentThreadId());
			//Api.SetWindowsHookEx(Api.WH_CALLWNDPROCRET, _hookProc, Zero, Api.GetCurrentThreadId());

			var x = new CsvTable(); x.Separator = '|';
			_collectionDir = Path.GetDirectoryName(_collectionCsvFile) + "\\";
			x.FromFile(_collectionCsvFile);
			//var dat = File.ReadAllText(_collectionCsvFile); x.FromString(dat + dat+dat+dat);
			//Perf.Next();
			_items = new List<CatItem>(x.RowCount + 20);
			_items.Add(new CatItem(0, "<file>", 0, null, null));
			int prevId = 0, selectId = 0;
			var stack = new Stack<int>(); stack.Push(0);
			//Print(x.RowCount);
			//Perf.Next();
			p1.Next();
			//var u = new Perf.Inst(); u.Incremental = true;
			for(int i = 0, n = x.RowCount; i < n; i++) {
				//var p = new Perf.Inst(true);
				n = 50; //TODO: remove
				var s = x[i, 1]; uint flags = 0;
				if(!Empty(s)) {
					int numLen; flags = (uint)s.ToInt32_(0, out numLen);
					if(s.Length > numLen) {
						switch(s[numLen]) {
						case '>':
							stack.Push(prevId);
							break;
						case '<':
							int k = s.ToInt32_(numLen + 1);
							do stack.Pop(); while(--k > 0);
							break;
						}
					}
				}

				int imageIndex = 0;
				bool isFolder = (flags & 1) != 0;
				if(isFolder) imageIndex = 1;

				int id = _items.Count, idParent = stack.Peek();
				var item = new CatItem(idParent, x[i, 0], flags, x[i, 2], x[i, 3]);
				_items.Add(item);
				//p.Next();
				item.Htvi = _tvPane.TvAdd(_items[idParent].Htvi, id, imageIndex);
				//p.Next();
				//if(p.TimeTotal >= 100)
				//	p.Write();

				prevId = id;
				if(selectId == 0 && !isFolder) selectId = id;
			}
			//Perf.Next();
			p1.Next();
			_tvPane.TvSend(_Api.TVM_SETIMAGELIST, _Api.TVSIL_NORMAL, EImageList.Files.Handle);
			//_tv.Send(_Api.TVM_SELECTITEM, _Api.TVGN_CARET, _items[selectId].Htvi);

			//Speed for ~13000 items in ~30 root folders: 40 ms.

			//Perf.NW();
			//p1.NW();

			//note: DockPanel makes adding and deleting treeview items ~3 times slower by default.
			//	Workaround: in its source disable installing WH_CALLWNDPROCRET hook; then noticed some problems with focus, but not too big.
			//	Now disabled.
		}

		protected override void WndProc(ref Message m)
		{
			uint msg = (uint)m.Msg; LPARAM wParam = m.WParam, lParam = m.LParam;

			base.WndProc(ref m);

			switch(msg) {
			case Api.WM_ENABLE:
				//.NET ignores this. Eg if an owned form etc disables this window, the Enabled property is not changed and no EnabledChanged event.
				//PrintList(wParam, Enabled);
				//Enabled = wParam != 0; //not good
				_dock.EnableDisableAllFloatingWindows(wParam != 0);
				break;
			}
		}

		public class CatFilesPane :Panel
		{
			//idea: when file clicked, open it and show CatMenu of its functions (if > 1).

			Wnd _w;
			Native.WNDPROC _tvWndproc;

			public CatFilesPane()
			{
				this.Text = this.Name = "Files";
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				uint style = Native.WS_CHILD | Native.WS_VISIBLE | _Api.TVS_INFOTIP | _Api.TVS_FULLROWSELECT | _Api.TVS_SHOWSELALWAYS;
				if(true) style |= _Api.TVS_HASBUTTONS | _Api.TVS_HASLINES | _Api.TVS_LINESATROOT | _Api.TVS_EDITLABELS;
				else style |= _Api.TVS_SINGLEEXPAND | _Api.TVS_TRACKSELECT;
				_w = Wnd.Misc.CreateWindow(0, "SysTreeView32", "Files", style, 0, 0, 100, 100, (Wnd)Handle, 2202);
				_tvWndproc = (Native.WNDPROC)Marshal.GetDelegateForFunctionPointer(_w.GetWindowLong(Native.GWL_WNDPROC), typeof(Native.WNDPROC));

				base.OnHandleCreated(e);
			}

			/// <summary>
			/// Creates native treeview control and returns its handle.
			/// </summary>
			/// <returns></returns>
			public Wnd Init()
			{
				//OleInitialize(Zero);
				return _w;
			}

			protected override void OnClientSizeChanged(EventArgs e)
			{
				//Print("tree OnClientSizeChanged");
				if(!_w.Is0) {
					var z = this.ClientSize;
					_w.ResizeLL(z.Width, z.Height);
				}
				base.OnClientSizeChanged(e);
			}

			protected override unsafe void WndProc(ref Message m)
			{
				switch((uint)m.Msg) {
				//case Api.WM_SIZE:
				//	Print("tree WM_SIZE");
				//	break;
				case Api.WM_NOTIFY:
					if(!_w.Is0) {
						_Api.NMHDR* nh = (_Api.NMHDR*)m.LParam;
						//PrintList("notify", nh->hwndFrom, nh->code);
					}
					return;
				}
				base.WndProc(ref m);
			}

			internal LPARAM TvSend(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				if(_w.Is0) { var t = (Wnd)Handle; } //create handles of this and _w
				return _w.Send(message, wParam, lParam);
				//return _tvWndproc(_w, message, wParam, lParam);
			}

			internal unsafe IntPtr TvAdd(IntPtr hparent, LPARAM param, int image)
			{
				var x = new _Api.TVINSERTSTRUCT();
				x.hParent = hparent;
				x.hInsertAfter = (IntPtr)_Api.TVI_LAST;
				x.item.mask = _Api.TVIF_TEXT | _Api.TVIF_PARAM | _Api.TVIF_IMAGE | _Api.TVIF_SELECTEDIMAGE;
				x.item.lParam = param;
				x.item.iImage = x.item.iSelectedImage = image;
				x.item.pszText = (IntPtr)_Api.LPSTR_TEXTCALLBACK;
				//return _tv.Send(_Api.TVM_INSERTITEM, 0, &x);
				return _tvWndproc(_w, _Api.TVM_INSERTITEM, 0, &x);
			}

		}

		public class CatOpenPane :Panel
		{
			public CatOpenPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
				c.AccessibleName = this.Text = this.Name = "Open";
				this.Controls.Add(c);
			}

		}

		public class CatRunningPane :Panel
		{
			public CatRunningPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
				c.AccessibleName = this.Text = this.Name = "Running";
				this.Controls.Add(c);

			}
		}

		public class CatRecentPane :Panel
		{
			public CatRecentPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
				c.AccessibleName = this.Text = this.Name = "Recent";
				this.Controls.Add(c);

			}
		}

		public class CatOutputPane :Panel
		{
			Scintilla _c;

			public CatOutputPane()
			{
				_c = new Scintilla();
				_c.BorderStyle = BorderStyle.None;
				_c.Dock = DockStyle.Fill;
				_c.AccessibleName = this.Text = this.Name = "Output";

				//c.BackColor = Calc.ColorFromNative(0xFFF8F8F8); //does not work


				this.Controls.Add(_c);
			}

			public void Write(object text)
			{
				_c.AppendText(text.ToString() + "\r\n");
			}
		}

		public class CatFindPane :Panel
		{
			public CatFindPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
				c.AccessibleName = this.Text = this.Name = "Find";
				this.Controls.Add(c);

			}
		}

		public class CatItem
		{
			public CatItem(int idParent, string name, uint flags, string guid, string etc)
			{
				IdParent = idParent; Name = name; _flags = flags; Guid = guid; _etc = etc;
			}

			public string Name { get; private set; }
			public string Guid { get; private set; }
			internal IntPtr Htvi { get; set; }
			string _etc;
			uint _flags;
			public int IdParent { get; private set; }

			public bool IsFolder { get { return (_flags & 7) == 1; } }
			public bool IsExternal { get { return (_flags & 7) == 2; } }
			public bool IsDisabled { get { return (_flags & 8) != 0; } }
			//public CatItem Parent { get { return } }

			public string FilePath
			{
				get
				{
					if(IsExternal) return _etc;
					return Name; //TODO
								 //var s = Node.FullPath;
								 //if(IsFolder) return _collectionDir + s;
								 //return _collectionDir + s + ".cs";
				}
			}
		}









		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(keyData == (Keys.Control | Keys.T)) {
				//_Test();
				_dock.Test();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		void _Test()
		{
			//foreach(var p in Controls.OfType<GPanel>()) {
			//	PrintList(p, p.Dock, p.DPSplitter.IsVisible);
			//}

			//_dock.HidePanel(_openPane);
			//TaskDialog.Show("");
			//_dock.ShowPanel(_openPane);
		}

#if DEBUG
		private void _strips_Click(object sender, EventArgs e)
		{
			_dock.SetPanelText(_recentPane, "new text");
			_dock.SetPanelText(_findPane, "new text");
			_dock.SetPanelToolTipText(_findPane, "new tooltip");
		}
#endif
	}
}
