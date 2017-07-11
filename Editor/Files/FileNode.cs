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

partial class FileNode
{
	FilesModel _model;
	XElement _x;

	public FilesModel Model { get => _model; }
	public XElement Xml { get => _x; }

	public FileNode(FilesModel model, XElement x, bool isRoot = false)
	{
		_model = model;
		_x = x;
		x.AddAnnotation(this);

		if(!isRoot) {
			var guid = this.GUID;
			g1:
			if(guid == null || guid.Length != 22) {
				var g = Guid.NewGuid();
				guid = Convert.ToBase64String(g.ToByteArray()).TrimEnd('=').Replace("/", "-");
				_x.SetAttributeValue("g", guid);
				_model.Save?.CollectionLater(); //_model.Save is null when importing this collection
			}
			try {
				_model.GuidMap.Add(guid, this);
			}
			catch(ArgumentException) {
				Output.Warning("Duplicate GUID of '" + this.ItemPath + "'. Creating new.");
				guid = null;
				goto g1;
			}
		}

		foreach(var xx in x.Elements()) {
			new FileNode(model, xx);
		}
	}

	public bool HasChildren { get => _x.HasElements; }
	public IEnumerable<FileNode> Children { get => _x.Elements().Select(x => FromX(x)); }

	//public bool IsFolder { get => _x.Name == "d"; }
	public bool IsFolder { get => _x.Name != "f"; } //"d" or "files" (Root)

	/// <summary>
	/// Returns true if f is not null and this is its ancestor.
	/// </summary>
	public bool ContainsDescendant(FileNode f)
	{
		return f != null && f._x.Ancestors().Contains(_x);
	}

	public string Name
	{
		get => _x.Attribute_("n");
	}

	public string GUID
	{
		get => _x.Attribute_("g");
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
	/// Returns item path in XML, like @"\Folder\Name.cs" or @"\Name.cs".
	/// </summary>
	public string ItemPath
	{
		get
		{
			XElement x = _x, xRoot = Root.Xml;
			var a = new Stack<string>();
			while(x != xRoot) {
				a.Push(x.Attribute_("n"));
				a.Push("\\");
				x = x.Parent;
			}
			return string.Concat(a);
		}
	}

	/// <summary>
	/// Gets file path.
	/// </summary>
	public string FilePath
	{
		get
		{
			if(this == Root) return _model.FilesDirectory;
			if(IsLink(out string path)) return path;
			return _model.FilesDirectory + ItemPath;
		}
	}

	public bool IsLink() { return _x.HasAttribute_("path"); }

	public bool IsLink(out string targetPath) { return _x.Attribute_(out targetPath, "path"); }

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

	//fileFolder: 0 any, 1 file, 2 folder
	FileNode _FindDescendant(string name, int fileFolder)
	{
		if(Empty(name)) return null;
		XElement x;
		if(name[0] == '\\') {
			var a = name.Split_("\\", StringSplitOptions.RemoveEmptyEntries);
			if(a.Length == 0) return null;
			x = _x;
			for(int i = 0; i < a.Length; i++) {
				var s = a[i];
				if(i == a.Length - 1) {
					var xx = x;
					x = x.Element_((fileFolder == 2) ? "d" : "f", "n", s, true);
					if(x == null && fileFolder == 0) x = xx.Element_("d", "n", s, true);
				} else {
					x = x.Element_("d", "n", s, true);
				}
				if(x == null) break;
			}
		} else {
			g2: x = _x.Descendant_((fileFolder == 2) ? "d" : "f", "n", name, true);
			if(x == null && fileFolder == 0) { fileFolder = 2; goto g2; }
		}
		return FromX(x);

		//FUTURE: support XPath: x = _x.XPathSelectElement(name);
	}

	/// <summary>
	/// Finds descendant by name or @"\relative path".
	/// </summary>
	/// <param name="name">Name like "name.cs" or relative path like @"\name.cs" or @"\subfolder\name.cs".</param>
	public FileNode FindDescendantFileOrFolder(string name)
	{
		return _FindDescendant(name, 0);
	}

	/// <summary>
	/// Finds descendant file by name or @"\relative path".
	/// </summary>
	/// <param name="name">Name like "name.cs" or relative path like @"\name.cs" or @"\subfolder\name.cs".</param>
	public FileNode FindDescendantFile(string name)
	{
		return _FindDescendant(name, 1);
	}

	/// <summary>
	/// Finds descendant folder by name or @"\relative path".
	/// </summary>
	/// <param name="name">Name like "name.cs" or relative path like @"\name.cs" or @"\subfolder\name.cs".</param>
	public FileNode FindDescendantFolder(string name)
	{
		return _FindDescendant(name, 2);
	}

	/// <summary>
	/// Returns FileNode from x annotation.
	/// </summary>
	/// <param name="x">Can be null.</param>
	internal static FileNode FromX(XElement x)
	{
		return x?.Annotation<FileNode>();
	}

	/// <summary>
	/// Unselects all and selects this.
	/// If this is root, just unselects all.
	/// </summary>
	public void SelectSingle()
	{
		var c = Control;
		if(this == Root) c.ClearSelection();
		else c.SelectedNode = TreeNodeAdv;
	}

	public void AddChildOrSibling(FileNode f, NodePosition inBeforeAfter)
	{
		Debug.Assert(f.Parent == null);
		switch(inBeforeAfter) {
		case NodePosition.Before: _x.AddBeforeSelf(f._x); break;
		case NodePosition.After: _x.AddAfterSelf(f._x); break;
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

	/// <summary>
	/// Gets control's object of this item.
	/// </summary>
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

	/// <summary>
	/// Creates TreePath used to communicate with the control.
	/// </summary>
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

	/// <summary>
	/// Returns index of this XML element in parent.
	/// Returns -1 if this is Root.
	/// </summary>
	internal int Index
	{
		get
		{
			var p = _x.Parent;
			if(p == null) { Debug.Assert(this == Root); return -1; }
			int i = 0;
			foreach(var t in p.Elements()) {
				if(t == _x) return i;
				i++;
			}
			Debug.Assert(false);
			return -1;
		}
	}

	/// <summary>
	/// Returns Name.
	/// </summary>
	public override string ToString()
	{
		return Name;
	}
}
