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
	/// <summary>
	/// Changes Name of this object and renames its file (if not link).
	/// Returns false if name is empty or fails to rename its file.
	/// </summary>
	/// <param name="name">
	/// Name, like "New name.cs" or "New name".
	/// If no extension, and it is not folder, adds previous extension if it's an existing file type.
	/// If invalid filename, replaces invalid characters etc.
	/// </param>
	/// <param name="notifyControl">true if called not from the control edit notification.</param>
	public bool FileRename(string name, bool notifyControl)
	{
		name = Path_.CorrectFileName(name);
		if(name.IndexOf('.') < 0 && !IsFolder) {
			var ext = Path_.GetExtension(Name);
			if(ext.Length > 0 && Registry_.CanOpen(ext, Registry.ClassesRoot)) name += ext;
		}
		if(name == Name) return true;

		if(!this.IsLink()) {
			try {
				Files.Rename(this.FilePath, name, IfExists.Fail);
			}
			catch(Exception ex) { Print(ex.Message); return false; }
			//if(IsLink(out string sp)) _x.SetAttributeValue("path", Path_.GetDirectoryPath(sp, true) + name); //if we would rename the taget file
		}

		_x.SetAttributeValue("n", name);
		if(notifyControl) UpdateControl(true);
		_model.Save.CollectionLater();
		return true;
	}

	/// <summary>
	/// Returns true if can move the XML element into the specified position.
	/// For example, cannot move parent into child etc.
	/// Does not check whether can move the file.
	/// </summary>
	public bool CanMove(FileNode target, NodePosition pos)
	{
		//cannot move into self or descendants
		for(XElement p = target._x, r = Root.Xml; p != r; p = p.Parent)
			if(p == _x) return false;

		//cannot move into a non-folder or before/after self
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
	/// Also moves file if need.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="pos"></param>
	internal bool FileMove(FileNode target, NodePosition pos)
	{
		if(!CanMove(target, pos)) return false;

		//move file or directory
		if(!IsLink()) {
			var oldParent = Parent;
			var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
			if(newParent != oldParent) {
				try {
					Files.Move(this.FilePath, newParent.FilePath + "\\" + Name, IfExists.Fail);
				}
				catch(Exception ex) { Print(ex.Message); return false; }
			}
		}

		//move XML element
		_model.OnNodeRemoved(this);
		_x.Remove();
		_Common_MoveCopyNew(target, pos);
		return true;
	}

	void _Common_MoveCopyNew(FileNode target, NodePosition pos)
	{
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

		_model.Save.CollectionLater();
	}

	/// <summary>
	/// Copies this into, before or after target.
	/// Also copies file if need.
	/// Returns the copy, or null if fails.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="pos"></param>
	/// <param name="newModel">Used when importing collection.</param>
	internal FileNode FileCopy(FileNode target, NodePosition pos, FilesModel newModel = null)
	{
		//clone XML (with attributes and descendants) and set unique GUIDs
		var xNew = new XElement(_x);
		foreach(var v in xNew.DescendantsAndSelf()) v.SetAttributeValue("g", null); //will auto-create

		//create unique name
		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		string name = CreateNameUniqueInFolder(newParent, Name, this.IsFolder);
		xNew.SetAttributeValue("n", name);

		//copy file or directory
		if(!IsLink()) {
			try {
				Files.Copy(this.FilePath, newParent.FilePath + "\\" + name, IfExists.Fail);
			}
			catch(Exception ex) { Print(ex.Message); return null; }
		}

		//create new FileNode and insert at the specified place
		var f = new FileNode(newModel ?? _model, xNew);
		f._Common_MoveCopyNew(target, pos);
		return f;
	}

	/// <summary>
	/// Creates new item in, before or after target.
	/// Also creates new file or directory.
	/// Returns the new item, or null if fails.
	/// Does not open it.
	/// </summary>
	/// <param name="model"></param>
	/// <param name="target"></param>
	/// <param name="pos"></param>
	/// <param name="template"></param>
	/// <param name="fileName">Suggested fileName. Will be changed if exists in that folder.</param>
	public static FileNode NewItem(FilesModel model, FileNode target, NodePosition pos, NewItemTemplate template, string fileName = null)
	{
		bool isFolder = false, isProject = false;
		switch(template) {
		case NewItemTemplate.Folder: isFolder = true; break;
		case NewItemTemplate.AppProject:
		case NewItemTemplate.LibraryProject: isFolder = isProject = true; break;
		}

		var xNew = new XElement(isFolder ? "d" : "f");

		//create unique name
		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		//if(fileName == null) fileName = template.ToString() + (isFolder ? " 1" : (template == NewItemTemplate.Script ? " 1.csx" : " 1.cs"));
		if(fileName == null) fileName = template.ToString() + ((isFolder || template == NewItemTemplate.Script) ? " 1" : " 1.cs");
		string name = CreateNameUniqueInFolder(newParent, fileName, isFolder);
		xNew.SetAttributeValue("n", name);
		if(isProject) xNew.SetAttributeValue("project", template == NewItemTemplate.AppProject ? "app" : "library");

		//create empty file or folder
		try {
			var path = newParent.FilePath + "\\" + name;
			if(isFolder) Files.CreateDirectory(path);
			else File.WriteAllText(path, _Template(template));
		}
		catch(Exception ex) { Print(ex.Message); return null; }

		//create new FileNode and insert at the specified place
		var f = new FileNode(model, xNew);
		f._Common_MoveCopyNew(target, pos);

		if(isProject) {
			template = template == NewItemTemplate.AppProject ? NewItemTemplate.AppClass : NewItemTemplate.Class;
			return NewItem(model, f, NodePosition.Inside, template);
		}
		return f;
	}

	public static string CreateNameUniqueInFolder(FileNode folder, string fromName, bool forFolder)
	{
		if(!_Exists(fromName)) return fromName;

		string ext = null;
		if(!forFolder) {
			int i = fromName.LastIndexOf('.');
			if(i >= 0) { ext = fromName.Substring(i); fromName = fromName.Remove(i); }
		}
		fromName = fromName.RegexReplace_(@" \d+$", "");
		for(int i = 2; ; i++) {
			var s = fromName + " " + i + ext;
			if(!_Exists(s)) return s;
		}

		bool _Exists(string s)
		{
			if(null != folder.Xml.Element_(null, "n", s, true)) return true;
			if(Files.ExistsAsAny(folder.FilePath + "\\" + s)) return true; //orphaned file?
			return false;
		}
	}

	/// <summary>
	/// Deletes this item and optionally its file. If folder - deletes descendants too.
	/// Before deleting, calls CloseFile (for descendants too).
	/// By default does not delete the link target file.
	/// </summary>
	public bool FileDelete(bool tryRecycleBin = true, bool doNotDeleteFile = false, bool canDeleteLinkTarget = false)
	{
		var e = _x.DescendantsAndSelf().Select(v => FromX(v));

		foreach(var f in e) _model.CloseFile(f);

		if(!doNotDeleteFile && (canDeleteLinkTarget || !IsLink())) {
			try {
				Files.Delete(this.FilePath, tryRecycleBin);
			}
			catch(Exception ex) { Print(ex.Message); return false; }
		}

		foreach(var f in e) _model.GuidMap.Remove(f.GUID);
		_model.OnNodeRemoved(this);
		_x.Remove();

		_model.Save.CollectionLater();
		return true;
	}

	#region templates

	static string _Template(NewItemTemplate template) //TODO: make editable etc
	{
		string r = "";
		switch(template) {
		case NewItemTemplate.Class:
			r = Compiler.DefaultUsingsForTemplate + Project.Properties.Resources.TemplateClass;
			break;
		case NewItemTemplate.AppClass:
			r = Compiler.DefaultUsingsForTemplate + Project.Properties.Resources.TemplateApp;
			break;
		}
		return r;
	}

	public enum NewItemTemplate
	{
		Script,
		Class,
		AppClass,
		Folder,
		AppProject,
		LibraryProject,
	}

	#endregion

}
