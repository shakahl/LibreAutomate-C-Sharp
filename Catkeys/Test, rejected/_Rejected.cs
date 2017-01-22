//Classes and functions that were (almost) finished, but rejected for some reason. Maybe still can be useful in the future.
//For example, when tried to make faster/better than existing .NET classes/functions, but the result was not fast/good enough.



/// <summary>
/// This function together with <see cref="OptimizedMatch"/> can be used for code speed/memory optimization, when you want to have Wildex features in your find-item function but want to avoid creating new Wildex when not necessary.
/// If wildcardExpression starts with "**options|", creates and returns new Wildex. Else returns wildcardExpression.
/// Then you can pass the returned object to OptimizedMatch, which calls <see cref="Match"/> if Wildex, or <see cref="String_.Like_(string, string, bool)"/> if string.
/// Returns null if wildcardExpression is null.
/// </summary>
/// <param name="wildcardExpression">
/// String (<see cref="Wildex">wildcard expression</see>) to compare with other strings when calling OptimizedMatch.<br/>
/// </param>
public static object OptimizedCreate(string wildcardExpression)
{
	var w = wildcardExpression;
	if(w == null) return null;
	if(w.Length < 3 || !(w[0] == '*' && w[1] == '*')) return wildcardExpression;
	return new Wildex(w);
}

/// <summary>
/// Compares a string with object (string or Wildex) created by <see cref="OptimizedCreate"/>.<br/>
/// Returns true if they match.<br/>
/// Calls <see cref="Match"/> if the object is Wildex, or <see cref="String_.Like_(string, string, bool)"/> (ignoreCase true) if string.
/// </summary>
/// <param name="o">Object created by OptimizedCreate.</param>
/// <param name="s">String. If null, returns true if o is null. If "", returns true if o is "" or "*" or a regular expression that matches "".</param>
public static bool OptimizedMatch(object o, string s)
{
	if(o == null) return s == null;
	if(s == null) return false;
	var os = o as string;
	if(os != null) return s.Like_(os, true);
	return ((Wildex)o).Match(s);
}


#if false
	/// <summary>
	/// Text type for WildString class.
	/// </summary>
	public enum WildStringType
	{
		/// <summary>String possibly containing wildcard characters. Default.</summary>
		Wildcard,
		/// <summary>Regular expression.</summary>
		Regex,
		/// <summary>Full string, no special characters.</summary>
		Full,
		/// <summary>Partial string, no special characters.</summary>
		Part,
	}

	/// <summary>
	/// WildString.WildcardType property.
	/// </summary>
	public enum WildcardType
	{
		/// <summary>pattern is "*"</summary>
		Any,
		/// <summary>pattern is "?*"</summary>
		AnyNotEmpty,
		/// <summary>match empty string (pattern is "" or null with the main constructor, or "[]" with the cast string->WildString operator)</summary>
		Empty,
		/// <summary>no wildcard chars in pattern, use Equals_</summary>
		Literal,
		/// <summary>pattern is "literal*", use StartsWith_</summary>
		StartsWith,
		/// <summary>pattern is "*literal", use EndsWith_</summary>
		EndsWith,
		/// <summary>pattern is "*literal*", use IndexOf_</summary>
		Contains,
		/// <summary>pattern is "lite*ral", use Compare_</summary>
		StartsEnds,
		/// <summary>complex pattern, use LikeEx_</summary>
		Complex,
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
	///		Options: i - case-insensitive, c - use current culture, n - text must not match (or is "text!![options]nottext" where nottext is text that must not match; see option n examples).
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
				return s.Like_(_text, false);
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
		/// Creates WildString object of WildStringType.Regex type from a Regex object.
		/// </summary>
		/// <param name="rx">A Regex object (compiled regular expression).</param>
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
		/// <summary>
		/// Implicit cast Regex->WildString.
		/// See <see cref="WildString(Regex, WildString)"/>.
		/// </summary>
		public static implicit operator WildString(Regex rx) { return new WildString(rx); }

		///
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
		/// Creates WildStringI object of WildStringType.Regex type from a Regex object.
		/// Calls WildString constructor <see cref="WildString(Regex, WildString)"/>.
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
		/// <summary>
		/// Implicit cast Regex->WildStringI.
		/// See <see cref="WildString(Regex, WildString)"/>.
		/// </summary>
		public static implicit operator WildStringI(Regex rx) { return new WildStringI(rx); }
	}
#endif





//for LikeEx_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
/// <summary>
/// Calls Microsoft.VisualBasic.CompilerServices.Operators.LikeString().
/// More info: https://msdn.microsoft.com/en-us/library/swf8kaxw%28v=vs.100%29.aspx
/// Wildcards: * (0 or more characters), ? (any 1 character), # (any digit), [chars], [!chars]. Escape sequences: [*], [?], [#], [[].
/// Much slower than Like_(), especially when ignoreCase is true.
/// Throws exception if pattern is invalid (eg "text[text") and in some cases even when valid.
/// </summary>
[MethodImpl(MethodImplOptions.NoInlining)] //prevent compiling Operators.LikeString (slow first time) when actually not used
public static bool LikeEx_(this string t, string pattern, bool ignoreCase = false)
{
	return Operators.LikeString(t, pattern, ignoreCase ? CompareMethod.Text : CompareMethod.Binary);
	//Much slower than our Like_().
	//Exception if pattern contains unclosed [, for example "abc[5" (must be "abc[5]" or "abc[[]5"). Also if pattern is "?*" and string is empty (or "A?*" and "A", etc).
	//Like_() (which is used eg to compare window class names etc) does not support # (number) and [charset] that are supported by LikeEx_().
	//	Usually they are too limited to be useful. It's better to keep Like_ as simple as possible, and use Regex when need something more.
	//	For example, if we support #[, users that are unaware/forget of this would use window class name like "#32770", or with [ characters, and wonder why it does not work.
	//Not tested System.Management.Automation.WildcardPattern, because assembly System.Management.Automation is not installed. It seems need to install it through NuGet.
}

/// <summary>
/// Calls LikeEx_() for each wildcard pattern specified in the argument list until one matches this string.
/// Returns 1-based index of matching pattern, or 0 if none.
/// </summary>
public static int LikeEx_(this string t, bool ignoreCase = false, params string[] patterns)
{
	for(int i = 0; i < patterns.Length; i++) if(t.LikeEx_(patterns[i], ignoreCase)) return i + 1;
	return 0;
}

/// <summary>
/// Returns true if this string matches wildcard pattern, which can contain these special characters:
///   * - 0 or more characters;
///   ? - 1 character;
///   ?* - 1 or more characters;
///   ** - literal *;
///   *? - literal ?.
/// </summary>
/// <param name="t"></param>
/// <param name="pattern"></param>
/// <param name="ignoreCase">Case-insensitive.</param>
/// <param name="useCurrentCulture">Use current culture. If false, uses ordinal matching which does not depend on a culture.</param>
/// <remarks>
/// Both "" and null match pattern "". This variable can be null because this is an extension method.
/// Uses class WildString, but does not support "[options]". You can use the class when comparing multiple strings with the same pattern, it will be faster.
/// </remarks>
public static bool Like_(this string t, string pattern, bool ignoreCase = false, bool useCurrentCulture = false)
{
	var x = new WildString(pattern, WildStringType.Wildcard, ignoreCase, useCurrentCulture);
	return x.Match(t, true);
}




//From https://www.codeproject.com/Articles/1088/Wildcard-string-compare-globbing
static unsafe bool _IsMatch_old(char* s, char* wild, int sLen, int wildLen)
{
	char* sEnd = s + sLen, wildEnd = wild + wildLen;

	//find '*' from start
	while(s < sEnd && wild < wildEnd && (*wild != '*')) {
		if((*wild != *s) && (*wild != '?')) return false;
		wild++;
		s++;
	}
	if(wild == wildEnd) return s == sEnd; //faster than if the return is in the loop

	//find '*' from end. Not necessary but makes "*wildcard" much faster.
	while(sEnd > s && wildEnd > wild && (wildEnd[-1] != '*')) {
		if((wildEnd[-1] != sEnd[-1]) && (wildEnd[-1] != '?')) return false;
		wildEnd--;
		sEnd--;
	}
	Debug.Assert(wildEnd > wild); //impossible because either the first loop found '*' or returned false or ended with s==sEnd
								  //if(wild == wildEnd) return s == sEnd;

	char* cp = null, mp = null;
	g1:
	while(s < sEnd && wild < wildEnd) {
		if(*wild == '*') {
			if(++wild >= wildEnd) return true;
			mp = wild;
			cp = s + 1;
		} else if((*wild == *s) || (*wild == '?')) {
			s++;
			wild++;
		} else {
			wild = mp;
			s = cp++;
		}
		//speed: slower if char chWild=*wild and then use chWild instead of *wild
	}
	if(wild >= wildEnd && s < sEnd) { //here much faster than in the loop
		wild = mp;
		s = cp++;
		goto g1;
	}

	while(wild < wildEnd && *wild == '*') {
		wild++;
	}
	return wild >= wildEnd;
}




#if false //initially was used by SharedMemory
	/// <summary>
	/// Manages a named kernel handle (mutex, event, memory mapping, etc).
	/// Normally calls CloseHandle when dies or is called Close.
	/// But does not call CloseHandle for the variable that uses the name first time in current process.
	/// Therefore the kernel object survives, even when the first appdomain ends.
	/// It ensures that all variables in all appdomains will use the same kernel object (although different handle to it) if they use the same name.
	/// Most CreateX API work in "create or open" way. Pass such a created-or-opened object handle to the constructor.
	/// </summary>
	[DebuggerStepThrough]
	class LibInterDomainHandle
	{
		IntPtr _h;
		bool _noClose;

		public IntPtr Handle { get { return _h; } }

		/// <param name="handle">Kernel object handle</param>
		/// <param name="name">Kernel object name. Note: this function adds local atom with that name.</param>
		public LibInterDomainHandle(IntPtr handle, string name)
		{
			_h = handle;

			if(_h != Zero && 0 == Api.FindAtom(name)) {
				Api.AddAtom(name);
				_noClose = true;
			}
		}

		~LibInterDomainHandle() { Close(); }

		public void Close()
		{
			if(_h != Zero && !_noClose) { Api.CloseHandle(_h); _h = Zero; }
		}
	}
#endif




/// <summary>
/// Memory that can be used by multiple processes.
/// Wraps Api.CreateFileMapping(), Api.MapViewOfFile().
/// Faster and more "unsafe" than System.IO.MemoryMappedFiles.MemoryMappedFile.
/// </summary>
[DebuggerStepThrough]
public unsafe class SharedMemory
{
	void* _mem;
	LibInterDomainHandle _hmap;

	/// <summary>
	/// Pointer to the base of the shared memory.
	/// </summary>
	public void* mem { get { return _mem; } }

	/// <summary>
	/// Creates shared memory of specified size. Opens if already exists.
	/// Calls Api.CreateFileMapping() and Api.MapViewOfFile().
	/// </summary>
	/// <param name="name"></param>
	/// <param name="size"></param>
	/// <exception cref="Win32Exception">When fails.</exception>
	/// <remarks>
	/// Once the memory is created, it is alive until this process (not variable or appdomain) dies, even if you call Close.
	/// All variables in all appdomains will get the same physical memory for the same name, but they will get different virtual address.
	/// </remarks>
	public SharedMemory(string name, uint size)
	{
		IntPtr hm = Api.CreateFileMapping((IntPtr)(~0), Zero, 4, 0, size, name);
		if(hm != Zero) {
			_mem = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
			if(_mem == null) Api.CloseHandle(hm); else _hmap = new LibInterDomainHandle(hm, name);
		}
		if(_mem == null) throw new Win32Exception();
		//todo: option to use SECURITY_ATTRIBUTES to allow low IL processes open the memory.
		//todo: use single handle/address for all appdomains.
		//PrintList(_hmap, _mem);
	}

	//This works but not useful.
	///// <summary>
	///// Opens shared memory.
	///// Calls Api.OpenFileMapping() and Api.MapViewOfFile().
	///// </summary>
	///// <param name="name"></param>
	///// <exception cref="Win32Exception">When fails, eg the memory does not exist.</exception>
	//public SharedMemory(string name)
	//{
	//	_hmap = Api.OpenFileMapping(0x000F001F, false, name);
	//	if(_hmap != Zero) {
	//		_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
	//	}
	//	if(_mem == Zero) throw new Win32Exception();
	//}

	~SharedMemory() { if(_mem != null) Api.UnmapViewOfFile(_mem); }

	/// <summary>
	/// Unmaps the memory.
	/// D
	/// </summary>
	public void Close()
	{
		if(_mem != null) { Api.UnmapViewOfFile(_mem); _mem = null; }
		if(_hmap != null) { _hmap.Close(); _hmap = null; }
	}
}



//Almost complete. Need just to implement screen and EndThread option. Now some used library functions deleted/moved/renamed/etc.
//Instead use TaskDialog. If need classic message box, use MessageBox.Show(). Don't need 3 functions for the same.
#region MessageDialog

/// <summary>
/// MessageDialog return value (user-clicked button).
/// </summary>
public enum MDResult
{
	OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7/*, Timeout = 9*/, TryAgain = 10, Continue = 11,
}

/// <summary>
/// MessageDialog buttons.
/// </summary>
public enum MDButtons
{
	OK = 0, OKCancel = 1, AbortRetryIgnore = 2, YesNoCancel = 3, YesNo = 4, RetryCancel = 5, CancelTryagainContinue = 6,
}

/// <summary>
/// MessageDialog icon.
/// </summary>
public enum MDIcon
{
	None = 0, Error = 0x10, Question = 0x20, Warning = 0x30, Info = 0x40, Shield = 0x50, App = 0x60,
}

/// <summary>
/// MessageDialog flags.
/// </summary>
[Flags]
public enum MDFlag :uint
{
	DefaultButton2 = 0x100, DefaultButton3 = 0x200, DefaultButton4 = 0x300,
	SystemModal = 0x1000, DisableThreadWindows = 0x2000, HelpButton = 0x4000,
	TryActivate = 0x10000, DefaultDesktopOnly = 0x20000, Topmost = 0x40000, RightAlign = 0x80000, RtlLayout = 0x100000, ServiceNotification = 0x200000,
	//not API flags
	NoSound = 0x80000000,
	//todo: EndThread.
}

public static class MessageDialog
{
	/// <summary>
	/// Shows classic message box dialog.
	/// Like System.Windows.Forms.MessageBox.Show but has more options and is always-on-top by default.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="buttons">Example: MDButtons.YesNo.</param>
	/// <param name="icon">One of standard icons. Example: MDIcon.Info.</param>
	/// <param name="flags">One or more options. Example: MDFlag.NoTopmost|MDFlag.DefaultButton2.</param>
	/// <param name="owner">Owner window or null.</param>
	/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
	/// </remarks>
	public static MDResult Show(string text, MDButtons buttons, MDIcon icon = 0, MDFlag flags = 0, IWin32Window owner = null, string title = null)
	{
		//const uint MB_SYSTEMMODAL = 0x1000; //same as MB_TOPMOST + adds system icon in title bar (why need it?)
		const uint MB_USERICON = 0x80;
		const uint IDI_APPLICATION = 32512;
		const uint IDI_ERROR = 32513;
		const uint IDI_QUESTION = 32514;
		const uint IDI_WARNING = 32515;
		const uint IDI_INFORMATION = 32516;
		const uint IDI_SHIELD = 106; //32x32 icon. The value is undocumented, but we use it instead of the documented IDI_SHIELD value which on Win7 displays clipped 128x128 icon. Tested: the function does not fail with invalid icon resource id.

		var p = new MSGBOXPARAMS();
		p.cbSize = Api.SizeOf(p);
		p.lpszCaption = _Util.Title(title);
		p.lpszText = text;

		Wnd ow =
		bool alien = (flags & (MDFlag.DefaultDesktopOnly | MDFlag.ServiceNotification)) != 0;
		if(alien) owner = Wnd0; //API would fail. The dialog is displayed in csrss process.

		if(icon == MDIcon.None) { } //no sound
		else if(icon == MDIcon.Shield || icon == MDIcon.App || flags.HasFlag(MDFlag.NoSound)) {
			switch(icon) {
			case MDIcon.Error: p.lpszIcon = (IntPtr)IDI_ERROR; break;
			case MDIcon.Question: p.lpszIcon = (IntPtr)IDI_QUESTION; break;
			case MDIcon.Warning: p.lpszIcon = (IntPtr)IDI_WARNING; break;
			case MDIcon.Info: p.lpszIcon = (IntPtr)IDI_INFORMATION; break;
			case MDIcon.Shield: p.lpszIcon = (IntPtr)IDI_SHIELD; break;
			case MDIcon.App:
				p.lpszIcon = (IntPtr)IDI_APPLICATION;
				if(Util.Misc.GetAppIconHandle(32) != Zero) p.hInstance = Util.Misc.GetModuleHandleOfAppdomainEntryAssembly();
				//info: C# compiler adds icon to the native resources as IDI_APPLICATION.
				//	If assembly without icon, we set hInstance=0 and then the API shows common app icon.
				//	In any case, it will be the icon displayed in File Explorer etc.
				break;
			}
			p.dwStyle |= MB_USERICON; //disables sound
			icon = 0;
		}

		if(Script.Option.dialogRtlLayout) flags |= MDFlag.RtlLayout;
		if(owner.Is0) {
			flags |= MDFlag.TryActivate; //if foreground lock disabled, activates, else flashes taskbar button; without this flag the dialog woud just sit behind other windows, often unnoticed.
			if(Script.Option.dialogTopmostIfNoOwner) flags |= MDFlag.SystemModal; //makes topmost, always works, but also adds an unpleasant system icon in title bar
																				  //if(Script.Option.dialogTopmostIfNoOwner) flags|=MDFlag.Topmost; //often ignored, without a clear reason and undocumented, also noticed other anomalies
		}
		//tested: if owner is child, the API disables its top-level parent.
		//consider: if owner 0, create hidden parent window to:
		//	Avoid adding taskbar icon.
		//	Apply Option.dialogScreenIfNoOwner.
		//consider: if owner 0, and current foreground window is of this thread, let it be owner. Maybe a flag.
		//consider: if owner of other thread, don't disable it. But how to do it without hook? Maybe only inherit owner's monitor.
		//consider: play user-defined sound.

		p.hwndOwner = owner;

		flags &= ~(MDFlag.NoSound); //not API flags
		p.dwStyle |= (uint)buttons | (uint)icon | (uint)flags;

		int R = MessageBoxIndirect(ref p);
		if(R == 0) throw new CatException();

		_Util.DoEventsAndWaitForAnActiveWindow();

		return (MDResult)R;

		//tested:
		//user32:MessageBoxTimeout. Undocumented. Too limited etc to be useful. If need timeout, use TaskDialog.
		//shlwapi:SHMessageBoxCheck. Too limited etc to be useful.
		//wtsapi32:WTSSendMessageW. In csrss process, no themes, etc. Has timeout.
	}

	/// <summary>
	/// Shows classic message box dialog.
	/// Returns clicked button's character (as in style), eg 'O' for OK.
	/// You can specify buttons etc in style string, which can contain:
	/// <para>Buttons: OC OKCancel, YN YesNo, YNC YesNoCancel, ARI AbortRetryIgnore, RC RetryCancel, CTE CancelTryagainContinue.</para>
	/// <para>Icon: x error, ! warning, i info, ? question, v shield, a app.</para>
	/// <para>Flags: s no sound, t topmost, d disable windows.</para>
	/// <para>Default button: 2 or 3.</para>
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="style">Example: "YN!".</param>
	/// <param name="owner">Owner window or null.</param>
	/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
	/// </remarks>
	public static char Show(string text, string style = null, IWin32Window owner = null, string title = null)
	{
		MDButtons buttons = 0;
		MDIcon icon = 0;
		MDFlag flags = 0;

		if(!string.IsNullOrEmpty(style)) {
			if(style.Contains("OC")) buttons = MDButtons.OKCancel;
			else if(style.Contains("YNC")) buttons = MDButtons.YesNoCancel;
			else if(style.Contains("YN")) buttons = MDButtons.YesNo;
			else if(style.Contains("ARI")) buttons = MDButtons.AbortRetryIgnore;
			else if(style.Contains("RC")) buttons = MDButtons.RetryCancel;
			else if(style.Contains("CT")) buttons = MDButtons.CancelTryagainContinue; //not CTC, because Continue returns E

			if(style.Contains("x")) icon = MDIcon.Error;
			else if(style.Contains("?")) icon = MDIcon.Question;
			else if(style.Contains("!")) icon = MDIcon.Warning;
			else if(style.Contains("i")) icon = MDIcon.Info;
			else if(style.Contains("v")) icon = MDIcon.Shield;
			else if(style.Contains("a")) icon = MDIcon.App;

			if(style.Contains("t")) flags |= MDFlag.SystemModal; //MDFlag.Topmost often ignored etc
			if(style.Contains("s")) flags |= MDFlag.NoSound;
			if(style.Contains("d")) flags |= MDFlag.DisableThreadWindows;

			if(style.Contains("2")) flags |= MDFlag.DefaultButton2;
			else if(style.Contains("3")) flags |= MDFlag.DefaultButton3;
		}

		int r = (int)Show(text, buttons, icon, flags, owner, title);

		return (r > 0 && r < 12) ? "COCARIYNCCTE"[r] : 'C';
	}

	struct MSGBOXPARAMS
	{
		public uint cbSize;
		public Wnd hwndOwner;
		public IntPtr hInstance;
		public string lpszText;
		public string lpszCaption;
		public uint dwStyle;
		public IntPtr lpszIcon;
		public LPARAM dwContextHelpId;
		public IntPtr lpfnMsgBoxCallback;
		public uint dwLanguageId;
	}

	[DllImport("user32.dll", EntryPoint = "MessageBoxIndirectW")]
	static extern int MessageBoxIndirect([In] ref MSGBOXPARAMS lpMsgBoxParams);

}
#endregion MessageDialog






public partial class Files
{

	/// <summary>
	/// Gets shell icon of a file or protocol etc where SHGetFileInfo would fail.
	/// Also can get icons of sizes other than 16 or 32.
	/// Cannot get file extension icons.
	/// If pidl is not Zero, uses it and ignores file, else uses file.
	/// Returns Zero if failed.
	/// </summary>
	[HandleProcessCorruptedStateExceptions]
	static unsafe IntPtr _Icon_GetSpec(string file, IntPtr pidl, int size)
	{
		IntPtr R = Zero;
		bool freePidl = false;
		Api.IShellFolder folder = null;
		Api.IExtractIcon eic = null;
		try { //possible exceptions in shell32.dll or in shell extensions
			if(pidl == Zero) {
				pidl = Misc.PidlFromString(file);
				if(pidl == Zero) return Zero;
				freePidl = true;
			}

			IntPtr pidlItem;
			int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
			if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }

			object o;
			hr = folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o);
			//if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			if(0 != hr) {
				if(hr == Api.REGDB_E_CLASSNOTREG) return Zero;
				PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}");
				return Zero;
			}
			eic = o as Api.IExtractIcon;

			var sb = new StringBuilder(300); int ii; uint fl;
			hr = eic.GetIconLocation(0, sb, 300, out ii, out fl);
			if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			string loc = sb.ToString();

			if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) != 0 || 1 != Api.PrivateExtractIcons(loc, ii, size, size, out R, Zero, 1, 0)) {
				IntPtr* hiSmall = null, hiBig = null;
				if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
				hr = eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16));
				if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			}
		}
		catch(Exception e) { PrintDebug($"Exception in _Icon_GetSpec: {file}, {e.Message}, {e.TargetSite}"); }
		finally {
			if(eic != null) Marshal.ReleaseComObject(eic);
			if(folder != null) Marshal.ReleaseComObject(folder);
			if(freePidl) Marshal.FreeCoTaskMem(pidl);
		}
		return R;
	}

}
}
