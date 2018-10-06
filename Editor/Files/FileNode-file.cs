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
using LiteDB;

partial class FileNode
{
	/// <summary>
	/// Changes Name of this object and renames its file (if not link).
	/// Returns false if name is empty or fails to rename its file.
	/// </summary>
	/// <param name="name">
	/// Name, like "New name.cs" or "New name".
	/// If not folder: if no extension, adds previous extension; else if new name would change type script/cs/other, corrects name to prevent it.
	/// If invalid filename, replaces invalid characters etc.
	/// </param>
	/// <param name="notifyControl">true if called not from the control edit notification.</param>
	public bool FileRename(string name, bool notifyControl)
	{
		name = Path_.CorrectFileName(name);
		if(!IsFolder) {
			if(name.IndexOf('.') < 0) {
				var ext = Path_.GetExtension(Name);
				if(ext.Length > 0) name += ext;
			} else if(Name.IndexOf('.') < 0) {
				name = name.Replace('.', ';');
			} else if(Name.EndsWith_(".cs", true) != name.EndsWith_(".cs", true)) {
				name += Path_.GetExtension(Name);
			}
		}
		if(name == Name) return true;

		if(!this.IsLink()) {
			try {
				File_.Rename(this.FilePath, name, IfExists.Fail);
			}
			catch(Exception ex) { Print(ex.Message); return false; }
			//if(IsLink(out string sp)) _x.SetAttributeValue(s_xnPath, Path_.GetDirectoryPath(sp, true) + name); //if we would rename the target file
		}

		_x.SetAttributeValue(XN.n, name);
		if(notifyControl) UpdateControlRow();
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
					File_.Move(this.FilePath, newParent.FilePath + "\\" + Name, IfExists.Fail);
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
		//clone XML (with attributes and descendants) and set unique ids
		var xNew = new XElement(_x);
		foreach(var v in xNew.DescendantsAndSelf()) v.SetAttributeValue(XN.i, null); //will auto-create new ids

		//create unique name
		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		string name = CreateNameUniqueInFolder(newParent, Name, this.IsFolder);
		xNew.SetAttributeValue(XN.n, name);

		//copy file or directory
		if(!IsLink()) {
			try {
				File_.Copy(this.FilePath, newParent.FilePath + "\\" + name, IfExists.Fail);
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
	/// <param name="target">Can be null, then adds at the top.</param>
	/// <param name="pos"></param>
	/// <param name="template">
	/// Item type and template.
	/// Can be filename or relative path of a file or folder from the Templates folder.
	/// Examples: "Script", "Class.cs", "Text.txt", "Subfolder", "Subfolder\File.cs".
	/// If "Folder", creates simple folder. If the file/folder does not exist, creates script or class (.cs) or other file.
	/// If folder name starts with '@', creates multi-file project from template if exists. To sort the main code file first, its name can start with '!' character.
	/// Files without extension are considered C# scripts.
	/// </param>
	/// <param name="name">If not null, creates with this name (made unique). Else gets name from template. In any case, makes unique name.</param>
	public static FileNode NewItem(FilesModel model, FileNode target, NodePosition pos, string template, string name = null)
	{
		Debug.Assert(!Empty(template)); if(Empty(template)) return null;

		if(target == null) {
			var root = model.Root;
			target = root.FirstChild;
			if(target != null) pos = NodePosition.Before; else { target = root; pos = NodePosition.Inside; }
		}
		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;

		int i;
		string text = "";
		bool isFolder = template == "Folder";
		if(!isFolder) {
			string templFile = s_dirTemplatesBS + template;
			switch(File_.ExistsAs(templFile, true)) {
			case FileDir.Directory: isFolder = true; break;
			case FileDir.File: text = _NI_GetTemplateText(templFile, template, newParent); break;
			}
		}

		var xNew = new XElement(isFolder ? XN.d : XN.f);

		//create unique name
		if(name == null) {
			name = Path_.GetFileName(template);
			//let unique names start from 1
			if(!isFolder && (i = name.LastIndexOf('.')) > 0) name = name.Insert(i, "1"); else name += "1";
		}
		name = CreateNameUniqueInFolder(newParent, name, isFolder);
		xNew.SetAttributeValue(XN.n, name);

		//create file or folder
		try {
			var path = newParent.FilePath + "\\" + name;
			if(isFolder) File_.CreateDirectory(path);
			else File_.Save(path, text);
		}
		catch(Exception ex) { Print(ex.Message); return null; }

		//create new FileNode and insert at the specified place
		var f = new FileNode(model, xNew);
		f._Common_MoveCopyNew(target, pos);

		if(isFolder && Path_.GetFileName(template)[0] == '@') {
			_NI_FillProjectFolder(model, f, s_dirTemplatesBS + template);
		}
		return f;
	}
	static string s_dirTemplatesBS = Folders.ThisAppBS + @"Templates\";

	static void _NI_FillProjectFolder(FilesModel model, FileNode fnParent, string dirParent)
	{
		foreach(var v in File_.EnumDirectory(dirParent, FEFlags.UseRawPath | FEFlags.SkipHiddenSystem)) {
			bool isFolder = v.IsDirectory;
			var name = v.Name;
			if(isFolder && name[0] == '@') continue; //error, project in project
			if(name[0] == '!' && name.Length > 1) name = name.Substring(1); //!name can be used to make the file sorted first; then it will become the main file of project.
			string template = v.FullPath.Substring(s_dirTemplatesBS.Length);
			var f = NewItem(model, fnParent, NodePosition.Inside, template, name);
			if(isFolder) _NI_FillProjectFolder(model, f, v.FullPath);
		}
	}

	static string _NI_GetTemplateText(string templFile, string template, FileNode newParent)
	{
		string s = File_.LoadText(templFile);
		//replace //"#include file" with text of file from "include" subfolder
		s = s.RegexReplace_(@"(?m)^//#include +(.+)$", m =>
		{
			var si = s_dirTemplatesBS + @"include\" + m[1];
			if(File_.ExistsAsFile(si)) return File_.LoadText(si);
			return null;
		});

		//when adding classes to library project, if the main file contains a namespace, add that namespace in the new file too.
		if(template == "Class.cs" && newParent.FindProject(out var projFolder, out var projMain)) {
			var rx = @"(?m)^namespace [\w\.]+";
			if(!s.RegexIsMatch_(rx) && projMain.GetText(true).RegexMatch_(rx, 0, out var ns)) {
				s = s.RegexReplace_(@"(?ms)^public class .+\}", ns + "\r\n{\r\n$0\r\n}", 1);
			}
		}

		return s;
	}

	public static string CreateNameUniqueInFolder(FileNode folder, string fromName, bool forFolder)
	{
		if(!_Exists(fromName)) return fromName;

		string ext = null;
		if(!forFolder) {
			int i = fromName.LastIndexOf('.');
			if(i >= 0) { ext = fromName.Substring(i); fromName = fromName.Remove(i); }
		}
		fromName = fromName.RegexReplace_(@"\d+$", "");
		for(int i = 2; ; i++) {
			var s = fromName + i + ext;
			if(!_Exists(s)) return s;
		}

		bool _Exists(string s)
		{
			if(null != folder.Xml.Element_(null, XN.n, s, true)) return true;
			if(File_.ExistsAsAny(folder.FilePath + "\\" + s)) return true; //orphaned file?
			return false;
		}
	}

}
