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

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Au.Tools;

class CmdHandlers : IGStripManagerCallbacks
{
	Dictionary<string, Action> _dict = new Dictionary<string, Action>(200);

	internal Dictionary<string, Action> Dict => _dict; //used to debug

	EventHandler _onClick;

	//Common Click even handler of all items.
	//Calls item's command handler.
	void _OnClick(object sender, EventArgs args)
	{
		var item = sender as ToolStripItem;
		//Print(item.Name);
		if(!_dict.TryGetValue(item.Name, out var d)) { Debug.Assert(false); return; }
		d();
	}

	#region IGStripManagerCallbacks
	public EventHandler GetClickHandler(string itemName)
	{
		if(_dict.ContainsKey(itemName)) return _onClick;
		return null;
	}

	public Image GetImage(string imageName)
	{
		return EdResources.GetImageUseCache(imageName);
	}

	public void ItemAdding(ToolStripItem item, ToolStrip owner)
	{
		//Print(item, owner);
	}
	#endregion

	internal CmdHandlers()
	{
		_onClick = _OnClick;

		#region add to _dict

		_dict.Add(nameof(File_NewScript), File_NewScript);
		_dict.Add(nameof(File_NewClass), File_NewClass);
		_dict.Add(nameof(File_NewFolder), File_NewFolder);
		_dict.Add(nameof(File_Import), File_Import);
		//_dict.Add(nameof(File_Disable), File_Disable);
		_dict.Add(nameof(File_Rename), File_Rename);
		_dict.Add(nameof(File_Delete), File_Delete);
		_dict.Add(nameof(File_Properties), File_Properties);
		_dict.Add(nameof(File_Open), File_Open);
		//_dict.Add(nameof(File_OpenInNewWindow), File_OpenInNewWindow);
		_dict.Add(nameof(File_OpenInDefaultApp), File_OpenInDefaultApp);
		_dict.Add(nameof(File_SelectInExplorer), File_SelectInExplorer);
		_dict.Add(nameof(File_PreviousDocument), File_PreviousDocument);
		_dict.Add(nameof(File_Close), File_Close);
		_dict.Add(nameof(File_CloseAll), File_CloseAll);
		_dict.Add(nameof(File_CollapseFolders), File_CollapseFolders);
		_dict.Add(nameof(File_Cut), File_Cut);
		_dict.Add(nameof(File_Copy), File_Copy);
		_dict.Add(nameof(File_Paste), File_Paste);
		_dict.Add(nameof(File_CopyRelativePath), File_CopyRelativePath);
		_dict.Add(nameof(File_CopyFullPath), File_CopyFullPath);
		_dict.Add(nameof(File_PrintSetup), File_PrintSetup);
		_dict.Add(nameof(File_Print), File_Print);
		_dict.Add(nameof(File_OpenWorkspace), File_OpenWorkspace);
		_dict.Add(nameof(File_NewWorkspace), File_NewWorkspace);
		_dict.Add(nameof(File_ExportWorkspace), File_ExportWorkspace);
		_dict.Add(nameof(File_ImportWorkspace), File_ImportWorkspace);
		_dict.Add(nameof(File_FindInWorkspaces), File_FindInWorkspaces);
		_dict.Add(nameof(File_WorkspaceProperties), File_WorkspaceProperties);
		_dict.Add(nameof(File_SaveNow), File_SaveNow);
		_dict.Add(nameof(File_CloseWindow), File_CloseWindow);
		_dict.Add(nameof(File_Exit), File_Exit);
		_dict.Add(nameof(Edit_Undo), Edit_Undo);
		_dict.Add(nameof(Edit_Redo), Edit_Redo);
		_dict.Add(nameof(Edit_Cut), Edit_Cut);
		_dict.Add(nameof(Edit_Copy), Edit_Copy);
		_dict.Add(nameof(Edit_Paste), Edit_Paste);
		_dict.Add(nameof(Edit_ForumCopy), Edit_ForumCopy);
		_dict.Add(nameof(Edit_Find), Edit_Find);
		_dict.Add(nameof(Edit_FindReferences), Edit_FindReferences);
		_dict.Add(nameof(Edit_AutocompletionList), Edit_AutocompletionList);
		_dict.Add(nameof(Edit_ParameterInfo), Edit_ParameterInfo);
		_dict.Add(nameof(Edit_GoToDefinition), Edit_GoToDefinition);
		//_dict.Add(nameof(Edit_PeekDefinition), Edit_PeekDefinition);
		_dict.Add(nameof(Edit_Comment), Edit_Comment);
		_dict.Add(nameof(Edit_Uncomment), Edit_Uncomment);
		_dict.Add(nameof(Edit_Indent), Edit_Indent);
		_dict.Add(nameof(Edit_Unindent), Edit_Unindent);
		_dict.Add(nameof(Edit_SelectAll), Edit_SelectAll);
		//_dict.Add(nameof(Edit_HideRegion), Edit_HideRegion);
		//_dict.Add(nameof(Edit_Output), Edit_Output);
		_dict.Add(nameof(Edit_WrapLines), Edit_WrapLines);
		_dict.Add(nameof(Edit_ImagesInCode), Edit_ImagesInCode);
		_dict.Add(nameof(Code_Wnd), Code_Wnd);
		_dict.Add(nameof(Code_Acc), Code_Acc);
		_dict.Add(nameof(Code_WinImage), Code_WinImage);
		_dict.Add(nameof(Code_Regex), Code_Regex);
		_dict.Add(nameof(Code_Keys), Code_Keys);
		_dict.Add(nameof(Code_WindowsAPI), Code_WindowsAPI);
		_dict.Add(nameof(Run_Run), Run_Run);
		_dict.Add(nameof(Run_End), Run_End);
		_dict.Add(nameof(Run_Pause), Run_Pause);
		_dict.Add(nameof(Run_Compile), Run_Compile);
		_dict.Add(nameof(Run_Recent), Run_Recent);
		_dict.Add(nameof(Run_DisableTriggers), Run_DisableTriggers);
		_dict.Add(nameof(Debug_RunToBreakpoint), Debug_RunToBreakpoint);
		_dict.Add(nameof(Debug_RunToCursor), Debug_RunToCursor);
		_dict.Add(nameof(Debug_StepInto), Debug_StepInto);
		_dict.Add(nameof(Debug_StepOver), Debug_StepOver);
		_dict.Add(nameof(Debug_StepOut), Debug_StepOut);
		_dict.Add(nameof(Debug_ToggleBreakpoint), Debug_ToggleBreakpoint);
		_dict.Add(nameof(Debug_AddDebuggerBreakCode), Debug_AddDebuggerBreakCode);
		_dict.Add(nameof(Debug_ClearLocalBreakpoints), Debug_ClearLocalBreakpoints);
		_dict.Add(nameof(Debug_ClearAllBreakpoints), Debug_ClearAllBreakpoints);
		_dict.Add(nameof(Debug_DebugOptions), Debug_DebugOptions);
		_dict.Add(nameof(Tools_Record), Tools_Record);
		_dict.Add(nameof(Tools_RecordMenu), Tools_RecordMenu);
		_dict.Add(nameof(Tools_RecordSingleAction), Tools_RecordSingleAction);
		_dict.Add(nameof(Tools_FilesAndTriggers), Tools_FilesAndTriggers);
		_dict.Add(nameof(Tools_DialogEditor), Tools_DialogEditor);
		_dict.Add(nameof(Tools_ToolbarEditor), Tools_ToolbarEditor);
		_dict.Add(nameof(Tools_MenuEditor), Tools_MenuEditor);
		_dict.Add(nameof(Tools_ImagelistEditor), Tools_ImagelistEditor);
		_dict.Add(nameof(Tools_Resources), Tools_Resources);
		_dict.Add(nameof(Tools_Icons), Tools_Icons);
		_dict.Add(nameof(Tools_HelpEditor), Tools_HelpEditor);
		_dict.Add(nameof(Tools_ExploreWindows), Tools_ExploreWindows);
		_dict.Add(nameof(Tools_RemapKeys), Tools_RemapKeys);
		_dict.Add(nameof(Tools_Portable), Tools_Portable);
		_dict.Add(nameof(Tools_Options), Tools_Options);
		_dict.Add(nameof(Tools_Output_Clear), Tools_Output_Clear);
		_dict.Add(nameof(Tools_Output_Copy), Tools_Output_Copy);
		_dict.Add(nameof(Tools_Output_FindSelectedText), Tools_Output_FindSelectedText);
		_dict.Add(nameof(Tools_Output_History), Tools_Output_History);
		_dict.Add(nameof(Tools_Output_LogWindowEvents), Tools_Output_LogWindowEvents);
		_dict.Add(nameof(Tools_Output_LogAccEvents), Tools_Output_LogAccEvents);
		_dict.Add(nameof(Tools_Output_WrapLines), Tools_Output_WrapLines);
		_dict.Add(nameof(Tools_Output_WhiteSpace), Tools_Output_WhiteSpace);
		_dict.Add(nameof(Tools_Output_Topmost), Tools_Output_Topmost);
		_dict.Add(nameof(Help_Program), Help_Program);
		_dict.Add(nameof(Help_Library), Help_Library);
		_dict.Add(nameof(Help_Context), Help_Context);
		//_dict.Add(nameof(Help_Download), Help_Download);
		_dict.Add(nameof(Help_Forum), Help_Forum);
		_dict.Add(nameof(Help_Email), Help_Email);
		//_dict.Add(nameof(Help_Donate), Help_Donate);
		_dict.Add(nameof(Help_About), Help_About);
		
		#endregion add

	}


	#region menu File

	public void File_NewScript()
	{
		Program.Model.NewItem("Script.cs", beginRenaming: true);
	}

	public void File_NewClass()
	{
		Program.Model.NewItem("Class.cs", beginRenaming: true);
	}

	public void File_NewFolder()
	{
		Program.Model.NewItem("Folder", beginRenaming: true);
	}

	public void File_Import()
	{
		Program.Model.ImportFiles();
	}

	public void File_Disable()
	{

	}

	public void File_Rename()
	{
		Program.Model.RenameSelected();
	}

	public void File_Delete()
	{
		Program.Model.DeleteSelected();
	}

	public void File_Properties()
	{
		Program.Model.Properties();
	}

	public void File_Open()
	{
		Program.Model.OpenSelected(1);
	}

	//public void File_OpenInNewWindow()
	//{
	//	Program.Model.OpenSelected(2);
	//}

	public void File_OpenInDefaultApp()
	{
		Program.Model.OpenSelected(3);
	}

	public void File_SelectInExplorer()
	{
		Program.Model.OpenSelected(4);
	}

	public void File_PreviousDocument()
	{
		var a = Program.Model.OpenFiles;
		if(a.Count > 1) Program.Model.SetCurrentFile(a[1]);
	}

	public void File_Close()
	{
		Program.Model.CloseEtc(1);
	}

	public void File_CloseAll()
	{
		Program.Model.CloseEtc(2);
	}

	public void File_CollapseFolders()
	{
		Program.Model.CloseEtc(3);
	}

	public void File_Cut()
	{
		Program.Model.CutCopySelected(true);
	}

	public void File_Copy()
	{
		Program.Model.CutCopySelected(false);
	}

	public void File_Paste()
	{
		Program.Model.Paste();
	}

	public void File_CopyRelativePath()
	{
		Program.Model.SelectedCopyPath(false);
	}

	public void File_CopyFullPath()
	{
		Program.Model.SelectedCopyPath(true);
	}

	public void File_PrintSetup()
	{

	}

	public void File_Print()
	{

	}

	public void File_OpenWorkspace()
	{
		Panels.Files.ZLoadAnotherWorkspace();
	}

	public void File_NewWorkspace()
	{
		Panels.Files.ZLoadNewWorkspace();
	}

	public void File_ExportWorkspace()
	{
		Program.Model.ExportSelected();
	}

	public void File_ImportWorkspace()
	{
		Program.Model.ImportWorkspace();
	}

	public void File_FindInWorkspaces()
	{

	}

	public void File_WorkspaceProperties()
	{

	}

	public void File_SaveNow()
	{
		Program.Model?.Save.AllNowIfNeed();
	}

	public void File_CloseWindow()
	{
		Program.MainForm.Close(); //if visible, hides by default
	}

	public void File_Exit()
	{
		Program.MainForm.Hide();
		Program.MainForm.Close();
	}

	#endregion

	#region menu Edit

	public void Edit_Undo()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_UNDO);
	}

	public void Edit_Redo()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_REDO);
	}

	public void Edit_Cut()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_CUT);
	}

	public void Edit_Copy()
	{
		var doc = Panels.Editor.ZActiveDoc;
		doc.ZCopyModified(onlyInfo: true);
		doc.Call(Sci.SCI_COPY);
	}

	public void Edit_ForumCopy()
	{
		Panels.Editor.ZActiveDoc.ZCopyModified();
	}

	public void Edit_Paste()
	{
		var doc = Panels.Editor.ZActiveDoc;
		if(!doc.ZPasteModified()) doc.Call(Sci.SCI_PASTE);
	}

	public void Edit_Find()
	{
		Panels.Find.ZCtrlF();
	}

	public void Edit_AutocompletionList()
	{
		CodeInfo.ShowCompletionList(Panels.Editor.ZActiveDoc);
	}

	public void Edit_ParameterInfo()
	{
		CodeInfo.ShowSignature(Panels.Editor.ZActiveDoc);
	}

	public void Edit_GoToDefinition()
	{
		CiGoTo.GoToSymbolFromPos();
	}

	public void Edit_PeekDefinition()
	{

	}

	public void Edit_FindReferences()
	{

	}

	public void Edit_Comment()
	{
		Panels.Editor.ZActiveDoc.ZCommentLines(true);
	}

	public void Edit_Uncomment()
	{
		Panels.Editor.ZActiveDoc.ZCommentLines(false);
	}

	public void Edit_Indent()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_TAB);
	}

	public void Edit_Unindent()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_BACKTAB);
	}

	public void Edit_SelectAll()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_SELECTALL);
	}

	public void Edit_HideRegion()
	{

	}

	public void Edit_Output()
	{
		//get selected text and show in output with <code> tag.
		//It can be used to copy/paste text to another place in the document, instead of split-view.
	}

	public void Edit_WrapLines()
	{
		Panels.Editor.ZActiveDoc.ZToggleView(SciCode.EView.Wrap);
	}

	public void Edit_ImagesInCode()
	{
		Panels.Editor.ZActiveDoc.ZToggleView(SciCode.EView.Images);
	}

	#endregion

	#region menu Code

	public void Code_Wnd()
	{
		new FormAWnd().ZShow();
	}

	public void Code_Acc()
	{
		new FormAAcc().ZShow();
	}

	public void Code_WinImage()
	{
		new FormAWinImage().ZShow();
	}

	public void Code_Regex()
	{
		CiTools.CmdShowRegexOrKeysWindow(true);
	}

	public void Code_Keys()
	{
		CiTools.CmdShowRegexOrKeysWindow(false);
	}

	public void Code_WindowsAPI()
	{
		FormWinapi.ZShowDialog();
	}

	#endregion

	#region menu Run

	public void Run_Compile()
	{
		Run.CompileAndRun(false, Program.Model.CurrentFile);
	}

	public void Run_Run()
	{
		Run.CompileAndRun(true, Program.Model.CurrentFile);
	}

	public void Run_End()
	{
		if(Program.Tasks.EndTasksOf(Program.Model.CurrentFile)) return;
		var t = Program.Tasks.GetGreenTask(); if(t == null) return;
		var m = new AMenu();
		m.Add("End task:", null).Enabled = false;
		m[t.f.DisplayName] = o => Program.Tasks.EndTask(t);
		m.Show(Program.MainForm);
	}

	public void Run_Pause()
	{

	}

	public void Run_Recent()
	{
		Au.Util.LibLog.Run.Show();
	}

	public void Run_DisableTriggers()
	{
		Run.DisableTriggers(null);
	}

	#endregion

	#region menu Debug

	public void Debug_AddDebuggerBreakCode()
	{
		TUtil.InsertUsingDirectiveInEditor("System.Diagnostics");
		TUtil.InsertStatementInEditor("if(Debugger.IsAttached) Debugger.Break(); else Debugger.Launch();");
	}

	public void Debug_ToggleBreakpoint()
	{

	}

	public void Debug_ClearLocalBreakpoints()
	{

	}

	public void Debug_ClearAllBreakpoints()
	{

	}

	public void Debug_RunToBreakpoint()
	{

	}

	public void Debug_RunToCursor()
	{

	}

	public void Debug_StepInto()
	{

	}

	public void Debug_StepOver()
	{

	}

	public void Debug_StepOut()
	{

	}

	public void Debug_DebugOptions()
	{

	}

	#endregion

	#region menu Tools

	public void Tools_Record()
	{

	}

	public void Tools_RecordMenu()
	{

	}

	public void Tools_RecordSingleAction()
	{

	}

	public void Tools_FilesAndTriggers()
	{

	}

	public void Tools_DialogEditor()
	{

	}

	public void Tools_ToolbarEditor()
	{

	}

	public void Tools_MenuEditor()
	{

	}

	public void Tools_ImagelistEditor()
	{

	}

	public void Tools_Resources()
	{

	}

	public void Tools_Icons()
	{

	}

	public void Tools_HelpEditor()
	{

	}

	public void Tools_ExploreWindows()
	{

	}

	public void Tools_RemapKeys()
	{

	}

	public void Tools_Portable()
	{

	}

	public void Tools_Options()
	{
		FOptions.ZShow();
	}

	#endregion

	#region menu_Output

	public void Tools_Output_Clear()
	{
		Panels.Output.ZClear();
	}

	public void Tools_Output_Copy()
	{
		Panels.Output.ZCopy();
	}

	public void Tools_Output_FindSelectedText()
	{
		Panels.Output.ZFind();
	}

	public void Tools_Output_History()
	{
		Panels.Output.ZHistory();
	}

	public void Tools_Output_LogWindowEvents()
	{

	}

	public void Tools_Output_LogAccEvents()
	{

	}

	public void Tools_Output_WrapLines()
	{
		var v = Panels.Output;
		v.ZWrapLines = !v.ZWrapLines;
	}

	public void Tools_Output_WhiteSpace()
	{
		var v = Panels.Output;
		v.ZWhiteSpace = !v.ZWhiteSpace;
	}

	public void Tools_Output_Topmost()
	{
		var v = Panels.Output;
		v.ZTopmost ^= true;
	}

	#endregion

	#region menu Help

	public void Help_Program()
	{
		Au.Util.AHelp.AuHelp("");
	}

	public void Help_Library()
	{
		Au.Util.AHelp.AuHelp("api/");
	}

	public void Help_Context()
	{
		var c = AWnd.ThisThread.FocusedControl;
		if(c == null) return;
		if(c == Panels.Editor.ZActiveDoc) {
			CiUtil.OpenSymbolFromPosHelp();
		}
	}

	//public void Help_Download()
	//{

	//}

	public void Help_Forum()
	{

	}

	public void Help_Email()
	{
		AExec.TryRun("mailto:support@quickmacros.com?subject=" + Program.AppName);
	}

	//public void Help_Donate()
	//{

	//}

	public void Help_About()
	{

	}

	#endregion
}
