using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Registry functions.
	/// Unlike Microsoft.Win32.Registry, does not throw exception when fails. Instead uses ThreadError.
	/// Also has methods not supported by Microsoft.Win32.Registry, for example set/get struct variables easily.
	/// </summary>
	public static class Registry_
	{
		/// <summary>
		/// Default registry key, used when the key argument is null or @"\" or starts with @"\".
		/// </summary>
		public const string CatkeysKey = @"Software\Catkeys\User";

		/// <summary>
		/// Parses registry key string and returns hive as RegistryKey.
		/// If key starts with "HKEY_", removes hive name from it and returns that hive. For example, if key is @"HKEY_LOCAL_MACHINE\Software\Test", sets key=@"Software\Test" and returns Registry.LocalMachine.
		/// Else if key is null or @"\", sets key=Registry_.CatkeysKey (@"Software\Catkeys\User") and returns Registry.CurrentUser.
		/// Else if key starts with @"\", prepends Registry_.CatkeysKey (@"Software\Catkeys\User") and returns Registry.CurrentUser.
		/// Else just returns Registry.CurrentUser.
		/// Valid hive names: "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_CLASSES_ROOT", "HKEY_USERS", "HKEY_PERFORMANCE_DATA" or "HKEY_CURRENT_CONFIG".
		/// </summary>
		/// <param name="key">Registry key. Can start with a hive name.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static RegistryKey ParseKeyString(ref string key)
		{
			if(key == null) key = @"\";
			if(key.StartsWith_(@"\")) {
				key = (key.Length == 1) ? CatkeysKey : CatkeysKey + key;
				return Registry.CurrentUser;
			}
			if(!key.StartsWith_("HKEY_")) return Registry.CurrentUser;

			RegistryKey R = null;
			int i = key.IndexOf('\\');
			string s = i < 0 ? key : key.Remove(i);
			switch(s) {
			case "HKEY_CURRENT_USER": R = Registry.CurrentUser; break;
			case "HKEY_LOCAL_MACHINE": R = Registry.LocalMachine; break;
			case "HKEY_CLASSES_ROOT": R = Registry.ClassesRoot; break;
			case "HKEY_USERS": R = Registry.Users; break;
			case "HKEY_CURRENT_CONFIG": R = Registry.CurrentConfig; break;
			case "HKEY_PERFORMANCE_DATA": R = Registry.PerformanceData; break;
			//case "HKEY_DYN_DATA": R = Registry.DynData; break; //9x
			default: throw new ArgumentException("Invalid \"HKEY_x\".");
			}
			key = i < 0 ? "" : key.Substring(i + 1);
			return R;
		}

		static RegistryKey _Open(bool create, string key, RegistryKey parentKeyOrHive)
		{
			if(parentKeyOrHive == null) parentKeyOrHive = ParseKeyString(ref key);
			if(Empty(key)) return parentKeyOrHive;
			RegistryKey k = null;
			try {
				k = create ? parentKeyOrHive.CreateSubKey(key) : parentKeyOrHive.OpenSubKey(key);
				if(k == null) ThreadError.Set("Failed");
			}
			catch(Exception e) { ThreadError.SetException(e); }
			return k;
		}

		/// <summary>
		/// Retrieves a key as read-only.
		/// Uses <see cref="RegistryKey.OpenSubKey(string)"/>.
		/// Supports ThreadError. If fails, sets thread error and returns null.
		/// </summary>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", the function just returns parentKeyOrHive.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static RegistryKey Open(string key, RegistryKey parentKeyOrHive = null)
		{
			return _Open(false, key, parentKeyOrHive);
		}

		/// <summary>
		/// Creates a new key or opens an existing key for write access.
		/// Uses <see cref="RegistryKey.CreateSubKey(string)"/>.
		/// Supports ThreadError. If fails, sets thread error and returns null.
		/// </summary>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", the function just returns parentKeyOrHive.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static RegistryKey CreateOrOpen(string key, RegistryKey parentKeyOrHive = null)
		{
			return _Open(true, key, parentKeyOrHive);
		}

		/// <summary>
		/// Returns true if key exists and this process can open it to read.
		/// Uses <see cref="RegistryKey.OpenSubKey(string)"/>.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/>.</param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool KeyExists(string key, RegistryKey parentKeyOrHive = null)
		{
			var k = Open(key, parentKeyOrHive);
			if(k == null) return false;
			if(k != parentKeyOrHive) k.Dispose();
			return true;
		}

		enum _ResultType { Int, Long, String, StringArray }

		struct _Result
		{
			internal int rInt;
			internal long rLong;
			internal string rString;
			internal string[] rStringArray;
		}

		static bool _Get(_ResultType type, out _Result r, string valueName, string key, RegistryKey parentKeyOrHive)
		{
			r = new _Result();
			var k = Open(key, parentKeyOrHive);
			if(k == null) return false;
			try {
				object t = k.GetValue(valueName);
				if(t != null) {
					switch(type) {
					case _ResultType.Int: r.rInt = (int)t; break;
					case _ResultType.Long: r.rLong = (long)t; break;
					case _ResultType.String: r.rString = (string)t; break;
					case _ResultType.StringArray: r.rStringArray = (string[])t; break;
					}
					return true;
				}
				ThreadError.Clear();
			}
			catch(Exception e) { ThreadError.SetException(e); }
			finally { if(k != parentKeyOrHive) k.Dispose(); }
			return false;
		}

		/// <summary>
		/// Gets value of REG_DWORD type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool GetInt(out int data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = 0;
			_Result r;
			if(!_Get(_ResultType.Int, out r, valueName, key, parentKeyOrHive)) return false;
			data = r.rInt;
			return true;
		}

		/// <summary>
		/// Gets value of REG_QWORD type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool GetLong(out long data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = 0;
			_Result r;
			if(!_Get(_ResultType.Long, out r, valueName, key, parentKeyOrHive)) return false;
			data = r.rLong;
			return true;
		}

		/// <summary>
		/// Gets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool GetString(out string data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = null;
			_Result r;
			if(!_Get(_ResultType.String, out r, valueName, key, parentKeyOrHive)) return false;
			data = r.rString;
			return true;
		}

		/// <summary>
		/// Gets string value of REG_MULTI_SZ type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool GetStringArray(out string[] data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = null;
			_Result r;
			if(!_Get(_ResultType.StringArray, out r, valueName, key, parentKeyOrHive)) return false;
			data = r.rStringArray;
			return true;
		}

		static bool _Set(RegistryValueKind type, object data, string valueName, string key, RegistryKey parentKeyOrHive)
		{
			var k = CreateOrOpen(key, parentKeyOrHive);
			if(k == null) return false;
			try {
				k.SetValue(valueName, data, type);
				return true;
			}
			catch(Exception e) { ThreadError.SetException(e); return false; }
			finally { if(k != parentKeyOrHive) k.Dispose(); }
		}

		/// <summary>
		/// Sets value of REG_DWORD type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError. If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool SetInt(int data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			return _Set(RegistryValueKind.DWord, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets value of REG_QWORD type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError. If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static bool SetLong(long data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			return _Set(RegistryValueKind.QWord, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError. If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <param name="REG_EXPAND_SZ">Let the registry value type be REG_EXPAND_SZ.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static unsafe bool SetString(string data, string valueName, string key = null, RegistryKey parentKeyOrHive = null, bool REG_EXPAND_SZ = false)
		{
			return _Set(REG_EXPAND_SZ ? RegistryValueKind.ExpandString : RegistryValueKind.String, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets string value of REG_MULTI_SZ type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError. If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static unsafe bool SetStringArray(string[] data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			return _Set(RegistryValueKind.MultiString, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets binary value of REG_BINARY type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError. If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data. For example a struct variable (unsafe address).</param>
		/// <param name="size">Data size. For example, Marshal.SizeOf(variable) or Marshal.SizeOf(typeof(DATA)).</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static unsafe bool SetBinary(void* data, int size, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			var k = CreateOrOpen(key, parentKeyOrHive);
			if(k == null) return false;
			try {
				IntPtr h = k.Handle.DangerousGetHandle();
				int e = Api.RegSetValueEx(h, valueName, 0, RegistryValueKind.Binary, data, size);
				return e == 0 || ThreadError.Set(e);
			}
			catch(Exception e) { ThreadError.SetException(e); return false; }
			finally { if(k != parentKeyOrHive) k.Dispose(); }
		}

		/// <summary>
		/// Gets binary data.
		/// Returns registry data size that the function copied into the 'data' memory. It can be equal or less than the 'size' argument. If registry data is bigger than 'size', gets only 'size' part of it.
		/// If valueName does not exist, returns 0.
		/// If fails, returns -1.
		/// Supports ThreadError.
		/// Registry data can be of any type.
		/// </summary>
		/// <param name="data">Receives data. For example a struct variable (unsafe address).</param>
		/// <param name="size">Max data size to get. For example, Marshal.SizeOf(variable) or Marshal.SizeOf(typeof(DATA)).</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static unsafe int GetBinary(void* data, int size, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			var k = Open(key, parentKeyOrHive);
			if(k == null) return -1;
			try {
				IntPtr h = k.Handle.DangerousGetHandle();
				RegistryValueKind kind; int z = size;
				int e = Api.RegQueryValueEx(h, valueName, Zero, out kind, data, ref z);
				if(e == 0) { ThreadError.Clear(); return z; }
				ThreadError.Set(e);
				if(e == Api.ERROR_FILE_NOT_FOUND) return 0;
			}
			catch(Exception e) { ThreadError.SetException(e); }
			finally { if(k != parentKeyOrHive) k.Dispose(); }
			return -1;
		}
	}
}
