#define LAZY_MENUS
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
//using System.Linq;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Editor
{
	//[DebuggerStepThrough]
#if FORM2
	public partial class Form2
#else
	public partial class Form4
#endif
	{
		MenuStrip _tsMenu;
		ToolStrip _tsFile, _tsEdit, _tsRun, _tsTools, _tsHelp, _tsCustom1, _tsCustom2, _tsCustom3, _tsCustom4;
		_Commands _cmd;

		void InitStrips()
		{
			var p = new Perf.Inst(true);

			//load XML
			var x = new XmlDocument();
			x.Load(Folders.App + "CmdStrips.xml");
			p.Next();

			_cmd = new _Commands();

			p.Next();

			_tsMenu = new MenuStrip();
			_tsFile = new ToolStrip();
			_tsEdit = new ToolStrip();
			_tsRun = new ToolStrip();
			_tsTools = new ToolStrip();
			_tsHelp = new ToolStrip();
			_tsCustom1 = new ToolStrip();
			_tsCustom2 = new ToolStrip();
			_tsCustom3 = new ToolStrip();
			_tsCustom4 = new ToolStrip();

			var tsArr = new List<ToolStrip> { _tsMenu, _tsHelp, _tsTools, _tsFile, _tsRun, _tsEdit, _tsCustom1, _tsCustom2, _tsCustom3, _tsCustom4 };
			var tsNames = new string[] { "Menu", "Help", "Tools", "File", "Run", "Edit", "Custom1", "Custom2", "Custom3", "Custom4" };
#if FORM2
			var controls = this.toolStripContainer1.TopToolStripPanel.Controls;
#else
			var controls = _strips.Controls;
			_strips.SuspendLayout();
#endif
			foreach(var t in tsArr) {
				t.SuspendLayout();
			}

			var ddMenus = new Dictionary<string, ToolStripDropDownMenu>();
			//var il = EImageList.Strips;

			p.Next();
			XmlElement xDoc = x.DocumentElement;
			for(int i = 0; i < tsArr.Count; i++) {
				string name = tsNames[i];
				var xStrip = xDoc[name]; if(xStrip == null) continue;
				var t = tsArr[i];

				t.Name = t.Text = name;
				t.AllowItemReorder = true;
				t.Dock = DockStyle.None;
				//t.TabIndex = i+;
				if(i == 0) {
					t.Stretch = false;
					//t.AutoSize=false;
					t.GripStyle = ToolStripGripStyle.Visible;
					var mpad = t.Padding; mpad.Left = 0; t.Padding = mpad;
					//} else if(t== _tsFile) {

				} else {

				}

				if(xStrip.HasAttribute("hide")) t.Visible = false;

				_InitStrips_AddChildItems(xStrip, t, i == 0, ddMenus);

				//if(t == _tsFile) {

				//}else {
				t.Location = new Point(xStrip.GetAttribute("x").ToInt_(), xStrip.GetAttribute("y").ToInt_());
				var s = xStrip.GetAttribute("width");
				if(s.Length > 0) {
					t.AutoSize = false; //TODO
					t.Width = s.ToInt_();
				}
				//OutList(name, t.Location, t.Width);
				//}

				//t.ResumeLayout();
				//t.ResumeLayout(false);t.PerformLayout();
				//t.AutoSize=false;
				//t.Size = new Size(200, 25);
				//OutList(name, t.Size, t.Items.Count);

				//controls.Add(t);
				//t.ImageList = il;
				//t.ResumeLayout();
				//break;
			}

			p.Next();
			this.MainMenuStrip = _tsMenu;
			var il = EImageList.Strips;
			foreach(var t in tsArr) t.ImageList = il;
			foreach(var t in ddMenus) t.Value.ImageList = il;

			//tsArr.Sort((t1, t2) =>
			//{
			//	int y1 = t1.Top, y2 = t2.Top;
			//	if(y1 < y2) return -1;
			//	if(y1 > y2) return 1;
			//	int x1 = t1.Left, x2 = t2.Left;
			//	if(x1 < x2) return 1;
			//	if(x1 > x2) return -1;
			//	return 0;
			//});

			controls.AddRange(tsArr.ToArray());
			foreach(var t in tsArr) {
				//Out(t.Name);
				//controls.Add(t);
				t.ResumeLayout();
			}
#if FORM2
			this.toolStripContainer1.BottomToolStripPanel.ResumeLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout();
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout();
			this.toolStripContainer1.ResumeLayout();
#else
			_strips.ResumeLayout();
#endif

			//p.NW();
		}

		void _InitStrips_AddChildItems(XmlElement xParent, ToolStrip tsParent, bool isMenu, Dictionary<string, ToolStripDropDownMenu> ddMenus)
		{
			//TODO: in C#7 nest in caller.

			foreach(XmlNode xn in xParent) {
				XmlElement x = xn as XmlElement; if(x == null) continue;
				string s, tag = x.Name;

				if(tag == "sep") {
					tsParent.Items.Add(new ToolStripSeparator());
					continue;
				}

				_Commands.CommandHandler onClick = null;
				bool isCmd = _cmd.Commands.TryGetValue(tag, out onClick);

				string text = x.GetAttribute("t");
				if(text.Length == 0) {
					text = tag.Remove(0, tag.LastIndexOf('_') + 1);
					//TODO: insert spaces before uppercase letters, and measure speed.
					//TODO: hotkey.
				}

				ToolStripItem item = null;
				bool isControl = false, isDropDownButton = false;
				if(!isCmd && x.HasAttribute("type")) {
					isControl = true;
					s = x.GetAttribute("type");
					switch(s) {
					case "edit":
						var ed = new ToolStripTextBox();
						item = ed;
						//ed.BorderStyle = BorderStyle.FixedSingle;

						s = x.GetAttribute("cue");
						if(s.Length > 0) ((Wnd)ed.TextBox.Handle).SendS(_Api.EM_SETCUEBANNER, 0, s);

						break;
					default: continue;
					}

					int width = x.GetAttribute("width").ToInt_();
					if(width > 0) { item.AutoSize = false; item.Width = width; }

				} else if(isMenu) {
					var k = new ToolStripMenuItem(text);
					item = k;
#if !LAZY_MENUS
					if(!isCmd) {
						var dd = new ToolStripDropDownMenu();
						_InitStrips_AddChildItems(x, dd, true, ddMenus);
						k.DropDown = dd;
						ddMenus.Add(tag, dd);
					}
#endif
				} else if(isDropDownButton = x.HasAttribute("dd")) {
					var k = isCmd ? (new ToolStripSplitButton(text) as ToolStripDropDownItem) : (new ToolStripDropDownButton(text) as ToolStripDropDownItem);
					item = k;
#if !LAZY_MENUS
					ToolStripDropDownMenu dd;
					s = x.GetAttribute("dd");
					if(s.Length > 0) dd = ddMenus[s];
					else {
						dd = new ToolStripDropDownMenu();
						_InitStrips_AddChildItems(x, dd, true, ddMenus);
					}
					k.DropDown = dd;
#endif
				} else item = new ToolStripButton(text);

				s = x.GetAttribute("i"); if(s.Length > 0) item.ImageIndex = s.ToInt_();

				if(!isMenu && !isControl) {
					if(false) { //option: show labels
						item.AutoToolTip = false;
					} else {

						item.DisplayStyle = ToolStripItemDisplayStyle.Image;
					}
					//item.ToolTipText = text;
				}

				//xParent.SelectSingleNode("//")
				if(isCmd) {
				} else {
					//Out(tag);

					//_InitStrips_AddChildItems(x, , true);
				}


				tsParent.Items.Add(item);
			}
		}

		partial class _Commands
		{
			internal delegate void CommandHandler();
			Dictionary<string, CommandHandler> _commands = new Dictionary<string, CommandHandler>(200);

			internal Dictionary<string, CommandHandler> Commands { get { return _commands; } }

			internal bool m_inRightClick;
			EventHandler _onClick;
			System.Collections.Hashtable _clickDelegates = new System.Collections.Hashtable();

			//Common Click even handler of all items.
			//Calls true item's onClick delegate if need.
			void _OnClick(object sender, EventArgs args)
			{
				//Out(_inRightClick);
				if(m_inRightClick) return;
				var d = _clickDelegates[sender] as CommandHandler;
				Debug.Assert(d != null); if(d == null) return;
				d();
			}

			internal _Commands()
			{
				_onClick = _OnClick;

				//TODO: JIT-compiling all the delegates takes 7 ms when all empty.
				//	Probably will be much more when added code.
				//	Should add delegates later, or on demand, or in other thread.

#region add to _commands
				//Code generated by macro 'Generate Catkeys menu-toolbar command code'.

				_commands.Add(nameof(File_NewScript), File_NewScript);
				_commands.Add(nameof(File_NewLibrary), File_NewLibrary);
				_commands.Add(nameof(File_NewFolder), File_NewFolder);
				_commands.Add(nameof(File_Import), File_Import);
				_commands.Add(nameof(File_Disable), File_Disable);
				_commands.Add(nameof(File_Rename), File_Rename);
				_commands.Add(nameof(File_Delete), File_Delete);
				_commands.Add(nameof(File_Properties), File_Properties);
				_commands.Add(nameof(File_Open), File_Open);
				_commands.Add(nameof(File_OpenInNewWindow), File_OpenInNewWindow);
				_commands.Add(nameof(File_SelectInExplorer), File_SelectInExplorer);
				_commands.Add(nameof(File_PreviousDocument), File_PreviousDocument);
				_commands.Add(nameof(File_Close), File_Close);
				_commands.Add(nameof(File_CloseAll), File_CloseAll);
				_commands.Add(nameof(File_CollapseFolders), File_CollapseFolders);
				_commands.Add(nameof(File_CopyName), File_CopyName);
				_commands.Add(nameof(File_CopyPath), File_CopyPath);
				_commands.Add(nameof(File_SelectMultiple), File_SelectMultiple);
				_commands.Add(nameof(File_Cut), File_Cut);
				_commands.Add(nameof(File_Copy), File_Copy);
				_commands.Add(nameof(File_Paste), File_Paste);
				_commands.Add(nameof(File_Clone), File_Clone);
				_commands.Add(nameof(File_PrintSetup), File_PrintSetup);
				_commands.Add(nameof(File_Print), File_Print);
				_commands.Add(nameof(File_OpenCollection), File_OpenCollection);
				_commands.Add(nameof(File_ImportCollection), File_ImportCollection);
				_commands.Add(nameof(File_ExportCollection), File_ExportCollection);
				_commands.Add(nameof(File_FindInCollections), File_FindInCollections);
				_commands.Add(nameof(File_CollectionProperties), File_CollectionProperties);
				_commands.Add(nameof(File_SaveAllNow), File_SaveAllNow);
				_commands.Add(nameof(File_CloseWindow), File_CloseWindow);
				_commands.Add(nameof(File_ExitProgram), File_ExitProgram);
				_commands.Add(nameof(Edit_Undo), Edit_Undo);
				_commands.Add(nameof(Edit_Redo), Edit_Redo);
				_commands.Add(nameof(Edit_Cut), Edit_Cut);
				_commands.Add(nameof(Edit_Copy), Edit_Copy);
				_commands.Add(nameof(Edit_Paste), Edit_Paste);
				_commands.Add(nameof(Edit_Find), Edit_Find);
				_commands.Add(nameof(Edit_Members), Edit_Members);
				_commands.Add(nameof(Edit_ContextHelp), Edit_ContextHelp);
				_commands.Add(nameof(Edit_GoToDefinition), Edit_GoToDefinition);
				_commands.Add(nameof(Edit_PeekDefinition), Edit_PeekDefinition);
				_commands.Add(nameof(Edit_FindReferences), Edit_FindReferences);
				_commands.Add(nameof(Edit_Indent), Edit_Indent);
				_commands.Add(nameof(Edit_Unindent), Edit_Unindent);
				_commands.Add(nameof(Edit_Comment), Edit_Comment);
				_commands.Add(nameof(Edit_Uncomment), Edit_Uncomment);
				_commands.Add(nameof(Edit_HideRegion), Edit_HideRegion);
				_commands.Add(nameof(Edit_SelectAll), Edit_SelectAll);
				_commands.Add(nameof(Edit_ImagesInCode), Edit_ImagesInCode);
				_commands.Add(nameof(Edit_WrapLines), Edit_WrapLines);
				_commands.Add(nameof(Edit_LineNumbers), Edit_LineNumbers);
				_commands.Add(nameof(Edit_IndentationGuides), Edit_IndentationGuides);
				_commands.Add(nameof(Run_Run), Run_Run);
				_commands.Add(nameof(Run_End), Run_End);
				_commands.Add(nameof(Run_Pause), Run_Pause);
				_commands.Add(nameof(Run_Compile), Run_Compile);
				_commands.Add(nameof(Run_AutoMinimize), Run_AutoMinimize);
				_commands.Add(nameof(Run_DisableTriggers), Run_DisableTriggers);
				_commands.Add(nameof(Run_MakeExe), Run_MakeExe);
				_commands.Add(nameof(Debug_RunToBreakpoint), Debug_RunToBreakpoint);
				_commands.Add(nameof(Debug_RunToCursor), Debug_RunToCursor);
				_commands.Add(nameof(Debug_StepInto), Debug_StepInto);
				_commands.Add(nameof(Debug_StepOver), Debug_StepOver);
				_commands.Add(nameof(Debug_StepOut), Debug_StepOut);
				_commands.Add(nameof(Debug_ToggleBreakpoint), Debug_ToggleBreakpoint);
				_commands.Add(nameof(Debug_PersistentBreakpoint), Debug_PersistentBreakpoint);
				_commands.Add(nameof(Debug_ClearLocalBreakpoints), Debug_ClearLocalBreakpoints);
				_commands.Add(nameof(Debug_ClearAllBreakpoints), Debug_ClearAllBreakpoints);
				_commands.Add(nameof(Debug_DebugOptions), Debug_DebugOptions);
				_commands.Add(nameof(View_ToolbarFile), View_ToolbarFile);
				_commands.Add(nameof(View_ToolbarEdit), View_ToolbarEdit);
				_commands.Add(nameof(View_ToolbarRun), View_ToolbarRun);
				_commands.Add(nameof(View_ToolbarDebug), View_ToolbarDebug);
				_commands.Add(nameof(View_ToolbarView), View_ToolbarView);
				_commands.Add(nameof(View_ToolbarTools), View_ToolbarTools);
				_commands.Add(nameof(View_ToolbarHelp), View_ToolbarHelp);
				_commands.Add(nameof(View_ToolbarCustom1), View_ToolbarCustom1);
				_commands.Add(nameof(View_ToolbarCustom2), View_ToolbarCustom2);
				_commands.Add(nameof(View_ToolbarCustom3), View_ToolbarCustom3);
				_commands.Add(nameof(View_ToolbarCustom4), View_ToolbarCustom4);
				_commands.Add(nameof(View_Output), View_Output);
				_commands.Add(nameof(View_Find), View_Find);
				_commands.Add(nameof(View_Tips), View_Tips);
				_commands.Add(nameof(View_Running), View_Running);
				_commands.Add(nameof(View_Recent), View_Recent);
				_commands.Add(nameof(View_Tags), View_Tags);
				_commands.Add(nameof(Tools_Record), Tools_Record);
				_commands.Add(nameof(Tools_RecordMenu), Tools_RecordMenu);
				_commands.Add(nameof(Tools_RecordSingleAction), Tools_RecordSingleAction);
				_commands.Add(nameof(Tools_DialogEditor), Tools_DialogEditor);
				_commands.Add(nameof(Tools_ToolbarEditor), Tools_ToolbarEditor);
				_commands.Add(nameof(Tools_MenuEditor), Tools_MenuEditor);
				_commands.Add(nameof(Tools_ImagelistEditor), Tools_ImagelistEditor);
				_commands.Add(nameof(Tools_Resources), Tools_Resources);
				_commands.Add(nameof(Tools_Icons), Tools_Icons);
				_commands.Add(nameof(Tools_HelpEditor), Tools_HelpEditor);
				_commands.Add(nameof(Tools_RegularExpressions), Tools_RegularExpressions);
				_commands.Add(nameof(Tools_ExploreWindows), Tools_ExploreWindows);
				_commands.Add(nameof(Tools_RemapKeys), Tools_RemapKeys);
				_commands.Add(nameof(Tools_Components), Tools_Components);
				_commands.Add(nameof(Tools_Portable), Tools_Portable);
				_commands.Add(nameof(Tools_Options), Tools_Options);
				_commands.Add(nameof(Tools_Output_Clear), Tools_Output_Clear);
				_commands.Add(nameof(Tools_Output_Copy), Tools_Output_Copy);
				_commands.Add(nameof(Tools_Output_FindSelectedText), Tools_Output_FindSelectedText);
				_commands.Add(nameof(Tools_Output_History), Tools_Output_History);
				_commands.Add(nameof(Tools_Output_LogWindowEvents), Tools_Output_LogWindowEvents);
				_commands.Add(nameof(Tools_Output_LogAccEvents), Tools_Output_LogAccEvents);
				_commands.Add(nameof(Tools_Output_WrapLines), Tools_Output_WrapLines);
				_commands.Add(nameof(Tools_Output_WhiteSpace), Tools_Output_WhiteSpace);
				_commands.Add(nameof(Tools_Output_Topmost), Tools_Output_Topmost);
				_commands.Add(nameof(Tools_Statusbar_Floating), Tools_Statusbar_Floating);
				_commands.Add(nameof(Tools_Statusbar_MouseInfo), Tools_Statusbar_MouseInfo);
				_commands.Add(nameof(Tools_Statusbar_AutoHeight), Tools_Statusbar_AutoHeight);
				_commands.Add(nameof(Tools_Statusbar_SendToOutput), Tools_Statusbar_SendToOutput);
				_commands.Add(nameof(Help_QuickStart), Help_QuickStart);
				_commands.Add(nameof(Help_Reference), Help_Reference);
				_commands.Add(nameof(Help_ContextHelp), Help_ContextHelp);
				_commands.Add(nameof(Help_Download), Help_Download);
				_commands.Add(nameof(Help_Forum), Help_Forum);
				_commands.Add(nameof(Help_Email), Help_Email);
				_commands.Add(nameof(Help_Donate), Help_Donate);
				_commands.Add(nameof(Help_About), Help_About);

#endregion add

			}

		}
	}
}
