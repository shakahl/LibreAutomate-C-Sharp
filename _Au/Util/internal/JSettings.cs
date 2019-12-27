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

//FUTURE: public

namespace Au.Util
{
	/// <summary>
	/// Base of classes that load/save various settings from/to a JSON file.
	/// </summary>
	/// <remarks>
	/// Derived classes must provide public get/set properties.
	/// Getters usually just return a field.
	/// Setters must call a <b>SetX</b> method. Then changes will be automatically lazily saved.
	/// Most functions should be called from the same UI thread. The 'get' properties for string and atomic types (int/bool/etc) can be called from any thread.
	/// </remarks>
	class JSettings
	{
		protected string _file;
		protected bool _loaded;
		protected bool _save;
		EventHandler _onExit;
		[ThreadStatic] static List<JSettings> t_list;

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

			//autosave
			if(t_list == null) {
				t_list = new List<JSettings>();
				ATimer.Every(5000, _ => {
					foreach(var v in t_list) v.SaveIfNeed();
				});
			}
			t_list.Add(R);
			AProcess.Exit += R._onExit = (unu, sed) => R.SaveIfNeed(); //info: Core does not call finalizers when process exits

			return R;
		}

		/// <summary>
		/// Call this when finished using the settings. Saves now if need, and stops autosaving.
		/// Don't need to call if the settings are used until process exit.
		/// </summary>
		public void Dispose()
		{
			AProcess.Exit -= _onExit;
			t_list.Remove(this);
			SaveIfNeed();
		}

		/// <summary>
		/// Saves now if need.
		/// Don't need to call explicitly. In UI thread called automatically every 5 s. Also automatically called on process exit and by <b>Dispose</b>.
		/// </summary>
		public void SaveIfNeed()
		{
			//AOutput.QM2.Write(_save);
			if(_save) {
				try {
					var opt = new JsonSerializerOptions { IgnoreNullValues = true, IgnoreReadOnlyProperties = true, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
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
		/// Sets a string property value and will save later if need.
		/// If !_loaded, sets prop = value. Else if value != prop, sets prop = value and _save = true.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected void Set(ref string prop, string value)
		{
			if(!_loaded) {
				prop = value;
			} else if(value != prop) {
				prop = value;
				_save = true;
			}
		}

		/// <summary>
		/// Sets an int, bool or other IEquatable type property value and will save later if need.
		/// If !_loaded, sets prop = value. Else if value != prop, sets prop = value and _save = true.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected void Set<T>(ref T prop, T value) where T : IEquatable<T>
		{
			if(!_loaded) {
				prop = value;
			} else if(!value.Equals(prop)) { //speed: usually same or faster than Set2
				prop = value;
				_save = true;
			}
		}

		/// <summary>
		/// Sets an enum or other unmanaged type property value and will save later if need.
		/// If !_loaded, sets prop = value. Else if value != prop, sets prop = value and _save = true.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected unsafe void Set2<T>(ref T prop, T value) where T : unmanaged
		{
			if(!_loaded) {
				prop = value;
			} else {
				if(0 == (sizeof(T) & 3)) {
					var p1 = (int*)Unsafe.AsPointer(ref prop);
					var p2 = (int*)Unsafe.AsPointer(ref value);
					for(int i = 0; i < sizeof(T) / 4; i++) if(p1[i] != p2[i]) goto g1;
				} else {
					var p1 = (byte*)Unsafe.AsPointer(ref prop);
					var p2 = (byte*)Unsafe.AsPointer(ref value);
					for(int i = 0; i < sizeof(T); i++) if(p1[i] != p2[i]) goto g1;
				}
				return;
				g1:
				prop = value;
				_save = true;
			}
		}

		/// <summary>
		/// Sets a property value and will save later if need.
		/// Unlike <b>Set</b>, always sets _save = true if _loaded.
		/// Use for non-umanaged non-IEquatable types, eg arrays.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected void SetNoCmp<T>(ref T prop, T value)
		{
			prop = value;
			_save = _loaded;
		}

		//string _Get(ref string prop, Func<string> defValue) { if(prop == null) { prop = defValue(); _save = true; } return prop; }
	}
}
