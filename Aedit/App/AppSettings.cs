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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Util;

/// <summary>
/// Program settings.
/// AFolders.ThisAppDocuments + @".settings\Settings.json"
/// </summary>
class AppSettings : ASettings
{
	//This is loaded at startup and therefore must be fast.
	//	Don't use types that would cause to load UI dlls (WPF etc). Eg when it is a nested type and its parent class is a WPF etc control.
	//	Tested with .NET 5: first time takes ~40 ms. Mostly to load/jit/etc dlls used in JSON deserialization, which then is fast regardless of data size.

	public static AppSettings Load() => Load<AppSettings>(DirBS + "Settings.json");

	public static readonly string DirBS = AFolders.ThisAppDocuments + @".settings\";

	public string user { get => _user; set => Set(ref _user, value); }
	string _user;

	public string workspace { get => _workspace; set => Set(ref _workspace, value); }
	string _workspace;

	public string[] recentWS { get => _recent; set => SetNoCmp(ref _recent, value); }
	string[] _recent;

	public bool runHidden { get => _runHidden; set => Set(ref _runHidden, value); }
	bool _runHidden;

	public string wndPos { get => _wndpos; set => Set(ref _wndpos, value); }
	string _wndpos;

	public string tools_AWnd_wndPos { get => _tools_AWnd_wndPos; set => Set(ref _tools_AWnd_wndPos, value); }
	string _tools_AWnd_wndPos;

	public string tools_AAcc_wndPos { get => _tools_AAcc_wndPos; set => Set(ref _tools_AAcc_wndPos, value); }
	string _tools_AAcc_wndPos;

	public string tools_AWinImage_wndPos { get => _tools_AWinImage_wndPos; set => Set(ref _tools_AWinImage_wndPos, value); }
	string _tools_AWinImage_wndPos;

	public FRRecentItem[] find_recent { get => _find_recent; set => SetNoCmp(ref _find_recent, value); }
	FRRecentItem[] _find_recent;

	public FRRecentItem[] find_recentReplace { get => _find_recentReplace; set => SetNoCmp(ref _find_recentReplace, value); }
	FRRecentItem[] _find_recentReplace;

	public string find_skip { get => _find_skip; set => Set(ref _find_skip, value); }
	string _find_skip;

	public int find_searchIn { get => _find_searchIn; set => Set(ref _find_searchIn, value); }
	int _find_searchIn;

	public bool edit_wrap { get => _edit_wrap; set => Set(ref _edit_wrap, value); }
	bool _edit_wrap;

	public bool edit_noImages { get => _edit_noImages; set => Set(ref _edit_noImages, value); }
	bool _edit_noImages;

	public bool output_wrap { get => _output_wrap; set => Set(ref _output_wrap, value); }
	bool _output_wrap;

	public bool output_white { get => _output_white; set => Set(ref _output_white, value); }
	bool _output_white;

	public bool output_topmost { get => _output_topmost; set => Set(ref _output_topmost, value); }
	bool _output_topmost;

	public int templ_use { get => _templ_use; set => Set(ref _templ_use, value); }
	int _templ_use;

	public bool ci_complGroup { get => _ci_complGroup; set => Set(ref _ci_complGroup, value); }
	bool _ci_complGroup = true;

	public bool ci_complParenSpace { get => _ci_complParenSpace; set => Set(ref _ci_complParenSpace, value); }
	bool _ci_complParenSpace;

	public byte ci_correctStringEnter { get => _ci_correctStringEnter; set => Set(ref _ci_correctStringEnter, value); }
	byte _ci_correctStringEnter;

	//public SIZE ci_sizeSignXaml { get => _ci_sizeSignXaml; set => Set(ref _ci_sizeSignXaml, value); }
	//SIZE _ci_sizeSignXaml;

	//public SIZE ci_sizeComplXaml { get => _ci_sizeComplXaml; set => Set(ref _ci_sizeComplXaml, value); }
	//SIZE _ci_sizeComplXaml;

	//public SIZE ci_sizeComplList { get => _ci_sizeComplList; set => Set(ref _ci_sizeComplList, value); }
	//SIZE _ci_sizeComplList;

	public string db_copy_ref { get => _db_copy_ref; set => Set(ref _db_copy_ref, value); }
	string _db_copy_ref;

	public string db_copy_doc { get => _db_copy_doc; set => Set(ref _db_copy_doc, value); }
	string _db_copy_doc;

	public string db_copy_winapi { get => _db_winapi; set => Set(ref _db_winapi, value); }
	string _db_winapi;
}

/// <summary>
/// Workspace settings.
/// WorkspaceDirectory + @"\settings.json"
/// </summary>
class WorkspaceSettings : ASettings
{
	public static WorkspaceSettings Load(string jsonFile) => Load<WorkspaceSettings>(jsonFile);

	public FilesModel.UserData[] users { get => _users; set => SetNoCmp(ref _users, value); }
	FilesModel.UserData[] _users;

}
