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

using Au;
using static Au.NoClass;

namespace Au.Types
{
	[DebuggerStepThrough]
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal unsafe class Cpp
	{
		static Cpp()
		{
			string s = Ver.Is64BitProcess ? @"Dll\64bit\AuCpp.dll" : @"Dll\32bit\AuCpp.dll";
			if(default != Api.LoadLibrary(Folders.ThisApp + s)) return; //normal
			if(default != Api.LoadLibrary(Folders.ThisAppTemp + s)) return; //extracted from resources
			throw new AuException(0, "*load AuCpp.dll");

			//TODO: the dll is unavailable if running in a nonstandard environment, eg VS C# Interactive (then Folders.ThisApp is "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\InteractiveComponents").
		}

		//speed:
		//	Calling DllImport functions is 4-5 times slower than C# functions.
		//	Calling COM functions is 2-10 times slower than DllImport functions.
		//	Tested with int and string parameters, with default marshaling and with 'fixed'.
		//	If only int parameters, DllImport is only 50% slower than C#. But COM slow anyway.
		//	Strings passed to COM methods by default are converted to BSTR, and a new BSTR is allocated/freed.

		internal struct Cpp_Acc
		{
			public IntPtr acc;
			public int elem;
			public Acc._Misc misc;

			public Cpp_Acc(IntPtr iacc, int elem_) { acc = iacc; elem = elem_; misc = default; }
			public Cpp_Acc(Acc a) { acc = a._iacc; elem = a._elem; misc = a._misc; }
			public static implicit operator Cpp_Acc(Acc a) =>new Cpp_Acc(a);
		}

		internal delegate int AccCallbackT(Cpp_Acc a);

		internal struct Cpp_AccParams
		{
			string _role, _name, _prop;
			int _roleLength, _nameLength, _propLength;
			public AFFlags flags;
			public int skip;
			char resultProp; //Acc.Finder.RProp

			public Cpp_AccParams(string role, string name, string prop, AFFlags flags, int skip, char resultProp) :this()
			{
				if(role != null) { _role = role; _roleLength = role.Length; }
				if(name != null) { _name = name; _nameLength = name.Length; }
				if(prop != null) { _prop = prop; _propLength = prop.Length; }
				this.flags = flags;
				this.skip = skip;
				this.resultProp = resultProp;
			}
		}

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern EError Cpp_AccFind(Wnd w, Cpp_Acc* aParent, [In] ref Cpp_AccParams ap, AccCallbackT also, out Cpp_Acc aResult, [MarshalAs(UnmanagedType.BStr)] out string sResult);

		internal enum EError
		{
			NotFound = 0x1001, //AO not found. With FindAll - no errors. This is actually not an error.
			InvalidParameter = 0x1002, //invalid parameter, for example wildcard expression (or regular expression in it)
			WindowClosed = 0x1003, //the specified window handle is invalid or the window was destroyed while injecting
			WaitChromeDisabled = 0x1004, //need to wait while enabling Chrome AOs finished
		}

		internal static bool IsCppError(int hr)
		{
			return hr >= (int)EError.NotFound && hr <= (int)EError.WaitChromeDisabled;
		}

		/// <summary>
		/// flags: 1 inproc, 2 get only name.
		/// </summary>
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccFromWindow(int flags, Wnd w, AccOBJID objId, out Cpp_Acc aResult, out BSTR sResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccNavigate(Cpp_Acc aFrom, string navig, out Cpp_Acc aResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetProp(Cpp_Acc a, char prop, out BSTR sResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccWeb(Cpp_Acc a, string what, out BSTR sResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetRect(Cpp_Acc a, out RECT r);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetRole(Cpp_Acc a, out AccROLE roleInt, out BSTR roleStr);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetInt(Cpp_Acc a, char what, out int R);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccAction(Cpp_Acc a, char action, [MarshalAs(UnmanagedType.BStr)] string param = null);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccSelect(Cpp_Acc a, AccSELFLAG flagsSelect);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetSelection(Cpp_Acc a, out BSTR sResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetProps(Cpp_Acc a, string props, out BSTR sResult);

		//flags: 1 get UIA, 2 prefer LINK.
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccFromPoint(Point p, AXYFlags flags, out Cpp_Acc aResult);

		//flags: 1 get UIA.
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccGetFocused(Wnd w, int flags, out Cpp_Acc aResult);


		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_Unload();

		// STRING

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern char* Cpp_LowercaseTable();

		// PCRE

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Cpp_RegexParse(Regex_ x, string rx, LPARAM len, RXFlags flags, [MarshalAs(UnmanagedType.BStr)] out string errStr);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Cpp_RegexMatch(Regex_ x, string s, LPARAM len, LPARAM start = default, RMFlags flags = 0);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_RegexDtor(Regex_ x);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_RegexSize(Regex_ x);


		// TEST

		//TODO: remove tests
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_Test();

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int Cpp_TestInt(int a, int b, int c);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int Cpp_TestString(string a, int b, int c);

		//[ComImport, Guid("3426CF3C-F7C2-4322-A292-463DB8729B54"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		//internal interface ICppTest
		//{
		//	[PreserveSig] int TestInt(int a, int b, int c);
		//	[PreserveSig] int TestString([MarshalAs(UnmanagedType.LPWStr)] string a, int b, int c);
		//	[PreserveSig] int TestBSTR(string a, int b, int c);
		//}

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern ICppTest Cpp_Interface();


		//[ComImport, Guid("57017F56-E7CA-4A7B-A8F8-2AE36077F50D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		//internal interface IThreadExitEvent
		//{
		//	[PreserveSig] int Unsubscribe();
		//}

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IThreadExitEvent Cpp_ThreadExitEvent(IntPtr callback);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern void Cpp_ThreadExitEvent2(IntPtr callback);
	}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	[Flags]
	public enum RXFlags :long
	{
		ANCHORED = 0x80000000,
		NO_UTF_CHECK = 0x40000000,
		ENDANCHORED = 0x20000000,
		ALLOW_EMPTY_CLASS = 0x00000001,
		ALT_BSUX = 0x00000002,
		AUTO_CALLOUT = 0x00000004,
		CASELESS = 0x00000008,
		DOLLAR_ENDONLY = 0x00000010,
		DOTALL = 0x00000020,
		DUPNAMES = 0x00000040,
		EXTENDED = 0x00000080,
		FIRSTLINE = 0x00000100,
		MATCH_UNSET_BACKREF = 0x00000200,
		MULTILINE = 0x00000400,
		NEVER_UCP = 0x00000800,
		NEVER_UTF = 0x00001000,
		NO_AUTO_CAPTURE = 0x00002000,
		NO_AUTO_POSSESS = 0x00004000,
		NO_DOTSTAR_ANCHOR = 0x00008000,
		NO_START_OPTIMIZE = 0x00010000,
		UCP = 0x00020000,
		UNGREEDY = 0x00040000,
		UTF = 0x00080000,
		NEVER_BACKSLASH_C = 0x00100000,
		ALT_CIRCUMFLEX = 0x00200000,
		ALT_VERBNAMES = 0x00400000,
		USE_OFFSET_LIMIT = 0x00800000,
		EXTENDED_MORE = 0x01000000,
		LITERAL = 0x02000000,

		//EXTRA_ALLOW_SURROGATE_ESCAPES = 0x1_00000000, //not used with UTF-16
		//EXTRA_BAD_ESCAPE_IS_LITERAL = 0x2_00000000, //dangerous
		EXTRA_MATCH_WORD = 0x4_00000000,
		EXTRA_MATCH_LINE = 0x8_00000000,
	}

	[Flags]
	public enum RMFlags :uint
	{
		ANCHORED = 0x80000000,
		NO_UTF_CHECK = 0x40000000,
		ENDANCHORED = 0x20000000,
		NOTBOL = 0x00000001,
		NOTEOL = 0x00000002,
		NOTEMPTY = 0x00000004,
		NOTEMPTY_ATSTART = 0x00000008,
		PARTIAL_SOFT = 0x00000010,
		PARTIAL_HARD = 0x00000020,
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
