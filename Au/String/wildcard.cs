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
using System.Text.RegularExpressions;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	public static unsafe partial class String_
	{
		#region Like_

		/// <summary>
		/// Compares this string with a string that possibly contains wildcard characters.
		/// Returns true if the strings match.
		/// 
		/// Wildcard characters:
		/// * - zero or more of any characters.
		/// ? - any character.
		/// </summary>
		/// <param name="t">This string. If null, returns false. If "", returns true if pattern is "" or "*".</param>
		/// <param name="pattern">String that possibly contains wildcard characters. Cannot be null. If "", returns true if this string is "". If "*", always returns true except when this string is null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <remarks>
		/// Like all String_ functions, performs ordinal comparison, ie does not depend on current culture.
		/// Much faster than Regex.IsMatch and not much slower than Equals_, EndsWith_, IndexOf_ etc.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// string s = @"C:\abc\mno.xyz";
		/// if(s.Like_(@"C:\abc\mno.xyz")) Print("matches whole text (no wildcard characters)");
		/// if(s.Like_(@"C:\abc\*")) Print("starts with");
		/// if(s.Like_(@"*.xyz")) Print("ends with");
		/// if(s.Like_(@"*mno*")) Print("contains");
		/// if(s.Like_(@"C:\*.xyz")) Print("starts and ends with");
		/// if(s.Like_(@"?:*")) Print("any character, : and possibly more text");
		/// ]]></code>
		/// </example>
		/// <seealso cref="Wildex"/>
		/// <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">Wildcard expression</conceptualLink>
#if false //somehow speed depends on dll version. With some versions same as C# code, with some slower. Also depends on string. With shortest strings 50% slower.
		public static bool Like_(this string t, string pattern, bool ignoreCase = false)
		{
			if(t == null) return false;
			fixed (char* pt = t, pw = pattern)
				return Cpp.Cpp_StringLike(pt, t.Length, pw, pattern.Length, ignoreCase);
		}
#else
		public static bool Like_(this string t, string pattern, bool ignoreCase = false)
		{
			if(t == null) return false;
			int patLen = pattern.Length;
			if(patLen == 0) return t.Length == 0;
			if(patLen == 1 && pattern[0] == '*') return true;
			if(t.Length == 0) return false;

			fixed (char* str = t, pat = pattern) {
				return _WildcardCmp(str, pat, t.Length, patLen, ignoreCase ? Util.LibTables.LowerCase : null);
			}

			//info:
			//	Similar .NET function Microsoft.VisualBasic.CompilerServices.Operators.LikeString()
			//	supports more wildcard characters etc, depends on current culture, is 6-250 times slower, has bugs.
		}

		static bool _WildcardCmp(char* s, char* w, int lenS, int lenW, char* table)
		{
			char* se = s + lenS, we = w + lenW;

			//find '*' from start. Makes faster in some cases.
			for(; (w < we && s < se); w++, s++) {
				char cS = s[0], cW = w[0];
				if(cW == '*') goto g1;
				if(cW == cS || cW == '?') continue;
				if((table == null) || (table[cW] != table[cS])) return false;
			}
			if(w == we) return s == se; //w ended?
			goto gr; //s ended
			g1:

			//find '*' from end. Makes "*text" much faster.
			for(; (we > w && se > s); we--, se--) {
				char cS = se[-1], cW = we[-1];
				if(cW == '*') break;
				if(cW == cS || cW == '?') continue;
				if((table == null) || (table[cW] != table[cS])) return false;
			}

			//Algorithm by Alessandro Felice Cantatore, http://xoomer.virgilio.it/acantato/dev/wildcard/wildmatch.html
			//Changes: supports '\0' in string; case-sensitive or not; restructured, in many cases faster.

			int i = 0;
			gStar: //info: goto used because C# compiler makes the loop faster when it contains less code
			w += i + 1;
			if(w == we) return true;
			s += i;

			for(i = 0; s + i < se; i++) {
				char sW = w[i];
				if(sW == '*') goto gStar;
				if(sW == s[i] || sW == '?') continue;
				if((table != null) && (table[sW] == table[s[i]])) continue;
				s++; i = -1;
			}

			w += i;
			gr:
			while(w < we && *w == '*') w++;
			return w == we;

			//info: Could implement escape sequence ** for * and maybe *? for ?.
			//	But it makes code slower etc.
			//	Not so important.
			//	Most users would not know about it.
			//	Usually can use ? for literal * and ?.
			//	Usually can use regular expression if need such precision.
			//	Then cannot use "**options " for wildcard expressions.
			//	Could use other escape sequences, eg [*], [?] and [[], but it makes slower and is more harmful than useful.

			//The first two loops are fast, but Equals_ much faster when !ignoreCase. We cannot use such optimizations that it can.
			//The slowest case is "*substring*", because then the first two loops don't help.
			//	Then similar speed as string.IndexOf(ordinal) and API <msdn>FindStringOrdinal</msdn>.
			//	Possible optimization, but need to add much code, and makes not much faster, and makes other cases slower, difficult to avoid it.
		}
#endif

		/// <summary>
		/// Calls <see cref="Like_(string, string, bool)"/> for each wildcard pattern specified in the argument list until it returns true.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="patterns">One or more wildcard strings. The array and strings cannot be null.</param>
		public static int Like_(this string t, bool ignoreCase = false, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.Like_(patterns[i], ignoreCase)) return i + 1;
			return 0;
		}

		#endregion Like_
	}
}

namespace Au.Types
{
	/// <summary>
	/// This class implements <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink> parsing and matching (comparing).
	/// Typically used in 'find' functions. For example, <see cref="Wnd.Find"/> uses it to compare window name, class name and program.
	/// The 'find' function creates a Wildex instance (which parses the wildcard expression), then calls <see cref="Match"/> for each item (eg window) to compare some its property text.
	/// </summary>
	/// <exception cref="ArgumentException">Invalid "**options " or regular expression.</exception>
	/// <example>
	/// <code><![CDATA[
	/// //This version does not support wildcard expressions.
	/// Document Find1(string name, string date)
	/// {
	/// 	return Documents.Find(x => x.Name.Equals_(name) && x.Date.Equals_(date));
	/// }
	/// 
	/// //This version supports wildcard expressions.
	/// //null-string arguments are not compared.
	/// Document Find2(string name, string date)
	/// {
	/// 	Wildex n = name, d = date; //null if the string is null
	/// 	return Documents.Find(x => (n == null || n.Match(x.Name)) && (d == null || d.Match(x.Date)));
	/// }
	/// 
	/// //Example of calling such function.
	/// //Find item whose name is "example" (case-insensitive) and date starts with "2017-".
	/// var item = x.Find2("example", "2017-*");
	/// ]]></code>
	/// </example>
	public class Wildex
	{
		//note: could be struct, but somehow then slower. Slower instance creation, calling methods, in all cases.

		object _obj; //string, Regex or Wildex[]. Tested: getting string etc with '_obj as string' is fast.
		WildType _type;
		bool _ignoreCase;
		bool _not;

		/// <param name="wildcardExpression">
		/// <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">Wildcard expression</conceptualLink>.
		/// Cannot be null (throws exception).
		/// "" will match "".
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">Invalid "**options " or regular expression.</exception>
		public Wildex(string wildcardExpression)
		{
			var w = wildcardExpression;
			if(w == null) throw new ArgumentNullException();
			_type = WildType.Wildcard;
			_ignoreCase = true;
			string[] split = null;

			if(w.Length >= 3 && w[0] == '*' && w[1] == '*') {
				for(int i = 2, j; i < w.Length; i++) {
					switch(w[i]) {
					case 't': _type = WildType.Text; break;
					case 'r': _type = WildType.RegexPcre; break;
					case 'R': _type = WildType.RegexNet; break;
					case 'm': _type = WildType.Multi; break;
					case 'c': _ignoreCase = false; break;
					case 'n': _not = true; break;
					case ' ': w = w.Substring(i + 1); goto g1;
					case '(':
						if(w[i - 1] != 'm') goto ge;
						for(j = ++i; j < w.Length; j++) if(w[j] == ')') break;
						if(j >= w.Length || j == i) goto ge;
						split = new string[] { w.Substring(i, j - i) };
						i = j;
						break;
					default: goto ge;
					}
				}
				ge:
				throw new ArgumentException("Invalid \"**options \" in wildcard expression.");
				g1:
				switch(_type) {
				case WildType.RegexNet:
					var ro = _ignoreCase ? (RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) : RegexOptions.CultureInvariant;
					_obj = new Regex(w, ro);
					return;
				case WildType.RegexPcre:
					_obj = new Regex_(w);
					return;
				case WildType.Multi:
					var a = w.Split(split ?? _splitMulti, StringSplitOptions.None);
					var multi = new Wildex[a.Length];
					for(int i = 0; i < a.Length; i++) multi[i] = new Wildex(a[i]);
					_obj = multi;
					return;
				}
			}

			if(_type == WildType.Wildcard && !HasWildcards(w)) _type = WildType.Text;
			_obj = w;
		}
		static string[] _splitMulti = new string[] { "||" };

		/// <summary>
		/// Creates new Wildex from wildcard expression string.
		/// If the string is null, returns null.
		/// </summary>
		/// <param name="wildcardExpression">
		/// <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">Wildcard expression</conceptualLink>.
		/// </param>
		public static implicit operator Wildex(string wildcardExpression)
		{
			if(wildcardExpression == null) return null;

			//rejected. It's job for code tools. This would be used mostly for 'name' parameter of Wnd.Find and Wnd.Child, where 'match any' is rare; but 'match any' is very often used for 'className' and 'program' parameters, where "" causes exception, so they will quickly learn.
			///// If the string is "", calls <see cref="PrintWarning"/>. To match "", use "**empty" instead.
			//	if(wildcardExpression.Length == 0) PrintWarning("To match \"\", better use \"**empty\". To match any, use null, or omit the argument if it's optional.");

			return new Wildex(wildcardExpression);
		}

		/// <summary>
		/// Compares a string with the <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink> used to create this <see cref="Wildex"/>.
		/// Returns true if they match.
		/// </summary>
		/// <param name="s">String. If null, returns false. If "", returns true if it was "" or "*" or a regular expression that matches "".</param>
		public bool Match(string s)
		{
			if(s == null) return false;

			bool R = false;
			switch(_type) {
			case WildType.Wildcard:
				R = s.Like_(_obj as string, _ignoreCase);
				break;
			case WildType.Text:
				var t = _obj as string;
				R = s.Equals_(t, _ignoreCase);
				break;
			case WildType.RegexPcre:
				R = (_obj as Regex_).IsMatch(s);
				break;
			case WildType.RegexNet:
				R = (_obj as Regex).IsMatch(s);
				break;
			case WildType.Multi:
				var multi = _obj as Wildex[];
				//[n] parts: all must match (with their option n applied)
				int nNot = 0;
				for(int i = 0; i < multi.Length; i++) {
					var v = multi[i];
					if(v.Not) {
						if(!v.Match(s)) return _not; //!v.Match(s) means 'matches if without option n applied'
						nNot++;
					}
				}
				if(nNot == multi.Length) return !_not; //there are no parts without option n

				//non-[n] parts: at least one must match
				for(int i = 0; i < multi.Length; i++) {
					var v = multi[i];
					if(!v.Not && v.Match(s)) return !_not;
				}
				break;
			}
			return R ^ _not;
		}

		/// <summary>
		/// The type of text (wildcard expression) used when creating the Wildex variable.
		/// </summary>
		public enum WildType :byte
		{
			/// <summary>
			/// Simple text (option t, or no *? characters and no t r R options).
			/// <b>Match</b> calls <see cref="String_.Equals_(string, string, bool)"/>.
			/// </summary>
			Text,

			/// <summary>
			/// Wildcard (has *? characters and no t r R options).
			/// <b>Match</b> calls <see cref="String_.Like_(string, string, bool)"/>.
			/// </summary>
			Wildcard,

			/// <summary>
			/// PCRE regular expression (option r).
			/// <b>Match</b> calls <see cref="Regex_.IsMatch"/>.
			/// </summary>
			RegexPcre,

			/// <summary>
			/// .NET egular expression (option R).
			/// <b>Match</b> calls <see cref="Regex.IsMatch(string)"/>.
			/// </summary>
			RegexNet,

			/// <summary>
			/// Multiple parts (option m).
			/// <b>Match</b> calls <b>Match</b> for each part (see <see cref="MultiArray"/>) and returns true if all negative (option n) parts return true (or there are no such parts) and some positive (no option n) part returns true (or there are no such parts).
			/// If you want to implement a different logic, call <b>Match</b> for each <see cref="MultiArray"/> element (instead of calling <b>Match</b> for this variable).
			/// </summary>
			Multi,
		}

		/// <summary>
		/// Gets the wildcard or simple text.
		/// null if TextType is Regex or Multi.
		/// </summary>
		public string Text => _obj as string;

		/// <summary>
		/// Gets the Regex object created from regular expression string.
		/// null if TextType is not RegexPcre (no option r).
		/// </summary>
		public Regex RegexPcre => _obj as Regex;

		/// <summary>
		/// Gets the Regex object created from regular expression string.
		/// null if TextType is not RegexNet (no option R).
		/// </summary>
		public Regex RegexNet => _obj as Regex;

		/// <summary>
		/// Array of Wildex variables, one for each part in multi-part text.
		/// null if TextType is not Multi (no option m).
		/// </summary>
		public Wildex[] MultiArray => _obj as Wildex[];

		/// <summary>
		/// Gets the type of text (wildcard, regex, etc).
		/// </summary>
		public WildType TextType => _type;

		/// <summary>
		/// Is case-insensitive?
		/// </summary>
		public bool IgnoreCase => _ignoreCase;

		/// <summary>
		/// Has option n?
		/// </summary>
		public bool Not => _not;

		///
		public override string ToString()
		{
			return _obj.ToString();
		}

		/// <summary>
		/// Returns true if string contains wildcard characters: '*', '?'.
		/// </summary>
		/// <param name="s">Can be null.</param>
		public static bool HasWildcards(string s) //CONSIDER: IsWildcard
		{
			if(s == null) return false;
			for(int i = 0; i < s.Length; i++) {
				var c = s[i];
				if(c == '*' || c == '?') goto yes;
			}
			return false; yes: return true;
		}
	}

	//rejected: struct WildexStruct - struct version of Wildex class. Moved to the Unused project.
	//	Does not make faster, although in most cases creates less garbage.
}
