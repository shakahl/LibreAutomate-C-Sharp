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

using ScintillaNET;
using G.Controls;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Editor
{
	public partial class Form4 :Form
	{
		GDockPanels _docker;
		Scintilla _code;
		CatTreePane _tvPane;
		CatOutputPane _outputPane;
		CatFindPane _findPane;
		CatOpenPane _openPane;
		CatRunningPane _runningPane;
		CatRecentPane _recentPane;
		Scintilla _statusBar;
		Panel _strips;

		static Form4 _form;
		static List<CatItem> _items;
		static string _collectionCsvFile = @"q:\test\ok\List.csv";
		//static string _collectionCsvFile = @"q:\test\Main\Main.csv";
		static string _collectionDir;

		public Form4()
		{
			//InitializeComponent();
			_form = this;

			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(1100, 700);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Catkeys Editor";

			var contentPanel = this;

			//code
			_code = new Scintilla();
			_code.Name = "Code";
			_code.BorderStyle = BorderStyle.None;

			//treeview
			_tvPane = new CatTreePane();
			_tvPane.Text = _tvPane.Name = "Files";

			//open
			_openPane = new CatOpenPane();
			_openPane.Text = _openPane.Name = "Open";

			//output
			_outputPane = new CatOutputPane();
			_outputPane.Text = _outputPane.Name = "Output";

			//find
			_findPane = new CatFindPane();
			_findPane.Text = _findPane.Name = "Find";

			//running
			_runningPane = new CatRunningPane();
			_runningPane.Text = _runningPane.Name = "Running";

			////recent
			//_recentPane = new CatRecentPane();
			//_recentPane.Text = _recentPane.Name = "Recent";

			//status bar
			_statusBar = new Scintilla();
			_statusBar.Name = nameof(_statusBar);
			_statusBar.BorderStyle = BorderStyle.None;
			_statusBar.Size = new Size(0, 40);
			_statusBar.Dock = DockStyle.Bottom;

			//strips
			_strips = new Panel();
			_strips.Name = nameof(_strips);
			_strips.Size = new Size(0, 50);
			_strips.Dock = DockStyle.Top;
#if !FORM2
			Perf.Next();
			InitStrips();
			Perf.Next();
#endif
			_docker = new GDockPanels();
			_docker.AddControl(_code);
			_docker.AddControl(_tvPane);
			_docker.AddControl(_openPane, 34);
			_docker.AddControl(_outputPane, 29);
			_docker.AddControl(_findPane, 23);
			_docker.AddControl(_runningPane);
			//_docker.AddControl(,);
			_docker.Create(Folders.App + "Panels.xml", EImageList.Strips, _strips);

			contentPanel.Controls.Add(_docker);
			contentPanel.Controls.Add(_strips);
			contentPanel.Controls.Add(_statusBar);

			this.ResumeLayout(false);
			Perf.Next();

			//floaty.Docking += floaty_Docking;
			//floaty.Undocking += (unu, sed) => { Out(1); };

			Perf.Next();
			_LoadCollection();
			Perf.Next();
			Time.SetTimer(1, true, t =>
			{
				//Perf.NW();
				Perf.Next();
				//TaskDialog.Show("", Perf.Times);
				_code.Text = Perf.Times;
				//TaskDialog.Show(Perf.Times, IsWinEventHookInstalled(EVENT_OBJECT_CREATE).ToString()); //IsWinEventHookInstalled always true (false positive, as documented)
				//GC.Collect();

				//Close();
			});
		}

		void _LoadCollection()
		{
			var createTvHandle = (Wnd)_tvPane;

			var p1 = new Perf.Inst(true);

			//_tv.Visible = false;

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
			//Out(x.RowCount);
			//Perf.Next();
			p1.Next();
			//var u = new Perf.Inst(); u.Incremental = true;
			for(int i = 0, n = x.RowCount; i < n; i++) {
				//var p = new Perf.Inst(true);
				n = 50; //TODO: remove
				var s = x[i, 1]; uint flags = 0;
				if(!Empty(s)) {
					int numLen; flags = (uint)s.ToInt_(0, out numLen);
					if(s.Length > numLen) {
						switch(s[numLen]) {
						case '>':
							stack.Push(prevId);
							break;
						case '<':
							int k = s.ToInt_(numLen + 1);
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

		//static Api.HOOKPROC _hookProc = _HookProc;
		//static LPARAM _HookProc(int code, LPARAM wParam, LPARAM lParam)
		//{
		//	return 0;
		//}

		private void floaty_Docking(object sender, EventArgs e)
		{
			//Out(0);
		}

		public class CatTreePane :Panel
		{
			Wnd _w;
			Api.WNDPROC _tvWndproc;

			public CatTreePane()
			{
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				uint style = Api.WS_CHILD | Api.WS_VISIBLE | _Api.TVS_INFOTIP | _Api.TVS_FULLROWSELECT | _Api.TVS_SHOWSELALWAYS;
				if(true) style |= _Api.TVS_HASBUTTONS | _Api.TVS_HASLINES | _Api.TVS_LINESATROOT | _Api.TVS_EDITLABELS;
				else style |= _Api.TVS_SINGLEEXPAND | _Api.TVS_TRACKSELECT;
				_w = Api.CreateWindowEx(0, "SysTreeView32", "Files", style, 0, 0, 100, 100, (Wnd)Handle, 2202, Zero, 0);
				_tvWndproc = (Api.WNDPROC)Marshal.GetDelegateForFunctionPointer(_w.GetWindowLong(Api.GWL_WNDPROC), typeof(Api.WNDPROC));

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
				//Out("tree OnClientSizeChanged");
				if(!_w.Is0) {
					var z = this.ClientSize;
					_w.ResizeRaw(z.Width, z.Height);
				}
				base.OnClientSizeChanged(e);
			}

			protected override unsafe void WndProc(ref Message m)
			{
				switch((uint)m.Msg) {
				//case Api.WM_SIZE:
				//	Out("tree WM_SIZE");
				//	break;
				case Api.WM_NOTIFY:
					if(!_w.Is0) {
						_Api.NMHDR* nh = (_Api.NMHDR*)m.LParam;
						//OutList("notify", nh->hwndFrom, nh->code);
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

		public class CatOutputPane :Panel
		{
			public CatOutputPane()
			{
				var c = new Scintilla();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;

				//c.BackColor = Calc.ColorFromNative(0xFFF8F8F8); //does not work


				this.Controls.Add(c);
			}
		}

		public class CatFindPane :Panel
		{
			public CatFindPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
				this.Controls.Add(c);

			}
		}

		public class CatOpenPane :Panel
		{
			public CatOpenPane()
			{
				var c = new ListView();
				c.BorderStyle = BorderStyle.None;
				c.Dock = DockStyle.Fill;
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
				_docker.Test();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		void _Test()
		{
			//foreach(var p in Controls.OfType<GPanel>()) {
			//	OutList(p, p.Dock, p.DPSplitter.Visible);
			//}

			//_docker.HidePanel(_openPane);
			//TaskDialog.Show("");
			//_docker.ShowPanel(_openPane);
		}
	}
}
