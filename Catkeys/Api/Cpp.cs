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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys.Types
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
			if(default != Api.LoadLibrary(Folders.ThisApp + @"..\" + s)) return; //development PC
			throw new CatException(0, "*load AuCpp.dll");
		}

		//tested: calling DllImport functions is 4-5 times slower than C# functions.
		//tested: calling COM functions is 4-5 times slower than DllImport functions.
		//This is when passing strings, either with default marshaling or with 'fixed'.
		//Without 'fixed', calling DllImport functions is only 50% slower than C#. Did not test COM.

		internal struct Cpp_Acc
		{
			public Acc.IAccessible iacc;
			public int elem;
		}

		//internal struct Cpp_AccFindResults
		//{
		//	public int countOrError;

		//}

		//internal struct Cpp_AccTemp
		//{
		//	public int iacc;
		//	public int elem;
		//}

		//internal struct Cpp_AccFindTempResults
		//{
		//	public int version;
		//	public int memorySize;
		//	public int countOrError;
		//	public Cpp_AccTemp a;
		//}

		internal delegate int AccCallbackT(ref Cpp_Acc a);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_AccFind([MarshalAs(UnmanagedType.I1)] bool inject, Wnd w, Acc.IAccessible iaccparent, string role, string name, AFFlags flags, AccCallbackT also, int skip, out Cpp_Acc aResult);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_Unload();

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_Free(void* p);

		// STRING

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern char* Cpp_LowercaseTable();

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.I1)]
		//internal static extern bool Cpp_StringLike(char* s, LPARAM lenS, char* w, LPARAM lenW, [MarshalAs(UnmanagedType.I1)] bool ignoreCase = false);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.I1)]
		//internal static extern bool Cpp_StringEquals(char* s, LPARAM lenS, char* w, LPARAM lenW, [MarshalAs(UnmanagedType.I1)] bool ignoreCase = false);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.I1)]
		//internal static extern bool Cpp_StringEqualsI(char* a, char* b, int len);

		// PCRE

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Cpp_RegexParse(Regex_ x, string rx, LPARAM len, RXFlags flags = 0, char** errStr = null);

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

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_TestPCRE(string s, string p, uint flags = 0);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_TestWildex(string s, string p);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Cpp_TestSimpleStringBuilder();

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern void Cpp_TestClrHost();


		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.I1)]
		//internal static extern bool Cpp_WildexParse([In, Out] Wildex2 x, string w, LPARAM lenW, char** errStr = null);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//[return: MarshalAs(UnmanagedType.I1)]
		//internal static extern bool Cpp_WildexMatch(Wildex2 x, string s, LPARAM lenS);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern void Cpp_WildexDtor(Wildex2 x);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int Cpp_TestAdd(int a, int b);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern byte Cpp_TestAdd2(char* a, char* b);

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern byte Cpp_TestAdd3(char* a, char* b, LPARAM len);


		//[ComImport, Guid("3426CF3C-F7C2-4322-A292-463DB8729B54"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		//internal interface ICppTest
		//{
		//	[PreserveSig] byte One(char* a, char* b);
		//}

		//[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern ICppTest Cpp_Interface();
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
