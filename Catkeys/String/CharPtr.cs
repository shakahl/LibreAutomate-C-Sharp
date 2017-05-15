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
using System.Xml.Linq;
//using System.Xml.XPath;

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// String functions that work with char* and ANSI byte* strings.
	/// </summary>
	//[DebuggerStepThrough]
	internal static unsafe class CharPtr
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
		public static bool StartsWith(char* p, string s)
		{
			if(p == null) return false;
			int i, n = s.Length;
			for(i = 0; i < n; i++, p++) {
				if(*p != s[i]) return false;
				if(*p == '\0') return false;
			}
			return true;
		}
#endif
		/// <summary>
		/// Returns true if unmanaged ANSI string p starts with string s. Case-sensitive.
		/// </summary>
		/// <param name="p">'\0'-terminated ANSI string. Can be null.</param>
		/// <param name="s">Must contain only ASCII characters. Cannot be null.</param>
		public static bool AsciiStartsWith(byte* p, string s)
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
		public static bool AsciiStartsWithI(byte* p, string s)
		{
			if(p == null) return false;
			var t = Util.LibTables.LowerCase;
			int i, n = s.Length;
			for(i = 0; i < n; i++, p++) {
				if(t[*p] != t[s[i]]) return false;
				if(*p == '\0') return false;
			}
			return true;
		}

		/// <summary>
		/// Finds character in unmanaged ANSI string which can be binary.
		/// </summary>
		/// <param name="s">ANSI string. Can be null.</param>
		/// <param name="len">Length of s to search in.</param>
		/// <param name="ch">ASCII character.</param>
		public static int AsciiFindChar(byte* s, int len, byte ch)
		{
			if(s != null)
				for(int i = 0; i < len; i++) if(s[i] == ch) return i;
			return -1;
		}

		/// <summary>
		/// Finds substring in unmanaged ANSI string which can be binary.
		/// Returns -1 if not found or if s is null/"" or s2 is "".
		/// </summary>
		/// <param name="s">ANSI string. Can be null.</param>
		/// <param name="len">Length of s to search in.</param>
		/// <param name="s2">Substring to find. Must contain only ASCII characters. Cannot be null.</param>
		public static int AsciiFindString(byte* s, int len, string s2)
		{
			int len2 = s2.Length;
			if(s != null && len2 <= len && len2 > 0) {
				var ch = s2[0];
				for(int i = 0, n = len - len2; i <= n; i++) if(s[i] == ch) {
						for(int j = 1; j < len2; j++) if(s[i + j] != s2[j]) goto g1;
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
	}
}
