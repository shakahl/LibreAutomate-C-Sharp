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
//using System.Linq;

using Au;
using Au.Types;
using Au.More;

/// <summary>
/// Program settings.
/// folders.ThisAppDocuments + @".settings\Settings.json"
/// </summary>
record AppSettings : JSettings
{
	//This is loaded at startup and therefore must be fast.
	//	Don't use types that would cause to load UI dlls (WPF etc). Eg when it is a nested type and its parent class is a WPF etc control.
	//	Tested with .NET 5: first time takes ~40 ms. Mostly to load/jit/etc dlls used in JSON deserialization, which then is fast regardless of data size.

	public static AppSettings Load() => Load<AppSettings>(DirBS + "Settings.json");
	//CONSIDER: it takes 60 ms. We can Jit_ something in other thread. But it isn't good when runs at PC startup.

	public static readonly string DirBS = folders.ThisAppDocuments + @".settings\";

	public string user, workspace;
	public string[] recentWS;

	public bool runHidden, files_multiSelect;

	public string wndPos, tools_Dwnd_wndPos, tools_Delm_wndPos, tools_Duiimage_wndPos;

	public FRRecentItem[] find_recent, find_recentReplace;
	public string find_skip;
	public int find_searchIn, find_printSlow = 50;

	public bool edit_wrap, edit_noImages, output_wrap, output_white, output_topmost;

	public int templ_use;

	public bool ci_complGroup = true;
	public int ci_complParen; //0 spacebar, 1 always, 2 never
	public byte ci_shiftEnterAlways, ci_shiftTabAlways;
	public string ci_usings = @"Au
Au.Types
Au.More
System
System.Collections.Generic
System.Collections.Concurrent
System.Linq
System.Text
System.Threading.Tasks
System.Threading
System.Diagnostics
System.Globalization
System.IO
System.IO.Compression
System.Media
System.Runtime.CompilerServices
System.Runtime.InteropServices
Microsoft.Win32
";
	//public SIZE ci_sizeSignXaml, ci_sizeComplXaml, ci_sizeComplList;

	public string db_copy_ref, db_copy_doc, db_copy_winapi;
}

/// <summary>
/// Workspace settings.
/// WorkspaceDirectory + @"\settings.json"
/// </summary>
record WorkspaceSettings : JSettings
{
	public static WorkspaceSettings Load(string jsonFile) => Load<WorkspaceSettings>(jsonFile);

	public FilesModel.UserData[] users;

}
