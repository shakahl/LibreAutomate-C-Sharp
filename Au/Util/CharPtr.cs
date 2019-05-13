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

namespace Au.Util
{
	/// <summary>
	/// String functions that work with char* and ANSI byte* strings.
	/// </summary>
	//[DebuggerStepThrough]
	internal static unsafe class LibCharPtr
	{
		/// <summary>
		/// Finds unmanaged UTF-16 string length (number of characters).
		/// Scans the string until '\0' character found.
		/// </summary>
		/// <param name="p">'\0'-terminated UTF-16 string. Can be null.</param>
		public static int Length(char* p)
		{
			if(p == null) return 0;
			for(int i = 0; ; i++) if(p[i] == '\0') return i;
		}

		/// <summary>
		/// Finds unmanaged UTF-16 string length (number of characters).
		/// Scans the string until '\0' character found, but not exceeding the specified length.
		/// </summary>
		/// <param name="p">'\0'-terminated UTF-16 string. Can be null.</param>
		/// <param name="nMax">Max allowed string length. The function returns nMax if does not find '\0' character within first nMax characters.</param>
		public static int Length(char* p, int nMax)
		{
			if(p == null) return 0;
			for(int i = 0; i < nMax; i++) if(p[i] == '\0') return i;
			return nMax;
		}

		/// <summary>
		/// Finds unmanaged ANSI string length (number of bytes).
		/// Scans the string until '\0' character found.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be any ANSI encoding, for example UTF8; this function counts bytes, not multi-byte characters. Can be null.</param>
		public static int Length(byte* p)
		{
			if(p == null) return 0;
			for(int i = 0; ; i++) if(p[i] == 0) return i;
		}

		/// <summary>
		/// Finds unmanaged ANSI string length (number of bytes).
		/// Scans the string until '\0' character found, but not exceeding the specified length.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be any encoding, for example UTF8; this function counts bytes, not multi-byte characters. Can be null.</param>
		/// <param name="nMax">Max allowed string length. The function returns nMax if does not find '\0' character within first nMax characters.</param>
		public static int Length(byte* p, int nMax)
		{
			if(p == null) return 0;
			for(int i = 0; i < nMax; i++) if(p[i] == 0) return i;
			return nMax;
		}

#if unused
		/// <summary>
		/// Returns true if unmanaged UTF-16 string p starts with string s.
		/// </summary>
		/// <param name="p">'\0'-terminated UTF-16 string. Can be null.</param>
		/// <param name="s">Cannot be null.</param>
		public static bool Starts(char* p, string s)
		{
			if(p == null) return false;
			for(int i = 0, n = s.Length; i < n; i++, p++) {
				if(*p != s[i]) return false;
				if(*p == '\0') return false;
			}
			return true;
		}
#endif
		/// <summary>
		/// Returns true if unmanaged UTF-16 string p ends with string s.
		/// </summary>
		/// <param name="p">UTF-16 string. Can be null.</param>
		/// <param name="len">p length.</param>
		/// <param name="s">Cannot be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		public static bool Ends(char* p, int len, string s, bool ignoreCase)
		{
			if(p == null || len < s.Length) return false;
			p += len - s.Length;
			if(ignoreCase) {
				var t = LibTables.LowerCase;
				for(int i = 0; i < s.Length; i++, p++) {
					if(t[*p] != t[s[i]]) return false;
				}
			} else {
				for(int i = 0; i < s.Length; i++, p++) {
					if(*p != s[i]) return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if unmanaged ANSI string p starts with string s. Case-sensitive.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be null.</param>
		/// <param name="s">Must contain only ASCII characters. Cannot be null.</param>
		public static bool AsciiStarts(byte* p, string s)
		{
			if(p == null) return false;
			int i, n = s.Length;
			for(i = 0; i < n; i++, p++) {
				if(*p != s[i]) return false;
				if(*p == '\0') return false;
			}
			return true;
		}
		/// <summary>
		/// Returns true if unmanaged ANSI string p starts with string s. Case-insensitive.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be null.</param>
		/// <param name="s">Must contain only ASCII characters. Cannot be null.</param>
		public static bool AsciiStartsi(byte* p, string s)
		{
			if(p == null) return false;
			var t = LibTables.LowerCase;
			int i, n = s.Length;
			for(i = 0; i < n; i++, p++) {
				if(t[*p] != t[s[i]]) return false;
				if(*p == '\0') return false;
			}
			return true;
		}

		/// <summary>
		/// Returns true if unmanaged ANSI string p and string s are equal. Case-sensitive.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be null.</param>
		/// <param name="s">Must contain only ASCII characters. Cannot be null.</param>
		public static bool AsciiEq(byte* p, string s) => AsciiStarts(p, s) && p[s.Length] == 0;

		/// <summary>
		/// Returns true if unmanaged ANSI string p and string s are equal. Case-insensitive.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be null.</param>
		/// <param name="s">Must contain only ASCII characters. Cannot be null.</param>
		public static bool AsciiEqi(byte* p, string s) => AsciiStartsi(p, s) && p[s.Length] == 0;

		/// <summary>
		/// Finds character in unmanaged ANSI string which can be binary.
		/// </summary>
		/// <param name="p">ANSI string. Can be null.</param>
		/// <param name="len">Length of p to search in.</param>
		/// <param name="ch">ASCII character.</param>
		public static int AsciiFindChar(byte* p, int len, byte ch)
		{
			if(p != null)
				for(int i = 0; i < len; i++) if(p[i] == ch) return i;
			return -1;
		}

		/// <summary>
		/// Finds substring in unmanaged ANSI string which can be binary.
		/// Returns -1 if not found or if s is null/"" or s2 is "".
		/// </summary>
		/// <param name="p">ANSI string. Can be null.</param>
		/// <param name="len">Length of p to search in.</param>
		/// <param name="s">Substring to find. Must contain only ASCII characters. Cannot be null.</param>
		public static int AsciiFindString(byte* p, int len, string s)
		{
			int len2 = s.Length;
			if(p != null && len2 <= len && len2 > 0) {
				var ch = s[0];
				for(int i = 0, n = len - len2; i <= n; i++) if(p[i] == ch) {
						for(int j = 1; j < len2; j++) if(p[i + j] != s[j]) goto g1;
						return i;
						g1:;
					}
			}
			return -1;

			//speed: with long strings slightly slower than strstr.
		}

		///// <summary>
		///// Finds substring in unmanaged '\0'-terminated ANSI string.
		///// Returns -1 if not found or if s is null/"" or s2 is "".
		///// </summary>
		///// <param name="s">ANSI string. Can be null.</param>
		///// <param name="s2">Substring to find. Must contain only ASCII characters. Cannot be null.</param>
		//public static int AsciiFindString(byte* s, string s2)
		//{
		//	int len2 = s2.Length;
		//	if(s != null len2 > 0) {
		//		var ch = s2[0];
		//		for(int i = 0, n = len - len2; i <= n; i++) if(s[i] == ch) {
		//				for(int j = 1; j < len2; j++) if(s[i + j] != s2[j]) goto g1;
		//				return i;
		//				g1:;
		//			}
		//	}
		//	return -1;

		//	//speed: with long strings slightly slower than strstr.
		//}

		/// <summary>
		/// Case-sensitive compares native string with managed string and returns true if they are equal.
		/// </summary>
		/// <param name="p">Native string.</param>
		/// <param name="len">p length. Returns false if it is != s.Length.</param>
		/// <param name="s">Managed string.</param>
		public static bool Eq(char* p, int len, string s)
		{
			if(p == null) return s == null; if(s == null) return false;
			if(len != s.Length) return false;
			int i;
			for(i = 0; i < s.Length; i++) if(s[i] != p[i]) break;
			return i == s.Length;
		}

		/// <summary>
		/// Case-sensitive compares native ANSI string with managed ANSI string and returns true if they are equal.
		/// </summary>
		/// <param name="p">Native string.</param>
		/// <param name="s">Managed string.</param>
		public static bool Eq(byte* p, byte[] s)
		{
			if(p == null) return s == null; if(s == null) return false;
			int i;
			for(i = 0; i < s.Length; i++) if(s[i] != p[i]) return false;
			return p[i] == 0;
		}
	}
}
