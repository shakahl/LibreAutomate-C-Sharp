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
	/// <summary>
	/// Provides various version info, for example the true Windows OS version.
	/// </summary>
	/// <remarks>
	/// The Windows version properties return true Windows version. If you need version that depends on manifest and debugger, instead use <see cref="Environment.OSVersion"/>.
	/// </remarks>
	[DebuggerStepThrough]
	public static unsafe class Ver
	{
		static Ver()
		{
			Api.RTL_OSVERSIONINFOW x = default; x.dwOSVersionInfoSize = Api.SizeOf(x);
			if(0 == Api.RtlGetVersion(ref x)) {
				_winver = AMath.MakeUshort(x.dwMinorVersion, x.dwMajorVersion);
				//use this because Environment.OSVersion.Version (GetVersionEx) lies, even if we have correct manifest when is debugger present
			} else {
				Debug.Fail("RtlGetVersion");
				var v = Environment.OSVersion.Version;
				_winver = AMath.MakeUshort(v.Minor, v.Major);
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

		/// <summary>
		/// Gets classic Windows major+minor version value:
		/// Win7 (0x601), Win8 (0x602), Win8_1 (0x603), Win10 (0xA00).
		/// Example: <c>if(Ver.WinVer >= Ver.Win8) ...</c>
		/// </summary>
		public static int WinVer => _winver;

		/// <summary>
		/// Classic Windows version major+minor values that can be used with <see cref="WinVer"/>.
		/// Example: <c>if(Ver.WinVer >= Ver.Win8) ...</c>
		/// </summary>
		public const int Win7 = 0x601, Win8 = 0x602, Win8_1 = 0x603, Win10 = 0xA00;

		/// <summary>
		/// true if Windows 8.0 or later.
		/// </summary>
		public static bool MinWin8 => _minWin8;

		/// <summary>
		/// true if Windows 8.1 or later.
		/// </summary>
		public static bool MinWin8_1 => _minWin8_1;

		/// <summary>
		/// true if Windows 10 or later.
		/// </summary>
		public static bool MinWin10 => _minWin10;

		/// <summary>
		/// true if this process is 64-bit, false if 32-bit.
		/// The same as <see cref="Environment.Is64BitProcess"/>.
		/// </summary>
		public static bool Is64BitProcess => Environment.Is64BitProcess;
		//Environment.Is64BitProcess is fast, just returns true or false depending on #if

		/// <summary>
		/// true if Windows is 64-bit, false if 32-bit.
		/// The same as <see cref="Environment.Is64BitOperatingSystem"/>, but fast. The .NET function is slow in 32-bit process; they forgot to optimize it.
		/// </summary>
		public static bool Is64BitOS => _is64BitOS;

		/// <summary>
		/// Returns true if this process is a 32-bit process running on 64-bit Windows. Also known as WOW64 process.
		/// </summary>
		public static bool Is32BitProcessAnd64BitOS => _isWow64;
	}
}
