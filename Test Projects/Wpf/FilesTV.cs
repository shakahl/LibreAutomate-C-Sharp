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

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

using ScintillaNET;

namespace Wpf
{
	public partial class MainWindow
	{
		Scintilla _code;
		CatTreeView _tv;

		List<CatItem> _items;
		static string _collectionCsvFile = @"q:\test\ok\List.csv";
		//static string _collectionCsvFile = @"q:\test\Main\Main.csv";
		static string _collectionDir;


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
					int numLen; flags = (uint)s.ToInt32_(0, out numLen);
					if(s.Length > numLen) {
						switch(s[numLen]) {
						case '>':
							stack.Push(prevNode.Nodes);
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

			//The slow part, where the native SysTreeView32 control is created an populated.
			//Speed for ~13000 items in ~30 root folders: In 32-bit process ~150 ms, in 64-bit process ~320 ms.
			var h = _tv.Handle; //this does not work: _tv.CreateControl();

			//Perf.NW();

			//note: in 64-bit process adding and deleting treeview items is ~2 times slower. Now this project has "Prefer 32-bit".
			//note: DockPanel makes adding and deleting treeview items is ~2 times slower by default. Workaround: in its source disable installing WH_CALLWNDPROCRET hook; then noticed some problems with focus, but not too big. Now disabled.
		}

		public class CatTreeView :TreeView
		{
			MainWindow _form;

			public CatTreeView(MainWindow form)
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
	}

	public static class ImageList_
	{
		/// <summary>
		/// Loads imagelist from Image, eg png resource.
		/// Appends if the ImageList is not empty.
		/// Calls <see cref="ImageList.ImageCollection.AddStrip"/> and returns its return value (index of the first new image).
		/// </summary>
		/// <param name="pngImage">Png image as horizontal strip of images, eg Properties.Resources.il_tv. Don't dispose, because ImageList will use it later on demand (lazy).</param>
		public static int LoadFromImage_(this ImageList t, Image pngImage)
		{
			t.ColorDepth = ColorDepth.Depth32Bit;
			return t.Images.AddStrip(pngImage);
		}
	}
}
