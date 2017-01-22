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
using G.Controls;

namespace Editor
{
	//[DebuggerStepThrough]
	public partial class MainForm
	{
		MenuStrip _tsMenu;
		CatToolStrip _tsFile, _tsEdit, _tsRun, _tsTools, _tsHelp, _tsCustom1, _tsCustom2;
		_Commands _cmd;

		void InitStrips()
		{
			var p = new Perf.Inst(true);

			//load XML
			var x = new XmlDocument();
			x.Load(Folders.ThisApp + "CmdStrips.xml");
			p.Next();

			_cmd = new _Commands();

			p.Next();

			_tsMenu = new MenuStrip();
			_tsFile = new CatToolStrip();
			_tsEdit = new CatToolStrip();
			_tsRun = new CatToolStrip();
			_tsTools = new CatToolStrip();
			_tsHelp = new CatToolStrip();
			_tsCustom1 = new CatToolStrip();
			_tsCustom2 = new CatToolStrip();

			var tsArr = new List<ToolStrip> { _tsMenu, _tsFile, _tsEdit, _tsRun, _tsTools, _tsHelp, _tsCustom1, _tsCustom2 };
			var tsNames = new string[] { "Menu", "File", "Edit", "Run", "Tools", "Help", "Custom1", "Custom2" };

			foreach(var t in tsArr) {
				t.SuspendLayout();
			}

			var ddMenus = new Dictionary<string, ToolStripDropDownMenu>();
			//var il = EImageList.Strips;

			var tsRenderer = new DockedToolStripRenderer();

			p.Next();
			XmlElement xDoc = x.DocumentElement;

			//common submenus, ie the same submenu can be used by one or more submenus and/or toolbar dropdown buttons
			var xCommon = xDoc["common"];
			var ddCommon = new ToolStripDropDownMenu();
			__InitStrips_AddChildItems(xCommon, ddCommon, true, ddMenus);

			for(int i = 0; i < tsArr.Count; i++) {
				string name = tsNames[i];
				var xStrip = xDoc[name]; if(xStrip == null) continue;
				var t = tsArr[i];

				//PrintList(name, t.Height, t.Width);

				t.Name = t.Text = name;
				t.AllowItemReorder = true;
				t.Renderer = tsRenderer;
				//GDockPanel will set other styles

				__InitStrips_AddChildItems(xStrip, t, i == 0, ddMenus);
			}

			p.Next();
			_tsMenu.Padding = new Padding();
			this.MainMenuStrip = _tsMenu;
			var il = EImageList.Strips;
			foreach(var t in tsArr) t.ImageList = il;
#if !LAZY_MENUS
			foreach(var t in ddMenus) t.Value.ImageList = il;
#endif

			//#if DEBUG
			//			var mi = _tsMenu.Items[0] as ToolStripMenuItem;
			//			(mi.DropDown.Items.Add("test", null, (unu, sed) => Print("test")) as ToolStripMenuItem).ShortcutKeys = Keys.Control | Keys.P;
			//#endif

			foreach(var t in tsArr) {
				//Print(t.Name);
				t.ResumeLayout(false);
			}

			//p.NW();
		}

		void __InitStrips_AddChildItems(XmlElement xParent, ToolStrip tsParent, bool isMenu, Dictionary<string, ToolStripDropDownMenu> ddMenus)
		{
			foreach(XmlNode xn in xParent) {
				XmlElement x = xn as XmlElement; if(x == null) continue;
				string s, tag = x.Name;

				if(tag == "sep") {
					tsParent.Items.Add(new ToolStripSeparator());
					continue;
				}

				_Commands.CommandHandler onClick = null;
				bool isCmd = _cmd.Commands.TryGetValue(tag, out onClick);

				string text = x.Attribute_("t");
				if(text == null) {
					text = tag.Remove(0, tag.LastIndexOf('_') + 1);
					//TODO: insert spaces before uppercase letters, and measure speed.
					//TODO: hotkey.
				}

				ToolStripItem item = null;
				bool isControl = false, isDropDownButton = false;
				if(!isCmd && x.HasAttribute("type")) {
					isControl = true;
					string cue = x.Attribute_("cue");
					s = x.GetAttribute("type");
					switch(s) {
					case "edit":
						var ed = new ToolStripSpringTextBox();
						if(cue != null) ed.SetCueBanner(cue);
						item = ed;
						break;
					case "combo":
						var combo = new ToolStripSpringComboBox();
						if(cue != null) combo.SetCueBanner(cue);
						item = combo;
						break;
					default: continue;
					}
					if(cue != null) item.AccessibleName = cue;
				} else if(isMenu) {
					var k = new ToolStripMenuItem(text);
					item = k;
					if(!isCmd) __InitStrips_AddChildItems_Submenu(x, k, tag, ddMenus);
				} else if(isDropDownButton = x.HasAttribute("dd")) {
					var k = isCmd ? (new ToolStripSplitButton(text) as ToolStripDropDownItem) : (new ToolStripDropDownButton(text) as ToolStripDropDownItem);
					item = k;
					__InitStrips_AddChildItems_Submenu(x, k, tag, ddMenus);
				} else {
					item = new ToolStripButton(text);
				}

				int ii = x.Attribute_("i", -1); if(ii >= 0) item.ImageIndex = ii;

				var tt = x.Attribute_("tt"); if(tt != null) item.ToolTipText = tt;

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
					//Print(tag);

					//_InitStrips_AddChildItems(x, , true);
				}

				item.Name = tag;
				tsParent.Items.Add(item);
			}
		}


		void __InitStrips_AddChildItems_Submenu(XmlElement x, ToolStripDropDownItem ddItem, string tag, Dictionary<string, ToolStripDropDownMenu> ddMenus)
		{
			ToolStripDropDownMenu dd;
			var s = x.GetAttribute("dd");
			if(s.Length > 0) {
				ddItem.DropDown = ddMenus[s];
			} else {
				dd = new ToolStripDropDownMenu();
				ddItem.DropDown = dd;
				ddMenus.Add(tag, dd);
#if !LAZY_MENUS
				__InitStrips_AddChildItems(x, dd, true, ddMenus);
#else
				//This saves ~50 ms of startup time, eg 170 -> 120 ms.
				//Can do it with Opening event or with timer. With timer easier. With event users cannot use MSAA etc to automate clicking menu items (with timer cannot use it only the first 1-2 seconds).
#if false
				dd.Items.Add(new ToolStripSeparator());
				CancelEventHandler eh = null;
				eh = (sender, e) =>
				{
					dd.Opening -= eh;
					dd.Items.Clear();
					dd.ImageList = EImageList.Strips;
					__InitStrips_AddChildItems(x, dd, true, ddMenus);
				};
				dd.Opening += eh;
#else
				Time.SetTimer(500, true, t =>
				{
					dd.ImageList = EImageList.Strips;
					__InitStrips_AddChildItems(x, dd, true, ddMenus);
				});
#endif
#endif
			}
		}

		//private void DdItem_DropDownOpening(object sender, EventArgs e)
		//{
		//	Print(1);
		//}

		public class DockedToolStripRenderer :ToolStripProfessionalRenderer
		{
			public DockedToolStripRenderer()
			{
				this.RoundedEdges = false;
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
				//Print(_inRightClick);
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
