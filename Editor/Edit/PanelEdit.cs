//#define TEST_COPYPASTE

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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using static Au.Controls.Sci;

class PanelEdit : UserControl
{
	List<SciCode> _docs = new List<SciCode>(); //documents that are actually open currently. Note: FilesModel.OpenFiles contains not only these.
	SciCode _activeDoc;

	public SciCode ZActiveDoc => _activeDoc;

	public event Action ZActiveDocChanged;

	public bool ZIsOpen => _activeDoc != null;

	public PanelEdit()
	{
		this.AccessibleName = this.Name = "Code";
		this.TabStop = false;
		this.BackColor = SystemColors.AppWorkspace;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		_UpdateUI_IsOpen();
	}

	//protected override void OnGotFocus(EventArgs e) { _activeDoc?.Focus(); }

	//public SciControl SC => _activeDoc;

	/// <summary>
	/// If f is open (active or not), returns its SciCode, else null.
	/// </summary>
	public SciCode ZGetOpenDocOf(FileNode f) => _docs.Find(v => v.ZFile == f);

	/// <summary>
	///	If f is already open, unhides its control.
	///	Else loads f text and creates control. If fails, does not change anything.
	/// Hides current file's control.
	/// Returns false if failed to read file.
	/// Does not save text of previously active document.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="newFile">Should be true if opening the file first time after creating.</param>
	public bool ZOpen(FileNode f, bool newFile)
	{
		Debug.Assert(!Program.Model.IsAlien(f));

		if(f == _activeDoc?.ZFile) return true;
		bool focus = _activeDoc != null ? _activeDoc.Focused : false;
		var doc = ZGetOpenDocOf(f);
		if(doc != null) {
			if(_activeDoc != null) _activeDoc.Visible = false;
			_activeDoc = doc;
			_activeDoc.Visible = true;
			ZActiveDocChanged?.Invoke();
			ZUpdateUI_Cmd();
		} else {
			var path = f.FilePath;
			byte[] text = null;
			SciText.FileLoaderSaver fls = default;
			try { text = fls.Load(path); }
			catch(Exception ex) { Print("Failed to open file. " + ex.Message); }
			if(text == null) return false;

			if(_activeDoc != null) _activeDoc.Visible = false;
			doc = new SciCode(f, fls);
			_docs.Add(doc);
			_activeDoc = doc;
			this.Controls.Add(doc);
			doc.ZInit(text, newFile);
			doc.AccessibleName = f.Name;
			doc.AccessibleDescription = path;

			ZActiveDocChanged?.Invoke();
			//CodeInfo.FileOpened(doc);
		}
		if(focus) _activeDoc.Focus();

		_UpdateUI_IsOpen();
		Panels.Find.ZUpdateQuickResults(true);
		return true;
	}

	/// <summary>
	/// If f is open, closes its document and destroys its control.
	/// f can be any, not necessary the active document.
	/// Saves text before closing the active document.
	/// Does not show another document when closed the active document.
	/// </summary>
	/// <param name="f"></param>
	public void ZClose(FileNode f)
	{
		Debug.Assert(f != null);
		SciCode doc;
		if(f == _activeDoc?.ZFile) {
			Program.Model.Save.TextNowIfNeed();
			doc = _activeDoc;
			_activeDoc = null;
			ZActiveDocChanged?.Invoke();
		} else {
			doc = ZGetOpenDocOf(f);
			if(doc == null) return;
		}
		//CodeInfo.FileClosed(doc);
		doc.Dispose();
		_docs.Remove(doc);
		_UpdateUI_IsOpen();
	}

	/// <summary>
	/// Closes all documents and destroys controls.
	/// </summary>
	public void ZCloseAll(bool saveTextIfNeed)
	{
		if(saveTextIfNeed) Program.Model.Save.TextNowIfNeed();
		_activeDoc = null;
		ZActiveDocChanged?.Invoke();
		foreach(var doc in _docs) doc.Dispose();
		_docs.Clear();
		_UpdateUI_IsOpen();
	}

	public bool ZSaveText()
	{
		return _activeDoc?.ZSaveText() ?? true;
	}

	public void ZSaveEditorData()
	{
		_activeDoc?.ZSaveEditorData();
	}

	//public bool ZIsModified => _activeDoc?.IsModified ?? false;

	/// <summary>
	/// Enables/disables Edit and Run toolbars/menus and some other UI parts depending on whether a document is open in editor.
	/// </summary>
	void _UpdateUI_IsOpen(bool asynchronously = true)
	{
		bool enable = _activeDoc != null;
		if(enable != _uiDisabled_IsOpen) return;

		if(asynchronously) {
			BeginInvoke(new Action(() => _UpdateUI_IsOpen(false)));
			return;
		}
		_uiDisabled_IsOpen = !enable;

		//toolbars
		Strips.tbEdit.Enabled = enable;
		Strips.tbRun.Enabled = enable;
		//menus
		Strips.Menubar.Items["Menu_Edit"].Enabled = enable;
		Strips.Menubar.Items["Menu_Code"].Enabled = enable;
		Strips.Menubar.Items["Menu_Run"].Enabled = enable;
		//toolbar buttons
		Strips.tbFile.Items["File_Properties"].Enabled = enable;
		//drop-down menu items and submenus
		//don't disable these because can right-click...
		//Strips.ddFile.Items["File_Disable"].Enabled = enable;
		//Strips.ddFile.Items["File_Rename"].Enabled = enable;
		//Strips.ddFile.Items["File_Delete"].Enabled = enable;
		//Strips.ddFile.Items["File_Properties"].Enabled = enable;
		//Strips.ddFile.Items["File_More"].Enabled = enable;
	}
	bool _uiDisabled_IsOpen;

	/// <summary>
	/// Enables/disables commands (toolbar buttons, menu items) depending on document state such as "can undo".
	/// Called on SCN_UPDATEUI.
	/// </summary>
	internal void ZUpdateUI_Cmd()
	{
		EUpdateUI disable = 0;
		var d = _activeDoc;
		if(d == null) return; //we disable the toolbar and menu
		if(0 == d.Call(SCI_CANUNDO)) disable |= EUpdateUI.Undo;
		if(0 == d.Call(SCI_CANREDO)) disable |= EUpdateUI.Redo;
		if(0 != d.Call(SCI_GETSELECTIONEMPTY)) disable |= EUpdateUI.Copy;
		if(disable.Has(EUpdateUI.Copy) || d.Z.IsReadonly) disable |= EUpdateUI.Cut;
		//if(0 == d.Call(SCI_CANPASTE)) disable |= EUpdateUI.Paste; //rejected. Often slow. Also need to see on focused etc.

		var dif = disable ^ _cmdDisabled; if(dif == 0) return;

		//Print(dif);
		_cmdDisabled = disable;
		if(dif.Has(EUpdateUI.Undo)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Undo), !disable.Has(EUpdateUI.Undo));
		if(dif.Has(EUpdateUI.Redo)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Redo), !disable.Has(EUpdateUI.Redo));
		if(dif.Has(EUpdateUI.Cut)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Cut), !disable.Has(EUpdateUI.Cut));
		if(dif.Has(EUpdateUI.Copy)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Copy), !disable.Has(EUpdateUI.Copy));
		//if(dif.Has(EUpdateUI.Paste)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Paste), !disable.Has_(EUpdateUI.Paste));

	}

	EUpdateUI _cmdDisabled;

	[Flags]
	enum EUpdateUI
	{
		Undo = 1,
		Redo = 2,
		Cut = 4,
		Copy = 8,
		//Paste = 16,

	}
}
