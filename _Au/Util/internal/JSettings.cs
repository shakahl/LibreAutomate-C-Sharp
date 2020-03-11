//#define TRACE_JS

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
//using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

using Au.Types;

//FUTURE: public

namespace Au.Util
{
	/// <summary>
	/// Base of classes that load/save various settings from/to a JSON file.
	/// </summary>
	/// <remarks>
	/// Derived classes must provide public get/set properties.
	/// - 'get' usually just return a field.
	/// - 'set' must call a <b>SetX</b> method provided by this base class. Then changes will be automatically lazily saved.
	/// 
	/// All functions are thread-safe, except when setting/getting non-atomic struct types (containing multiple fields, decimal, also long in 32-bit process).
	/// </remarks>
	class JSettings
	{
		string _file;
		bool _loaded;
		int _save;

		static readonly List<JSettings> s_list = new List<JSettings>();
		static int s_loadedOnce;

		/// <summary>
		/// Loads JSON file and deserializes to new object of type T.
		/// Sets to save automatically whenever a property changed.
		/// Returns new empty object if file does not exist or failed to load or parse (invalid JSON) or <i>useDefault</i> true. If failed, writes error info to the output.
		/// </summary>
		/// <param name="file">Full path of .json file.</param>
		/// <param name="useDefault">Use default settings, don't load from file. Delete file if exists.</param>
		protected static T _Load<T>(string file, bool useDefault = false) where T : JSettings => (T)_Load(file, typeof(T), useDefault);

		static JSettings _Load(string file, Type type, bool useDefault)
		{
			JSettings R = null;
			if(AFile.ExistsAsAny(file)) {
				try {
					if(useDefault) {
						AFile.Delete(file);
					} else {
						var b = AFile.LoadBytes(file);
						var opt = new JsonSerializerOptions { IgnoreNullValues = true, AllowTrailingCommas = true };
						R = JsonSerializer.Deserialize(b, type, opt) as JSettings;
					}
				}
				catch(Exception ex) {
					string es = ex.ToStringWithoutStack();
					if(useDefault) {
						AOutput.Write($"Failed to delete settings file '{file}'. {es}");
					} else {
						//ADialog.ShowWarning("Failed to load settings", $"Will backup '{file}' and use default settings.", expandedText: ex.ToStringWithoutStack());
						AOutput.Write($"Failed to load settings from '{file}'. Will use default settings. {es}");
						try { AFile.Rename(file, file + ".backup", IfExists.Delete); } catch { }
					}
				}
			}
			R ??= Activator.CreateInstance(type) as JSettings;
			R._file = file;
			R._loaded = true;

			//autosave
			if(Interlocked.Exchange(ref s_loadedOnce, 1) == 0) {
				AThread.Start(() => {
					for(; ; ) {
						Thread.Sleep(2000);
						_SaveAllIfNeed();
					}
				}, sta: false);

				AProcess.Exit += (unu, sed) => _SaveAllIfNeed(); //info: Core does not call finalizers when process exits
			}
			lock(s_list) s_list.Add(R);

			return R;
		}

		static void _SaveAllIfNeed()
		{
			lock(s_list) foreach(var v in s_list) v.SaveIfNeed();
		}

		/// <summary>
		/// Call this when finished using the settings. Saves now if need, and stops autosaving.
		/// Don't need to call if the settings are used until process exit.
		/// </summary>
		public void Dispose()
		{
			lock(s_list) s_list.Remove(this);
			SaveIfNeed();
		}

		/// <summary>
		/// Saves now if need.
		/// Don't need to call explicitly. Autosaving is every 2 s, also on process exit and <b>Dispose</b>.
		/// </summary>
		public void SaveIfNeed()
		{
			//AOutput.QM2.Write(_save);
			if(Interlocked.Exchange(ref _save, 0) != 0) {
				try {
					var opt = new JsonSerializerOptions { IgnoreNullValues = true, IgnoreReadOnlyProperties = true, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
					var b = JsonSerializer.SerializeToUtf8Bytes(this, GetType(), opt);
					AFile.SaveBytes(_file, b);
					//AOutput.QM2.Write(GetType().Name + " saved");
				}
				catch(Exception ex) {
					SaveLater();
					AOutput.Write($"Failed to save settings to '{_file}'. {ex.ToStringWithoutStack()}");
				}
			}
		}

		/// <summary>
		/// Call this when changed an array element etc directly without assigning the array etc to a property. Else changes may be not saved.
		/// </summary>
		public void SaveLater()
		{
#if TRACE_JS
			//if(_save == 0)
				AWarning.Write("JSettings.SaveLater", 1, "<>Trace: ");
#endif
			Interlocked.Exchange(ref _save, 1);
		}

		/// <summary>
		/// <c>_save || AFile.ExistsAsAny(_file)</c>
		/// </summary>
		[JsonIgnore]
		public bool Modified => _save != 0 || AFile.ExistsAsAny(_file);

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
				SaveLater();
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
				SaveLater();
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
				SaveLater();
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
			if(_loaded) SaveLater();
		}
	}
}
