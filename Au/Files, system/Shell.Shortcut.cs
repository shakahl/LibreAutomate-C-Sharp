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
//using System.Xml.Linq;
//using System.Xml.XPath;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	partial class Shell
	{
		/// <summary>
		/// Creates shell shortcuts (.lnk files) and gets shortcut properties.
		/// </summary>
		public unsafe class Shortcut :IDisposable
		{
			//info: name Shortcut used in .NET.

			Api.IShellLink _isl;
			Api.IPersistFile _ipf;
			string _lnkPath;
			bool _isOpen;
			bool _changedHotkey;

			/// <summary>
			/// Releases internally used COM objects (IShellLink, IPersistFile).
			/// </summary>
			public void Dispose()
			{
				if(_isl != null) {
					Api.ReleaseComObject(_ipf); _ipf = null;
					Api.ReleaseComObject(_isl); _isl = null;
				}
			}
			//~Shortcut() { Dispose(); } //don't need, we have only COM objects, GC will release them anyway

			/// <summary>
			/// Returns the internally used IShellLink COM interface.
			/// </summary>
			internal Api.IShellLink IShellLink => _isl;
			//This could be public, but then need to make IShellLink public. It is defined in a non-standard way. Never mind, it is not important.

			Shortcut(string lnkPath, uint mode)
			{
				_isl = new Api.ShellLink() as Api.IShellLink;
				_ipf = _isl as Api.IPersistFile;
				_lnkPath = lnkPath;
				if(mode != Api.STGM_WRITE && (mode == Api.STGM_READ || Files.ExistsAsFile(_lnkPath))) {
					AuException.ThrowIfHresultNot0(_ipf.Load(_lnkPath, mode), "*open");
					_isOpen = true;
				}
			}

			/// <summary>
			/// Creates a new instance of the Shortcut class that can be used to get shortcut properties.
			/// Exception if shortcut file does not exist or cannot open it for read access.
			/// </summary>
			/// <param name="lnkPath">Shortcut file (.lnk) path.</param>
			/// <exception cref="AuException">Failed to open .lnk file.</exception>
			public static Shortcut Open(string lnkPath)
			{
				return new Shortcut(lnkPath, Api.STGM_READ);
			}

			/// <summary>
			/// Creates a new instance of the Shortcut class that can be used to create or replace a shortcut file.
			/// You can set properties and finally call <see cref="Save"/>.
			/// If the shortcut file already exists, Save replaces it.
			/// </summary>
			/// <param name="lnkPath">Shortcut file (.lnk) path.</param>
			public static Shortcut Create(string lnkPath)
			{
				return new Shortcut(lnkPath, Api.STGM_WRITE);
			}

			/// <summary>
			/// Creates a new instance of the Shortcut class that can be used to create or modify a shortcut file.
			/// Exception if file exists but cannot open it for read-write access.
			/// You can get and set properties and finally call <see cref="Save"/>.
			/// If the shortcut file already exists, Save updates it.
			/// </summary>
			/// <param name="lnkPath">Shortcut file (.lnk) path.</param>
			/// <exception cref="AuException">Failed to open existing .lnk file.</exception>
			public static Shortcut OpenOrCreate(string lnkPath)
			{
				return new Shortcut(lnkPath, Api.STGM_READWRITE);
			}

			/// <summary>
			/// Saves the Shortcut variable properties to the shortcut file.
			/// </summary>
			/// <exception cref="AuException">Failed to save .lnk file.</exception>
			/// <remarks>
			/// Creates parent folder if need.
			/// </remarks>
			public void Save()
			{
				if(_changedHotkey && !_isOpen && Files.ExistsAsFile(_lnkPath)) _UnregisterHotkey(_lnkPath);

				Files.CreateDirectoryFor(_lnkPath);
				AuException.ThrowIfHresultNot0(_ipf.Save(_lnkPath, true), "*save");
			}

			/// <summary>
			/// Gets or sets shortcut target path.
			/// This property is null if target isn't a file system object, eg Control Panel or URL.
			/// </summary>
			/// <remarks>The 'get' function gets path with expanded environment variables. If possible, it corrects the target of MSI shortcuts and 64-bit Program Files shortcuts where IShellLink.GetPath() lies.</remarks>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string TargetPath
			{
				get => _CorrectPath(_GetString(_WhatString.Path, 300), true);
				set { AuException.ThrowIfHresultNot0(_isl.SetPath(value)); }
			}

			/// <summary>
			/// Gets shortcut target path and does not correct wrong MSI shortcut target.
			/// </summary>
			public string TargetPathRawMSI
			{
				get => _CorrectPath(_GetString(_WhatString.Path, 300));
			}

			/// <summary>
			/// Gets or sets a non-file-system target (eg Control Panel) through its ITEMIDLIST.
			/// </summary>
			/// <remarks>
			/// Also can be used for any target type, but gets raw value, for example MSI shortcut target is incorrect.
			/// Most but not all shortcuts have this property; the 'get' function returns Zero if the shortcut does not have it.
			/// </remarks>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public Shell.Pidl TargetPidl
			{
				get => (0 == _isl.GetIDList(out var pidl)) ? new Shell.Pidl(pidl) : null;
				set { AuException.ThrowIfHresultNot0(_isl.SetIDList(value)); }
			}

			/// <summary>
			/// Gets or sets a URL target.
			/// Note: it is a .lnk shortcut, not a .url shortcut.
			/// The 'get' function returns string "file:///..." if target is a file.
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string TargetURL
			{
				get
				{
					if(0 != _isl.GetIDList(out var pidl)) return null;
					try { return Shell.Pidl.LibToShellString(pidl, Native.SIGDN.SIGDN_URL); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
				set
				{
					TargetAnyType = value;
				}
			}

			/// <summary>
			/// Gets or sets target of any type - file/folder, URL, virtual shell object (see <see cref="Shell.Pidl"/>).
			/// The string can be used with <see cref="Shell.Run"/>.
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string TargetAnyType
			{
				get
				{
					var R = TargetPath; if(R != null) return R; //support MSI etc
					if(0 != _isl.GetIDList(out var pidl)) return null;
					try { return Shell.Pidl.LibToString(pidl); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
				set
				{
					var pidl = Shell.Pidl.LibFromString(value, true);
					try { AuException.ThrowIfHresultNot0(_isl.SetIDList(pidl)); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
			}

			/// <summary>
			/// Gets custom icon file path and icon index.
			/// Returns null if the shortcut does not have a custom icon (then you see its target icon).
			/// </summary>
			/// <param name="iconIndex">Receives 0 or icon index or negative icon resource id.</param>
			public string GetIconLocation(out int iconIndex)
			{
				var b = Util.Buffers.LibChar(300);
				if(0 != _isl.GetIconLocation(b, 300, out iconIndex)) return null;
				return _CorrectPath(b.ToString());
			}

			/// <summary>
			/// Sets icon file path and icon index.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="iconIndex">0 or icon index or negative icon resource id.</param>
			/// <exception cref="AuException"/>
			public void SetIconLocation(string path, int iconIndex = 0)
			{
				AuException.ThrowIfHresultNot0(_isl.SetIconLocation(path, iconIndex));
			}

			/// <summary>
			/// Gets or sets the working directory path (Start in).
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string WorkingDirectory
			{
				get => _CorrectPath(_GetString(_WhatString.WorkingDirectory, 300));
				set { AuException.ThrowIfHresultNot0(_isl.SetWorkingDirectory(value)); }
			}

			/// <summary>
			/// Gets or sets the command-line arguments.
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string Arguments
			{
				get => _GetString(_WhatString.Arguments, 1024);
				set { AuException.ThrowIfHresultNot0(_isl.SetArguments(value)); }
			}

			/// <summary>
			/// Gets or sets the description text (Comment).
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public string Description
			{
				get => _GetString(_WhatString.Description, 1024);
				set { AuException.ThrowIfHresultNot0(_isl.SetDescription(value)); }
			}

			/// <summary>
			/// Gets or sets hotkey.
			/// Example: <c>x.Hotkey = Keys.Control | Keys.Alt | Keys.E;</c>
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public Keys Hotkey
			{
				get
				{
					if(0 != _isl.GetHotkey(out ushort k2)) return 0;
					uint k = k2;
					return (Keys)((k & 0xFF) | ((k & 0x700) << 8));
				}
				set
				{
					uint k = (uint)value;
					AuException.ThrowIfHresultNot0(_isl.SetHotkey((ushort)((k & 0xFF) | ((k & 0x70000) >> 8))));
					_changedHotkey = true;
				}
			}

			/// <summary>
			/// Gets or sets the window show state.
			/// The value can be 1 (normal, default), 2 (minimized) or 3 (maximized).
			/// Most programs ignore it.
			/// </summary>
			/// <exception cref="AuException">The 'set' function failed.</exception>
			public int ShowState
			{
				get => (0 == _isl.GetShowCmd(out var R)) ? R : Api.SW_SHOWNORMAL;
				set { AuException.ThrowIfHresultNot0(_isl.SetShowCmd(value)); }
			}

			//Not implemented wrappers for these IShellLink methods:
			//SetRelativePath, Resolve - not useful.
			//All are easy to call through the IShellLink property.

			#region public static

			/// <summary>
			/// Gets shortcut target path or URL or virtual shell object ITEMIDLIST.
			/// Uses <see cref="Open"/> and <see cref="TargetAnyType"/>.
			/// </summary>
			/// <param name="lnkPath">Shortcut file (.lnk) path.</param>
			/// <exception cref="AuException">Failed to open.</exception>
			public static string GetTarget(string lnkPath)
			{
				return Open(lnkPath).TargetAnyType;
			}

			/// <summary>
			/// If shortcut file exists, unregisters its hotkey and deletes it.
			/// </summary>
			/// <param name="lnkPath">.lnk file path.</param>
			/// <exception cref="AuException">Failed to unregister hotkey.</exception>
			/// <exception cref="Exception">Exceptions of <see cref="Files.Delete(string, bool)"/>.</exception>
			public static void Delete(string lnkPath)
			{
				if(!Files.ExistsAsFile(lnkPath)) return;
				_UnregisterHotkey(lnkPath);
				Files.Delete(lnkPath);
			}

			#endregion
			#region private

			/// <exception cref="AuException">Failed to open or save.</exception>
			static void _UnregisterHotkey(string lnkPath)
			{
				Debug.Assert(Files.ExistsAsFile(lnkPath));
				using(var x = OpenOrCreate(lnkPath)) {
					var k = x.Hotkey;
					if(k != 0) {
						x.Hotkey = 0;
						x.Save();
					}
				}
			}

			enum _WhatString { Path, Arguments, WorkingDirectory, Description }

			string _GetString(_WhatString what, int bufferSize)
			{
				int hr = 1;
				var b = Util.Buffers.LibChar(bufferSize);
				switch(what) {
				case _WhatString.Path: hr = _isl.GetPath(b, bufferSize); break;
				case _WhatString.Arguments: hr = _isl.GetArguments(b, bufferSize); break;
				case _WhatString.WorkingDirectory: hr = _isl.GetWorkingDirectory(b, bufferSize); break;
				case _WhatString.Description: hr = _isl.GetDescription(b, bufferSize); break;
				}
				if(hr != 0) return null;
				var R = b.ToString();
				return (R.Length > 0) ? R : null;
			}

			string _CorrectPath(string R, bool fixMSI = false)
			{
				if(Empty(R)) return null;

				if(!fixMSI) {
					R = Path_.ExpandEnvVar(R);
				} else if(R.IndexOf_(@"\Installer\{") > 0) {
					//For MSI shortcuts GetPath gets like "C:\WINDOWS\Installer\{90110409-6000-11D3-8CFE-0150048383C9}\accicons.exe".
					var product = stackalloc char[40];
					var component = stackalloc char[40];
					if(0 != Api.MsiGetShortcutTarget(_lnkPath, product, null, component)) return null;
					//note: for some shortcuts MsiGetShortcutTarget gets empty component. Then MsiGetComponentPath fails.
					//	On my PC was 1 such shortcut - Microsoft Office Excel Viewer.lnk in start menu.
					//	Could not find a workaround.

					var b = Util.Buffers.LibChar(300, out int na);
					int hr = Api.MsiGetComponentPath(product, component, b, ref na);
					if(hr < 0) return null; //eg not installed, just advertised
					R = b.ToString(na);
					if(R.Length == 0) return null;
					//note: can be a registry key instead of file path. No such shortcuts on my PC.
				}

				//GetPath problem: replaces "c:\program files" to "c:\program files (x86)".
				//These don't help: SLGP_RAWPATH, GetIDList, disabled redirection.
				//GetWorkingDirectory and GetIconLocation get raw path, and envronment variables such as %ProgramFiles% are expanded to (x86) in 32-bit process.
				if(Ver.Is32BitProcessOn64BitOS) {
					if(_pf == null) { string s = Folders.ProgramFilesX86; _pf = s + "\\"; }
					if(R.StartsWith_(_pf, true) && !Files.ExistsAsAny(R)) {
						var s2 = R.Remove(_pf.Length - 7, 6);
						if(Files.ExistsAsAny(s2)) R = s2;
						//info: "C:\\Program Files (x86)\\" in English, "C:\\Programme (x86)\\" in German etc.
						//never mind: System32 folder also has similar problem, because of redirection.
						//note: ShellExecuteEx also has this problem.
					}
				}

				return R;
			}
			static string _pf;

			#endregion
		}
	}
}
