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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using Au.Util;
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
		//AOutput.Write(item.Name);
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
		//AOutput.Write(item, owner);
	}
	#endregion

	internal CmdHandlers()
	{
		_onClick = _OnClick;

		var ta = typeof(Action);
		foreach(var mi in typeof(CmdHandlers).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)) {
			string name = mi.Name;
			if(name.IndexOf('_', 1) < 0) continue;
			//AOutput.Write(name);
			_dict.Add(name, (Action)mi.CreateDelegate(ta, this));
		}
		//AOutput.Write(_dict.Count);
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

	public void File_NewPartial()
	{
		Program.Model.NewItem("Partial.cs", beginRenaming: true);
	}

	public void File_NewFolder()
	{
		Program.Model.NewItemX(null, beginRenaming: true);
	}

	public void File_Import()
	{
		Program.Model.ImportFiles();
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
		Program.Model.CloseEtc(FilesModel.ECloseCmd.CloseSelectedOrCurrent);
	}

	public void File_CloseAll()
	{
		Program.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll);
	}

	public void File_CollapseFolders()
	{
		Program.Model.CloseEtc(FilesModel.ECloseCmd.CollapseFolders);
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

	public void File_ImportWorkspace()
	{
		using var d = new OpenFileDialog { Title = "Import workspace", Filter = "files.xml|files.xml" };
		if(d.ShowDialog(Program.MainForm) != DialogResult.OK) return;
		Program.Model.ImportWorkspace(APath.GetDirectory(d.FileName));
	}

	public void File_ImportZip()
	{
		using var d = new OpenFileDialog { Title = "Import .zip", Filter = "Zip files|*.zip" };
		if(d.ShowDialog(Program.MainForm) != DialogResult.OK) return;
		Program.Model.ImportWorkspace(d.FileName);
	}

	public void File_ExportWorkspace()
	{
		Program.Model.ExportSelected(zip: false);
	}

	public void File_ExportZip()
	{
		Program.Model.ExportSelected(zip: true);
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
		doc.ZForumCopy(onlyInfo: true);
		doc.Call(Sci.SCI_COPY);
	}

	public void Edit_ForumCopy()
	{
		Panels.Editor.ZActiveDoc.ZForumCopy();
	}

	public void Edit_Paste()
	{
		var doc = Panels.Editor.ZActiveDoc;
		if(!doc.ZForumPaste()) doc.Call(Sci.SCI_PASTE);
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
		CodeInfo.ShowSignature();
	}

	public void Edit_GoToDefinition()
	{
		CiGoTo.GoToSymbolFromPos();
	}

	//public void Edit_PeekDefinition()
	//{

	//}

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

	public void Edit_IndentLines()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_TAB);
	}

	public void Edit_UnindentLines()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_BACKTAB);
	}

	public void Edit_SelectAll()
	{
		Panels.Editor.ZActiveDoc.Call(Sci.SCI_SELECTALL);
	}

	//public void Edit_Output()
	//{
	//	//get selected text and show in output with <code> tag.
	//	//It can be used to copy/paste text to another place in the document, instead of split-view.
	//}

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

	public void Code_AWnd()
	{
		new FormAWnd().ZShow();
	}

	public void Code_AAcc()
	{
		new FormAAcc().ZShow();
	}

	public void Code_AWinImage()
	{
		new FormAWinImage().ZShow();
	}

	public void Code_Keys()
	{
		CiTools.CmdShowKeysWindow();
	}

	public void Code_Regex()
	{
		CiTools.CmdShowRegexWindow();
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
		Run.CompileAndRun(true, Program.Model.CurrentFile, runFromEditor: true);
	}

	public void Run_EndTask()
	{
		var f = Program.Model.CurrentFile;
		if(f != null) {
			if(f.FindProject(out _, out var fMain)) f = fMain;
			if(Program.Tasks.EndTasksOf(f)) return;
		}
		var t = Program.Tasks.GetRunsingleTask(); if(t == null) return;
		var m = new AMenu();
		m["End task  " + t.f.DisplayName] = o => Program.Tasks.EndTask(t);
		m.Show(Program.MainForm);
	}

	public void Run_Pause()
	{

	}

	public void Run_Recent()
	{
		Log_.Run.Show();
	}

	#endregion

	#region menu Debug

	public void Debug_AddDebuggerBreakCode()
	{
		InsertCode.UsingDirective("System.Diagnostics");
		InsertCode.Statements("if(Debugger.IsAttached) Debugger.Break(); else Debugger.Launch();");
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

	#region menu T&T

	public void TT_AddTrigger()
	{
		TriggersAndToolbars.AddTrigger();
	}

	public void TT_AddToolbar()
	{
		TriggersAndToolbars.AddToolbar();
	}

	public void TT_EditHotkeyTriggers()
	{
		TriggersAndToolbars.Edit(@"Triggers\Hotkey triggers.cs");
	}

	public void TT_EditAutotextTriggers()
	{
		TriggersAndToolbars.Edit(@"Triggers\Autotext triggers.cs");
	}

	public void TT_EditMouseTriggers()
	{
		TriggersAndToolbars.Edit(@"Triggers\Mouse triggers.cs");
	}

	public void TT_EditWindowTriggers()
	{
		TriggersAndToolbars.Edit(@"Triggers\Window triggers.cs");
	}

	public void TT_EditCommonToolbars()
	{
		TriggersAndToolbars.Edit(@"Toolbars\Common toolbars.cs");
	}

	public void TT_EditWindowToolbars()
	{
		TriggersAndToolbars.Edit(@"Toolbars\Window toolbars.cs");
	}

	public void TT_EditScript()
	{
		TriggersAndToolbars.Edit(@"Triggers and toolbars.cs");
	}

	public void TT_RestartScript()
	{
		TriggersAndToolbars.Restart();
	}

	public void TT_DisableTriggers()
	{
		TriggersAndToolbars.DisableTriggers(null);
	}

	public void TT_RecentTriggers()
	{
		Log_.Run.Show();
	}

	public void TT_ActiveToobars()
	{
		TriggersAndToolbars.ShowActiveTriggers();
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
		AHelp.AuHelp("");
	}

	public void Help_Library()
	{
		AHelp.AuHelp("api/");
	}

	public void Help_Context()
	{
		var c = AWnd.ThisThread.FocusedWinformsControl;
		if(c == null) return;
		if(c == Panels.Editor.ZActiveDoc) {
			CiUtil.OpenSymbolOrKeywordFromPosHelp();
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
		AFile.TryRun("mailto:support@quickmacros.com?subject=" + Program.AppName);
	}

	//public void Help_Donate()
	//{

	//}

	public void Help_About()
	{

	}

	#endregion
}
