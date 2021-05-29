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
//using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

using Au.Types;

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
	public class ASettings
	{
		string _file;
		bool _loaded;
		bool _loadedFile;
		int _save;

		static readonly List<ASettings> s_list = new List<ASettings>();
		static int s_loadedOnce;

		/// <summary>
		/// Loads JSON file and deserializes to new object of type T.
		/// Sets to save automatically whenever a property changed.
		/// Returns new empty object if file does not exist or failed to load or parse (invalid JSON) or <i>useDefault</i> true. If failed, writes error info to the output.
		/// </summary>
		/// <param name="file">Full path of .json file. If null, does not load and will not save.</param>
		/// <param name="useDefault">Use default settings, don't load from file. Delete file if exists.</param>
		protected static T Load<T>(string file, bool useDefault = false) where T : ASettings
			=> (T)_Load(file, typeof(T), useDefault);

		static ASettings _Load(string file, Type type, bool useDefault) {
			ASettings R = null;
			if (file != null) {
				if (AFile.Exists(file)) {
					try {
						if (useDefault) {
							AFile.Delete(file);
						} else {
							var b = AFile.LoadBytes(file);
							var opt = new JsonSerializerOptions { IgnoreNullValues = true, AllowTrailingCommas = true };
							R = JsonSerializer.Deserialize(b, type, opt) as ASettings;
						}
					}
					catch (Exception ex) {
						string es = ex.ToStringWithoutStack();
						if (useDefault) {
							AOutput.Write($"Failed to delete settings file '{file}'. {es}");
						} else {
							string backup = file + ".backup";
							try { AFile.Move(file, backup, FIfExists.Delete); } catch { backup = "failed"; }
							AOutput.Write(
	$@"Failed to load settings from {file}. Will use default settings.
	{es}
	Backup: {backup}");
						}
					}
				}
			}

			if (R == null) R = Activator.CreateInstance(type) as ASettings; else R._loadedFile = true;
			R._loaded = true;

			if (file != null) {
				R._file = file;

				//autosave
				if (Interlocked.Exchange(ref s_loadedOnce, 1) == 0) {
					AThread.Start(() => {
						for (; ; ) {
							Thread.Sleep(2000);
							_SaveAllIfNeed();
						}
					}, sta: false);

					AThisProcess.Exit += _ => _SaveAllIfNeed(); //info: now .NET does not call finalizers when process exits
				}
				lock (s_list) s_list.Add(R);
			}

			return R;
		}

		static void _SaveAllIfNeed() {
			lock (s_list) foreach (var v in s_list) v.SaveIfNeed();
		}

		/// <summary>
		/// Call this when finished using the settings. Saves now if need, and stops autosaving.
		/// Don't need to call if the settings are used until process exit.
		/// </summary>
		public void Dispose() {
			lock (s_list) s_list.Remove(this);
			SaveIfNeed();
		}

		/// <summary>
		/// Saves now if need.
		/// Don't need to call explicitly. Autosaving is every 2 s, also on process exit and <b>Dispose</b>.
		/// </summary>
		public void SaveIfNeed() {
			if (_file == null) return;
			//AOutput.QM2.Write(_save);
			if (Interlocked.Exchange(ref _save, 0) != 0) {
				try {
					var opt = new JsonSerializerOptions { IgnoreNullValues = true, IgnoreReadOnlyProperties = true, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
					var b = JsonSerializer.SerializeToUtf8Bytes(this, GetType(), opt);
					AFile.SaveBytes(_file, b);
					//AOutput.QM2.Write(GetType().Name + " saved");
				}
				catch (Exception ex) {
					SaveLater();
					AOutput.Write($"Failed to save settings to '{_file}'. {ex.ToStringWithoutStack()}");
				}
			}
		}

		/// <summary>
		/// Call this when changed an array element etc directly without assigning the array etc to a property. Else changes may be not saved.
		/// </summary>
		public void SaveLater() {
#if TRACE_JS
			//if(_save == 0)
				AOutput.Warning("ASettings.SaveLater", 1, "<>Trace: ");
#endif
			Interlocked.Exchange(ref _save, 1);
		}

		/// <summary>
		/// true if settings were loaded from file.
		/// </summary>
		/// <remarks>
		/// Returns false if <b>Load</b> did not find the file (the settings were not saved) or failed to load/parse or parameter <i>useDefault</i> = true or parameter <i>file</i> = null.
		/// </remarks>
		[JsonIgnore]
		public bool LoadedFile => _loadedFile;

		/// <summary>
		/// true if settings were loaded from file (see <see cref="LoadedFile"/>) or modified later.
		/// </summary>
		[JsonIgnore]
		public bool Modified => _loadedFile || _save != 0;

		/// <summary>
		/// Sets a property value and will save later if need.
		/// If !_loaded, sets prop = value. Else if value != prop, sets prop = value and _save = true.
		/// To compare, uses <see cref="EqualityComparer{T}"/>; it compares value types and string by value, but most other reference types (eg arrays) by reference.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected void Set<T>(ref T prop, T value) {
			if (!_loaded) {
				prop = value;
			} else if (!EqualityComparer<T>.Default.Equals(prop, value)) {
				prop = value;
				SaveLater();
			}
			//faster than with IEquatable<T>. Tested with int, bool, enum, string, null string. Tested with string[], compares pointer, not elements.
		}

		/// <summary>
		/// Sets a property value and will save later if need.
		/// If !_loaded, sets prop = value. Else if value != prop (<i>comparer</i> returns false), sets prop = value and _save = true.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		/// <param name="comparer">Called by this method to compare prop and value. Return true if equal.</param>
		protected void Set<T>(ref T prop, T value, Func<T, T, bool> comparer) {
			if (!_loaded) {
				prop = value;
			} else if (!comparer(prop, value)) {
				prop = value;
				SaveLater();
			}
			//slower than with EqualityComparer<T> and slightly slower than with IEquatable<T>
		}

		/// <summary>
		/// Sets a property value and will save later if need.
		/// Unlike <b>Set</b>, always sets _save = true if _loaded.
		/// </summary>
		/// <param name="prop">Property.</param>
		/// <param name="value">New value.</param>
		protected void SetNoCmp<T>(ref T prop, T value) {
			prop = value;
			if (_loaded) SaveLater();
		}
	}
}
