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
using System.Security;
using System.Runtime.ExceptionServices;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Registry functions. Extends <see cref="Registry"/>.
	/// </summary>
	public static class Registry_
	{
		/// <summary>
		/// Default registry key, used when the key argument is null or <c>@"\"</c> or starts with <c>@"\"</c>.
		/// </summary>
		public const string AuKey = @"Software\Au";

		///
		public static RegistryKey HKEY_CURRENT_USER => Registry.CurrentUser;

		///
		public static RegistryKey HKEY_CLASSES_ROOT => Registry.ClassesRoot;

		///
		public static RegistryKey HKEY_LOCAL_MACHINE => Registry.LocalMachine;

		/// <summary>
		/// Parses registry key string and returns hive as RegistryKey.
		/// If key starts with <c>"HKEY_"</c>, removes hive name from it and returns that hive. For example, if key is <c>@"HKEY_LOCAL_MACHINE\Software\Test"</c>, sets key=<c>@"Software\Test"</c> and returns HKEY_LOCAL_MACHINE.
		/// Else if key is null or <c>@"\"</c>, sets key=<c>Registry_.AuKey (@"Software\Au")</c> and returns HKEY_CURRENT_USER.
		/// Else if key starts with <c>@"\"</c>, prepends <c>Registry_.AuKey (@"Software\Au")</c> and returns HKEY_CURRENT_USER.
		/// Else just returns HKEY_CURRENT_USER.
		/// Valid hive names: <c>"HKEY_CURRENT_USER"</c>, <c>"HKEY_LOCAL_MACHINE"</c>, <c>"HKEY_CLASSES_ROOT"</c>, <c>"HKEY_USERS"</c>, <c>"HKEY_PERFORMANCE_DATA"</c> or <c>"HKEY_CURRENT_CONFIG"</c>.
		/// </summary>
		/// <param name="key">Registry key. Can start with a hive name.</param>
		/// <exception cref="ArgumentException"><i>key</i> starts with <c>"HKEY_"</c> but it is an invalid hive name.</exception>
		public static RegistryKey ParseKeyString(ref string key)
		{
			if(key == null) key = @"\";
			if(key.Starts('\\')) {
				key = (key.Length == 1) ? AuKey : AuKey + key;
				return Registry.CurrentUser;
			}
			if(!key.Starts("HKEY_")) return Registry.CurrentUser;

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
			RegistryKey k = create ? parentKeyOrHive.CreateSubKey(key) : parentKeyOrHive.OpenSubKey(key);
			Debug.Assert(!(create && k == null)); //CreateSubKey doc says it returns null when failed, but it is not true, I saw the code
			return k;
		}

		/// <summary>
		/// Opens a key for read access.
		/// Returns null if the key does not exist.
		/// Uses <see cref="RegistryKey.OpenSubKey(string)"/>.
		/// </summary>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", the function just returns parentKeyOrHive.</param>
		/// <exception cref="ArgumentException">'key' starts with "HKEY_" but it is an invalid hive name.</exception>
		/// <exception cref="ObjectDisposedException">'parentKeyOrHive' is a closed key handle.</exception>
		/// <exception cref="SecurityException">The user does not have the permissions required to read the registry key.</exception>
		public static RegistryKey Open(string key, RegistryKey parentKeyOrHive = null)
		{
			return _Open(false, key, parentKeyOrHive);
		}

		/// <summary>
		/// Creates a new key or opens an existing key for write access.
		/// Uses <see cref="RegistryKey.CreateSubKey(string)"/>.
		/// </summary>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", the function just returns parentKeyOrHive.</param>
		/// <exception cref="ArgumentException">'key' starts with "HKEY_" but it is an invalid hive name.</exception>
		/// <exception cref="ObjectDisposedException">'parentKeyOrHive' is a closed key handle.</exception>
		/// <exception cref="SecurityException">The user does not have the permissions required to create or open the registry key.</exception>
		/// <exception cref="UnauthorizedAccessException">parentKeyOrHive was not opened with write access.</exception>
		/// <exception cref="IOException">See <see cref="RegistryKey.CreateSubKey(string)"/>.</exception>
		public static RegistryKey CreateOrOpen(string key, RegistryKey parentKeyOrHive = null)
		{
			return _Open(true, key, parentKeyOrHive);
		}

		/// <summary>
		/// Returns true if key exists and you can open it to read.
		/// Uses <see cref="RegistryKey.OpenSubKey(string)"/>.
		/// </summary>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/>.</param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive.</param>
		/// <exception cref="ArgumentException">'key' starts with "HKEY_" but it is an invalid hive name.</exception>
		/// <exception cref="ObjectDisposedException">'parentKeyOrHive' is a closed key handle.</exception>
		public static bool CanOpen(string key, RegistryKey parentKeyOrHive = null)
		{
			try {
				var k = Open(key, parentKeyOrHive);
				if(k == null) return false;
				if(k != parentKeyOrHive) k.Dispose();
				return true;
			}
			catch(SecurityException) { return false; }
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
			if(k != null) {
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
				}
				finally { if(k != parentKeyOrHive) k.Dispose(); }
			}
			return false;
		}

		/// <summary>
		/// Gets value of REG_DWORD type.
		/// Returns false if the value or key does not exist.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">'key' starts with "HKEY_" but it is an invalid hive name.</exception>
		/// <exception cref="ObjectDisposedException">'parentKeyOrHive' is a closed key handle.</exception>
		/// <exception cref="SecurityException">The user does not have the permissions required to read the registry key.</exception>
		/// <exception cref="IOException">The key has been marked for deletion.</exception>
		/// <exception cref="InvalidCastException">Wrong value type.</exception>
		public static bool GetInt(out int data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = 0;
			if(!_Get(_ResultType.Int, out var r, valueName, key, parentKeyOrHive)) return false;
			data = r.rInt;
			return true;
		}

		/// <summary>
		/// Gets value of REG_QWORD type.
		/// Returns false if the value or key does not exist.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="GetInt"/> topic.</exception>
		public static bool GetLong(out long data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = 0;
			if(!_Get(_ResultType.Long, out var r, valueName, key, parentKeyOrHive)) return false;
			data = r.rLong;
			return true;
		}

		/// <summary>
		/// Gets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Returns false if the value or key does not exist.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="GetInt"/> topic.</exception>
		public static bool GetString(out string data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = null;
			if(!_Get(_ResultType.String, out var r, valueName, key, parentKeyOrHive)) return false;
			data = r.rString;
			return true;
		}

		/// <summary>
		/// Gets string value of REG_MULTI_SZ type.
		/// Returns false if the value or key does not exist.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="GetInt"/> topic.</exception>
		public static bool GetStringArray(out string[] data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			data = null;
			if(!_Get(_ResultType.StringArray, out var r, valueName, key, parentKeyOrHive)) return false;
			data = r.rStringArray;
			return true;
		}

		static void _Set(RegistryValueKind type, object data, string valueName, string key, RegistryKey parentKeyOrHive)
		{
			var k = CreateOrOpen(key, parentKeyOrHive);
			try {
				k.SetValue(valueName, data, type);
			}
			finally { if(k != parentKeyOrHive) k.Dispose(); }
		}

		/// <summary>
		/// Sets value of REG_DWORD type.
		/// Creates key and value if don't exist.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="ArgumentException">'key' starts with "HKEY_" but it is an invalid hive name.</exception>
		/// <exception cref="ObjectDisposedException">'parentKeyOrHive' is a closed key handle.</exception>
		/// <exception cref="SecurityException">The user does not have the permissions required to create, open or modify the registry key.</exception>
		/// <exception cref="UnauthorizedAccessException">parentKeyOrHive was not opened with write access.</exception>
		/// <exception cref="IOException">See <see cref="RegistryKey.CreateSubKey(string)"/>.</exception>
		public static void SetInt(int data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			_Set(RegistryValueKind.DWord, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets value of REG_QWORD type.
		/// Creates key and value if don't exist.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="SetInt"/> topic.</exception>
		public static void SetLong(long data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			_Set(RegistryValueKind.QWord, data, valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Creates key and value if don't exist.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <param name="REG_EXPAND_SZ">Let the registry value type be REG_EXPAND_SZ.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="SetInt"/> topic.</exception>
		public static unsafe void SetString(string data, string valueName, string key = null, RegistryKey parentKeyOrHive = null, bool REG_EXPAND_SZ = false)
		{
			_Set(REG_EXPAND_SZ ? RegistryValueKind.ExpandString : RegistryValueKind.String, data ?? "", valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets string value of REG_MULTI_SZ type.
		/// Creates key and value if don't exist.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions are listed in <see cref="SetInt"/> topic.</exception>
		public static unsafe void SetStringArray(string[] data, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			_Set(RegistryValueKind.MultiString, data ?? Array.Empty<string>(), valueName, key, parentKeyOrHive);
		}

		/// <summary>
		/// Sets value of REG_BINARY type.
		/// Creates key and value if don't exist.
		/// </summary>
		/// <param name="data">Data. For example a struct variable (unsafe address).</param>
		/// <param name="size">Data size. For example, Marshal.SizeOf(variable) or Marshal.SizeOf(typeof(DATA)).</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions thrown by <see cref="CreateOrOpen"/>.</exception>
		/// <exception cref="Win32Exception">Failed to write the value to the key.</exception>
		public static unsafe void SetBinary(void* data, int size, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			var k = CreateOrOpen(key, parentKeyOrHive);
			try {
				IntPtr h = k.Handle.DangerousGetHandle();
				int e = Api.RegSetValueEx(h, valueName, 0, RegistryValueKind.Binary, data, size);
				if(e != 0) throw new Win32Exception(e);
			}
			finally { if(k != parentKeyOrHive) k.Dispose(); }
		}

		/// <summary>
		/// Gets binary data. The registry value type can be REG_BINARY or any other.
		/// Returns registry data size that the function copied into the 'data' memory.
		/// Returns 0 if the key or value does not exist.
		/// </summary>
		/// <param name="data">Receives data. For example a struct variable (unsafe address).</param>
		/// <param name="size">data memory size. For example, Marshal.SizeOf(variable). Must be &gt;= registry data size.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. See <see cref="ParseKeyString"/></param>
		/// <param name="parentKeyOrHive">If not null, the 'key' argument is a subkey of this key or hive; if the 'key' argument is null or "", parentKeyOrHive is the direct parent key of the value.</param>
		/// <exception cref="Exception">Exceptions thrown by <see cref="Open"/>.</exception>
		/// <exception cref="Win32Exception">The value exists but failed to get it, for example the specified size is smaller than registry data size.</exception>
		public static unsafe int GetBinary(void* data, int size, string valueName, string key = null, RegistryKey parentKeyOrHive = null)
		{
			var k = Open(key, parentKeyOrHive);
			if(k != null) {
				try {
					IntPtr h = k.Handle.DangerousGetHandle();
					int e = Api.RegQueryValueEx(h, valueName, default, out var kind, data, ref size);
					if(e == 0) return size;
					if(e != Api.ERROR_FILE_NOT_FOUND) throw new Win32Exception(e);
				}
				finally { if(k != parentKeyOrHive) k.Dispose(); }
			}
			return 0;
		}
	}
}
