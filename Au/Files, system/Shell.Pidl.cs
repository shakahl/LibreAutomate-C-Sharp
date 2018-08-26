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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	partial class Shell
	{
		/// <summary>
		/// Manages an ITEMIDLIST structure that is used with shell API to identify files and other shell objects. Like a file-system path, but not string.
		/// </summary>
		/// <remarks>
		/// When calling shell API, virtual objects can be identified only by ITEMIDLIST. Some API also support "parsing name", which usually looks like "::{CLSID-1}\::{CLSID-2}". File-system objects can be identified by path as well as by ITEMIDLIST. URLs can be identified by URL as well as by ITEMIDLIST.
		/// 
		/// The ITEMIDLIST structure is in unmanaged memory, therefore this class implements IDisposable.
		/// </remarks>
		public unsafe sealed class Pidl :IDisposable
		{
			IntPtr _pidl;

			/// <summary>
			/// Gets the ITEMIDLIST pointer (PIDL).
			/// </summary>
			/// <remarks>
			/// The ITEMIDLIST memory is managed by this variable and will be freed when disposing or GC-collecting it. Use <see cref="GC.KeepAlive"/> where need.
			/// </remarks>
			public IntPtr UnsafePtr => _pidl;

			/// <summary>
			/// Gets the ITEMIDLIST pointer (PIDL).
			/// </summary>
			/// <remarks>
			/// Use to pass to API where the parameter type is HandlePtr. It is safer than <see cref="UnsafePtr"/> because ensures that this variable will not be GC-collected during API call even if not referenced after the call.
			/// </remarks>
			public HandleRef HandleRef => new HandleRef(this, _pidl);

			/// <summary>
			/// Returns true if the PIDL is default(IntPtr).
			/// </summary>
			public bool IsNull => _pidl == default;

			/// <summary>
			/// Assigns an ITEMIDLIST to this variable.
			/// </summary>
			/// <param name="pidl">
			/// ITEMIDLIST pointer (PIDL).
			/// It can be created by any API that creates ITEMIDLIST. They allocate the memory with API CoTaskMemAlloc.
			/// This variable will finally free it with Marshal.FreeCoTaskMem.
			/// </param>
			public Pidl(IntPtr pidl) => _pidl = pidl;

			/// <summary>
			/// Frees the ITEMIDLIST with Marshal.FreeCoTaskMem and clears this variable.
			/// </summary>
			public void Dispose()
			{
				_Dispose();
				GC.SuppressFinalize(this);
			}

			void _Dispose()
			{
				if(_pidl != default) {
					Marshal.FreeCoTaskMem(_pidl);
					_pidl = default;
				}
			}

			///
			~Pidl() { _Dispose(); }

			/// <summary>
			/// Gets the ITEMIDLIST and clears this variable so that it cannot be used and will not free the ITEMIDLIST memory. To free it use Marshal.FreeCoTaskMem.
			/// </summary>
			public IntPtr Detach()
			{
				var R = _pidl;
				_pidl = default;
				return R;
			}

			/// <summary>
			/// Converts string to ITEMIDLIST and creates new Pidl variable that holds it.
			/// Returns null if failed.
			/// Note: Pidl is disposable.
			/// </summary>
			/// <param name="s">A file-system path or URL or shell object parsing name (see <see cref="ToShellString"/>) or ":: HexEncodedITEMIDLIST" (see <see cref="ToHexString"/>). Supports environment variables (see <see cref="Path_.ExpandEnvVar"/>).</param>
			/// <param name="throwIfFailed">If failed, throw AuException.</param>
			/// <exception cref="AuException">Failed, and throwIfFailed is true. Probably invalid s.</exception>
			/// <remarks>
			/// Calls <msdn>SHParseDisplayName</msdn>, except when string is ":: HexEncodedITEMIDLIST".
			/// Never fails if s is ":: HexEncodedITEMIDLIST", even if it creates an invalid ITEMIDLIST.
			/// </remarks>
			public static Pidl FromString(string s, bool throwIfFailed = false)
			{
				IntPtr R = LibFromString(s, throwIfFailed);
				return (R == default) ? null : new Pidl(R);
			}

			/// <summary>
			/// The same as <see cref="FromString"/>, just returns unmanaged ITEMIDLIST pointer (PIDL).
			/// Later need to free it with Marshal.FreeCoTaskMem.
			/// </summary>
			/// <param name="s"></param>
			/// <param name="throwIfFailed"></param>
			internal static IntPtr LibFromString(string s, bool throwIfFailed = false)
			{
				IntPtr R;
				s = _Normalize(s);
				if(s.StartsWith_(":: ")) { //hex-encoded ITEMIDLIST
					int n = (s.Length - 3) / 2;
					R = Marshal.AllocCoTaskMem(n + 2);
					byte* b = (byte*)R;
					n = Convert_.HexDecode(s, b, n, 3);
					b[n] = b[n + 1] = 0;
				} else { //file-system path or URL or shell object parsing name
					var hr = Api.SHParseDisplayName(s, default, out R, 0, null);
					if(hr != 0) {
						if(throwIfFailed) throw new AuException(hr);
						return default;
					}
				}
				return R;
			}

			/// <summary>
			/// Converts the ITEMIDLIST to file path or URL or shell object parsing name or display name, depending on stringType argument.
			/// Returns null if this variable does not have an ITEMIDLIST (eg disposed or detached).
			/// If failed, returns null or throws exception.
			/// </summary>
			/// <param name="stringType">
			/// String format.
			/// Often used:
			/// Native.SIGDN.NORMALDISPLAY - returns object name without path. It is best to display in UI but cannot be parsed to create ITEMIDLIST again.
			/// Native.SIGDN.FILESYSPATH - returns path if the ITEMIDLIST identifies a file system object (file or directory). Else returns null.
			/// Native.SIGDN.URL - if URL, returns URL. If file system object, returns its path like "file:///C:/a/b.txt". Else returns null.
			/// Native.SIGDN.DESKTOPABSOLUTEPARSING - returns path (if file system object) or URL (if URL) or shell object parsing name (if virtual object eg Control Panel). Note: not all returned parsing names can actually be parsed to create ITEMIDLIST again, therefore usually it's better to use <see cref="ToString"/> instead.
			/// </param>
			/// <param name="throwIfFailed">If failed, throw AuException.</param>
			/// <exception cref="AuException">Failed, and throwIfFailed is true.</exception>
			/// <remarks>
			/// Calls <msdn>SHGetNameFromIDList</msdn>.
			/// </remarks>
			public string ToShellString(Native.SIGDN stringType, bool throwIfFailed = false)
			{
				var R = LibToShellString(_pidl, stringType, throwIfFailed);
				GC.KeepAlive(this);
				return R;
			}

			/// <summary>
			/// The same as <see cref="ToShellString"/>, just uses an ITEMIDLIST pointer that is not stored in a Pidl variable.
			/// </summary>
			internal static string LibToShellString(IntPtr pidl, Native.SIGDN stringType, bool throwIfFailed = false)
			{
				if(pidl == default) return null;
				var hr = Api.SHGetNameFromIDList(pidl, stringType, out string R);
				if(hr == 0) return R;
				if(throwIfFailed) throw new AuException(hr);
				return null;
			}

			/// <summary>
			/// Converts the ITEMIDLIST to string.
			/// If it identifies an existing file-system object (file or directory), returns path. If URL, returns URL. Else returns ":: HexEncodedITEMIDLIST" (see <see cref="ToHexString"/>).
			/// Returns null if this variable does not have an ITEMIDLIST (eg disposed or detached).
			/// </summary>
			public override string ToString()
			{
				var R = LibToString(_pidl);
				GC.KeepAlive(this);
				return R;
			}

#if true
			/// <summary>
			/// The same as <see cref="ToString"/>, just uses an ITEMIDLIST pointer that is not stored in a Pidl variable.
			/// </summary>
			internal static string LibToString(IntPtr pidl)
			{
				if(pidl == default) return null;
				Api.IShellItem si = null;
				try {
					if(0 == Api.SHCreateShellItem(default, null, pidl, out si)) {
						//if(0 == Api.SHCreateItemFromIDList(pidl, Api.IID_IShellItem, out si)) { //same speed
						//if(si.GetAttributes(0xffffffff, out uint attr)>=0) Print(attr);
						if(si.GetAttributes(Api.SFGAO_BROWSABLE | Api.SFGAO_FILESYSTEM, out uint attr) >= 0 && attr != 0) {
							var f = (0 != (attr & Api.SFGAO_FILESYSTEM)) ? Native.SIGDN.FILESYSPATH : Native.SIGDN.URL;
							if(0 == si.GetDisplayName(f, out var R)) return R;
						}
					}
				}
				finally { Api.ReleaseComObject(si); }
				return LibToHexString(pidl);
			}
			//this version is 40% slower with non-virtual objects (why?), but with virtual objects same speed as SIGDN_DESKTOPABSOLUTEPARSING.
			//The fastest (update: actually not) version would be to call LibToShellString(SIGDN_DESKTOPABSOLUTEPARSING), and then call LibToHexString if it returns not a path or URL. But it is unreliable, because can return string in any format, eg "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
#elif false
			//this version works, but with virtual objects 2 times slower than SIGDN_DESKTOPABSOLUTEPARSING (which already is very slow with virtual).
			public static string ToString2(IntPtr pidl)
			{
				if(pidl == default) return null;
				var R = ToShellString2(pidl, Native.SIGDN.FILESYSPATH);
				if(R == null) R = ToShellString2(pidl, Native.SIGDN.URL);
				if(R == null) R = ToHexString2(pidl);
				return R;
			}
#elif true
			//this version works, but with virtual objects 30% slower. Also 30% slower for non-virtual objects (why?).
			public static string ToString2(IntPtr pidl)
			{
				if(pidl == default) return null;

				Api.IShellItem si = null;
				try {
					if(0 == Api.SHCreateShellItem(default, null, pidl, out si)) {
						string R = null;
						if(0 == si.GetDisplayName(Native.SIGDN.FILESYSPATH, out R)) return R;
						if(0 == si.GetDisplayName(Native.SIGDN.URL, out R)) return R;
					}
				}
				finally { Api.ReleaseComObject(si); }
				return ToHexString2(pidl);
			}
#else
			//SHGetPathFromIDList also slow.
			//SHBindToObject cannot get ishellitem.
#endif

			/// <summary>
			/// Returns string ":: HexEncodedITEMIDLIST".
			/// It can be used with some functions of this library, mostly of classes Shell, Shell.Pidl and Icon_. Cannot be used with native and .NET functions.
			/// Returns null if this variable does not have an ITEMIDLIST (eg disposed or detached).
			/// </summary>
			public string ToHexString()
			{
				var R = LibToHexString(_pidl);
				GC.KeepAlive(this);
				return R;
			}

			/// <summary>
			/// The same as <see cref="ToHexString"/>, just uses an ITEMIDLIST pointer that is not stored in a Pidl variable.
			/// </summary>
			internal static string LibToHexString(IntPtr pidl)
			{
				if(pidl == default) return null;
				int n = Api.ILGetSize(pidl) - 2; //API gets size with the terminating '\0' (2 bytes)
				if(n < 0) return null;
				if(n == 0) return ":: "; //shell root - Desktop
				return ":: " + Convert_.HexEncode((void*)pidl, n, true);
			}
		}
	}
}
