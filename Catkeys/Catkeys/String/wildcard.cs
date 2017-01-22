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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	public static partial class String_
	{
		#region Like_

		/// <summary>
		/// <para>
		/// Compares this string with a string that possibly contains wildcard characters.<br/>
		/// Returns true if the strings match.
		/// </para>
		/// <para>
		/// Wildcard characters:<br/>
		/// * - zero or more of any characters.<br/>
		/// ? - any character.
		/// </para>
		/// </summary>
		/// <param name="t">This string. If null, returns true if pattern is null. If "", returns true if pattern is "" or "*".</param>
		/// <param name="pattern">String that possibly contains wildcard characters. If null, returns true if this string is null. If "", returns true if this string is "". If "*", always returns true except when this string is null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <remarks>
		/// <para>Like all String_ functions, performs ordinal comparison, ie does not depend on current culture.</para>
		/// <para>This function is fast, not much slower than Equals_, EndsWith_, IndexOf_ etc, even when ignoreCase is true.</para>
		/// <para>Similar .NET function Microsoft.VisualBasic.CompilerServices.Operators.LikeString() supports more wildcard characters etc, depends on current culture, is 6-250 times slower (case-insensitive is even much slower than Regex), has bugs.</para>
		/// </remarks>
		/// <seealso cref="Wildex"/>
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
		public static unsafe bool Like_(this string t, string pattern, bool ignoreCase = false)
		{
			if(pattern == null) return t == null;
			if(t == null) return false;
			int patLen = pattern.Length;
			if(patLen == 0) return t.Length == 0;
			if(patLen == 1 && pattern[0] == '*') return true;
			if(t.Length == 0) return false;

			fixed (char* str = t, pat = pattern) {
				return __WildcardCmp(str, pat, t.Length, patLen, ignoreCase ? Util.LibProcessMemory.Ptr->str.GetCaseTable() : null);
			}
		}

		static unsafe bool __WildcardCmp(char* s, char* w, int lenS, int lenW, char* table)
		{
			char* se = s + lenS, we = w + lenW;

			//find '*' from start. Makes faster in some cases.
			for(; (w < we && s < se); w++, s++) {
				char cS = s[0], cW = w[0];
				if(cW == '*') goto g1;
				if(cW == cS || cW == '?') continue;
				if((table == null) || (table[cW] != table[cS])) return false;
			}
			if(w == we) return s == se; //p ended?
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
			//	Could not use "**options|text" for wildcard expressions.
			//	Could use other escape sequences, eg [*], [?] and [[], but it makes slower and is more harmful than useful.

			//The first two loops are fast, but Equals_ much faster when !ignoreCase. We cannot use such optimizations that it can.
			//The slowest case is "*substring*", because then the first two loops don't help.
			//	Then similar speed as string.IndexOf(ordinal) and Api.FindStringOrdinal.
			//	Possible optimization, but need to add much code, and makes not much faster, and makes other cases slower, difficult to avoid it.
		}

		/// <summary>
		/// Calls <see cref="Like_(string, string, bool)"/>  for each wildcard pattern specified in the argument list until one matches this string.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		public static int Like_(this string t, bool ignoreCase = false, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.Like_(patterns[i], ignoreCase)) return i + 1;
			return 0;
		}

		internal unsafe struct ProcessVariables
		{
			char* _caseTable; //char[0x10000] containing upper-case versions of the first 0x10000 characters

			internal char* GetCaseTable()
			{
				if(_caseTable == null) {
					var t = (char*)Api.VirtualAlloc(Zero, 0x20000); //faster than Marshal.AllocHGlobal when need big memory, especially when need to zero it
					if(t == null) throw new OutOfMemoryException();
					for(int i = 0; i < 0x10000; i++) t[i] = (char)i;
					Api.CharLowerBuff(t, 0x10000);
					if(_caseTable == null) _caseTable = t; else Api.VirtualFree((IntPtr)t); //another thread can outrun us
				} //speed: 350
				return _caseTable;
			}
		}

		#endregion Like_
	}

	/// <summary>
	/// <para>A Catkeys wildcard expression is a text string that can be compared with other string as wildcard text, regular expression, multi-part, etc.
	/// Like a regular expression, but in most cases easier to use, faster and more lightweight.
	/// Typically used for string parameters of find-item functions, like <see cref="Wnd.Find">Wnd.Find</see>.
	/// </para>
	/// <h3>Wildcard expression reference</h3>
	/// <para>
	/// By default case-insensitive. Always culture-insensitive.<br/>
	/// By default, if contains *? characters, is compared as wildcard pattern (see <see cref="String_.Like_(string, string, bool)">Like_</see>), else as simple text.
	/// </para>
	/// <para>
	/// Wildcard characters:<br/>
	/// * - zero or more of any characters.<br/>
	/// ? - any character.
	/// </para>
	/// <para>Can start with **options|, like <c>"**tc|text"</c>. Options:</para>
	/// <list type="bullet">
	/// <item>t - simple text, not wildcard. Example: <c>"**t|text"</c></item>
	/// <item>r - regular expression. See <see cref="System.Text.RegularExpressions.Regex"/>.</item>
	/// <item>c - case-sensitive. Example: <c>"**rc|regex, match case"</c>. Info: case-sensitive regular expression is about 4 times faster.</item>
	/// <item>n - must not match.</item>
	/// <item>m - multi-part. Use separator []. Example: <c>"**m|findThis[]orThis[]**r|orThisRegex[]**n|butNotThis[]**nr|andNotThisRegex".</c> Example 2: <c>"**mn|notThis[]andNotThis"</c>.</item>
	///	</list>
	/// <para>
	/// Pattern "" matches only "". Pattern null usually means 'match any'.
	/// Tip: to avoid many nulls in code, you can omit optional parameters, for example instaed of <c>Wnd.Find(null, null, "notepad")</c> use <c>Wnd.Find(program:"notepad")</c>.
	/// </para>
	/// <para>Exception <see cref="ArgumentException"/> if invalid **options| or regular expression.</para>
	/// <para>Example for users of such find-item functions.</para>
	/// <code><![CDATA[
	/// //Find item whose name is "example" (case-insensitive), date starts with "2017-" and some third property matches a case-sensitive regular expression.
	/// var item = x.FindItem("example", "2017-*", "**rc|regex");
	/// ]]></code>
	/// <para>That's all. The information below is only for authors of such find-item functions.</para>
	/// </summary>
	/// <remarks>
	/// The Wildex class implements wildcard expression parsing and matching (comparing).
	/// An instance of this class contains a wildcard or simple text string, or a Regex object, or a Wildex[] array.
	/// You use it like Regex. Create a Wildex instance, then call its Match method for each item text when searching for some item.<br/>
	/// </remarks>
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
		/// String (<see cref="Wildex">wildcard expression</see>) to compare with other strings when calling <see cref="Match"/>.<br/>
		/// null will match null. "" will match "".
		/// </param>
		/// <exception cref="ArgumentException">Invalid **options| or regular expression.</exception>
		public Wildex(string wildcardExpression)
		{
			var w = wildcardExpression;
			if(w == null) return;
			_type = WildType.Wildcard;
			_ignoreCase = true;

			if(w.Length >= 3 && w[0] == '*' && w[1] == '*') {
				for(int i = 2; i < w.Length; i++) {
					switch(w[i]) {
					case 't': _type = WildType.Text; break;
					case 'r': _type = WildType.Regex; break;
					case 'm': _type = WildType.Multi; break;
					case 'c': _ignoreCase = false; break;
					case 'n': _not = true; break;
					case '|': w = w.Substring(i + 1); goto g1;
					default: goto ge;
					}
				}
				ge:
				throw new ArgumentException("invalid **options|");
				g1:
				switch(_type) {
				case WildType.Regex:
					var ro = _ignoreCase ? (RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) : RegexOptions.CultureInvariant;
					_obj = new Regex(w, ro);
					return;
				case WildType.Multi:
					var a = w.Split(new string[] { "[]" }, StringSplitOptions.None);
					var multi = new Wildex[a.Length];
					for(int i = 0; i < a.Length; i++) multi[i] = new Wildex(a[i]);
					_obj = multi;
					return;
				}
			}

			if(_type == WildType.Wildcard && !Wildex.HasWildcards(w)) _type = WildType.Text;
			_obj = w;
		}

		/// <summary>
		/// Creates new Wildex from wildcard expression string.
		/// If the string is null, returns null, else creates and returns new Wildex.
		/// </summary>
		/// <param name="wildcardExpression">
		/// String (<see cref="Wildex">wildcard expression</see>) to compare with other strings when calling <see cref="Match"/>.<br/>
		/// </param>
		public static implicit operator Wildex(string wildcardExpression) { return (wildcardExpression == null) ? null : new Wildex(wildcardExpression); }

		/// <summary>
		/// Compares a string with the string (<see cref="Wildex">wildcard expression</see>) used to create this <see cref="Wildex"/>.<br/>
		/// Returns true if they match.<br/>
		/// </summary>
		/// <param name="s">String. If null, returns true if wildcardExpression was null. If "", returns true if it was "" or "*" or a regular expression that matches "".</param>
		public bool Match(string s)
		{
			if(s == null) return _obj == null;
			if(_obj == null) return false;

			bool R = false;
			switch(_type) {
			case WildType.Wildcard: R = s.Like_(_obj as string, _ignoreCase); break;
			case WildType.Text: R = s.Equals_(_obj as string, _ignoreCase); break;
			case WildType.Regex: R = (_obj as Regex).IsMatch(s); break;
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
			/// <summary>Simple text (option t, or no *? characters and no t r options). Match() calls <see cref="String_.Equals_(string, string, bool)"/>.</summary>
			Text,
			/// <summary>Wildcard (has *? characters and no t r options). Match() calls <see cref="String_.Like_(string, string, bool)"/>.</summary>
			Wildcard,
			/// <summary>Regular expression (option r). Match() calls <see cref="Regex.IsMatch(string)"/>.</summary>
			Regex,
			/// <summary>Multiple parts (option m). Match() calls Match() for each part (see <see cref="MultiArray"/>) and returns true if all negative (option n) parts return true (or there are no such parts) and some positive (no option n) part returns true (or there are no such parts).</summary>
			Multi,
		}

		/// <summary>
		/// Gets the wildcard or simple text.<br/>
		/// null if TextType is Regex or Multi.
		/// </summary>
		public string Text { get { return _obj as string; } }

		/// <summary>
		/// Gets the Regex object created from regular expression string.<br/>
		/// null if TextType is not Regex (no option r).
		/// </summary>
		public Regex Regex { get { return _obj as Regex; } }

		/// <summary>
		/// Array of Wildex variables, one for each part in multi-part text.<br/>
		/// null if TextType is not Multi (no option m).
		/// </summary>
		public Wildex[] MultiArray { get { return _obj as Wildex[]; } }

		/// <summary>
		/// Gets the type of text (wildcard, regex, etc).
		/// </summary>
		public WildType TextType { get { return _type; } }

		/// <summary>
		/// Is case-insensitive?
		/// </summary>
		public bool IgnoreCase { get { return _ignoreCase; } }

		/// <summary>
		/// Has option n?
		/// </summary>
		public bool Not { get { return _not; } }

		///
		public override string ToString()
		{
			return _obj.ToString();
		}

		/// <summary>
		/// Returns true if string contains wildcard characters: '*', '?'.
		/// </summary>
		/// <param name="s"></param>
		public static bool HasWildcards(string s)
		{
			return s != null && s.IndexOfAny(_wildcardChars) >= 0;
		}
		static readonly char[] _wildcardChars = new char[] { '*', '?' };
	}
}
