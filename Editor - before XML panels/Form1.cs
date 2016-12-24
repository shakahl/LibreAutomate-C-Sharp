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
//using System.Linq;

using ScintillaNET;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Editor
{
	public partial class Form1 :Form
	{
#if TV_NATIVE
		Wnd _tv;
#else
		CatTreeView _tv;
#endif
		Scintilla _code;
		CatTreePane _tvPane;
		CatEditPane _codePane;
		CatOutputPane _outputPane;
		CatFindPane _findPane;
		CatActiveItemsPane _aiPane;

		static Form1 _form;
		static List<CatItem> _items;
		static string _collectionCsvFile = @"q:\test\ok\List.csv";
		//static string _collectionCsvFile = @"q:\test\Main\Main.csv";
		static string _collectionDir;

		public Form1()
		{
			_form = this;

#if false
			InitializeComponent();

			this.SuspendLayout();

			var contentPanel =this.toolStripContainer1.ContentPanel;

#else
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(1152, 773);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Catkeys Editor";
			//Perf.Next(); //~70
#if false
			var contentPanel = this;
#else
			var contentPanel = new Panel();
			contentPanel.Location = new Point(0, 60);
			var cz =ClientSize;
			contentPanel.Size = new Size(cz.Width, cz.Height - 60);
			this.Controls.Add(contentPanel);
#endif

#endif
			//info: form font is set in designer: "Segoe UI", 9. It's the Windows UI font. Controls inherit it.
			//	SystemFonts.DefaultFont is "Microsoft Sans Serif". SystemFonts.DialogFont is "Tahoma". SystemFonts.MessageBoxFont is "Segoe UI", 9.

			//var tsPanel = new ToolStripPanel();
			//tsPanel.Dock = DockStyle.Top;
			//tsPanel.Join(menuStrip1);
			//tsPanel.Join(toolStrip1, 300, 30);
			//tsPanel.Rows[0].Controls.A
			//Controls.Add(tsPanel);

			Size z = contentPanel.ClientSize;

			//treeview
			_tvPane = new CatTreePane();
			_tvPane.Text = "Files";
			_tvPane.TopLevel = false;
			_tvPane.Location = new Point(0, 0);
#if TV_NATIVE
			_tv = _tvPane.Init();
			_tvWndproc = (Api.WNDPROC)Marshal.GetDelegateForFunctionPointer(_tv.GetWindowLong(Api.GWL_WNDPROC), typeof(Api.WNDPROC));

#else
			_tv = new CatTreeView();
			_tv.Name = nameof(_tv);
			_tv.ShowNodeToolTips = true;
			_tv.HideSelection = false;
			if(true) {
				_tv.LabelEdit = true;
			} else {
				_tv.ShowPlusMinus = false;
				_tv.FullRowSelect = true;
				_tv.ShowLines = false;
				//_tv.SingleClickExpand //no such prop
			}
			_tv.Dock = DockStyle.Fill;
			//_tv.Size = new Size(500, 2000);
			_tvPane.Controls.Add(_tv);

#endif
			_tvPane.Size = new Size(z.Width / 2, z.Height);
			contentPanel.Controls.Add(_tvPane);
			_tvPane.Show();

			//editor
			_codePane = new CatEditPane();
			_codePane.Text = "Code";
			_codePane.TopLevel = false;

			_code = new Scintilla();
			_code.Name = nameof(_code);
			_code.Dock = DockStyle.Fill;
			_codePane.Controls.Add(_code);

			_codePane.Location = new Point(z.Width / 2, 0);
			_codePane.Size = new Size(z.Width / 2, z.Height);
			contentPanel.Controls.Add(_codePane);
			_codePane.Show();

			this.ResumeLayout(false);

			//this.Location = new Point(200, 20); //currently Center, set in designer

			//TaskDialog.Show("-");
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
			});

			//Time.SetTimer(1000, true, t => { Perf.First(); _tv.Nodes.Clear(); Perf.NW(); });
		}
		//[DllImport("user32.dll")]
		//public static extern bool IsWinEventHookInstalled(uint @event);
		//public const uint EVENT_OBJECT_CREATE = 0x8000;

#if TV_NATIVE
		void _LoadCollection()
		{
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
				item.Htvi = _TvAdd(_items[idParent].Htvi, id, imageIndex);
				//p.Next();
				//if(p.TimeTotal >= 100)
				//	p.Write();

				prevId = id;
				if(selectId == 0 && !isFolder) selectId = id;
			}
			//Perf.Next();
			p1.Next();
			_tv.Send(_Api.TVM_SETIMAGELIST, _Api.TVSIL_NORMAL, EImageList.Files.Handle);
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

		unsafe IntPtr _TvAdd(IntPtr hparent, LPARAM param, int image)
		{
			var x = new _Api.TVINSERTSTRUCT();
			x.hParent = hparent;
			x.hInsertAfter = (IntPtr)_Api.TVI_LAST;
			x.item.mask = _Api.TVIF_TEXT | _Api.TVIF_PARAM | _Api.TVIF_IMAGE | _Api.TVIF_SELECTEDIMAGE;
			x.item.lParam = param;
			x.item.iImage = x.item.iSelectedImage = image;
			x.item.pszText = (IntPtr)_Api.LPSTR_TEXTCALLBACK;
			//return _tv.Send(_Api.TVM_INSERTITEM, 0, &x);
			return _tvWndproc(_tv, _Api.TVM_INSERTITEM, 0, &x);
		}

		Api.WNDPROC _tvWndproc;
#else
		void _LoadCollection()
		{
			//Load ImageList.
			ImageList il = new ImageList();
			//il.LoadFromImageFile_(Folders.App + "il_tv.png");
			il.LoadFromImage_(Properties.Resources.il_tv);
			_tv.ImageList = il;

			//Perf.First();
			var x = new CsvTable(); x.Separator = '|';
			_collectionDir = Path.GetDirectoryName(_collectionCsvFile) + "\\";
			x.FromFile(_collectionCsvFile);
			//var dat = File.ReadAllText(_collectionCsvFile); x.FromString(dat + dat+dat+dat);
			//Perf.Next();
			//_tv.SuspendLayout(); //same speed
			_items = new List<CatItem>(x.RowCount + 20);
			TreeNode prevNode = null, selectNode = null;
			var stack = new Stack<TreeNodeCollection>(); stack.Push(_tv.Nodes);
			//Out(x.RowCount);
			Perf.Next();
			for(int i = 0, n = x.RowCount; i < n; i++) {
				//n=1000; //TODO: remove
				var s = x[i, 1]; uint flags = 0;
				if(!Empty(s)) {
					int numLen; flags = (uint)s.ToInt_(0, out numLen);
					if(s.Length > numLen) {
						switch(s[numLen]) {
						case '>':
							stack.Push(prevNode.Nodes);
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

				var node = stack.Peek().Add(x[i, 2], x[i, 0], imageIndex, imageIndex);
				//note: We set TreeNode.Text although we could draw text from TreeNode.Name.
				//	Need it for accessibility, also to auto-calculate text rectangle etc.
				//	To save memory, could set it = null (then owner-draw does not work) or " " (then owner-draw does not auto-calculate node text rect).

				var item=new CatItem(node, flags, x[i, 2], x[i, 3]);
				_items.Add(item);
				node.Tag = item;
				prevNode = node;
				if(selectNode == null && !isFolder) selectNode = node;
			}
			//Perf.Next();
			//_tv.SelectedNode = selectNode;

			//The slow part, where the native SysTreeView32 control is created an populated.
			//Speed for ~13000 items in ~30 root folders: In 32-bit process ~150 ms, in 64-bit process ~320 ms.
			var h = _tv.Handle; //this does not work: _tv.CreateControl();

			//Perf.NW();

			//note: in 64-bit process adding and deleting treeview items is ~2 times slower. Now this project has "Prefer 32-bit".
			//note: DockPanel makes adding and deleting treeview items is ~2 times slower by default. Workaround: in its source disable installing WH_CALLWNDPROCRET hook; then noticed some problems with focus, but not too big. Now disabled.
		}
#endif

		public class CatTreePane :Form
		{
#if TV_NATIVE
			Wnd _w, _wPane;

			/// <summary>
			/// Creates native treeview control and returns its handle.
			/// </summary>
			/// <returns></returns>
			//public Wnd Init()
			//{
			//	_wPane = (Wnd)Handle;
			//	uint style = _Api.TVS_INFOTIP | _Api.TVS_FULLROWSELECT | _Api.TVS_SHOWSELALWAYS;
			//	if(true) style |= _Api.TVS_HASBUTTONS | _Api.TVS_HASLINES | _Api.TVS_LINESATROOT | _Api.TVS_EDITLABELS;
			//	else style |= _Api.TVS_SINGLEEXPAND | _Api.TVS_TRACKSELECT;
			//	_w =Api.CreateWindowEx(0, "SysTreeView32", "Files", style, 0, 0, 100, 100, Wnd.Misc.SpecHwnd.Message, 0, Zero, 0);
			//	Debug.Assert(!_w.Is0);
			//	return _w;
			//}
			public Wnd Init()
			{
				_wPane = (Wnd)Handle;
				uint style = Api.WS_CHILD | Api.WS_VISIBLE | _Api.TVS_INFOTIP | _Api.TVS_FULLROWSELECT | _Api.TVS_SHOWSELALWAYS;
				if(true) style |= _Api.TVS_HASBUTTONS | _Api.TVS_HASLINES | _Api.TVS_LINESATROOT | _Api.TVS_EDITLABELS;
				else style |= _Api.TVS_SINGLEEXPAND | _Api.TVS_TRACKSELECT;
				//for(int i=0; i<5; i++) _Api.OleUninitialize();
				_w = Api.CreateWindowEx(0, "SysTreeView32", "Files", style, 0, 0, 100, 100, _wPane, 2202, Zero, 0);
				//OleInitialize(Zero);
				return _w;
			}

			protected override void OnClientSizeChanged(EventArgs e)
			{
				if(!_w.Is0) {
					var z = _wPane.ClientSize;
					_w.ResizeRaw(z.cx, z.cy);
				}
				base.OnClientSizeChanged(e);
			}

			protected override unsafe void WndProc(ref Message m)
			{
				switch((uint)m.Msg) {
				case Api.WM_NOTIFY:
					if(!_w.Is0) {
						_Api.NMHDR* nh = (_Api.NMHDR*)m.LParam;
						//OutList("notify", nh->hwndFrom, nh->code);
					}
					return;
				}
				base.WndProc(ref m);
			}
#endif
		}

		public class CatEditPane :Form
		{

		}

		public class CatOutputPane :Form
		{

		}

		public class CatFindPane :Form
		{

		}

		public class CatActiveItemsPane :Form
		{

		}

		public class CatTreeView :TreeView
		{

			public CatTreeView()
			{
				DrawMode = TreeViewDrawMode.OwnerDrawText;
				//ForeColor = Color.Chocolate;
			}

			protected override void OnAfterSelect(TreeViewEventArgs e)
			{
				var x = e.Node.Tag as CatItem;
				if(x.IsFolder) {
					//Expanding/collapsing here has some problems, better do it in OnNodeMouseClick.
				} else {
					string path = x.FilePath;
					//OutList(path, Files.FileExists(path));
					if(Files.FileExists(path)) {

						_form._code.Text = File.ReadAllText(path);
					}
				}

				base.OnAfterSelect(e);
			}

			protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
			{
				var x = e.Node.Tag as CatItem;
				if(x.IsFolder && ShowPlusMinus == false && e.Button == MouseButtons.Left && e.Clicks == 1) {
					if(e.Node.IsExpanded) e.Node.Collapse(); else e.Node.Expand();
				}
				base.OnNodeMouseClick(e);
			}

			protected override void OnDrawNode(DrawTreeNodeEventArgs e)
			{
				//Out(e.Node.Text);
				if(e.Bounds.Width > 0 && e.Bounds.Height > 0) {
					//eg can be Height 0 when expanding a folder, don't know why; IsEmpty does not work.
					//OutList(e.Node.Text, e.State, e.Bounds);
					bool isSelected = e.State.HasFlag(TreeNodeStates.Selected | TreeNodeStates.Focused);
					if(!(isSelected && e.Node.IsEditing)) {
						var textColor = isSelected ? SystemColors.HighlightText : ForeColor;
						//if(isSelected) e.DrawDefault = true; else //not good
						TextRenderer.DrawText(e.Graphics, e.Node.Text, Font, new Point(e.Bounds.Left, e.Bounds.Top + 1), textColor);

						TextRenderer.DrawText(e.Graphics, "Ctrl+J", Font, new Point(e.Bounds.Right + 10, e.Bounds.Top + 1), Color.LimeGreen);

						//note: don't use e.Graphics.DrawString, with some fonts it draws wider, does not fit in the selected item rect.
					}
				}

				base.OnDrawNode(e);
			}

			protected override void OnAfterExpand(TreeViewEventArgs e)
			{
				e.Node.ImageIndex = 2; //open folder
				base.OnAfterExpand(e);
			}

			protected override void OnAfterCollapse(TreeViewEventArgs e)
			{
				e.Node.ImageIndex = 1; //normal folder
				base.OnAfterCollapse(e);
			}
		}

#if TV_NATIVE
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
#else
		public class CatItem
		{
			public CatItem(TreeNode node, uint flags, string guid, string etc)
			{
				Node = node; _flags = flags; Guid = guid; _etc = etc;
			}

			public TreeNode Node { get; private set; } //holds text, parent, etc
			public string Guid { get; private set; }
			string _etc;
			uint _flags;
			public bool IsFolder { get { return (_flags & 7) == 1; } }
			public bool IsExternal { get { return (_flags & 7) == 2; } }
			public bool IsDisabled { get { return (_flags & 8) != 0; } }

			public string FilePath
			{
				get
				{
					if(IsExternal) return _etc;
					var s = Node.FullPath;
					if(IsFolder) return _collectionDir + s;
					return _collectionDir + s + ".cs";
				}
			}
		}
#endif

		private void toolStripContainer1_Click(object sender, EventArgs e)
		{

		}
	}
}
