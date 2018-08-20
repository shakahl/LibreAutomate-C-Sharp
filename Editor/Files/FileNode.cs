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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
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
			var guid = this.Guid;
			g1:
			if(guid == null || guid.Length != 22) {
				var g = System.Guid.NewGuid();
				guid = Convert_.GuidToBase64Filename(g);
				_x.SetAttributeValue("g", guid);
				_model.Save?.CollectionLater(); //_model.Save is null when importing this collection
			}
			try {
				_model.GuidMap.Add(guid, this);
			}
			catch(ArgumentException) {
				PrintWarning("Duplicate GUID of '" + this.ItemPath + "'. Creating new.");
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

	/// <summary>
	/// GUID as Base64 string, as it is stored in collection file.
	/// </summary>
	public string Guid
	{
		get => _x.Attribute_("g");
	}

	/// <summary>
	/// Gets <see cref="Guid"/> converted to hex string.
	/// </summary>
	public unsafe string GuidHex
	{
		//get => Convert_.HexEncode(Convert_.Base64Decode(GUID));
		get
		{
			var g = GuidStruct;
			return Convert_.HexEncode(&g, sizeof(Guid));
		}
	}

	/// <summary>
	/// Gets <see cref="Guid"/> converted to <b>Guid</b> struct.
	/// </summary>
	public unsafe Guid GuidStruct
	{
		get
		{
			var s = Guid;
			Debug.Assert(s.Length == 22);
			fixed (char* p = s) {
				Guid g;
				Convert_.Base64Decode(p, s.Length, &g, sizeof(Guid));
				return g;
			}
		}
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
	/// Returns item path in collection and XML, like @"\Folder\Name.cs" or @"\Name.cs".
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

	/// <summary>
	/// Finds descendant file or folder by name or @"\relative path".
	/// Returns null if not found; also if name is null/"".
	/// </summary>
	/// <param name="name">Name like "name.cs" or relative path like @"\name.cs" or @"\subfolder\name.cs".</param>
	/// <param name="folder">true - folder, false - file, null - any.</param>
	public FileNode FindDescendant(string name, bool? folder)
	{
		if(Empty(name)) return null;
		if(name[0] == '\\') return _FindRelative(name, folder);
		var x = _x.Descendant_(folder.GetValueOrDefault() ? "d" : "f", "n", name, true);
		if(x == null && !folder.HasValue) x = _x.Descendant_("d", "n", name, true);
		return FromX(x);

		//FUTURE: support XPath: x = _x.XPathSelectElement(name);
	}

	FileNode _FindRelative(string name, bool? folder)
	{
		if(name.StartsWith_(@"\\")) return null;
		var x = _x; int lastSegEnd = -1;
		foreach(var seg in name.Segments_(@"\", SegFlags.NoEmpty)) {
			var s = seg.Value;
			if((lastSegEnd = seg.EndOffset) == name.Length) {
				var xx = x;
				x = x.Element_(folder.GetValueOrDefault() ? "d" : "f", "n", s, true);
				if(x == null && !folder.HasValue) x = xx.Element_("d", "n", s, true);
			} else {
				x = x.Element_("d", "n", s, true);
			}
			if(x == null) return null;
		}
		if(lastSegEnd != name.Length) return null; //prevents finding when name is "" or @"\" or @"xxx\".
		return FromX(x);
	}

	/// <summary>
	/// Finds file or folder by name or path relative to: this folder, parent folder (if this is file) or root (if relativePath starts with @"\").
	/// Returns null if not found; also if name is null/"".
	/// </summary>
	/// <param name="relativePath">Examples: "name", @"subfolder\name", @".\subfolder\name", @"..\parent\name", @"\root path\name".</param>
	/// <param name="folder">true - folder, false - file, null - any.</param>
	public FileNode FindRelative(string relativePath, bool? folder)
	{
		if(!IsFolder) return Parent.FindRelative(relativePath, folder);
		var s = relativePath;
		if(Empty(s)) return null;
		FileNode p = this;
		if(s[0] == '\\') p = Root;
		else if(s[0] == '.') {
			int i = 0;
			for(; s.EqualsAt_(i, @"..\"); i += 3) { p = p.Parent; if(p == null) return null; }
			if(i == 0 && s.StartsWith_(@".\")) i = 2;
			if(i != 0) {
				if(i == s.Length) return (p == Root || !(folder ?? true)) ? null : p;
				s = s.Substring(i);
			}
		}
		return p._FindRelative(s, folder);
	}

	/// <summary>
	/// Finds ancestor (including self) project folder and its main file.
	/// If folder not found, returns false; sets both out variables = null.
	/// If folder found but file not, returns null; sets folder = valid, file = null.
	/// </summary>
	public bool FindProject(out FileNode folder, out FileNode main)
	{
		folder = main = null;
		for(var x = IsFolder ? _x : _x.Parent; x != Root.Xml; x = x.Parent) {
			if(!x.Attribute_(out string guid, "project")) continue;
			folder = FromX(x);
			x = x.Descendant_("f", "g", guid);
			if(x == null) break;
			main = FromX(x);
			return true;
		}
		return false;

		//note: could use Model.FindByGUID instead. It can be faster when the main is somewhere far and deep, because uses GuidMap.
		//	But in most cases the main will be the first node or near.
	}

	/// <summary>
	/// Gets all .cs files of this project folder.
	/// </summary>
	/// <param name="fSkip">Skip this file.</param>
	public IEnumerable<FileNode> EnumProjectFiles(FileNode fSkip = null)
	{
		foreach(var v in _x.Descendants("f")) {
			//Print(v);
			var f = FromX(v);
			if(f != fSkip && f.Name.EndsWith_(".cs", true)) yield return f;
		}
	}

	public bool IsProjectFolder => IsFolder && _x.HasAttribute_("project");

	/// <summary>
	/// Gets whether this is folder, script, .cs or other file.
	/// </summary>
	public ENodeType NodeType
	{
		get
		{
			if(IsFolder) return ENodeType.Folder;
			var s = Name;
			if(s.EndsWith_(".cs", true)) return ENodeType.CS;
			if(s.IndexOf('.') < 0) return ENodeType.Script;
			return ENodeType.Other;
		}
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

enum ENodeType
{
	Folder,
	Script,
	CS,
	Other,
}
