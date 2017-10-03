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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Provides various version info, for example Windows OS version.
	/// The Windows version properties return true Windows version. If you need version that depends on manifest and debugger, instead use Environment.OSVersion.
	/// </summary>
	[DebuggerStepThrough]
	public static unsafe class Ver
	{
		static Ver()
		{
			var x = new RTL_OSVERSIONINFOW(); x.dwOSVersionInfoSize = Api.SizeOf(x);
			if(0 == RtlGetVersion(ref x)) {
				_winver = Math_.MakeUshort(x.dwMinorVersion, x.dwMajorVersion);
				//use this because Environment.OSVersion.Version (GetVersionEx) lies, even if we have correct manifest when is debugger present
			} else {
				Debug.Fail("RtlGetVersion");
				var v = Environment.OSVersion.Version;
				_winver = Math_.MakeUshort(v.Minor, v.Major);
			}

			_minWin8 = _winver >= Win8;
			_minWin8_1 = _winver >= Win8_1;
			_minWin10 = _winver >= Win10;

			_is64BitOS = Environment.Is64BitProcess; //fast
			if(!_is64BitOS) _is64BitOS = Api.IsWow64Process(Api.GetCurrentProcess(), out _isWow64) && _isWow64;
		}

		static readonly int _winver;
		static readonly bool _minWin8, _minWin8_1, _minWin10;
		static readonly bool _is64BitOS, _isWow64;

		struct RTL_OSVERSIONINFOW
		{
			public uint dwOSVersionInfoSize;
			public uint dwMajorVersion;
			public uint dwMinorVersion;
			public uint dwBuildNumber;
			public uint dwPlatformId;
			public fixed char szCSDVersion[128];
		}

		[DllImport("ntdll.dll", ExactSpelling = true)]
		static extern int RtlGetVersion(ref RTL_OSVERSIONINFOW lpVersionInformation);

		/// <summary>
		/// Gets classic Windows major+minor version value:
		/// Win7 (0x601), Win8 (0x602), Win8_1 (0x603), Win10 (0xA00).
		/// Example: <c>if(Ver.WinVer >= Ver.Win8) ...</c>
		/// </summary>
		public static int WinVer { get => _winver; }

		/// <summary>
		/// Classic Windows version major+minor values that can be used with <see cref="WinVer"/>.
		/// Example: <c>if(Ver.WinVer >= Ver.Win8) ...</c>
		/// </summary>
		public const int Win7 = 0x601, Win8 = 0x602, Win8_1 = 0x603, Win10 = 0xA00;

		/// <summary>
		/// true if Windows 8.0 or later.
		/// </summary>
		public static bool MinWin8 { get => _minWin8; }

		/// <summary>
		/// true if Windows 8.1 or later.
		/// </summary>
		public static bool MinWin8_1 { get => _minWin8_1; }

		/// <summary>
		/// true if Windows 10 or later.
		/// </summary>
		public static bool MinWin10 { get => _minWin10; }

		/// <summary>
		/// true if this process is 64-bit, false if 32-bit.
		/// The same as <see cref="Environment.Is64BitProcess"/>.
		/// </summary>
		public static bool Is64BitProcess { get => Environment.Is64BitProcess; }
		//Environment.Is64BitProcess is fast, just returns true or false depending on #if

		/// <summary>
		/// true if Windows is 64-bit, false if 32-bit.
		/// The same as <see cref="Environment.Is64BitOperatingSystem"/>, but fast. The .NET function is slow in 32-bit process; they forgot to optimize it.
		/// </summary>
		public static bool Is64BitOS { get => _is64BitOS; }

		/// <summary>
		/// Returns true if this process is a 32-bit process running on 64-bit Windows. Also known as WOW64 process.
		/// </summary>
		public static bool Is32BitProcessOn64BitOS { get => _isWow64; }
	}
}
