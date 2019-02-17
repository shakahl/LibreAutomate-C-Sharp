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
using static Au.NoClass;
using static Program;
using Au.Controls;
using Au.Tools;

class CmdHandlers : IGStripManagerCallbacks
{
	internal delegate void CmdHandler();
	Dictionary<string, CmdHandler> _dict = new Dictionary<string, CmdHandler>(200);

	internal Dictionary<string, CmdHandler> Dict { get { return _dict; } }

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
		_dict.Add(nameof(File_Disable), File_Disable);
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
		_dict.Add(nameof(File_Exit), File_Exit);
		_dict.Add(nameof(Edit_Undo), Edit_Undo);
		_dict.Add(nameof(Edit_Redo), Edit_Redo);
		_dict.Add(nameof(Edit_Cut), Edit_Cut);
		_dict.Add(nameof(Edit_Copy), Edit_Copy);
		_dict.Add(nameof(Edit_Paste), Edit_Paste);
		_dict.Add(nameof(Edit_Find), Edit_Find);
		_dict.Add(nameof(Edit_Members), Edit_Members);
		_dict.Add(nameof(Edit_ContextHelp), Edit_ContextHelp);
		_dict.Add(nameof(Edit_GoToDefinition), Edit_GoToDefinition);
		_dict.Add(nameof(Edit_PeekDefinition), Edit_PeekDefinition);
		_dict.Add(nameof(Edit_FindReferences), Edit_FindReferences);
		_dict.Add(nameof(Edit_Indent), Edit_Indent);
		_dict.Add(nameof(Edit_Unindent), Edit_Unindent);
		_dict.Add(nameof(Edit_Comment), Edit_Comment);
		_dict.Add(nameof(Edit_Uncomment), Edit_Uncomment);
		_dict.Add(nameof(Edit_HideRegion), Edit_HideRegion);
		_dict.Add(nameof(Edit_SelectAll), Edit_SelectAll);
		_dict.Add(nameof(Edit_Output), Edit_Output);
		_dict.Add(nameof(Edit_ImagesInCode), Edit_ImagesInCode);
		_dict.Add(nameof(Edit_WrapLines), Edit_WrapLines);
		_dict.Add(nameof(Edit_LineNumbers), Edit_LineNumbers);
		_dict.Add(nameof(Edit_IndentationGuides), Edit_IndentationGuides);
		_dict.Add(nameof(Code_Wnd), Code_Wnd);
		_dict.Add(nameof(Code_Acc), Code_Acc);
		_dict.Add(nameof(Code_WinImage), Code_WinImage);
		_dict.Add(nameof(Run_Run), Run_Run);
		_dict.Add(nameof(Run_End), Run_End);
		_dict.Add(nameof(Run_Pause), Run_Pause);
		_dict.Add(nameof(Run_Compile), Run_Compile);
		_dict.Add(nameof(Run_AutoMinimize), Run_AutoMinimize);
		_dict.Add(nameof(Run_DisableTriggers), Run_DisableTriggers);
		_dict.Add(nameof(Run_MakeExe), Run_MakeExe);
		_dict.Add(nameof(Debug_RunToBreakpoint), Debug_RunToBreakpoint);
		_dict.Add(nameof(Debug_RunToCursor), Debug_RunToCursor);
		_dict.Add(nameof(Debug_StepInto), Debug_StepInto);
		_dict.Add(nameof(Debug_StepOver), Debug_StepOver);
		_dict.Add(nameof(Debug_StepOut), Debug_StepOut);
		_dict.Add(nameof(Debug_ToggleBreakpoint), Debug_ToggleBreakpoint);
		_dict.Add(nameof(Debug_PersistentBreakpoint), Debug_PersistentBreakpoint);
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
		_dict.Add(nameof(Tools_RegularExpressions), Tools_RegularExpressions);
		_dict.Add(nameof(Tools_ExploreWindows), Tools_ExploreWindows);
		_dict.Add(nameof(Tools_RemapKeys), Tools_RemapKeys);
		_dict.Add(nameof(Tools_Components), Tools_Components);
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
		_dict.Add(nameof(Tools_Statusbar_Floating), Tools_Statusbar_Floating);
		_dict.Add(nameof(Tools_Statusbar_MouseInfo), Tools_Statusbar_MouseInfo);
		_dict.Add(nameof(Tools_Statusbar_AutoHeight), Tools_Statusbar_AutoHeight);
		_dict.Add(nameof(Tools_Statusbar_SendToOutput), Tools_Statusbar_SendToOutput);
		_dict.Add(nameof(Help_QuickStart), Help_QuickStart);
		_dict.Add(nameof(Help_Reference), Help_Reference);
		_dict.Add(nameof(Help_ContextHelp), Help_ContextHelp);
		_dict.Add(nameof(Help_Download), Help_Download);
		_dict.Add(nameof(Help_Forum), Help_Forum);
		_dict.Add(nameof(Help_Email), Help_Email);
		_dict.Add(nameof(Help_Donate), Help_Donate);
		_dict.Add(nameof(Help_About), Help_About);

		#endregion add

	}


	#region menu File

	public void File_NewScript()
	{
		Model.NewItem("Script.cs", beginEdit: true);
	}

	public void File_NewClass()
	{
		Model.NewItem("Class.cs", beginEdit: true);
	}

	public void File_NewFolder()
	{
		Model.NewItem("Folder", beginEdit: true);
	}

	public void File_Import()
	{
		Model.ImportFiles();
	}

	public void File_Disable()
	{

	}

	public void File_Rename()
	{
		Model.RenameSelected();
	}

	public void File_Delete()
	{
		Model.DeleteSelected();
	}

	public void File_Properties()
	{
		Model.Properties();
	}

	public void File_Open()
	{
		Model.OpenSelected(1);
	}

	//public void File_OpenInNewWindow()
	//{
	//	Model.OpenSelected(2);
	//}

	public void File_OpenInDefaultApp()
	{
		Model.OpenSelected(3);
	}

	public void File_SelectInExplorer()
	{
		Model.OpenSelected(4);
	}

	public void File_PreviousDocument()
	{
		var a = Model.OpenFiles;
		if(a.Count > 1) Model.SetCurrentFile(a[1]);
	}

	public void File_Close()
	{
		Model.CloseEtc(1);
	}

	public void File_CloseAll()
	{
		Model.CloseEtc(2);
	}

	public void File_CollapseFolders()
	{
		Model.CloseEtc(3);
	}

	public void File_Cut()
	{
		Model.CutCopySelected(true);
	}

	public void File_Copy()
	{
		Model.CutCopySelected(false);
	}

	public void File_Paste()
	{
		Model.Paste();
	}

	public void File_CopyRelativePath()
	{
		Model.SelectedCopyPath(false);
	}

	public void File_CopyFullPath()
	{
		Model.SelectedCopyPath(true);
	}

	public void File_PrintSetup()
	{

	}

	public void File_Print()
	{

	}

	public void File_OpenWorkspace()
	{
		Panels.Files.LoadAnotherWorkspace();
	}

	public void File_NewWorkspace()
	{
		Panels.Files.LoadNewWorkspace();
	}

	public void File_ExportWorkspace()
	{
		Model.ExportSelected();
	}

	public void File_ImportWorkspace()
	{
		Model.ImportWorkspace();
	}

	public void File_FindInWorkspaces()
	{

	}

	public void File_WorkspaceProperties()
	{

	}

	public void File_SaveNow()
	{
		Model?.Save.AllNowIfNeed();
	}

	public void File_Exit()
	{
		MainForm.Close();
	}

	#endregion

	#region menu Edit

	public void Edit_Undo()
	{
		Panels.Editor.ActiveDoc.Call(Sci.SCI_UNDO);
	}

	public void Edit_Redo()
	{
		Panels.Editor.ActiveDoc.Call(Sci.SCI_REDO);
	}

	public void Edit_Cut()
	{
		Panels.Editor.ActiveDoc.Call(Sci.SCI_CUT);
	}

	public void Edit_Copy()
	{
		Panels.Editor.ActiveDoc.Call(Sci.SCI_COPY);
	}

	public void Edit_Paste()
	{
		Panels.Editor.ActiveDoc.Call(Sci.SCI_PASTE);
	}

	public void Edit_Find()
	{

	}

	public void Edit_Members()
	{

	}

	public void Edit_ContextHelp()
	{

	}

	public void Edit_GoToDefinition()
	{

	}

	public void Edit_PeekDefinition()
	{

	}

	public void Edit_FindReferences()
	{

	}

	public void Edit_Indent()
	{

	}

	public void Edit_Unindent()
	{

	}

	public void Edit_Comment()
	{

	}

	public void Edit_Uncomment()
	{

	}

	public void Edit_HideRegion()
	{

	}

	public void Edit_SelectAll()
	{

	}

	public void Edit_Output()
	{
		//get selected text and show in output with <code> tag.
		//It can be used to copy/paste text to another place in the document, instead of split-view.
	}

	public void Edit_ImagesInCode()
	{

	}

	public void Edit_WrapLines()
	{

	}

	public void Edit_LineNumbers()
	{

	}

	public void Edit_IndentationGuides()
	{

	}

	#endregion

	#region menu Code

	/// <summary>
	/// Shows non-modal tool form and on OK inserts its result code in the active document. If readonly - prints in the output.
	/// </summary>
	/// <param name="f"></param>
	static void _ShowTool(ToolForm f)
	{
		f.FormClosed += (unu, e) => {
			if(e.CloseReason == CloseReason.UserClosing && f.DialogResult == DialogResult.OK) {
				var s = f.ResultCode;
				var d = Panels.Editor.ActiveDoc;
				var t = d.ST;
				if(t.IsReadonly) {
					Print(s);
				} else {
					d.Focus();
					int i = t.LineStartFromPosition(t.CurrentPositionBytes);
					s += "\r\n";
					t.ReplaceSel(s, i);
				}
			}
		};
		f.Show(MainForm);
	}

	public void Code_Wnd()
	{
		_ShowTool(new Form_Wnd());
	}

	public void Code_Acc()
	{
		_ShowTool(new Form_Acc());
	}

	public void Code_WinImage()
	{
		_ShowTool(new Form_WinImage());
	}

	#endregion

	#region menu Run

	public void Run_Compile()
	{
		Run.CompileAndRun(false, Model.CurrentFile);
	}

	public void Run_Run()
	{
		Run.CompileAndRun(true, Model.CurrentFile);
	}

	public void Run_End()
	{
		if(Tasks.EndTasksOf(Model.CurrentFile)) return;
		var t = Tasks.GetGreenTask(); if(t == null) return;
		var m = new AuMenu();
		m.Add("End task:", null).Enabled = false;
		m[t.f.DisplayName] = o => Tasks.EndTask(t);
		m.Show(MainForm);
	}

	public void Run_Pause()
	{

	}

	public void Run_AutoMinimize()
	{

	}

	public void Run_DisableTriggers()
	{

	}

	public void Run_MakeExe()
	{

	}

	#endregion

	#region menu Debug

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

	public void Debug_ToggleBreakpoint()
	{

	}

	public void Debug_PersistentBreakpoint()
	{

	}

	public void Debug_ClearLocalBreakpoints()
	{

	}

	public void Debug_ClearAllBreakpoints()
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

	public void Tools_RegularExpressions()
	{

	}

	public void Tools_ExploreWindows()
	{

	}

	public void Tools_RemapKeys()
	{

	}

	public void Tools_Components()
	{

	}

	public void Tools_Portable()
	{

	}

	public void Tools_Options()
	{

	}

	public void Tools_Output_Clear()
	{
		Panels.Output.Clear();
	}

	public void Tools_Output_Copy()
	{
		Panels.Output.Copy();
	}

	public void Tools_Output_FindSelectedText()
	{

	}

	public void Tools_Output_History()
	{
		Panels.Output.History();
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
		v.WrapLines = !v.WrapLines;
	}

	public void Tools_Output_WhiteSpace()
	{
		var v = Panels.Output;
		v.WhiteSpace = !v.WhiteSpace;
	}

	public void Tools_Output_Topmost()
	{
		var v = Panels.Output;
		v.Topmost = !v.Topmost;
	}

	public void Tools_Statusbar_Floating()
	{

	}

	public void Tools_Statusbar_MouseInfo()
	{

	}

	public void Tools_Statusbar_AutoHeight()
	{

	}

	public void Tools_Statusbar_SendToOutput()
	{

	}

	#endregion

	#region menu Help

	public void Help_QuickStart()
	{

	}

	public void Help_Reference()
	{

	}

	public void Help_ContextHelp()
	{

	}

	public void Help_Download()
	{

	}

	public void Help_Forum()
	{

	}

	public void Help_Email()
	{

	}

	public void Help_Donate()
	{

	}

	public void Help_About()
	{

	}

	#endregion
}
