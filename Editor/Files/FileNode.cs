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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

class FileNode
{
	FilesModel _model;
	XElement _x;

	public XElement Xml { get => _x; }

	public FileNode(FilesModel model, XElement x, bool isRoot = false)
	{
		_model = model;
		_x = x;
		x.AddAnnotation(this);

		if(!isRoot) {
			var guid = _x.Attribute_("g");
			if(guid == null || guid.Length != 22) {
				Output.Warning("Invalid GUID of " + this.ItemPath + ". Creating new.");
				guid = SetNewGUID();
			}
			try {
				_model.GuidMap.Add(guid, this);
			}
			catch(ArgumentException) {
				Output.Warning("Duplicate GUID of " + this.ItemPath + ". Creating new.");
				guid = SetNewGUID();
			}
		}

		foreach(var xx in x.Elements()) {
			new FileNode(model, xx);
		}
	}

	public bool HasChildren { get => _x.HasElements; }
	public IEnumerable<FileNode> Children { get => _x.Elements().Select(x => FromX(x)); }

	public bool IsFolder { get => _x.Name == "d"; }

	public string Name
	{
		get => _x.Attribute_("n");
	}

	/// <summary>
	/// Changes Name of this object and renames its file.
	/// Returns false if name is empty or fails to rename its file.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="notifyControl">true if called not from the control edit notification.</param>
	public bool Rename(string name, bool notifyControl)
	{
		name = Path_.CorrectFileName(name);
		if(name == Name) return true;

		_x.SetAttributeValue("n", name);
		if(notifyControl) UpdateControl(true);
		//_model.SaveCollectionLater();
		_model.SaveCollectionNow();
		return true;
	}

	public string GUID
	{
		get => _x.Attribute_("g");
	}

	string SetNewGUID()
	{
		var s = _CreateGUID();
		_x.SetAttributeValue("g", s);
		return s;
	}

	static string _CreateGUID()
	{
		var g = Guid.NewGuid();
		return Convert.ToBase64String(g.ToByteArray()).TrimEnd('=');
	}

#if TEST_MANY_COLUMNS
				public bool Checked
				{
					get;
					set;
				}

				public object Combo
				{
					get; set;
					//get { return "test"; }
					//set { }
				}

				public RegexOptions ComboEnum
				{
					get; set;
					//get { return "test"; }
					//set { }
				}

				public decimal Decimal
				{
					get; set;
					//get { return "10.5"; }
					//set { }
				}

				public int Integer
				{
					get; set;
					//get { return "10"; }
					//set { }
				}

				public decimal UpDown
				{
					get; set;
					//get { return "10"; }
					//set { }
				}
#endif

	/// <summary>
	/// Returns item path, like @"\Folder\Name" or @"\Name".
	/// It is without extension.
	/// </summary>
	public string ItemPath
	{
		get
		{
			XElement x = _x, xRoot = Root.Xml;
			var a = new Stack<string>();
			do {
				a.Push(x.Attribute_("n"));
				a.Push("\\");
				x = x.Parent;
			} while(x != xRoot);
			return string.Concat(a);
		}
	}

	public string FilePath
	{
		get
		{
			if(IsFolder) return _model.FilesDirectory + ItemPath;
			if(_x.Attribute_(out string path, "path")) return path;
			return _model.FilesDirectory + ItemPath + ".cs";
		}
	}

	public TreeViewAdv Control
	{
		get => _model.TV;
	}

	public FileNode Root
	{
		get => _model.Root;
	}

	public FileNode Parent
	{
		get => FromX(_x.Parent);
	}

	public FileNode Next
	{
		get => FromX(_x.NextElement_());
	}

	public FileNode Previous
	{
		get => FromX(_x.PreviousElement_());
	}

	public FileNode FindDescendantFile(string name)
	{
		return FromX(_x.Descendant_("f", "n", name, true));
	}

	public FileNode FindDescendantFolder(string name)
	{
		return FromX(_x.Descendant_("d", "n", name, true));
	}

	public FileNode FindDescendantFileOrFolder(string name)
	{
		return FindDescendantFile(name) ?? FindDescendantFolder(name);
	}

	/// <summary>
	/// Returns FileNode from x annotation.
	/// </summary>
	/// <param name="x">Can be null.</param>
	/// <returns></returns>
	internal static FileNode FromX(XElement x)
	{
		return x?.Annotation<FileNode>();
	}

	public bool CanMove(FileNode target, NodePosition pos)
	{
		//cannot move into/before/after self or into descendants
		for(XElement p = target._x, r = Root.Xml; p != r; p = p.Parent)
			if(p == _x) return false;

		switch(pos) {
		case NodePosition.Inside:
			if(!target.IsFolder) return false;
			break;
		case NodePosition.Before:
			if(Next == target) return false;
			break;
		case NodePosition.After:
			if(Previous == target) return false;
			break;
		}
		return true;
	}

	/// <summary>
	/// Moves this into, before or after target.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="pos"></param>
	internal void Move(FileNode target, NodePosition pos)
	{
		if(!CanMove(target, pos)) return;

		//move file
		var oldParent = Parent;
		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		if(newParent != oldParent) {
			//Print("move file");
			//TODO
		}

		//move XML element and notify control
		_model.OnNodeRemoved(this);
		_x.Remove();
		switch(pos) {
		case NodePosition.Inside:
			target._x.Add(_x);
			break;
		case NodePosition.Before:
			target._x.AddBeforeSelf(_x);
			break;
		case NodePosition.After:
			target._x.AddAfterSelf(_x);
			break;
		}
		_model.OnNodeInserted(this);
	}

	public void Select()
	{
		var c = Control;
		if(this == Root) c.ClearSelection();
		else c.SelectedNode = TreeNodeAdv;
	}

	public void Remove()
	{
		var c = Control;
		if(this == Root) {
			_x.RemoveNodes();
			_model.GuidMap.Clear();
			_model.OnStructureChanged();
		} else {
			_model.OnNodeRemoved(this);
			_model.GuidMap.Remove(this.GUID);
			_x.Remove();
		}
	}

	public void AddChild(FileNode f)
	{
		_AddChild(f, 0);
	}

	public void AddBefore(FileNode f)
	{
		_AddChild(f, 1);
	}

	public void AddAfter(FileNode f)
	{
		_AddChild(f, 2);
	}

	void _AddChild(FileNode f, int inBeforeAfter)
	{
		Debug.Assert(f.Parent == null);
		switch(inBeforeAfter) {
		case 1: _x.AddBeforeSelf(f._x); break;
		case 2: _x.AddAfterSelf(f._x); break;
		default: _x.Add(f._x); break;
		}
		_model.OnNodeInserted(f);
	}

	/// <summary>
	/// Call this to update control view when changed node data (text, image, checked, color, etc).
	/// Calls <see cref="FilesModel.OnNodeChanged"/>.
	/// </summary>
	/// <param name="justInvalidateRow">Just invalidate row rectangle. Much faster. Use when don't need to change row height.</param>
	public void UpdateControl(bool justInvalidateRow = false)
	{
		_model.OnNodeChanged(this, justInvalidateRow);
	}

	public TreeNodeAdv TreeNodeAdv
	{
		get
		{
			var c = Control;
			if(this == Root) return c.Root;
			return c.FindNode(TreePath, true);

			//note: don't use c.FindNodeByTag. It does not find nodes in folders that were never expanded, unless c.LoadOnDemand is false.
		}
	}

	internal TreePath TreePath
	{
		get
		{
			FileNode node = this, root = Root;
			if(node == root) return TreePath.Empty;
			var stack = new Stack<object>(); //use Stack because its ToArray creates array in reverse order
			while(node != root) {
				stack.Push(node);
				node = node.Parent;
			}
			return new TreePath(stack.ToArray());
		}
	}

	internal int Index
	{
		get
		{
			int i = 0;
			foreach(var t in _x.Parent.Elements()) {
				if(t == _x) return i;
				i++;
			}
			Debug.Assert(false);
			return -1;
		}
	}

	public override string ToString()
	{
		return Name;
	}
}
