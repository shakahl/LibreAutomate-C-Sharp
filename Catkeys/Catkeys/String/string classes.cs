using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Text type for WildString class.
	/// </summary>
	public enum WildStringType
	{
		Wildcard, Regex, Full, Part,
	}

	/// <summary>
	/// WildString.WildcardType property.
	/// </summary>
	public enum WildcardType
	{
		Any, //pattern is "*"
		AnyNotEmpty, //pattern is "?*"
		Empty, //match empty string (pattern is "" or null with the main constructor, or "[]" with the cast string->WildStrin operator)
		Literal, //no wildcard chars in pattern, use Equals_
		StartsWith, //pattern is "literal*", use StartsWith_
		EndsWith, //pattern is "*literal", use EndsWith_
		Contains, //pattern is "*literal*", use IndexOf_
		StartsEnds, //pattern is "lite*ral", use Compare_
		Complex, //complex pattern, use LikeEx_
	}

	/// <summary>
	/// Contains a string to find.
	/// The string can be interpreted as wildcard (default), regular expression, or simple full or partial text.
	/// Used for parameters of 'find object' functions. The functions then call the Match() method.
	/// Accepts values (arguments) of type WildString, Regex and string.
	/// <para>
	/// If the type is string:
	///		The text by default is interpreted as wildcard, match case.
	///		Can start with "[options]", where options can contain a type character and one or more option characters.
	///		Type: w - wildcard (default), r - regular expression, f - full text, p - partial text.
	///		Options: i - case-insensitive, c - use current culture, n - text must not match or is text that must match followed by !![options]text that must not match (see option n examples).
	///		Example: "[fi]full text, ignore case".
	///		Text "[]" means 'match only empty text'.
	///		Text null or "" or "*" means 'match any text'; then the cast string->WildString operator returns null.
	///		Tip: to match any of several strings, use regular expression, like @"[r]^text1|text2|text3$", and escape special characters.
	///		Option n example 1: "[pn]partial text that must match!!wildcard text that must not match".
	///		Option n example 2: "[rn]regex that must match!![fi]full text that must not match, ignore case".
	///		Option n example 3: "[fn]full text that must not match".
	/// </para>
	/// <para>
	/// Wildcard characters:
	///   * - 0 or more characters.
	///   ? - 1 character.
	///   ?* - 1 or more characters.
	///   ** - literal *.
	///   *? - literal ?.
	/// </para>
	/// </summary>
	public class WildString
	{
		string _text; //simple text or wildcard. null = 'match any', "" = 'match empty'.
		Regex _regex;
		WildStringType _type; //wildcard, regex, full, partial
		WildcardType _wildType; //wildcard pattern type
		int _wildAsteriskPos; //* index in _text when text*text
		StringComparison _stringComp;

		/// <summary>
		/// Gets the 'text' argument. Wildcard pattern can be modified.
		/// null if regular expression.
		/// </summary>
		public string Text { get { return _text; } }
		/// <summary>
		/// Gets the 'rx' argument or the internal Regex object created from regular expression string.
		/// null if not regular expression.
		/// </summary>
		public Regex RegexObject { get { return _regex; } }
		/// <summary>
		/// Gets text type (wildcard, regex, full, part).
		/// </summary>
		public WildStringType StringType { get { return _type; } }
		/// <summary>
		/// Gets wildcard type (starts with, ends with, complex, etc).
		/// </summary>
		public WildcardType WildcardType { get { return _wildType; } }
		/// <summary>
		/// Gets the 'ignoreCase' argument.
		/// </summary>
		public bool IgnoreCase { get; private set; }
		/// <summary>
		/// Gets the 'useCurrentCulture' argument.
		/// </summary>
		public bool UseCurrentCulture { get; private set; }
		/// <summary>
		/// Gets the 'not' argument or sets a new WildString object that must not match.
		/// </summary>
		public WildString Not { get; set; }
		/// <summary>
		/// Returns true if would match any string, ie wildcard "*" and no 'not' and no Regex.
		/// </summary>
		public bool MatchAny { get { return _text == null && _regex == null && Not == null; } }

		//public WildString() { }

		#region wildcard

		/// <summary>
		/// Scans the wildcard pattern and detects its type (as enum WildcardType).
		/// Then _WildcardMatch() will use the type to faster compare with these often used patterns: "text", "text*", "*text", "*text*", "text*text", "", "*", "?*".
		/// For these patterns it calls fast string functions: Equals, StartsWith, EndsWith, IndexOf, Compare.
		/// For more complex patterns calls String.LikeEx_, which is slower (especially when ignoreCase is true); this function escapes # and [ characters.
		/// </summary>
		void _WildcardInit(string pattern)
		{
			Debug.Assert(!Empty(pattern));

			string p = pattern;
			int i = 0, n = p.Length;
			_text = p;

			if(n == 1) {
				if(p[0] == '*') { Debug.Assert(WildcardType.Any == default(WildcardType)); _text = null; return; }
			} else if(n == 2) {
				if(p[0] == '?' && p[1] == '*') { _wildType = WildcardType.AnyNotEmpty; return; }
			}

			int f = 0; //0 no *, 1 *pattern, 2 pattern*, 3 *pattern*, |4 pat#[tern, |8 pat?tern, 16 pat*tern, 32 ** or *?
			int astPos = 0;

			for(; i < n; i++) {
				switch(p[i]) {
				case '*':
					if(i == n - 1) f |= 2;
					else if(p[i + 1] == '*' || p[i + 1] == '?') f |= 32;
					else if(i == 0) f |= 1;
					else { f |= 16; astPos = (astPos == 0) ? i : -1; }
					break;
				case '?':
					f |= 8;
					break;
				case '#':
				case '[':
					f |= 4;
					break;
				}
			}

			if(f >= 4) {
				if(astPos > 0 && (f & (1 | 2 | 8 | 16 | 32)) == 16) { //single * in middle, and no ?
					_wildType = WildcardType.StartsEnds;
					_wildAsteriskPos = astPos;
				} else { //need LikeEx_ (? or multiple * or ** or *?)
					_wildType = WildcardType.Complex;
					if((f & 4) != 0) _text = _text.Replace("[", "[[]").Replace("#", "[#]"); //we don't support them
					if((f & 32) != 0) _text = _text.Replace("**", "[*]").Replace("*?", "[?]"); //escape * and ?
					if(IgnoreCase) _text = UseCurrentCulture ? _text.ToUpper() : _text.ToUpperInvariant(); //LikeEx_ with ignoreCase=true is ~10 times slower; this makes 2-10 times faster. This does not help: Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
				}
			} else {
				switch(f) {
				case 0: _type = WildStringType.Full; break;
				case 1: _wildType = WildcardType.EndsWith; _text = p.Substring(1); break;
				case 2: _wildType = WildcardType.StartsWith; _text = p.Remove(n - 1); break;
				case 3: _type = WildStringType.Part; _text = p.Substring(1, n - 2); break;
				}
			}
		}

		bool _WildcardMatch(string s)
		{
			Debug.Assert(s != null); //if null, Match returns false or makes ""
			Debug.Assert(_text != null && _wildType != WildcardType.Any); //if Any, _text is null and _Match returns true

			if(s.Length == 0) return _wildType == WildcardType.Empty;

			switch(_wildType) {
			//case WildcardType.Literal: return s.Equals(_text, _stringComp); //handled by _Match, as well as Contains and Any
			//case WildcardType.Contains: return s.IndexOf(_text, _stringComp) >= 0;
			case WildcardType.EndsWith: return s.EndsWith(_text, _stringComp);
			case WildcardType.StartsWith: return s.StartsWith(_text, _stringComp);
			case WildcardType.StartsEnds: return _WildcardStartsEnds(s);
			case WildcardType.Complex:
				if(IgnoreCase) s = UseCurrentCulture ? s.ToUpper() : s.ToUpperInvariant();
				return s.LikeEx_(_text, false);
			case WildcardType.Empty: return false;
			default: Debug.Assert(_wildType == WildcardType.AnyNotEmpty); return true;
			}
		}

		bool _WildcardStartsEnds(string s)
		{
			int lens = s.Length, lenp = _text.Length; if(lens < lenp - 1) return false;
			if(string.Compare(s, 0, _text, 0, _wildAsteriskPos, _stringComp) != 0) return false;
			int pos = _wildAsteriskPos + 1, lene = lenp - pos;
			return string.Compare(s, lens - lene, _text, pos, lene, _stringComp) == 0;
		}

		/// <summary>
		/// Returns true if string contains wildcard characters: '*', '?'.
		/// </summary>
		public static bool HasWildcards(string s)
		{
			return s != null && s.IndexOfAny(_wildcardChars) >= 0;
		}
		static readonly char[] _wildcardChars = new char[] { '*', '?' };

		#endregion

		void _Init(string text, WildStringType textType, bool ignoreCase, bool useCurrentCulture, WildString not)
		{
			Not = not;
			if(Empty(text)) { _type = WildStringType.Full; _text = ""; return; } //match only empty text

			_type = textType;
			IgnoreCase = ignoreCase;
			UseCurrentCulture = useCurrentCulture;
			if(useCurrentCulture) _stringComp = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
			else _stringComp = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

			switch(textType) {
			case WildStringType.Wildcard:
				_WildcardInit(text);
				break;
			case WildStringType.Regex:
				_regex = new Regex(text, (ignoreCase ? RegexOptions.IgnoreCase : 0) | (useCurrentCulture ? 0 : RegexOptions.CultureInvariant));
				break;
			default:
				_text = text;
				break;
			}
		}

		/// <summary>
		/// Creates WildString object of specified type.
		/// </summary>
		/// <param name="text">
		/// Text. Depending on textType, it will be interpreted as wildcard, regex, or simple full or partial text.
		/// "" or null means 'match only empty text'.
		/// </param>
		/// <param name="textType">Text type (wildcard, regex etc).</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="useCurrentCulture">Use current culture. If false, uses ordinal matching which does not depend on a culture; for Regex then uses RegexOptions.CultureInvariant.</param>
		/// <param name="not">Another WildString object that specifies text that must not match.</param>
		public WildString(string text, WildStringType textType, bool ignoreCase = false, bool useCurrentCulture = false, WildString not = null)
		{
			_Init(text, textType, ignoreCase, useCurrentCulture, not);
		}

		/// <summary>
		/// Creates WildString object of WildStringType.Regexp type from a Regexp object.
		/// </summary>
		/// <param name="rx">A Regexp object (compiled regular expression).</param>
		/// <param name="not">Another WildString object that specifies text that must not match.</param>
		public WildString(Regex rx, WildString not = null)
		{
			Not = not;
			if(rx == null) return;
			_regex = rx;
			_type = WildStringType.Regex;
			IgnoreCase = rx.Options.HasFlag(RegexOptions.IgnoreCase);
			UseCurrentCulture = !rx.Options.HasFlag(RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Creates WildString object from string that can be like "[options]text" where options specifies text type and/or options.
		/// More info in WildString class help.
		/// </summary>
		/// <param name="text">
		/// Text. Depending on w r f p characters specified in [options], it will be interpreted as wildcard (default), regex, or simple full or partial text.
		/// "" or null or "[]" means 'match only empty text'.
		/// </param>
		/// <param name="ignoreCase">Case-insensitive by default. Also you can use option i to make case-insensitive or -i to make case-sensitive.</param>
		/// <param name="useCurrentCulture">Use current culture by default. Also you can use option c to use current culture or -c to not use.</param>
		/// <param name="not">Another WildString object that specifies text that must not match. Not used if option n specified.</param>
		public WildString(string text, bool ignoreCase = false, bool useCurrentCulture = false, WildString not = null)
		{
			if(!Empty(text) && text[0] == '[') { //parse options: "[wrfpio]text"
				WildStringType t = WildStringType.Wildcard; bool ic = ignoreCase, cult = false, n = false;
				for(int i = 1; i < text.Length; i++) {
					switch(text[i]) {
					case 'w': t = WildStringType.Wildcard; break;
					case 'r': t = WildStringType.Regex; break;
					case 'f': t = WildStringType.Full; break;
					case 'p': t = WildStringType.Part; break;
					case '-': continue;
					case 'i': ic = text[i - 1] != '-'; break;
					case 'c': cult = text[i - 1] != '-'; break;
					case 'n': n = true; break;
					case ']':
						text = text.Substring(i + 1);
						if(n) {
							i = text.IndexOf_("!!");
                            if(i < 0) {
								not = new WildString(text, t, ic, cult);
								text = "*"; t = WildStringType.Wildcard;
							} else {
								not = new WildString(text.Substring(i + 2), ignoreCase, useCurrentCulture);
								text = text.Remove(i);
							}
						}
						_Init(text, t, ic, cult, not);
						return;
					default: goto g1; //invalid [options]
					}
				}
			}
			g1:
			_Init(text, WildStringType.Wildcard, ignoreCase, useCurrentCulture, not);
		}

		static WildString _CreateFromString(string text)
		{
			if(Empty(text) || (text.Length == 1 && text[0] == '*')) return null; //match any
			return new WildString(text);
		}

		/// <summary>
		/// Implicit cast string->WildString.
		/// If text is null, "" or "*", returns null.
		/// Else creates and returns new WildString object. Creates it with the "[options]text" constructor.
		/// </summary>
		public static implicit operator WildString(string text) { return _CreateFromString(text); }
		public static implicit operator WildString(Regex rx) { return new WildString(rx); }

		public override string ToString()
		{
			if(_regex != null) return _regex.ToString();
			return _text;
		}

		#region Match

		bool _Match(string s)
		{
			if(_text == null && _regex == null) return true; //match any

			switch(_type) {
			case WildStringType.Full: return s.Equals(_text, _stringComp);
			case WildStringType.Part: return s.IndexOf(_text, _stringComp) >= 0;
			case WildStringType.Wildcard: return _WildcardMatch(s);
			case WildStringType.Regex: return _regex.IsMatch(s);
			}
			return false;
		}

		/// <summary>
		/// Returns true if string s matches the text/wildcard/regex specified in constructor and does not match the 'not' object (if used).
		/// </summary>
		/// <param name="s">A string.</param>
		/// <param name="canMatchNull">If false, always returns false if s is null, even if the pattern would match an empty string. If true, null is the same as "".</param>
		public bool Match(string s, bool canMatchNull = false)
		{
			if(s == null) { if(!canMatchNull) return false; s = ""; }
			if(!_Match(s)) return false;
			if(Not != null && Not._Match(s)) return false;
			return true;
		}

		#endregion
	}


	/// <summary>
	/// The same as WildString but case-insensitive by default.
	/// </summary>
	public class WildStringI :WildString
	{
		//public WildStringI() { }

		/// <summary>
		/// Creates WildStringI object of specified type.
		/// Calls WildString constructor WildString(string, WildStringType, bool, bool, WildString).
		/// </summary>
		public WildStringI(string text, WildStringType textType, bool ignoreCase = true, bool useCurrentCulture = false, WildString not = null)
			: base(text, textType, ignoreCase, useCurrentCulture, not)
		{ }

		/// <summary>
		/// Creates WildStringI object of WildStringType.Regexp type from a Regexp object.
		/// Calls WildString constructor WildString(Regexp, WildString).
		/// </summary>
		public WildStringI(Regex rx, WildString not = null)
			: base(rx, not)
		{ }

		/// <summary>
		/// Creates WildStringI object from string that can be like "[options]text" where options specifies text type and/or options.
		/// Calls WildString constructor WildString(string, bool, bool, WildString).
		/// More info in WildString class help and WildString constructor help.
		/// </summary>
		public WildStringI(string text, bool ignoreCase = true, bool useCurrentCulture = false, WildString not = null)
			: base(text, ignoreCase, useCurrentCulture, not)
		{ }

		static WildStringI _CreateFromString(string text)
		{
			if(Empty(text) || (text.Length == 1 && text[0] == '*')) return null; //match any
			return new WildStringI(text);
		}

		/// <summary>
		/// Implicit cast string->WildStringI.
		/// If text is null, "" or "*", returns null.
		/// Else creates and returns new WildStringI object. Initializes the base WildString with its "[options]text" constructor, ignoreCase=true.
		/// </summary>
		public static implicit operator WildStringI(string text) { return _CreateFromString(text); }
		public static implicit operator WildStringI(Regex rx) { return new WildStringI(rx); }
	}

}
