using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

using BrightIdeasSoftware;

partial class ThisIsNotAFormFile { }

class PanelFilesOLV :Control
{
	//idea: when file clicked, open it and show CatMenu of its functions (if > 1).

	TreeListView _c;

	static string _collectionFile = Folders.ThisApp + @"test\ok\Main.xml";

	public PanelFilesOLV()
	{
		//var p = new Perf.Inst(true);
		_c = new TreeListView();
		//p.Next();
		//((ISupportInitialize)_c).BeginInit(); //just makes slower. But test again later.
		//_c.SuspendLayout();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Files";
		//_c.LabelEdit = true; //no, it is of ListView
		_c.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
		var hf = new HeaderFormatStyle(); hf.Normal.BackColor = Color.FromKnownColor(KnownColor.ButtonFace);
		_c.HeaderFormatStyle = hf;
		_c.RevealAfterExpand = false;
		_c.SelectAllOnControlA = false;

		_c.IsSimpleDragSource = true;
		_c.IsSimpleDropSink = true;
		SimpleDropSink sink1 = (SimpleDropSink)_c.DropSink;
		//sink1.AcceptExternal = true;
		sink1.CanDropBetween = true;
		sink1.CanDropOnBackground = true;
		_c.ModelCanDrop += _c_ModelCanDrop;
		_c.ModelDropped += _c_ModelDropped;

		//_c.CellClick += _c_CellClick;
		_c.CellRightClick += _c_CellRightClick;
		_c.SelectionChanged += _c_SelectionChanged; //fired on button-up
													//_c.SelectedIndexChanged += _c_SelectedIndexChanged; //fired on button-up //fired more frequently
													//_c.ItemSelectionChanged += _c_ItemSelectionChanged; //similar to SelectionChanged, but fired on button-down
													//all 3 also fired on right-click

		//PrintList(_c.);

		//_c.CellEditUseWholeCell = false; //default
		//_c.ShowGroups = false; //default
		_c.UseCompatibleStateImageBehavior = false;
		//_c.View = View.Details; //default
		//_c.VirtualMode = true; //default
		//_c.UseTranslucentSelection = true;
		//_c.UseTranslucentHotItem = true;
		_c.UseHotItem = true;

		//((ISupportInitialize)_c).EndInit();
		//_c.ResumeLayout(false);
		//p.Next();

		this.Controls.Add(_c);
		//p.NW();
	}

	public TreeListView TV { get => _c; }

	//private void _c_SelectedIndexChanged(object sender, EventArgs e)
	//{
	//	PrintList("SelectedIndexChanged", _c.SelectedObject);
	//}

	//private void _c_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	//{
	//	PrintList("ItemSelectionChanged", _c.SelectedObject);
	//}

	private void _c_SelectionChanged(object sender, EventArgs e)
	{
		//PrintList("SelectionChanged", _c.SelectedObject);
		_OnItemSelected(_c.SelectedObject as XElement);
	}

	XElement _xSelected;

	private void _c_CellClick(object sender, CellClickEventArgs e)
	{
		_OnItemSelected(e.Model as XElement);
	}

	void _OnItemSelected(XElement x)
	{
		if(x == null) return;
		if(_IsFolder(x)) return;
		if(x == _xSelected) return;
		_xSelected = x;
		//Print(x);
		try {
			var s = File.ReadAllText(_GetFilePath(x));
			EForm.MainForm.Code.Text = s;
		}
		catch(Exception ex) {
			Print(ex);
		}
	}

	private void _c_CellRightClick(object sender, CellRightClickEventArgs e)
	{
		Print(e.Model);
		//TODO: context menu
	}

	bool _IsFolder(XElement x)
	{
		return x.Name == "d";
	}

	/// <summary>
	/// Returns item path, like @"\Folder\Name" or @"\Name".
	/// It is without extension.
	/// </summary>
	string _GetPath(XElement x)
	{
		var a = new List<string>();
		do {
			a.Add(x.Attribute_("n"));
			a.Add("\\");
			x = x.Parent;
		} while(x != _xfiles);
		a.Reverse();
		return string.Concat(a);
	}

	string _GetFilePath(XElement x)
	{
		if(x.Name == "extern") return x.Attribute_("path");
		return _filesDir + _GetPath(x) + ".cs";
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	#region drag-drop
	void _c_ModelCanDrop(object sender, ModelDropEventArgs e)
	{
		var target = e.TargetModel as XElement;
		e.Handled = true;
		e.Effect = DragDropEffects.None;

		if(e.DropTargetLocation != DropTargetLocation.Background) {
			if(e.SourceModels.Cast<XElement>().Any(x => target.AncestorsAndSelf().Contains(x))) return;
		}
		e.Effect = DragDropEffects.Move;

		//if target is item and not folder, allow to drop only between items, not on item
		if(e.DropTargetLocation == DropTargetLocation.Item && !_IsFolder(target)) {
			var r = e.DropTargetItem.Bounds;
			var p = e.MouseLocation;
			e.DropTargetLocation = (p.Y > r.Y + r.Height / 2) ? DropTargetLocation.BelowItem : DropTargetLocation.AboveItem;
		}
	}

	void _c_ModelDropped(object sender, ModelDropEventArgs e)
	{
		_c.SelectedItem = null;

		var loc = e.DropTargetLocation;
		switch(loc) {
		case DropTargetLocation.Background:
		case DropTargetLocation.Item:
			_MoveObjectsInto(e.ListView as TreeListView, e.SourceListView as TreeListView, (XElement)e.TargetModel, e.SourceModels);
			break;
		case DropTargetLocation.AboveItem:
		case DropTargetLocation.BelowItem:
			bool below = loc == DropTargetLocation.BelowItem;
			_MoveObjectsBy(e.ListView as TreeListView, e.SourceListView as TreeListView, (XElement)e.TargetModel, e.SourceModels, below);
			break;
		default:
			//Print(e.DropTargetLocation);
			return;
		}

		e.RefreshObjects();
	}

	void _MoveObjectsInto(TreeListView targetTree, TreeListView sourceTree, XElement target, IList toMove)
	{
		if(target != null && !_IsFolder(target)) return;

		foreach(XElement x in toMove) {
			if(x.Parent == _xfiles) sourceTree.RemoveObject(x);
			x.Remove(); (target ?? _xfiles).Add(x);
			if(target == null) sourceTree.AddObject(x);
		}
	}

	void _MoveObjectsBy(TreeListView targetTree, TreeListView sourceTree, XElement target, IList toMove, bool below)
	{
		foreach(XElement x in toMove) {
			if(x == target) return;
		}

		var roots = sourceTree.Roots as ArrayList;

		foreach(XElement x in toMove) {
			if(x.Parent == _xfiles) roots.Remove(x);
			x.Remove();
		}

		if(target.Parent == _xfiles) roots.InsertRange(roots.IndexOf(target) + (below ? 1 : 0), toMove);

		var en = toMove.Cast<XElement>();
		if(below) target.AddAfterSelf(en); else target.AddBeforeSelf(en);

		sourceTree.Roots = roots;
	}
	#endregion

	string _filesDir;
	XElement _xml;
	XElement _xfiles;

	internal void LoadCollection()
	{
		_filesDir = Path.GetDirectoryName(_collectionFile) + @"\files";
		_xml = XElement.Load(_collectionFile);
		_xfiles = _xml.Element("files");

		var col = new OLVColumn("Name", null);
		//_c.AllColumns.Add(col);
		_c.Columns.Add(col);
		col.FillsFreeSpace = true;

		col.AspectGetter = delegate (object o)
		{
			var x = o as XElement;
			return x.Attribute_("n");
		};

		col.AspectPutter = delegate (object o, object value)
		{
			var x = o as XElement;
				//Print(value is string);
				x.SetAttributeValue("n", value);
		};

		col.ImageGetter = delegate (object o)
		{
			var x = o as XElement;
			return EResources.GetImage(_IsFolder(x) ? "Folder_16x" : "CS_16x");
		};

		var col2 = new OLVColumn("GUID", null);
		//_c.AllColumns.Add(col2);
		_c.Columns.Add(col2);
		col2.IsEditable = false;

		col2.AspectGetter = delegate (object o)
		{
			var x = o as XElement;
			return x.Attribute_("g");
		};

		_c.CanExpandGetter = delegate (object o)
		{
			var x = o as XElement;
			return _IsFolder(x);
		};

		_c.ChildrenGetter = delegate (object o)
		{
			var x = o as XElement;
			return x.Elements();
		};

		_c.ParentGetter = delegate (object o)
		{
			var x = o as XElement;
			return x.Parent;
		};

		_c.Roots = _xfiles.Elements();
	}

#if TEST
	public void Test()
	{
		Perf.First();
		//n.IsSelected = true;
		_c.Invalidate(false);
		Perf.Next();
		_c.Update();
		Perf.NW();
	}
#endif
}
