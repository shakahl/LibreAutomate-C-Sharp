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
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

/// <summary>
/// Manages program settings in XML format.
/// Must be single static variable. Now it is Program.Settings.
/// </summary>
class ProgramSettings
{
	XElement _x;
	string _settFile;
	bool _isDirty;

	/// <summary>
	/// Loads settings from AFolders.ThisAppDocuments + @"!Settings\Settings.xml".
	/// </summary>
	public ProgramSettings()
	{
		_settFile = AFolders.ThisAppDocuments + @"!Settings\Settings.xml";
		try {
			_x = AExtXml.LoadElem(_settFile);
		}
		catch(Exception ex1) {
			try {
				if(AFile.ExistsAsAny(_settFile))
					ADialog.ShowWarning("Failed to load settings", $"Will backup '{_settFile}' and use default settings.", expandedText: ex1.Message);
				AFile.Copy(AFolders.ThisAppBS + @"Default\Settings.xml", _settFile, IfExists.RenameExisting);
				_x = AExtXml.LoadElem(_settFile);
			}
			catch(Exception ex2) {
				ADialog.ShowError("Failed to load settings", "Try again or reinstall the application.", expandedText: ex2.Message);
				Environment.Exit(1);
			}
		}

		Program.Timer1s += _Program_Timer1s;
		_x.Changed += _x_Changed; //use event because our XML can be modified externally. It is public through Xml and XmlOf.
	}

	~ProgramSettings()
	{
		_Program_Timer1s();
	}

	private void _Program_Timer1s()
	{
		if(_isDirty) {
			lock(this) {
				_x.SaveElem(_settFile);
			}
			_isDirty = false;
			//ADebug.Print("settings saved");
		}
	}

	private void _x_Changed(object sender, XObjectChangeEventArgs e)
	{
		//ADebug.PrintFunc(); //note: SetElementValue sends 2 events, because internally it removes/adds node's content. Setting the Value property is the same.
		Debug.Assert(Monitor.IsEntered(this));
		_isDirty = true;
	}

	bool _Get(string name, out string value)
	{
		lock(this) {
			var x = _x.Element(name);
			if(x == null) { value = null; return false; }
			value = x.Value;
			return true;
		}
	}

	/// <summary>
	/// Gets a setting of type string.
	/// If exists, returns true. Else sets value=defaultValue and returns false.
	/// </summary>
	public bool GetString(string name, out string value, string defaultValue = null)
	{
		if(_Get(name, out value)) return true;
		value = defaultValue; return false;
	}

	/// <summary>
	/// Gets a setting of type string.
	/// If does not exist, returns defaultValue.
	/// </summary>
	public string GetString(string name, string defaultValue = null)
	{
		GetString(name, out string value, defaultValue);
		return value;
	}

	/// <summary>
	/// Gets a setting of type int.
	/// If exists, returns true. Else sets value=defaultValue and returns false.
	/// </summary>
	public bool GetInt(string name, out int value, int defaultValue = 0)
	{
		if(_Get(name, out var s)) { value = s.ToInt(); return true; }
		value = defaultValue; return false;
	}

	/// <summary>
	/// Gets a setting of type int.
	/// If does not exist, returns defaultValue.
	/// </summary>
	public int GetInt(string name, int defaultValue = 0)
	{
		GetInt(name, out int value, defaultValue);
		return value;
	}

	/// <summary>
	/// Gets a setting of type bool.
	/// If exists, returns true. Else sets value=defaultValue and returns false.
	/// </summary>
	public bool GetBool(string name, out bool value, bool defaultValue = false)
	{
		if(_Get(name, out var s)) { value = s == "true" ? true : false; return true; }
		value = defaultValue; return false;
	}

	/// <summary>
	/// Gets a setting of type bool.
	/// If does not exist, returns defaultValue.
	/// </summary>
	public bool GetBool(string name, bool defaultValue = false)
	{
		GetBool(name, out bool value, defaultValue);
		return value;
	}

	/// <summary>
	/// Sets a setting of type string, or deletes a setting of any type.
	/// If the setting already has this value, does nothing and returns false. Else returns true.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value">If null, deletes the setting.</param>
	/// <remarks>
	/// The settings XML file is saved asynchronously, in 1 s timer and in finalizer.
	/// </remarks>
	public bool Set(string name, string value)
	{
		lock(this) {
			if(value == (string)_x.Element(name)) return false;
			_x.SetElementValue(name, value);
			return true;
		}
	}

	/// <summary>
	/// Sets a setting of type int.
	/// Does nothing if the setting is the same.
	/// </summary>
	public bool Set(string name, int value)
	{
		return Set(name, value.ToString());
	}

	/// <summary>
	/// Sets a setting of type bool.
	/// Does nothing if the setting is the same.
	/// </summary>
	public bool Set(string name, bool value)
	{
		return Set(name, value ? "true" : "false");
	}

	/// <summary>
	/// Gets the root XML element.
	/// You then can manipulate settings directly. The changes will be saved automatically.
	/// Need to lock(thisProgramSettingsObject). Asserts if not.
	/// </summary>
	public XElement Xml
	{
		get
		{
			Debug.Assert(Monitor.IsEntered(this));
			return _x;
		}
	}

	/// <summary>
	/// Gets XML element of a setting. Can add if does not exist.
	/// You then can manipulate the setting directly, eg add child elements. The changes will be saved automatically.
	/// Need to lock(thisProgramSettingsObject). Asserts if not.
	/// </summary>
	public XElement XmlOf(string name, bool addIfDoesNotExist = false)
	{
		Debug.Assert(Monitor.IsEntered(this));
		var r = _x.Element(name);
		if(r == null && addIfDoesNotExist) _x.Add(r = new XElement(name));
		return r;
	}
}
