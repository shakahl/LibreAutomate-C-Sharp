/// <summary>
/// Program settings.
/// folders.ThisAppDocuments + @".settings\Settings.json"
/// </summary>
record AppSettings : JSettings
{
	//This is loaded at startup and therefore must be fast.
	//	NOTE: Don't use types that would cause to load UI dlls (WPF etc). Eg when it is a nested type and its parent class is a WPF etc control.
	//	Speed tested with .NET 5: first time 40-60 ms. Mostly to load/jit/etc dlls used in JSON deserialization, which then is fast regardless of data size.
	//	CONSIDER: Jit_ something in other thread. But it isn't good when runs at PC startup.

	public static AppSettings Load() => Load<AppSettings>(DirBS + "Settings.json");

	public static readonly string DirBS = folders.ThisAppDocuments + @".settings\";

	public string user, workspace;
	public string[] recentWS;

	public bool runHidden, files_multiSelect;

	public string wndPos, tools_Dwnd_wndPos, tools_Delm_wndPos, tools_Duiimage_wndPos;

	public record hotkeys_t //somehow does not support struct and record struct, InvalidCastException
	{
		public string capture_menu = "Ctrl+Shift+Q", capture_wnd = "Ctrl+Shift+W", capture_elm = "Ctrl+Shift+E";
		//tested: adding/removing members works well.
		//tested: tuple does not work well. New members are null. Also item names in file are like "Item1".
	}
	public hotkeys_t hotkeys = new();

	public bool edit_wrap, edit_noImages, output_wrap, output_white, output_topmost;

	public int templ_use;
	public int templ_flags;

	//public byte ci_shiftEnterAlways, ci_shiftTabAlways;
	//public SIZE ci_sizeSignXaml, ci_sizeComplXaml, ci_sizeComplList;
	public bool ci_complGroup = true;
	public int ci_complParen; //0 spacebar, 1 always, 2 never

	public int tools_Delm_flags;

	public string db_copy_ref, db_copy_doc, db_copy_winapi;

	public Dictionary<string, CiGoTo.AssemblySett> ci_gotoAsm;

	public string find_skip;
	public int find_searchIn, find_printSlow = 50;
	public FRRecentItem[] find_recent, find_recentReplace; //possibly-big arrays should be at the end
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
