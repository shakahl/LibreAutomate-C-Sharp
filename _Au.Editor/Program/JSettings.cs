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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

using Au;
using Au.Types;
using static Au.AStatic;

/// <summary>
/// Base of classes that load/save various settings from/to a JSON file.
/// The derived class must provide public get/set properties.
/// Getters usually just return a field.
/// Setters must call <see cref="_Set"/> or <see cref="_SetNoCmp"/>. Then changes will be automatically lazily saved.
/// </summary>
class JSettings
{
	protected string _file;
	protected bool _loaded;
	protected bool _save;
	Action _onTimer;
	EventHandler _onExit;

	/// <summary>
	/// Loads JSON file and deserializes to new object of type T.
	/// Sets to save automatically whenever a property changed.
	/// Returns new empty object if file does not exist or failed to load or parse (invalid JSON). If failed, prints error.
	/// </summary>
	protected static T _Load<T>(string file) where T : JSettings => (T)_Load(file, typeof(T));

	static JSettings _Load(string file, Type type)
	{
		JSettings R = null;
		if(AFile.ExistsAsAny(file)) {
			try {
				var b = AFile.LoadBytes(file);
				var opt = new JsonSerializerOptions { IgnoreNullValues = true, AllowTrailingCommas = true };
				R = JsonSerializer.Deserialize(b, type, opt) as JSettings;
			}
			catch(Exception ex) {
				//ADialog.ShowWarning("Failed to load settings", $"Will backup '{file}' and use default settings.", expandedText: ex.ToStringWithoutStack());
				Print($"Failed to load settings from '{file}'. Will use default settings. {ex.ToStringWithoutStack()}");
				try { AFile.Rename(file, file + ".backup", IfExists.Delete); } catch { }
			}
		}
		R ??= Activator.CreateInstance(type) as JSettings;
		R._file = file;
		R._loaded = true;
		Program.Timer1s += R._onTimer = R._SaveIfNeed;
		AProcess.Exit += R._onExit = (unu, sed) => R._SaveIfNeed(); //info: Core does not call finalizers when process exits
		return R;
	}

	/// <summary>
	/// Call this when finished using the settings. Saves now if need, and will not save later.
	/// Don't need to call if the settings are used until process exit.
	/// </summary>
	public void Dispose()
	{
		Program.Timer1s -= _onTimer;
		AProcess.Exit -= _onExit;
		_SaveIfNeed();
	}

	void _SaveIfNeed()
	{
		if(_save) {
			try {
				var opt = new JsonSerializerOptions { IgnoreNullValues = true, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
				var b = JsonSerializer.SerializeToUtf8Bytes(this, GetType(), opt);
				AFile.SaveBytes(_file, b);
				_save = false;
				//AOutput.QM2.Write(GetType().Name + " saved");
			}
			catch(Exception ex) {
				Print($"Failed to save settings to '{_file}'. {ex.ToStringWithoutStack()}");
			}
		}
	}

	/// <summary>
	/// Call this when changed an array element etc directly without assigning the array etc to a property. Else changes may be not saved.
	/// </summary>
	public void SaveLater() => _save = true;

	/// <summary>
	/// Sets a property value and will save later if need.
	/// If !_loaded, sets prop = value. Else if !value.Equals(prop), sets prop = value and _save = true.
	/// Use for simple IEquatable types, eg string, int, bool.
	/// </summary>
	/// <param name="prop">Property.</param>
	/// <param name="value">New value.</param>
	protected void _Set<T>(ref T prop, T value) where T : IEquatable<T>
	{
		if(!_loaded) {
			prop = value;
		} else if(!value.Equals(prop)) {
			prop = value;
			_save = true;
		}
	}

	/// <summary>
	/// Sets a property value and will save later if need.
	/// Unlike <see cref="_Set"/>, always sets _save = true if _loaded.
	/// Use for non-IEquatable types, eg arrays.
	/// </summary>
	/// <param name="prop">Property.</param>
	/// <param name="value">New value.</param>
	protected void _SetNoCmp<T>(ref T prop, T value)
	{
		prop = value;
		_save = _loaded;
	}

	//string _Get(ref string prop, Func<string> defValue) { if(prop == null) { prop = defValue(); _save = true; } return prop; }
}

/// <summary>
/// Program settings.
/// AFolders.ThisAppDocuments + @"!Settings\Settings.json"
/// </summary>
class ProgramSettings : JSettings
{
	public static ProgramSettings Load() => _Load<ProgramSettings>(AFolders.ThisAppDocuments + @"!Settings\Settings.json");

	public string user { get => _user; set => _Set(ref _user, value); }
	string _user;

	public string workspace { get => _workspace; set => _Set(ref _workspace, value); }
	string _workspace;

	public string[] recentWS { get => _recent; set => _SetNoCmp(ref _recent, value); }
	string[] _recent;

	public bool runHidden { get => _runHidden; set => _Set(ref _runHidden, value); }
	bool _runHidden;

	public string wndPos { get => _wndpos; set => _Set(ref _wndpos, value); }
	string _wndpos;

	public string tools_AWnd_wndPos { get => _tools_AWnd_wndPos; set => _Set(ref _tools_AWnd_wndPos, value); }
	string _tools_AWnd_wndPos;

	public string tools_AAcc_wndPos { get => _tools_AAcc_wndPos; set => _Set(ref _tools_AAcc_wndPos, value); }
	string _tools_AAcc_wndPos;

	public string tools_AWinImage_wndPos { get => _tools_AWinImage_wndPos; set => _Set(ref _tools_AWinImage_wndPos, value); }
	string _tools_AWinImage_wndPos;

	public PanelFind.RecentItem[] find_recent { get => _find_recent; set => _SetNoCmp(ref _find_recent, value); }
	PanelFind.RecentItem[] _find_recent;

	public PanelFind.RecentItem[] find_recentReplace { get => _find_recentReplace; set => _SetNoCmp(ref _find_recentReplace, value); }
	PanelFind.RecentItem[] _find_recentReplace;

	public string find_skip { get => _find_skip; set => _Set(ref _find_skip, value); }
	string _find_skip;

	public int find_searchIn { get => _find_searchIn; set => _Set(ref _find_searchIn, value); }
	int _find_searchIn;

	public bool ci_complGroup { get => _ci_complGroup; set => _Set(ref _ci_complGroup, value); }
	bool _ci_complGroup = true;

	public bool edit_wrap { get => _edit_wrap; set => _Set(ref _edit_wrap, value); }
	bool _edit_wrap;

	public bool edit_noImages { get => _edit_noImages; set => _Set(ref _edit_noImages, value); }
	bool _edit_noImages;

	public bool output_wrap { get => _output_wrap; set => _Set(ref _output_wrap, value); }
	bool _output_wrap;

	public bool output_white { get => _output_white; set => _Set(ref _output_white, value); }
	bool _output_white;

	public bool output_topmost { get => _output_topmost; set => _Set(ref _output_topmost, value); }
	bool _output_topmost;

}

/// <summary>
/// Workspace settings.
/// WorkspaceDirectory + @"\settings.json"
/// </summary>
class WorkspaceSettings : JSettings
{
	public static WorkspaceSettings Load(string jsonFile) => _Load<WorkspaceSettings>(jsonFile);

	public FilesModel.UserData[] users { get => _users; set => _SetNoCmp(ref _users, value); }
	FilesModel.UserData[] _users;

}
