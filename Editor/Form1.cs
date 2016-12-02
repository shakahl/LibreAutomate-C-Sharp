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
		CatTreeView _tv;
		Scintilla _editor;
		//SplitContainer _splitV1; //cannot use because steals focus. Also creates 1 more controls, and 1 more ancestor level of the true controls.
		Splitter _splitV1;

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

		List<CatItem> _items;
		static string _collectionCsvFile = @"q:\test\ok\List.csv";
		//static string _collectionCsvFile = @"q:\test\Main\Main.csv";
		static string _collectionDir;

		public Form1()
		{
			InitializeComponent();

			this.SuspendLayout();

			//info: form font is set in designer: "Segoe UI", 9. It's the Windows UI font. Controls inherit it.
			//	SystemFonts.DefaultFont is "Microsoft Sans Serif". SystemFonts.DialogFont is "Tahoma". SystemFonts.MessageBoxFont is "Segoe UI", 9.

			var contentPanel = toolStripContainer1.ContentPanel;
			Size contentSize = contentPanel.Size;
			int tvWidth = 300;

			//treeview
			_tv = new CatTreeView(this);
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
			_tv.Size = new Size(tvWidth, contentSize.Height);

			//editor
			_editor = new Scintilla();
			_editor.Name = nameof(_editor);

			//vertical splitter between treeview and editor
			_splitV1 = new Splitter();
			_splitV1.Name = nameof(_splitV1);
			_splitV1.Size = new Size(4, contentSize.Height); //default 3 3
			_splitV1.Dock = DockStyle.Left;
			//_splitV1.MinSize = 25; _splitV1.MinExtra = 25; //default 25 25
			_splitV1.SplitPosition = tvWidth;
			_tv.Dock = DockStyle.Left;
			_editor.Dock = DockStyle.Fill;

			//note: the order of the following lines is important for the splitter.
			contentPanel.Controls.Add(_editor);
			contentPanel.Controls.Add(_splitV1);
			contentPanel.Controls.Add(_tv);

			this.ResumeLayout(false);

			//this.Location = new Point(200, 20); //currently Center, set in designer

			Perf.Next();
			_LoadCollection();
			Perf.Next();
			//Time.SetTimer(5, true, t => Perf.NW());
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}

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
			//Perf.Next();
			//_tv.SuspendLayout(); //same speed
			_items = new List<CatItem>(x.RowCount + 20);
			TreeNode prevNode = null, selectNode = null;
			var stack = new Stack<TreeNodeCollection>(); stack.Push(_tv.Nodes);
			for(int i = 0, n = x.RowCount; i < n; i++) {
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
				bool isFolder =(flags & 1) != 0;
				if(isFolder) imageIndex = 1;

				var node = stack.Peek().Add(x[i, 2], x[i, 0], imageIndex, imageIndex);
				//note: We set TreeNode.Text although we could draw text from TreeNode.Name.
				//	Need it for accessibility, also to auto-calculate text rectangle etc.
				//	To save memory, could set it = null (then owner-draw does not work) or " " (then owner-draw does not auto-calculate node text rect).

				prevNode = node;
				node.Tag = new CatItem(node, flags, x[i, 2], x[i, 3]);
				if(selectNode == null && !isFolder) selectNode = node;
			}
			//Perf.Next();
			_tv.SelectedNode = selectNode;
			var h=_tv.Handle; //the slow part, where the native SysTreeView32 control is created an populated.
			//Perf.NW();

		}

		public class CatTreeView :TreeView
		{
			Form1 _form;

			public CatTreeView(Form1 form)
			{
				_form = form;
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

						_form._editor.Text = File.ReadAllText(path);
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
	}
}
