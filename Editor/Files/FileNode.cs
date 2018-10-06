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
using Au.Compiler;

partial class FileNode :ICollectionFile
{
	//[Flags]
	//enum _NodeFlags
	//{
	//	Deleted=1,
	//	InDisposedColl=2,
	//	//Link=4,
	//}

	FilesModel _model;
	XElement _x;
	long _id;
	//_NodeFlags _nflags;

	public FilesModel Model => _model;
	public XElement Xml => _x;

	public FileNode(FilesModel model, XElement x, bool isRoot = false)
	{
		_model = model;
		_x = x;
		x.AddAnnotation(this);

		if(!isRoot) {
			if(_x.Name != XN.d) {
				var name = Name;
				if(Empty(name)) { Debug_.Print(_x); return; }
				if(name.EndsWith_(".cs", true)) NodeType = ENodeType.CS;
				else if(name.IndexOf('.') < 0) NodeType = ENodeType.Script;
				else NodeType = ENodeType.Other;

				//if(_x.HasAttribute_(s_xnPath)) _nflags |= _NodeFlags.Link;
			}

			_id = this.IdString?.ToLong_() ?? 0;
			g1:
			if(_id == 0) { //probably new item
				var m = _model.IdMap;
				_id = (m.Count > 0 ? m.Keys.Max() : 0) + 1; //TODO
				_x.SetAttributeValue(XN.i, _id.ToString());
				_model.Save?.CollectionLater(); //_model.Save is null when importing this collection
			}
			try {
				_model.IdMap.Add(_id, this);
			}
			catch(ArgumentException) {
				PrintWarning($"Duplicate id of '{ItemPath}'. Creating new.");
				_id = 0;
				goto g1;
			}
		}

		if(IsFolder) {
			foreach(var xx in x.Elements()) {
				new FileNode(model, xx);
			}
		}
	}

	/// <summary>
	/// Gets whether this is folder, script, .cs or other file.
	/// </summary>
	public ENodeType NodeType { get; private set; }

	/// <summary>
	/// true if folder or root.
	/// </summary>
	public bool IsFolder => NodeType == ENodeType.Folder;

	/// <summary>
	/// true if <see cref="NodeType"/> is Script or CS.
	/// </summary>
	public bool IsCodeFile => NodeType == ENodeType.Script || NodeType == ENodeType.CS;

	/// <summary>
	/// File name with extension.
	/// </summary>
	public string Name => _x.Attribute_(XN.n);

	public long Id => _id;

	/// <summary>
	/// <see cref="Id"/> as string.
	/// </summary>
	public string IdString => _x.Attribute_(XN.i);

	/// <summary>
	/// Formats string "&lt;<see cref="Id"/>.<see cref="FilesModel.CollectionSN"/>&gt;".
	/// Such string can be passed to <see cref="FilesModel.Find"/>.
	/// </summary>
	public string IdStringWithColl => $"<{IdString}.{_model.CollectionSN}>";

	/// <summary>
	/// true if is external file, ie not in this collection folder.
	/// </summary>
	public bool IsLink() => _x.HasAttribute_(XN.path);

	/// <summary>
	/// true if is external file, ie not in this collection folder.
	/// </summary>
	public bool IsLink(out string targetPath) => _x.Attribute_(out targetPath, XN.path);

	public TreeViewAdv Control => _model.TV;

	/// <summary>
	/// Returns FileNode from x annotation.
	/// </summary>
	/// <param name="x">Can be null.</param>
	internal static FileNode FromX(XElement x) => x?.Annotation<FileNode>();

	public FileNode Root => _model.Root;

	public FileNode Parent => FromX(_x.Parent);

	public FileNode Next => FromX(_x.NextElement_());

	public FileNode Previous => FromX(_x.PreviousElement_());

	public FileNode FirstChild => FromX(_x.Elements().FirstOrDefault());

	public bool HasChildren => _x.HasElements;

	public IEnumerable<FileNode> Children => _x.Elements().Select(x => FromX(x));

	public IEnumerable<FileNode> Descendants(bool andSelf)
	{
		var e = andSelf ? _x.DescendantsAndSelf() : _x.Descendants();
		return e.Select(v => FromX(v));
	}

	/// <summary>
	/// Returns true if f is a descendant of this. Can be null.
	/// </summary>
	public bool ContainsDescendant(FileNode f) => f?._x.Ancestors().Contains(_x) ?? false;

	public bool IsDeleted
	{
		get
		{
			for(XElement x = _x, xRoot = Root.Xml; x != xRoot;) { //can be in deleted folder, then this.Parent!=null
				x = x.Parent;
				if(x == null) return true;
			}
			return false;
		}
	}

	/// <summary>
	/// true if is in current collection and not deleted.
	/// </summary>
	public bool IsInCurrentCollection => _model == Program.Model && !IsDeleted;
	//note: currently not used and not tested.

	/// <summary>
	/// Returns item path in collection and XML, like @"\Folder\Name.cs" or @"\Name.cs".
	/// </summary>
	public string ItemPath
	{
		get
		{
			XElement x = _x, xRoot = Root.Xml;
			var a = new Stack<string>(); //SHOULDDO: optimize; eg use common Stack
			while(x != xRoot) {
				a.Push(x.Attribute_(XN.n));
				a.Push("\\");
				x = x.Parent;
				if(x == null) return null; //deleted
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
			if(IsDeleted) return null;
			if(IsLink(out string path)) return path;
			return _model.FilesDirectory + ItemPath;
		}
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
		var x = _x.Descendant_(folder.GetValueOrDefault() ? XN.d : XN.f, XN.n, name, true);
		if(x == null && !folder.HasValue) x = _x.Descendant_(XN.d, XN.n, name, true);
		return FromX(x);

		//FUTURE: support "<id>comments", "<collectionId.id>comments"
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
				x = x.Element_(folder.GetValueOrDefault() ? XN.d : XN.f, XN.n, s, true);
				if(x == null && !folder.HasValue) x = xx.Element_(XN.d, XN.n, s, true);
			} else {
				x = x.Element_(XN.d, XN.n, s, true);
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
	/// Finds all descendant files (and not folders) that have the specified name.
	/// Returns empty array if not found.
	/// </summary>
	/// <param name="name">File name. If starts with backslash, works like <see cref="FindDescendant"/>.</param>
	public FileNode[] FindAllDescendantFiles(string name)
	{
		if(!Empty(name)) {
			if(name[0] == '\\') {
				var f1 = _FindRelative(name, false);
				if(f1 != null) return new FileNode[] { f1 };
			} else {
				return _x.Descendants_(XN.f, XN.n, name, true).Select(x => FromX(x)).ToArray();
			}
		}
		return Array.Empty<FileNode>();
	}

	/// <summary>
	/// Finds ancestor (including self) project folder and its main file.
	/// If both found, sets folder and main and returns true. If some not found, sets folder=null, main=null, and returns false.
	/// </summary>
	public bool FindProject(out FileNode folder, out FileNode main)
	{
		folder = main = null;
		XElement xRoot = Root.Xml, x = IsFolder ? _x : _x.Parent;
		for(; x != xRoot && x != null; x = x.Parent) {
			var t = FromX(x);
			if(!t.IsProjectFolder(out main)) continue;
			if(main == null) break;
			folder = t;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets all .cs files of this project folder.
	/// </summary>
	/// <param name="fSkip">Skip this file.</param>
	public IEnumerable<FileNode> EnumProjectFiles(FileNode fSkip = null)
	{
		foreach(var v in _x.Descendants(XN.f)) {
			//Print(v);
			var f = FromX(v);
			if(f != fSkip && f.NodeType == ENodeType.CS) yield return f;
		}
	}

	/// <summary>
	/// Returns true if this is a folder and Name starts with '@'.
	/// </summary>
	/// <param name="main">Receives the main code file or null. It is the first direct child code file.</param>
	public bool IsProjectFolder(out FileNode main)
	{
		main = null;
		if(IsProjectFolder()) {
			foreach(var x in _x.Elements()) {
				var f = FromX(x);
				if(f.IsCodeFile) { main = f; return true; }
			}
		}
		return false;
	}

	/// <summary>
	/// Returns true if this is a folder and Name starts with '@'.
	/// </summary>
	public bool IsProjectFolder() => IsFolder && Name[0] == '@';

	/// <summary>
	/// Unselects all and selects this. Does not open document.
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
	/// Call this to update/redraw control row view when changed node data (text, image, checked, color, etc) and don't need to change row height.
	/// </summary>
	public void UpdateControlRow() => Control.UpdateNode(TreeNodeAdv);

	/// <summary>
	/// Call this to update/redraw control view when changed node data (text, image, etc) and need to change row height.
	/// </summary>
	public void UpdateControlRowHeight() => _model.OnNodeChanged(this);

	/// <summary>
	/// Gets control's object of this item.
	/// </summary>
	public TreeNodeAdv TreeNodeAdv
	{
		get
		{
			var c = Control;
			if(this == Root) return c.Root;
			var tp = TreePath;
			if(tp == null) return null; //deleted node
			return c.FindNode(tp, true);

			//CONSIDER: cache in a field. But can be difficult to manage. Currently this func is not called frequently.
			//note: don't use c.FindNodeByTag. It does not find in never-expanded folders, unless c.LoadOnDemand is false. And slower.
		}
	}

	/// <summary>
	/// Creates TreePath used to communicate with the control.
	/// </summary>
	internal TreePath TreePath
	{
		get
		{
			int n = 0;
			for(XElement x = _x, root = Root.Xml; x != root; x = x.Parent, n++)
				if(x == null) return null; //deleted node
			if(n == 0) return TreePath.Empty; //root
			var a = new object[n];
			for(var x = _x; n > 0;) { a[--n] = FromX(x); x = x.Parent; }
			return new TreePath(a);
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
	/// Gets text from file or editor.
	/// </summary>
	/// <param name="editorTextIfCurrent">If this is current item, gets editor text.</param>
	public string GetText(bool editorTextIfCurrent = false)
	{
		if(editorTextIfCurrent && this == _model.CurrentFile) {
			return Panels.Editor.ActiveDoc.Text;
		}
		return File_.LoadText(FilePath);
	}

	public Bitmap GetIcon(bool expandedFolder = false)
	{
		string k;
		if(IsDeleted) {
			k = "delete";
		} else {
			switch(NodeType) {
			case ENodeType.Folder:
				//if(IsProjectFolder()) k = "project"; else //rejected. Name starts with '@' character, it's visible without a different icon.
				if(expandedFolder) k = "folderOpen";
				else k = "folder";
				break;
			case ENodeType.Script: k = "fileScript"; break;
			case ENodeType.CS: k = "fileClass"; break;
			default:
				if(!IsLink(out string s)) s = FilePath;
				return IconCache.GetImage(s, true);
			}
		}
		return EResources.GetImageUseCache(k);
	}

	public static Icon_.ImageCache IconCache = new Icon_.ImageCache(Folders.ThisAppDataLocal + @"fileIconCache.xml", (int)IconSize.SysSmall);

	/// <summary>
	/// Formats &lt;open&gt; link tag to open this file.
	/// </summary>
	public string LinkTag => $"<open \"{IdStringWithColl}\">{Name}<>";

	/// <summary>
	/// Returns Name.
	/// </summary>
	public override string ToString()
	{
		return Name;
	}

	#region Au.Compiler.ICollectionFile

	public bool IcfIsScript => NodeType == ENodeType.Script;

	public ICollectionFiles IcfCollection => _model;

	public ICollectionFile IcfFindRelative(string relativePath, bool? folder) => FindRelative(relativePath, folder);

	public IEnumerable<ICollectionFile> IcfEnumProjectFiles(ICollectionFile fSkip = null) => EnumProjectFiles(fSkip as FileNode);

	public bool IcfFindProject(out ICollectionFile folder, out ICollectionFile main)
	{
		if(!FindProject(out var fo, out var ma)) { folder = main = null; return false; }
		folder = fo; main = ma;
		return true;
	}

	#endregion

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
}

enum ENodeType
{
	Folder,
	Script,
	CS,
	Other,
}

/// <summary>
/// Static XName instances for XML tag/attribute names used in this app.
/// XML functions much faster with XName than with string. When with string, .NET looks for it in its static hashtable.
/// The variable names match the strings.
/// </summary>
class XN
{
	public static readonly XName f = "f", d = "d", n = "n", i = "i", path = "path", run = "run";
}
