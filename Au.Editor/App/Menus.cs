using Au.Controls;
using Au.Tools;
using System.Windows.Controls;

//CONSIDER: Add top menu item "Insert". Move there the "Add ..." items from the "Code" menu.
//	Also add "Add script.setup", "Add try/catch (surround selected code)", etc.

static class Menus {
	[Command(target = "Files")]
	public static class File {
		[Command(target = "", image = "*EvaIcons.FileAddOutline #008EEE")]
		public static class New {
			static FileNode _New(string name) => App.Model.NewItem(name, beginRenaming: true);

			[Command('s', keys = "Ctrl+N", image = FileNode.c_imageScript)]
			public static void New_script() { _New("Script.cs"); }

			[Command('c', image = FileNode.c_imageClass)]
			public static void New_class() { _New("Class.cs"); }

			[Command('f', image = FileNode.c_imageFolder)]
			public static void New_folder() { _New(null); }
		}

		[Command("Delete...", separator = true, keysText = "Delete", image = "*Typicons.DocumentDelete #585858")]
		public static void Delete() { App.Model.DeleteSelected(); }

		[Command(keys = "F2", image = "*BoxIcons.RegularRename #008EEE")]
		public static void Rename() { App.Model.RenameSelected(); }

		[Command(image = "*RemixIcon.ChatSettingsLine #99BF00")]
		public static void Properties() { App.Model.Properties(); }

		[Command("Copy, paste")]
		public static class CopyPaste {
			[Command("Multi-select", checkable = true, image = "*Modern.ListTwo #99BF00", tooltip = "Multi-select (with Ctrl or Shift).\nDouble click to open.")]
			public static void MultiSelect_files() { Panels.Files.TreeControl.SetMultiSelect(toggle: true); }

			[Command("Cu_t", separator = true, keysText = "Ctrl+X")]
			public static void Cut_file() { App.Model.CutCopySelected(true); }

			[Command("Copy", keysText = "Ctrl+C")]
			public static void Copy_file() { App.Model.CutCopySelected(false); }

			[Command("Paste", keysText = "Ctrl+V")]
			public static void Paste_file() { App.Model.Paste(); }

			[Command("Cancel Cut/Copy", keysText = "Esc")]
			public static void CancelCutCopy_file() { App.Model.Uncut(); }

			[Command('r', separator = true)]
			public static void Copy_relative_path() { App.Model.SelectedCopyPath(false); }

			[Command('f')]
			public static void Copy_full_path() { App.Model.SelectedCopyPath(true); }
		}

		[Command("Open, close, go")]
		public static class OpenCloseGo {
			[Command(keysText = "Enter")]
			public static void Open() { App.Model.OpenSelected(1); }

			[Command]
			public static void Open_in_default_app() { App.Model.OpenSelected(3); }

			[Command]
			public static void Select_in_explorer() { App.Model.OpenSelected(4); }

			[Command(separator = true, target = "", keys = "Ctrl+F4", keysText = "M-click")]
			public static void Close() { App.Model.CloseEtc(FilesModel.ECloseCmd.CloseSelectedOrCurrent); }

			[Command(target = "")]
			public static void Close_all() { App.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll); }

			[Command(target = "")]
			public static void Collapse_all_folders() { App.Model.CloseEtc(FilesModel.ECloseCmd.CollapseAllFolders); }

			[Command(target = "")]
			public static void Collapse_inactive_folders() { App.Model.CloseEtc(FilesModel.ECloseCmd.CollapseInactiveFolders); }

			[Command(separator = true, target = "", keys = "Ctrl+Tab")]
			public static void Previous_document() { var a = App.Model.OpenFiles; if (a.Count > 1) App.Model.SetCurrentFile(a[1]); }

			[Command(keys = "Alt+Left", target = "", image = "*EvaIcons.ArrowBack #585858")]
			public static void Go_back() { App.Model.EditGoBack.GoBack(); }

			[Command(keys = "Alt+Right", target = "", image = "*EvaIcons.ArrowForward #585858")]
			public static void Go_forward() { App.Model.EditGoBack.GoForward(); }
		}

		//[Command]
		//public static class More
		//{
		//	//[Command("...", separator = true)]
		//	//public static void Print_setup() { }

		//	//[Command("...")]
		//	//public static void Print() { }
		//}

		[Command("Export, import", separator = true)]
		public static class ExportImport {
			[Command("Export as .zip...")]
			public static void Export_as_zip() { App.Model.ExportSelected(zip: true); }

			[Command("...")]
			public static void Export_as_workspace() { App.Model.ExportSelected(zip: false); }

			[Command("Import .zip...", separator = true)]
			public static void Import_zip() {
				var d = new FileOpenSaveDialog("{4D1F3AFB-DA1A-45AC-8C12-41DDA5C51CDA}") { Title = "Import .zip", FileTypes = "Zip files|*.zip" };
				if (d.ShowOpen(out string s, App.Hmain))
					App.Model.ImportWorkspace(s);
			}

			[Command("...")]
			public static void Import_workspace() {
				var d = new FileOpenSaveDialog("{4D1F3AFB-DA1A-45AC-8C12-41DDA5C51CDA}") { Title = "Import workspace" };
				if (d.ShowOpen(out string s, App.Hmain, selectFolder: true))
					App.Model.ImportWorkspace(s);
			}

			[Command("...")]
			public static void Import_files() { App.Model.ImportFiles(); }
		}

		[Command(target = "")]
		public static class Workspace {
			[Command("...", separator = true)]
			public static void Open_workspace() { FilesModel.OpenWorkspaceUI(); }

			[Command("...")]
			public static void New_workspace() { FilesModel.NewWorkspaceUI(); }

			[Command(separator = true, keys = "Ctrl+S", image = "*BoxIcons.RegularSave #585858")]
			public static void Save_now() { App.Model?.Save.AllNowIfNeed(); }
		}

		[Command(separator = true, target = "", keysText = "Alt+F4")]
		public static void Close_window() { App.Wmain.Close(); }

		[Command(target = "")]
		public static void Exit() {
			App.Wmain.Hide();
			App.Wmain.Close();
		}
	}

	[Command(target = "Edit")]
	public static class Edit {
		[Command(keysText = "Ctrl+Z", image = "*Ionicons.UndoiOS #9F5300")]
		public static void Undo() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_UNDO); }

		[Command(keysText = "Ctrl+Y", image = "*Ionicons.RedoiOS #9F5300")]
		public static void Redo() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_REDO); }

		[Command('t', separator = true, keysText = "Ctrl+X", image = "*Zondicons.EditCut #9F5300")]
		public static void Cut() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_CUT); }

		[Command(keysText = "Ctrl+C", image = "*Material.ContentCopy #9F5300")]
		public static void Copy() { Panels.Editor.ZActiveDoc.ECopy(); }

		[Command(keysText = "Ctrl+V", image = "*Material.ContentPaste #9F5300")]
		public static void Paste() { Panels.Editor.ZActiveDoc.EPaste(); }

		[Command]
		public static class Other_formats {
			[Command(image = "*Material.ForumOutline #9F5300", text = "Copy _forum code")]
			public static void Forum_copy() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.Forum); }

			[Command("Copy HTML <span style>")]
			public static void Copy_HTML_span_style() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.HtmlSpanStyle); }

			[Command("Copy HTML <span class> and CSS")]
			public static void Copy_HTML_span_class_CSS() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.HtmlSpanClassCss); }

			[Command("Copy HTML <span class>")]
			public static void Copy_HTML_span_class() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.HtmlSpanClass); }

			[Command]
			public static void Copy_markdown() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.Markdown); }

			[Command]
			public static void Copy_without_screenshots() { Panels.Editor.ZActiveDoc.ECopy(SciCode.ECopyAs.TextWithoutScreenshots); }
		}

		[Command(separator = true, keys = "Ctrl+F", image = "*Material.FindReplace #008EEE")]
		public static void Find() { Panels.Find.ZCtrlF(Panels.Editor.ZActiveDoc); }

		//[Command(keys = "Ctrl+Shift+F")]
		//public static void Find_in_files() { Panels.Find.ZCtrlF(Panels.Editor.ZActiveDoc, findInFiles: true); }

		[Command(separator = true, keysText = "Ctrl+Space", image = "*FontAwesome.ListUlSolid #B340FF")]
		public static void Autocompletion_list() { CodeInfo.ShowCompletionList(Panels.Editor.ZActiveDoc); }

		[Command(keysText = "Ctrl+Shift+Space", image = "*RemixIcon.ParenthesesLine #B340FF")]
		public static void Parameter_info() { CodeInfo.ShowSignature(); }

		[Command(keysText = "F12", image = "*RemixIcon.WalkFill #B340FF")]
		public static void Go_to_definition() { CiGoTo.GoToDefinition(); }

		[Command(separator = true)]
		public static class Document {
			[Command(image = "*Material.CommentEditOutline #9F5300")]
			public static void Add_file_description() { InsertCode.AddFileDescription(); }

			[Command(image = "*Codicons.SymbolClass #9F5300")]
			public static void Add_class_Program() { InsertCode.AddClassProgram(); }

			[Command(image = "*PixelartIcons.AlignLeft #9F5300")]
			public static void Format_document() { ModifyCode.Format(false); }
		}

		[Command]
		public static class Selection {
			[Command(keysText = "R-click margin", keys = "Ctrl+/", image = "*BoxIcons.RegularCommentAdd #9F5300")]
			public static void Comment() { ModifyCode.CommentLines(true); }

			[Command(keysText = "R-click margin", keys = "Ctrl+\\", image = "*BoxIcons.RegularCommentMinus #9F5300")]
			public static void Uncomment() { ModifyCode.CommentLines(false); }

			[Command(keysText = "Tab", image = "*Material.FormatIndentIncrease #9F5300")]
			public static void Indent() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_TAB); }
			//SHOULDDO: now does not indent empty lines if was no indentation.

			[Command(keysText = "Shift+Tab", image = "*Material.FormatIndentDecrease #9F5300")]
			public static void Unindent() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_BACKTAB); }

			[Command(keysText = "Ctrl+D")]
			public static void Duplicate() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_SELECTIONDUPLICATE); }

			[Command]
			public static void Format_selection() { ModifyCode.Format(true); }

			[Command]
			public static void Remove_screenshots() { Panels.Editor.ZActiveDoc.EImageRemoveScreenshots(); }

			[Command(separator = true, keysText = "Ctrl+A")]
			public static void Select_all() { Panels.Editor.ZActiveDoc.Call(Sci.SCI_SELECTALL); }
		}

		[Command]
		public static class Surround {
			[Command("for (repeat)")]
			public static void Surround_for() { InsertCode.SurroundFor(); }

			[Command("try (catch exceptions)")]
			public static void Surround_try_catch() { InsertCode.SurroundTryCatch(); }
		}

		[Command]
		public static class Generate {
			[Command(keys = "Ctrl+Shift+D")]
			public static void Create_delegate() { InsertCode.CreateDelegate(); }

			[Command(tooltip = "Implement interface or abstract class")]
			public static void Implement_interface() { InsertCode.ImplementInterfaceOrAbstractClass(explicitly: false); }

			[Command(tooltip = "Implement interface, private functions")]
			public static void Implement_interface_explicitly() { InsertCode.ImplementInterfaceOrAbstractClass(explicitly: true); }
		}

		[Command]
		public static class View {
			[Command(checkable = true, keys = "Ctrl+W", image = "*Codicons.WordWrap #99BF00")]
			public static void Wrap_lines() { SciCode.EToggleView_call_from_menu_only_(SciCode.EView.Wrap); }

			[Command(checkable = true, image = "*Material.TooltipImageOutline #99BF00")]
			public static void Images_in_code() { SciCode.EToggleView_call_from_menu_only_(SciCode.EView.Images); }

			[Command(checkable = true, image = "*Codicons.Preview #99BF00")]
			public static void WPF_preview(MenuItem mi) { SciCode.WpfPreviewStartStop(mi); }
		}
	}

	[Command(target = "Edit")]
	public static class Code {
		[Command(underlined: 'r', image = "*Material.RecordRec #008EEE")]
		//[Command(underlined: 'r', image = "*BoxIcons.RegularVideoRecording #008EEE")]
		public static void Input_recorder() { DInputRecorder.ShowRecorder(); }

		[Command("Find _window", image = "*BoxIcons.SolidWindowAlt #008EEE")]
		public static void wnd() { Dwnd.Dialog(); }

		[Command("Find UI _element", image = "*Material.CheckBoxOutline #008EEE")]
		public static void elm() { Delm.Dialog(); }

		[Command("Find _image", image = "*Material.ImageSearchOutline #008EEE")]
		public static void uiimage() { Duiimage.Dialog(); }

		[Command]
		public static void Quick_capturing() { QuickCapture.Info(); }

		[Command(keysText = "Ctrl+Space in string", image = "*Material.KeyboardOutline #008EEE")]
		public static void Keys() { CiTools.CmdShowKeysWindow(); }

		[Command(underlined: 'x', keysText = "Ctrl+Space in string")]
		public static void Regex() { CiTools.CmdShowRegexWindow(); }

		[Command(underlined: 'A')]
		public static void Windows_API() { new DWinapi().Show(); }
	}

	[Command("T\x2009T", target = ""/*, tooltip = "Triggers and toolbars"*/)] //FUTURE: support tooltip for menu items
	public static class TT {
		[Command('k'/*, separator = true*/)]
		public static void Hotkey_triggers() { TriggersAndToolbars.Edit(@"Triggers\Hotkey triggers.cs"); }

		[Command]
		public static void Autotext_triggers() { TriggersAndToolbars.Edit(@"Triggers\Autotext triggers.cs"); }

		[Command]
		public static void Mouse_triggers() { TriggersAndToolbars.Edit(@"Triggers\Mouse triggers.cs"); }

		[Command]
		public static void Window_triggers() { TriggersAndToolbars.Edit(@"Triggers\Window triggers.cs"); }

		[Command("...", image = "*Codicons.SymbolEvent #008EEE")]
		public static void New_trigger() { TriggersAndToolbars.NewTrigger(); }

		//rejected. It's in the quick capturing menu.
		//[Command("...")]
		//public static void Trigger_scope() { TriggersAndToolbars.TriggerScope(); }

		//[Command("...")]
		//public static void Active_triggers() {  }

		[Command]
		public static void Other_triggers() { TriggersAndToolbars.Edit(@"Triggers\Other triggers.cs"); Panels.Cookbook.OpenRecipe("Other triggers"); }

		[Command(separator = true)]
		public static void Toolbars() { TriggersAndToolbars.GoToToolbars(); }

		[Command("...", image = "*Material.ShapeRectanglePlus #008EEE")]
		public static void New_toolbar() { TriggersAndToolbars.NewToolbar(); }

		[Command("...")]
		public static void Toolbar_trigger() { TriggersAndToolbars.SetToolbarTrigger(); }

		[Command]
		public static void Active_toolbars() { TriggersAndToolbars.ShowActiveTriggers(); }

		[Command(separator = true)]
		public static void Disable_triggers() { TriggersAndToolbars.DisableTriggers(null); }

		[Command]
		public static void Restart_TT_script() { TriggersAndToolbars.Restart(); }

		[Command(separator = true)]
		public static void Script_triggers() { DCommandline.ZShow(); }
	}

	[Command(target = "Edit")]
	public static class Run {
		[Command(keys = "F5", image = "*Codicons.DebugStart #40B000")]
		public static void Run_script() { CompileRun.CompileAndRun(true, App.Model.CurrentFile, runFromEditor: true); }

		[Command(image = "*FontAwesome.StopCircleRegular #585858")]
		public static void End_task() {
			var f = App.Model.CurrentFile;
			if (f != null) {
				if (f.FindProject(out _, out var fMain)) f = fMain;
				if (App.Tasks.EndTasksOf(f)) return;
			}
			var a = App.Tasks.Items;
			if (a.Count > 0) {
				var m = new popupMenu { RawText = true };
				m.Submenu("End task", m => {
					foreach (var t in a) m[t.f.DisplayName] = o => App.Tasks.EndTask(t);
				});
				m.Show();
			}
		}

		//[Command(image = "")]
		//public static void Pause() { }

		[Command(image = "*VaadinIcons.Compile #008EEE")]
		public static void Compile() { CompileRun.CompileAndRun(false, App.Model.CurrentFile); }

		[Command("...")]
		public static void Recent() { RecentTT.Show(); } //CONSIDER: toolbar button

		[Command(separator = true)]
		public static class Debugger {
			[Command("Insert script.debug (wait for debugger to attach)")]
			public static void Debug_attach() { InsertCode.Statements("script.debug();\r\nDebugger.Break();"); }

			[Command("Insert Debugger.Break (debugger step mode)")]
			public static void Debug_break() { InsertCode.Statements("Debugger.Break();"); }

			[Command("Insert Debugger.Launch (launch VS debugger)")]
			public static void Debug_launch() { InsertCode.Statements("Debugger.Launch();"); }
		}
	}

	[Command(target = "")]
	public static class Tools {
		[Command(image = "*PicolIcons.Settings #99BF00")]
		public static void Options() { DOptions.ZShow(); }

		[Command(image = "*FontAwesome.IconsSolid #99BF00")]
		public static void Icons() { DIcons.ZShow(); }

		[Command(image = "*SimpleIcons.NuGet #99BF00")]
		public static void NuGet() { DNuget.ZShow(); }

		[Command(separator = true, target = "Output")]
		public static class Output {
			[Command(keysText = "M-click")]
			public static void Clear() { Panels.Output.ZClear(); }

			[Command("Copy", keysText = "Ctrl+C")]
			public static void Copy_output() { Panels.Output.ZCopy(); }

			[Command(keys = "Ctrl+F")]
			public static void Find_selected_text() { Panels.Output.ZFind(); }

			[Command]
			public static void History() { Panels.Output.ZHistory(); }

			[Command("Wrap lines", separator = true, checkable = true)]
			public static void Wrap_lines_in_output() { Panels.Output.ZWrapLines ^= true; }

			[Command("White space", checkable = true)]
			public static void White_space_in_output() { Panels.Output.ZWhiteSpace ^= true; }

			[Command(checkable = true)]
			public static void Topmost_when_floating() { Panels.Output.ZTopmost ^= true; }
		}
	}

	[Command(target = "")]
	public static class Help {
		[Command(image = "*FontAwesome.QuestionCircleRegular #BB54FF")]
		public static void Program_help() { HelpUtil.AuHelp(""); }

		[Command(image = "*FontAwesome.QuestionCircleRegular #EABB00")]
		public static void Library_help() { HelpUtil.AuHelp("api/"); }

		[Command(keys = "F1", image = "*FontAwesome.QuestionCircleRegular #008EEE")]
		public static void Context_help() {
			var w = Api.GetFocus();
			if (w.ClassNameIs("HwndWrapper*")) {
				//var e = Keyboard.FocusedElement as FrameworkElement;

			} else if (w == Panels.Editor.ZActiveDoc.Hwnd) {
				CiUtil.OpenSymbolOrKeywordFromPosHelp();
			}
		}

		[Command(separator = true)]
		public static void Forum() { run.itSafe("https://www.quickmacros.com/forum/forumdisplay.php?fid=19"); }

		[Command]
		public static void Email() { run.itSafe("mailto:support@quickmacros.com?subject=" + App.AppNameShort); }

		[Command]
		public static void About() {
			print.it($@"<>---- {App.AppNameLong} ----
Version: {Assembly.GetExecutingAssembly().GetName().Version}, beta.
Download: <link>https://www.quickmacros.com/au/help/<>
Source code: <link>https://github.com/qgindi/Au<>
Libraries and algorithms: <link https://dotnet.microsoft.com/download>.NET 6<>, <link https://github.com/dotnet/roslyn>Roslyn<>, <link https://github.com/dotnet/docfx>DocFX<>, <link https://www.scintilla.org/>Scintilla 5.1.5<>, <link https://www.pcre.org/>PCRE 10.33<>, <link https://www.sqlite.org/index.html>SQLite 3.38.2<>, <link https://github.com/MahApps/MahApps.Metro.IconPacks>MahApps.Metro.IconPacks<>, <link https://github.com/google/diff-match-patch>DiffMatchPatch<>, <link https://github.com/DmitryGaravsky/ILReader>ILReader<>, <link https://github.com/nemec/porter2-stemmer>Porter2Stemmer<>, Wu's Color Quantizer, Cantatore wildcard.
Folders: <link {folders.Workspace}>Workspace<>, <link {folders.ThisApp}>ThisApp<>, <link {folders.ThisAppDocuments}>ThisAppDocuments<>, <link {folders.ThisAppDataLocal}>ThisAppDataLocal<>, <link {folders.ThisAppTemp}>ThisAppTemp<>.
{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright}.
-----------------------");
		}
	}

#if TRACE
	[Command(keys = "F11", target = "")]
	public static void TEST() { Test.FromMenubar(); }

	[Command]
	public static void gc() {
		GC.Collect();
	}
#endif
}
