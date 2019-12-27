using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

/// <summary>
/// Program settings.
/// AFolders.ThisAppDocuments + @"!Settings\Settings.json"
/// </summary>
class ProgramSettings : Au.Util.JSettings
{
	public static ProgramSettings Load() => _Load<ProgramSettings>(AFolders.ThisAppDocuments + @"!Settings\Settings.json");

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

	public PanelFind.RecentItem[] find_recent { get => _find_recent; set => SetNoCmp(ref _find_recent, value); }
	PanelFind.RecentItem[] _find_recent;

	public PanelFind.RecentItem[] find_recentReplace { get => _find_recentReplace; set => SetNoCmp(ref _find_recentReplace, value); }
	PanelFind.RecentItem[] _find_recentReplace;

	public string find_skip { get => _find_skip; set => Set(ref _find_skip, value); }
	string _find_skip;

	public int find_searchIn { get => _find_searchIn; set => Set(ref _find_searchIn, value); }
	int _find_searchIn;

	public bool ci_complGroup { get => _ci_complGroup; set => Set(ref _ci_complGroup, value); }
	bool _ci_complGroup = true;

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

	public CiStyling.TStyles edit_styles { get => _edit_styles ??= new CiStyling.TStyles(); set => SetNoCmp(ref _edit_styles, value); }
	CiStyling.TStyles _edit_styles;

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
class WorkspaceSettings : Au.Util.JSettings
{
	public static WorkspaceSettings Load(string jsonFile) => _Load<WorkspaceSettings>(jsonFile);

	public FilesModel.UserData[] users { get => _users; set => SetNoCmp(ref _users, value); }
	FilesModel.UserData[] _users;

}
