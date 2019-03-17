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
using System.Linq;

using System.Text.RegularExpressions; //for XML doc links

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// PCRE regular expression.
	/// </summary>
	/// <remarks>
	/// PCRE is a regular expression library: <see href="https://www.pcre.org/"/>.
	/// PCRE regular expression syntax: <see href="https://www.pcre.org/current/doc/html/pcre2pattern.html">full</see>, <see href="https://www.pcre.org/current/doc/html/pcre2syntax.html">short</see>.
	/// Some websites with tutorials and info: <see href="http://www.rexegg.com/">rexegg</see>, <see href="https://www.regular-expressions.info/">regular-expressions.info</see>.
	/// 
	/// This class is an alternative to the .NET <see cref="Regex"/> class. The regular expression syntax is similar. PCRE has some features unavailable in .NET, and vice versa. In most cases PCRE is about 2 times faster. You can use any of these classes. Functions of <see cref="Acc"/> class support only PCRE.
	/// 
	/// Terms used in this documentation and in names of functions and types:
	/// <list type="bullet">
	///   <item> <term>regular expression</term> <description>Regular expression string. Also known as <i>pattern</i>.</description> </item>
	///   <item> <term>subject string</term> <description>The string in which to search for the regular expression. Also known as <i>input string</i>.</description> </item>
	///   <item> <term>match</term> <description>The part (substring) of the subject string that matches the regular expression.</description> </item>
	///   <item> <term>groups</term> <description>Regular expression parts enclosed in (). Except non-capturing parts, like (?:...) and (?options). Also known as <i>capturing group</i>, <i>capturing subpattern</i>. Often term <i>group</i> also is used for group matches.</description> </item>
	///   <item> <term>group match</term> <description>The part (substring) of the subject string that matches the group. Also known as <i>captured substring</i>.</description> </item>
	/// </list>
	/// 
	/// This library uses an unmanaged code dll AuCpp.dll that contains PCRE code. This class is a managed wrapper for it. The main PCRE API functions used by this class are <see href="https://www.pcre.org/current/doc/html/pcre2api.html">pcre2_compile and pcre2_match</see>. The Regex_ constructor calls pcre2_compile and stores the compiled code in the variable. Other Regex_ functions call pcre2_match. Compiling to native code (JIT) is not supported.
	/// 
	/// A Regex_ variable can be used by multiple threads simultaneously.
	/// 
	/// Also there are several String extension methods that use this class. The string variable is the subject string. These methods create and use cached Regex_ instances for speed. The Regex_ constructor does not use caching.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// var s = "one two22, three333,four"; //subject string
	/// var x = new Regex_(@"\b(\w+?)(\d+)\b"); //regular expression
	///  
	///  Print("//IsMatch:");
	/// Print(x.IsMatch(s));
	///  
	///  Print("//Match:");
	/// if(x.Match(s, out var m)) Print(m.Value, m[1].Value, m[2].Value);
	///  
	///  Print("//FindAll with foreach:");
	/// foreach(var v in x.FindAll(s)) Print(v.Value, v[1].Value, v[2].Value);
	///  Print("//FindAllS, get only strings of group 2:");
	/// Print(x.FindAllS(s, 2));
	///  
	///  Print("//Replace:");
	/// Print(x.Replace(s, "'$2$1'"));
	///  Print("//Replace with callback:");
	/// Print(x.Replace(s, o => o.Value.ToUpper_()));
	///  Print("//Replace with callback and ExpandReplacement:");
	/// Print(x.Replace(s, o => { if(o.Length > 5) return o.ExpandReplacement("'$2$1'"); else return o[1].Value; }));
	///  
	///  Print("//Split:");
	/// Print(new Regex_(@" *, *").Split(s));
	/// ]]></code>
	///  Examples with String extension methods. 
	/// <code><![CDATA[
	/// var s = "one two22, three333,four"; //subject string
	/// var rx = @"\b(\w+?)(\d+)\b"; //regular expression
	///  
	///  Print("//RegexIsMatch_:");
	/// Print(s.RegexIsMatch_(rx));
	///  
	///  Print("//RegexMatch_:");
	/// if(s.RegexMatch_(rx, out var m)) Print(m.Value, m[1].Value, m[2].Value);
	///  
	///  Print("//RegexMatch_, get only string:");
	/// if(s.RegexMatch_(rx, 0, out var s0)) Print(s0);
	///  Print("//RegexMatch_, get only string of group 1:");
	/// if(s.RegexMatch_(rx, 1, out var s1)) Print(s1);
	///  
	///  Print("//RegexFindAll_ with foreach:");
	/// foreach(var v in s.RegexFindAll_(rx)) Print(v.Value, v[1].Value, v[2].Value);
	///  
	///  Print("//RegexFindAll_ with foreach, get only strings:");
	/// foreach(var v in s.RegexFindAll_(rx, 0)) Print(v);
	///  Print("//RegexFindAll_ with foreach, get only strings of group 2:");
	/// foreach(var v in s.RegexFindAll_(rx, 2)) Print(v);
	///  
	///  Print("//RegexFindAll_, get array:");
	/// if(s.RegexFindAll_(rx, out var am)) foreach(var k in am) Print(k.Value, k[1].Value, k[2].Value);
	///  
	///  Print("//RegexFindAll_, get array of strings:");
	/// if(s.RegexFindAll_(rx, 0, out var av)) Print(av);
	///  Print("//RegexFindAll_, get array of group 2 strings:");
	/// if(s.RegexFindAll_(rx, 2, out var ag)) Print(ag);
	///  
	///  Print("//RegexReplace_:");
	/// Print(s.RegexReplace_(rx, "'$2$1'"));
	///  
	///  Print("//RegexReplace_ with callback:");
	/// Print(s.RegexReplace_(rx, o => o.Value.ToUpper_()));
	///  Print("//RegexReplace_ with callback and ExpandReplacement:");
	/// Print(s.RegexReplace_(rx, o => { if(o.Length > 5) return o.ExpandReplacement("'$2$1'"); else return o[1].Value; }));
	///  
	///  Print("//RegexReplace_, get replacement count:");
	/// if(0 != s.RegexReplace_(rx, "'$2$1'", out var s2)) Print(s2);
	///  
	///  Print("//RegexReplace_ with callback, get replacement count:");
	/// if(0 != s.RegexReplace_(rx, o => o.Value.ToUpper_(), out var s3)) Print(s3);
	///  
	///  Print("//RegexSplit_:");
	/// Print(s.RegexSplit_(@" *, *"));
	/// ]]></code></example>
	public unsafe class Regex_
	{
		IntPtr _codeUnsafe; //pcre2_code_16*. Don't pass to PCRE API directly, because then GC can collect this object
		Cpp.PcreCalloutT _pcreCallout; //our callout that calls the user's callout. This field protects the delegates from GC.
		byte _matchFlags; //RXMatchFlags specified in hi byte of ctor flags

		internal HandleRef _CodeHR => new HandleRef(this, _codeUnsafe); //pass this to PCRE API

		/// <summary>
		/// Compiles regular expression string.
		/// 
		/// PCRE regular expression syntax: <see href="https://www.pcre.org/current/doc/html/pcre2pattern.html">full</see>, <see href="https://www.pcre.org/current/doc/html/pcre2syntax.html">short</see>.
		/// </summary>
		/// <param name="rx">
		/// Regular expression. Cannot be null.
		/// </param>
		/// <param name="flags">
		/// Options.
		/// Default 0. Flag UTF is implicitly added if <paramref name="rx"/> contains non-ASCII characters and there is no flag NEVER_UTF.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">Invalid regular expression. Or failed to compile it for some other reason (unlikely).</exception>
		/// <remarks>
		/// Calls PCRE API function <see href="https://www.pcre.org/current/doc/html/pcre2api.html">pcre2_compile</see>.
		/// 
		/// Examples in class help: <see cref="Regex_"/>.
		/// </remarks>
		public Regex_(string rx, RXFlags flags = 0)
		{
			if(rx == null) throw new ArgumentNullException();
			_matchFlags = (byte)((ulong)flags >> 56); flags = (RXFlags)((ulong)flags & 0xffffff_ffffffff);
			_codeUnsafe = Cpp.Cpp_RegexCompile(rx, rx.Length, flags, out int codeSize, out BSTR errStr);
			if(_codeUnsafe == default) throw new ArgumentException(errStr.ToStringAndDispose(noCache: true));
			GC.AddMemoryPressure(codeSize);
		}

		///
		~Regex_()
		{
			//Print("dtor");
			int codeSize = Cpp.Cpp_RegexDtor(_codeUnsafe);
			GC.RemoveMemoryPressure(codeSize);
		}

		/// <summary>
		/// Sets callout callback function.
		/// </summary>
		/// <value>Callback delegate (eg lambda) or null.</value>
		/// <remarks>
		/// Callouts can be used to: 1. Track the matching progress. 2. Get all instances of a group that can match multiple times. 3. Evaluate and reject some matches or match parts. 4. Etc.
		/// The callback function is called by <see cref="IsMatch"/>, <see cref="Match"/>, <see cref="FindAll"/>, <see cref="Replace"/>, <see cref="Split"/> and similar functions, when they reach callout points in regular expression. To insert callout points use (?C), (?C1), (?C2), (?C'name') etc or pass flag AUTO_CALLOUT to the constructor.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// See also: <see href="http://www.rexegg.com/pcre-callouts.html"/>
		/// </remarks>
		/// <example>
		/// Track the matching progress.
		/// <code><![CDATA[
		/// var s = "text <a href='url'>link</a> text";
		/// var rx = @"(?C1)<a (?C2)href='.+?'>(?C3)[^<]*(?C4)</a>";
		/// var x = new Regex_(rx);
		/// x.Callout = o => { Print(o.callout_number, o.current_position, s.Substring(o.start_match, o.current_position), rx.Substring(o.pattern_position, o.next_item_length)); };
		/// Print(x.IsMatch(s));
		/// ]]></code>
		/// Track the matching progress with flag AUTO_CALLOUT.
		/// <code><![CDATA[
		/// var s = "one 'two' three";
		/// var rx = @"'(.+?)'";
		/// var x = new Regex_(rx, RXFlags.AUTO_CALLOUT);
		/// x.Callout = o => Print(o.current_position, o.pattern_position, rx.Substring(o.pattern_position, o.next_item_length));
		/// Print(x.IsMatch(s));
		/// ]]></code>
		/// Get all instances of a group that can match multiple times.
		/// <code><![CDATA[
		/// var s = "BEGIN 111 2222 333 END";
		/// var x = new Regex_(@"^(\w+) (?:(\d+) (?C1))+(\w+)$");
		/// var a = new List<string>();
		/// x.Callout = o => a.Add(o.LastGroupValue);
		/// if(!x.Match(s, out var m)) { Print("no match"); return; }
		/// Print(m[1]);
		/// Print(a); //all numbers. m[2] contains only the last number.
		/// Print(m[3]);
		/// ]]></code>
		/// Evaluate and reject some matches or match parts. This code rejects matches longer than 5.
		/// <code><![CDATA[
		/// var s = "one 123-5 two 12-456 three 1-34 four";
		/// var x = new Regex_(@"\b\d+-\d+\b(?C1)");
		/// x.Callout = o => { int len = o.current_position - o.start_match; /*Print(len);*/ if(len > 5) o.Result = 1; };
		/// Print(x.FindAllS(s));
		/// ]]></code>
		/// </example>
		public RXCalloutFunc Callout {
			set {
				lock(this) {
					if(value == null) {
						_pcreCallout = null;
					} else {
						_pcreCallout = (void* calloutBlock, void* param) => {
							var b = new RXCalloutData(calloutBlock);
							value(b);
							return b.Result;
						};
					}
				}
			}
		}

		/// <summary>
		/// Finds a named group and returns its 1-based index.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="groupName">
		/// Group name.
		/// In regular expression, to set name of group <c>(text)</c>, use <c>(?&lt;NAME&gt;text)</c>.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">More than 1 group have this name.</exception>
		/// <seealso cref="RXMatch.GroupNumberFromName(string)"/>
		/// <seealso cref="RXMatch.GroupNumberFromName(string, out bool)"/>
		public int GroupNumberFromName(string groupName)
		{
			if(groupName == null) throw new ArgumentNullException();
			fixed (char* p = groupName) {
				int R = Cpp.pcre2_substring_nametable_scan(_CodeHR, p, null, null);
				if(R <= 0) {
					if(R == -50) throw new ArgumentException("Multiple groups have name " + groupName); //-50 PCRE2_ERROR_NOUNIQUESUBSTRING
					R = -1;
				}
				return R;
			}
		}

		/// <summary>
		/// Returns true if string <paramref name="s"/> matches this regular expression.
		/// </summary>
		/// <param name="s">
		/// Subject string.
		/// If null, always returns false, even if the regular expression matches empty string.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// If partial match, returns true too. Partial match is possible if used a PARTIAL_ flag.
		/// 
		/// This function is similar to <see cref="Regex.IsMatch(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// Print(x.IsMatch(s));
		/// ]]></code>
		/// </example>
		public bool IsMatch(string s, RXMore more = null)
		{
			if(!_GetStartEnd(s, more, out int start, out int end)) return false;
			int rc = Cpp.Cpp_RegexMatch(_CodeHR, s, end, start, _GetMatchFlags(more), _pcreCallout, null, out BSTR errStr);
			//Print(rc);
			//info: 0 is partial match, -1 is no match, <-1 is error
			if(rc < -1) throw new AuException(errStr.ToStringAndDispose(noCache: true));
			return rc >= 0;
		}

		/// <summary>
		/// Returns true if string <paramref name="s"/> matches this regular expression.
		/// Gets match info as <see cref="RXMatch"/>.
		/// </summary>
		/// <param name="s">
		/// Subject string.
		/// If null, always returns false, even if the regular expression matches empty string.
		/// </param>
		/// <param name="result">Receives match info. Read more in Remarks.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// If full match, returns true, and <paramref name="result"/> contains the match and all groups that exist in the regular expressions.
		/// If partial match, returns true, and <paramref name="result"/> contains the match without groups. Partial match is possible if used a PARTIAL_ flag.
		/// If no match, returns false, and <paramref name="result"/> normally is null. But if a mark is available, <paramref name="result"/> is an object with two valid properties - <see cref="RXMatch.Exists"/> (false) and <see cref="RXMatch.Mark"/>; other properties have undefined values or throw exception.
		/// 
		/// This function is similar to <see cref="Regex.Match(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(x.Match(s, out var m)) Print(m.Value, m[1].Value, m[2].Value);
		/// ]]></code>
		/// </example>
		public bool Match(string s, out RXMatch result, RXMore more = null)
		{
			result = null;
			int rc = _Match(s, 0, more, out var m);
			if(rc >= 0 || m.mark != null) {
				result = new RXMatch(this, s, rc, in m);
			}
			return rc >= 0;
		}

		/// <summary>
		/// Returns true if string <paramref name="s"/> matches this regular expression.
		/// Gets whole match or some group, as <see cref="RXGroup"/> (index, length, value).
		/// </summary>
		/// <param name="s">
		/// Subject string.
		/// If null, always returns false, even if the regular expression matches empty string.
		/// </param>
		/// <param name="result">Receives match info.</param>
		/// <param name="group">
		/// Group number (1-based index) of result. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="Match"/>.
		/// If full match, returns true, and <paramref name="result"/> contains the match or the specifed group.
		/// If partial match, returns true too. Partial match is possible if used a PARTIAL_ flag. Then cannot get groups, therefore <paramref name="group"/> should be 0.
		/// If no match, returns false, and <paramref name="result"/> is empty.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(x.MatchG(s, out var g)) Print(g.Value, g.Index);
		/// ]]></code>
		/// </example>
		public bool MatchG(string s, out RXGroup result, int group = 0, RXMore more = null)
		{
			int rc = _Match(s, group, more, out var m);
			if(rc < 0) {
				result = default;
				return false;
			}
			result = new RXGroup(s, m.vec[group]);
			return true;
		}

		/// <summary>
		/// Returns true if string <paramref name="s"/> matches this regular expression.
		/// Gets whole match or some group, as string.
		/// </summary>
		/// <param name="s">
		/// Subject string.
		/// If null, always returns false, even if the regular expression matches empty string.
		/// </param>
		/// <param name="result">Receives the match value.</param>
		/// <param name="group">
		/// Group number (1-based index) of result. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="Match"/> and <see cref="MatchG"/>.
		/// If full match, returns true, and <paramref name="result"/> contains the value of the match or of the specifed group.
		/// If partial match, returns true too. Partial match is possible if used a PARTIAL_ flag. Then cannot get groups, therefore <paramref name="group"/> should be 0.
		/// If no match, returns false, and <paramref name="result"/> is null.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(x.MatchS(s, out var v)) Print(v);
		/// ]]></code>
		/// </example>
		public bool MatchS(string s, out string result, int group = 0, RXMore more = null)
		{
			result = null;
			if(!MatchG(s, out var g, group, more)) return false;
			result = g.Value;
			return true;
		}

		//If s (subject) is null, returns false.
		//Else if more is null, sets start=0, end=s.Length.
		//Else sets start etc = more.start etc. If more.start<0, uses s.Length. If some is invalid, throws exception.
		static bool _GetStartEnd(string s, RXMore more, out int start, out int end)
		{
			start = 0;
			if(s == null) { end = 0; return false; }
			if(more == null) {
				end = s.Length;
			} else {
				start = more.start; end = more.end;
				if((uint)start > s.Length) throw new ArgumentOutOfRangeException(nameof(more.start));
				if(end < 0) end = s.Length; else if(end < more.start) throw new ArgumentOutOfRangeException(nameof(more.end));
			}
			return true;
		}

		RXMatchFlags _GetMatchFlags(RXMore more)
		{
			var f = (RXMatchFlags)_matchFlags;
			if(more != null) f |= more.matchFlags;
			return f;
		}

		//Calls Cpp_RegexMatch and returns its results.
		//Throws if it returns less than -1. Throws if invalid start/end/group.
		//m.vec array is thread_local. Next call reallocates/overwrites it, except when called by a callout of the same call.
		//m.mark is set even if no match, if available.
		//s - subject. If null, returns rc -1.
		//group - 0 or group number. Used only to throw if invalid.
		int _Match(string s, int group, RXMore more, out Cpp.RegexMatch m)
		{
			if(!_GetStartEnd(s, more, out int start, out int end)) { m = default; return -1; }
			int rc = Cpp.Cpp_RegexMatch(_CodeHR, s, end, start, _GetMatchFlags(more), _pcreCallout, out m, out BSTR errStr);
			//Print(rc);
			//info: 0 is partial match, -1 is no match, <-1 is error
			if(rc < -1) throw new AuException(errStr.ToStringAndDispose(noCache: true));
			if(group != 0 && rc >= 0 && (uint)group >= m.vecCount) throw new ArgumentOutOfRangeException(nameof(group));
			return rc;
		}

		//Used by FindAllX and ReplaceAllX to easily find matches in loop.
		struct _MatchEnum
		{
			Regex_ _regex;
			string _subject;
			Cpp.RegexMatch _m;
			RXMatchFlags _matchFlags;
			int _group, _from, _to, _maxCount, _rc;
			public int foundCount;

			//Calls _GetFromTo and inits fields. Throws if s is null or if invalid start/end or used 'partial' flags.
			public _MatchEnum(Regex_ regex, string s, int group, RXMore more, int maxCount = -1)
			{
				_regex = regex; _subject = s; _group = group;
				if(!_GetStartEnd(s, more, out _from, out _to)) throw new ArgumentNullException(nameof(s));
				_matchFlags = regex._GetMatchFlags(more);
				if(0 != (_matchFlags & (RXMatchFlags.PARTIAL_SOFT | RXMatchFlags.PARTIAL_HARD)))
					throw new ArgumentException("This function does not support PARTIAL_ flags.", nameof(more));
				_maxCount = maxCount;
				foundCount = _rc = 0;
				_m = default;
			}

			//Calls Cpp_RegexMatch, remembers its results, increments foundCount if found.
			//Returns false if it returns -1. Throws if it returns < -1. Throws if invalid group.
			//To get results, use properties Match or GroupX. Don't call Next or any other match function before it.
			public bool Next()
			{
				if(foundCount >= (uint)_maxCount) return false;
				_rc = Cpp.Cpp_RegexMatch(_regex._CodeHR, _subject, _to, _from, _matchFlags, _regex._pcreCallout, out _m, out BSTR errStr);
				//Print(_rc);
				//info: 0 cannot be (partial match), -1 is no match, <-1 is error
				if(_rc < 0) {
					if(_rc < -1) throw new AuException(errStr.ToStringAndDispose(noCache: true));
					return false;
				}
				if(_group != 0 && (uint)_group >= _m.vecCount) throw new ArgumentOutOfRangeException("group");
				_SetNextFrom();
				_matchFlags |= RXMatchFlags.NO_UTF_CHECK;
				foundCount++;
				return true;
			}

			void _SetNextFrom()
			{
				var p = _m.vec[0]; //x=start, y=end
				_from = p.y;
				//empty match?
				if(_from <= p.x) {
					if(_from < p.x) throw new ArgumentException(@"This function does not support (?=...\K).");
					if(++_from < _to) {
						var c = _subject[_from];
						if(c == '\n') { //skip \n if inside \r\n
							if(_subject[_from - 1] == '\r') _from++;
						} else if((c & 0xfc00) == 0xdc00) { //skip the second part of surrogate pair
							if(0 != (_regex._InfoAllOptions & RXFlags.UTF)) _from++;
						}
					}
					if(_from > _to) _maxCount = 0;
				}
			}

			public RXMatch Match => new RXMatch(_regex, _subject, _rc, in _m);

			public POINT GroupP => _m.vec[_group];

			public RXGroup GroupG => new RXGroup(_subject, GroupP);

			public string GroupS { get { var p = GroupP; return p.x < 0 ? null : _subject.Substring(p.x, p.y - p.x); } }
		}

		/// <summary>
		/// Finds all match instances of the regular expression.
		/// Returns a lazy IEnumerable&lt;<see cref="RXMatch"/>&gt; object that can be used with foreach.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// foreach(var m in x.FindAll(s)) Print(m.Value, m[1].Value, m[2].Value);
		/// ]]></code>
		/// </example>
		public IEnumerable<RXMatch> FindAll(string s, RXMore more = null)
		{
			var e = new _MatchEnum(this, s, 0, more);
			while(e.Next()) yield return e.Match;
		}

		/// <summary>
		/// Finds all match instances of the regular expression. Gets array of <see cref="RXMatch"/>.
		/// Returns true if found 1 or more matches.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="result">Receives all found matches.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(!x.FindAll(s, out var a)) { Print("not found"); return; }
		/// foreach(var m in a) Print(m.Value, m[1].Value, m[2].Value);
		/// ]]></code>
		/// </example>
		public bool FindAll(string s, out RXMatch[] result, RXMore more = null)
		{
			result = FindAll(s, more).ToArray();
			return result.Length != 0;
		}

		/// <summary>
		/// Finds all match instances of the regular expression.
		/// Returns a lazy IEnumerable&lt;<see cref="RXGroup"/>&gt; object that can be used with foreach.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="group">
		/// Group number (1-based index) of results. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="FindAll(string, RXMore)"/>. Also it is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two three";
		/// var x = new Regex_(@"\b\w+\b");
		/// foreach(var g in x.FindAllG(s)) Print(g.Index, g.Value);
		/// ]]></code>
		/// </example>
		public IEnumerable<RXGroup> FindAllG(string s, int group = 0, RXMore more = null)
		{
			var e = new _MatchEnum(this, s, group, more);
			while(e.Next()) yield return e.GroupG;
		}

		/// <summary>
		/// Finds all match instances of the regular expression. Gets array of <see cref="RXGroup"/> (index, length, value).
		/// Returns true if found 1 or more matches.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="result">Receives all found matches.</param>
		/// <param name="group">
		/// Group number (1-based index) of results. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="FindAll(string, out RXMatch[], RXMore)"/>. Also it is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two three";
		/// var x = new Regex_(@"\b\w+\b");
		/// if(!x.FindAllG(s, out var a)) { Print("not found"); return; }
		/// foreach(var g in a) Print(g.Index, g.Value);
		/// ]]></code>
		/// </example>
		public bool FindAllG(string s, out RXGroup[] result, int group = 0, RXMore more = null)
		{
			result = FindAllG(s, group, more).ToArray();
			return result.Length != 0;
		}

		/// <summary>
		/// Finds all match instances of the regular expression.
		/// Returns a lazy IEnumerable&lt;string&gt; object that can be used with foreach.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="group">
		/// Group number (1-based index) of results. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="FindAll(string, RXMore)"/> and <see cref="FindAllG(string, int, RXMore)"/>. Also it is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two three";
		/// var x = new Regex_(@"\b\w+\b");
		/// foreach(var v in x.FindAllS(s)) Print(v);
		/// ]]></code>
		/// </example>
		public IEnumerable<string> FindAllS(string s, int group = 0, RXMore more = null)
		{
			var e = new _MatchEnum(this, s, group, more);
			while(e.Next()) yield return e.GroupS;
		}

		/// <summary>
		/// Finds all match instances of the regular expression. Gets array of strings.
		/// Returns true if found 1 or more matches.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="result">Receives all found matches.</param>
		/// <param name="group">
		/// Group number (1-based index) of results. If 0 (default) - whole match.
		/// See also <see cref="GroupNumberFromName"/>.
		/// </param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/> or from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is a simplified version of <see cref="FindAll(string, out RXMatch[], RXMore)"/> and <see cref="FindAllG(string, out RXGroup[], int, RXMore)"/>. Also it is similar to <see cref="Regex.Matches(string)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two three";
		/// var x = new Regex_(@"\b\w+\b");
		/// if(!x.FindAllS(s, out var a)) { Print("not found"); return; }
		/// foreach(var v in a) Print(v);
		/// ]]></code>
		/// </example>
		public bool FindAllS(string s, out string[] result, int group = 0, RXMore more = null)
		{
			result = FindAllS(s, group, more).ToArray();
			return result.Length != 0;
		}

		int _Replace(string s, out string result, string repl, Func<RXMatch, string> replFunc, int maxCount, RXMore more)
		{
			StringBuilder b = null;
			Util.LibStringBuilder bCache = default;
			int prevEnd = 0;
			int replType = 0; //0 empty, 1 simple, 2 with $, 3 callback

			var e = new _MatchEnum(this, s, 0, more, maxCount);
			while(e.Next()) {
				//init variables
				if(b == null) {
					bCache = new Util.LibStringBuilder(out b, s.Length + 100);
					if(replFunc != null) replType = 3; else if(!Empty(repl)) replType = repl.IndexOf('$') < 0 ? 1 : 2;
				}
				//append s part before this match
				var p = e.GroupP; //x=start, y=end
				int nBefore = p.x - prevEnd;
				if(nBefore != 0) b.Append(s, prevEnd, nBefore);
				prevEnd = p.y;
				//append replacement
				string re = null;
				if(replType >= 2) {
					var m = e.Match; //FUTURE: optimization: if no callback, use single instance and set fields.
					if(replFunc != null) re = replFunc(m);
					else LibExpandReplacement(m, repl, b);
				} else re = repl;
				if(!Empty(re)) b.Append(re);
			}

			//append s part after last match
			if(e.foundCount != 0) {
				int nAfter = s.Length - prevEnd;
				if(nAfter > 0) b.Append(s, prevEnd, nAfter);
				result = b.ToString();
				bCache.Dispose();
			} else result = s;

			return e.foundCount;
		}

		/// <summary>
		/// Finds and replaces all match instances of the regular expression.
		/// Returns the result string.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="repl">
		/// Replacement pattern.
		/// Can consist of any combination of literal text and substitutions like $1.
		/// Supports .NET regular expression substitution syntax. See <see cref="Regex.Replace(string, string, int)"/>. Also replaces $* with the name of the last encountered mark.
		/// </param>
		/// <param name="maxCount">The maximal count of replacements to make. If -1 (default), replaces all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Replace(string, string, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// s = x.Replace(s, "'$2$1'");
		/// Print(s);
		/// ]]></code>
		/// </example>
		public string Replace(string s, string repl = null, int maxCount = -1, RXMore more = null)
		{
			_Replace(s, out var R, repl, null, maxCount, more);
			return R;
		}

		/// <summary>
		/// Finds and replaces all match instances of the regular expression.
		/// Returns the number of replacements made. Returns the result string through an out parameter.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="repl">
		/// Replacement pattern.
		/// Can consist of any combination of literal text and substitutions like $1.
		/// Supports .NET regular expression substitution syntax. See <see cref="Regex.Replace(string, string, int)"/>. Also replaces $* with the name of the last encountered mark.
		/// </param>
		/// <param name="result">The result string. Can be <paramref name="s"/>.</param>
		/// <param name="maxCount">The maximal count of replacements to make. If -1 (default), replaces all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Replace(string, string, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(0 == x.Replace(s, "'$2$1'", out s)) Print("not found");
		/// else Print(s);
		/// ]]></code>
		/// </example>
		public int Replace(string s, string repl, out string result, int maxCount = -1, RXMore more = null)
		{
			return _Replace(s, out result, repl, null, maxCount, more);
		}

		/// <summary>
		/// Finds and replaces all match instances of the regular expression. Uses a callback function.
		/// Returns the result string.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="replFunc">
		/// Callback function's delegate, eg lambda. Called for each found match. Returns the replacement.
		/// In the callback function you can use <see cref="RXMatch.ExpandReplacement"/>.
		/// </param>
		/// <param name="maxCount">The maximal count of replacements to make. If -1 (default), replaces all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Replace(string, MatchEvaluator, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// s = x.Replace(s, o => o.Value.ToUpper_());
		/// Print(s);
		/// ]]></code>
		/// </example>
		public string Replace(string s, Func<RXMatch, string> replFunc, int maxCount = -1, RXMore more = null)
		{
			_Replace(s, out var R, null, replFunc, maxCount, more);
			return R;
		}

		/// <summary>
		/// Finds and replaces all match instances of the regular expression. Uses a callback function.
		/// Returns the number of replacements made. Returns the result string through an out parameter.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="replFunc">
		/// Callback function's delegate, eg lambda. Called for each found match. Returns the replacement.
		/// In the callback function you can use <see cref="RXMatch.ExpandReplacement"/>.
		/// </param>
		/// <param name="result">The result string. Can be <paramref name="s"/>.</param>
		/// <param name="maxCount">The maximal count of replacements to make. If -1 (default), replaces all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// This function is similar to <see cref="Regex.Replace(string, MatchEvaluator, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one two22 three333 four";
		/// var x = new Regex_(@"\b(\w+?)(\d+)\b");
		/// if(0 == x.Replace(s, o => o.Value.ToUpper_(), out s)) Print("not found");
		/// else Print(s);
		/// ]]></code>
		/// </example>
		public int Replace(string s, Func<RXMatch, string> replFunc, out string result, int maxCount = -1, RXMore more = null)
		{
			return _Replace(s, out result, null, replFunc, maxCount, more);
		}

		//Used by _ReplaceAll and RXMatch.ExpandReplacement.
		//Fully supports .NET regular expression substitution syntax. Also replaces $* with the name of the last encountered mark.
		internal static void LibExpandReplacement(RXMatch m, string repl, StringBuilder b)
		{
			fixed (char* s0 = repl) {
				char* s = s0, eos = s + repl.Length, e = s; //e is the end of s part added to b
				while(s < eos) {
					if(*s == '$') {
						if(s > e) { b.Append(e, (int)(s - e)); e = s; }

						char ch = *++s;

						if(ch == '$') { //escaped $
							e = s++;
							continue;
						}

						int group = -1;
						if(ch == '{') { //${name} or ${number}
							char* t = ++s; while(t < eos && *t != '}') t++;
							if(t == eos) break;
							ch = *s;
							if(ch >= '0' && ch <= '9') { //${number}. info: group name cannot start with digit, then PCRE returns error.
								group = repl.ToInt_((int)(s - s0), out int numEnd, STIFlags.NoHex);
								if(s0 + numEnd != t || group < 0) continue;
							} else { //${name}
								group = m.LibGroupNumberFromName(s, (int)(t - s), out _); //speed: 40-100 ns
								if(group < 0) continue;
							}
							s = t + 1;
						} else if(ch >= '0' && ch <= '9') { //$number
							group = repl.ToInt_((int)(s - s0), out int numEnd, STIFlags.NoHex);
							if(numEnd == 0 || group < 0) continue;
							s = s0 + numEnd;
						} else {
							s++;
							if(ch == '`') { //part before match
								int i = m.Index;
								if(i > 0) b.Append(m.Subject, 0, i);
							} else if(ch == '\'') { //part after match
								var subject = m.Subject;
								int i = m.EndIndex, len = subject.Length - i;
								if(len > 0) b.Append(subject, i, len);
							} else if(ch == '&') { //whole match
								group = 0;
							} else if(ch == '+') { //last group
								group = m.GroupCountPlusOne - 1;
							} else if(ch == '_') { //subject
								b.Append(m.Subject);
							} else if(ch == '*') { //last mark
								b.Append(m.Mark);
							} else continue;
						}

						if(group >= 0) {
							if(group >= m.GroupCountPlusOne) continue;
							var g = m[group];
							if(g.Length > 0) b.Append(g.LibSubject, g.Index, g.Length);
						}

						e = s;
					} else s++;
				}

				int tail = (int)(eos - e);
				if(tail > 0) b.Append(e, tail);
			}
		}

		/// <summary>
		/// Returns array of substrings delimited by regular expression matches.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="maxCount">The maximal count of substrings to get. The last substring contains the unsplit remainder of the subject string. If 0 (default) or negative, gets all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// Element 0 of the returned array is <paramref name="s"/> substring until the first match of the regular expression, element 1 is substring between the first and second match, and so on. If no matches, the array contains single element and it is <paramref name="s"/>.
		/// 
		/// This function is similar to <see cref="Regex.Split(string, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one, two,three , four";
		/// var x = new Regex_(@" *, *");
		/// var a = x.Split(s);
		/// for(int i = 0; i < a.Length; i++) Print(i, a[i]);
		/// ]]></code>
		/// </example>
		public string[] Split(string s, int maxCount = 0, RXMore more = null)
		{
			if(maxCount < 0) maxCount = 0;
			if(maxCount != 1) {
				var a = new List<string>();
				int prevEnd = 0;
				var e = new _MatchEnum(this, s, 0, more, maxCount - 1);
				while(e.Next()) {
					var p = e.GroupP;
					a.Add(s.Substring(prevEnd, p.x - prevEnd));
					prevEnd = p.y;
				}
				if(e.foundCount > 0) {
					a.Add(s.Substring(prevEnd));
					return a.ToArray();
				}
			}
			return new string[] { s };
		}

		/// <summary>
		/// Returns <see cref="RXGroup"/> array of substrings delimited by regular expression matches.
		/// </summary>
		/// <param name="s">Subject string. Cannot be null.</param>
		/// <param name="maxCount">The maximal count of substrings to get. The last substring contains the unsplit remainder of the subject string. If 0 (default) or negative, gets all.</param>
		/// <param name="more"></param>
		/// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid from/to fields in <paramref name="more"/>.</exception>
		/// <exception cref="ArgumentException">1. Used a PARTIAL_ flag. 2. The regular expression contains <c>(?=...\K)</c>.</exception>
		/// <exception cref="AuException">The PCRE API function pcre2_match failed. Unlikely.</exception>
		/// <remarks>
		/// Element 0 of the returned array is <paramref name="s"/> substring until the first match of the regular expression, element 1 is substring between the first and second match, and so on. If no matches, the array contains single element and it is <paramref name="s"/>.
		/// 
		/// This function is similar to <see cref="Regex.Split(string, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var s = "one, two,three , four";
		/// var x = new Regex_(@" *, *");
		/// var a = x.SplitG(s);
		/// foreach(var v in a) Print(v.Index, v.Value);
		/// ]]></code>
		/// </example>
		public RXGroup[] SplitG(string s, int maxCount = 0, RXMore more = null)
		{
			if(maxCount < 0) maxCount = 0;
			if(maxCount != 1) {
				var a = new List<RXGroup>();
				int prevEnd = 0;
				var e = new _MatchEnum(this, s, 0, more, maxCount - 1);
				while(e.Next()) {
					var p = e.GroupP;
					a.Add(new RXGroup(s, prevEnd, p.x));
					prevEnd = p.y;
				}
				if(e.foundCount > 0) {
					a.Add(new RXGroup(s, prevEnd, s.Length));
					return a.ToArray();
				}
			}
			return new RXGroup[] { new RXGroup(s, 0, s.Length) };
		}

		//rejected: probably rarely used. Or need IEnumerable<string> too.
		//public IEnumerable<RXGroup> SplitE(string s, int maxCount = 0, RXMore more = null)
		//{
		//	if(maxCount< 0) maxCount = 0;
		//	if(maxCount != 1) {
		//		int prevEnd = 0;
		//		var e = new _MatchEnum(this, s, 0, more, maxCount - 1);
		//		while(e.Next()) {
		//			var p = e.GroupP;
		//			yield return new RXGroup(s, prevEnd, p.x);
		//			prevEnd = p.y;
		//		}
		//		if(e.foundCount > 0) {
		//			yield return new RXGroup(s, prevEnd, s.Length);
		//			yield break;
		//		}
		//	}
		//	yield return new RXGroup(s, 0, s.Length);
		//}

		//Calls pcre2_pattern_info(ALLOPTIONS), which returns flags passed to the ctor and possibly modified by (*OPTION) and possibly added UTF if contains non-ASCII characters.
		//Actually RXFlags is long, where the high 32 bits is extended options. This func gets only the main options (the low 32 bits).
		RXFlags _InfoAllOptions {
			get {
				RXFlags R;
				Cpp.pcre2_pattern_info(_CodeHR, Cpp.PCRE2_INFO_.ALLOPTIONS, &R);
				return R;
			}
		}

		/// <summary>
		/// Encloses string in \Q \E if it contains metacharacters \^$.[|()?*+{ or if <paramref name="always"/> == true.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="always">Enclose always, even if the string does not contain metacharacters. Should be true if the regular expression in which this string will be used has option "extended", because then whitespace is ignored and # is a special character too.</param>
		/// <remarks>
		/// Such enclosed substring in a regular expression is interpreted as a literal string.
		/// This function also escapes \E, so that it does not end the literal string.
		/// </remarks>
		public static string EscapeQE(string s, bool always = false)
		{
			if(s == null) return s;
			if(always) goto g1;
			for(int i = 0; i < s.Length; i++) {
				char c = s[i];
				if((c >= '(' && c <= '+') || c == '\\' || c == '.' || c == '?' || c == '{' || c == '[' || c == '|' || c == '$' || c == '^') goto g1;
				//this is slower
				//if(c < 128) {
				//	if(c < 64) {
				//		if(0 != (0b1000000000000000010011110001000000000000000000000000000000000000UL & (1UL << c))) goto g1;
				//	} else {
				//		if(0 != (0b0001100000000000000000000000000001011000000000000000000000000000UL & (1UL << (c - 64)))) goto g1;
				//	}
				//}
			}
			return s;
			g1:
			return @"\Q" + s.Replace(@"\E", @"\E\\E\Q") + @"\E";
		}
	}

	#region static

	public static partial class String_
	{
		/// <summary>
		/// Returns true if this string matches PCRE regular expression <paramref name="rx"/>.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.IsMatch(string, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexIsMatch_(this string s, string rx, RXFlags flags = 0, RXMore more = null)
		{
			if(s == null) return false;
			var x = _cache.AddOrGet(rx, flags);
			return x.IsMatch(s, more);
		}

		/// <summary>
		/// Returns true if this string matches PCRE regular expression <paramref name="rx"/>.
		/// Gets match info as <see cref="RXMatch"/>.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Match(string, out RXMatch, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexMatch_(this string s, string rx, out RXMatch result, RXFlags flags = 0, RXMore more = null)
		{
			if(s == null) { result = null; return false; }
			var x = _cache.AddOrGet(rx, flags);
			return x.Match(s, out result, more);
		}

		/// <summary>
		/// Returns true if this string matches PCRE regular expression <paramref name="rx"/>.
		/// Gets whole match or some group, as string.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.MatchS(string, out string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/>.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexMatch_(this string s, string rx, int group, out string result, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.MatchS(s, out result, group, more);
		}

		/// <summary>
		/// Returns true if this string matches PCRE regular expression <paramref name="rx"/>.
		/// Gets whole match or some group, as index and length.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.MatchG(string, out RXGroup, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/>.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexMatch_(this string s, string rx, int group, out RXGroup result, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.MatchG(s, out result, group, more);
		}

		/// <summary>
		/// Finds all match instances of PCRE regular expression <paramref name="rx"/>.
		/// Returns a lazy IEnumerable&lt;<see cref="RXMatch"/>&gt; object that can be used with foreach.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.FindAll(string, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static IEnumerable<RXMatch> RegexFindAll_(this string s, string rx, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.FindAll(s, more);
		}

		/// <summary>
		/// Finds all match instances of PCRE regular expression <paramref name="rx"/>. Gets array of <see cref="RXMatch"/>.
		/// Returns true if found 1 or more matches.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.FindAll(string, out RXMatch[], RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexFindAll_(this string s, string rx, out RXMatch[] result, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.FindAll(s, out result, more);
		}

		/// <summary>
		/// Finds all match instances of PCRE regular expression <paramref name="rx"/>.
		/// Returns a lazy IEnumerable&lt;<see cref="RXGroup"/>&gt; object that can be used with foreach.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.FindAllS(string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/>.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static IEnumerable<string> RegexFindAll_(this string s, string rx, int group, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.FindAllS(s, group, more);
		}

		/// <summary>
		/// Finds all match instances of PCRE regular expression <paramref name="rx"/>. Gets array of strings.
		/// Returns true if found 1 or more matches.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.FindAllS(string, out string[], int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="group"/>.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static bool RegexFindAll_(this string s, string rx, int group, out string[] result, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.FindAllS(s, out result, group, more);
		}

		/// <summary>
		/// Finds and replaces all match instances of PCRE regular expression <paramref name="rx"/>.
		/// Returns the result string.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Replace(string, string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static string RegexReplace_(this string s, string rx, string repl, int maxCount = -1, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.Replace(s, repl, maxCount, more);
		}

		/// <summary>
		/// Finds and replaces all match instances of PCRE regular expression <paramref name="rx"/>.
		/// Returns the number of replacements made.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Replace(string, string, out string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static int RegexReplace_(this string s, string rx, string repl, out string result, int maxCount = -1, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.Replace(s, repl, out result, maxCount, more);
		}

		/// <summary>
		/// Finds and replaces all match instances of PCRE regular expression <paramref name="rx"/>. Uses a callback function.
		/// Returns the result string.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Replace(string, Func{RXMatch, string}, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static string RegexReplace_(this string s, string rx, Func<RXMatch, string> replFunc, int maxCount = -1, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.Replace(s, replFunc, maxCount, more);
		}

		/// <summary>
		/// Finds and replaces all match instances of PCRE regular expression <paramref name="rx"/>. Uses a callback function.
		/// Returns the number of replacements made.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Replace(string, Func{RXMatch, string}, out string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static int RegexReplace_(this string s, string rx, Func<RXMatch, string> replFunc, out string result, int maxCount = -1, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.Replace(s, replFunc, out result, maxCount, more);
		}

		/// <summary>
		/// Returns array of substrings delimited by PCRE regular expression <paramref name="rx"/> matches.
		/// Parameters etc are of <see cref="Regex_(string, RXFlags)"/> and <see cref="Regex_.Split(string, int, RXMore)"/>.
		/// Examples in <see cref="Regex_"/> class help.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid regular expression. Or used a PARTIAL_ flag.</exception>
		/// <exception cref="AuException">Failed (unlikely).</exception>
		public static string[] RegexSplit_(this string s, string rx, int maxCount = 0, RXFlags flags = 0, RXMore more = null)
		{
			var x = _cache.AddOrGet(rx, flags);
			return x.Split(s, maxCount, more);
		}

		static _RegexCache _cache = new _RegexCache();

		//Cache of compiled regular expressions.
		//Can make ~10 times faster when the subject string is short.
		//The algorithm is from .NET Regex source code.
		class _RegexCache
		{
			struct _RXCode
			{
				public string regex;
				public Regex_ code; //note: could instead cache only PCRE code (LPARAM), but it makes quite difficult
				public RXFlags flags;
			}

			LinkedList<_RXCode> _list = new LinkedList<_RXCode>();
			const int c_maxCount = 15;

			/// <summary>
			/// If rx/flags is in the cache, returns the cached code.
			/// Else compiles rx/flags, adds to the cache and returns the code.
			/// </summary>
			/// <param name="rx"></param>
			/// <param name="flags"></param>
			/// <exception cref="ArgumentException">Invalid regular expression. Or failed to compile it for some other reason.</exception>
			public Regex_ AddOrGet(string rx, RXFlags flags)
			{
				lock(this) {
					int len = rx.Length;
					for(var x = _list.First; x != null; x = x.Next) {
						var v = x.Value.regex;
						if(v.Length == len && v == rx && x.Value.flags == flags) {
							if(x != _list.First) {
								_list.Remove(x);
								_list.AddFirst(x);
							}
							return x.Value.code;
						}
					}
					{
						var code = new Regex_(rx, flags);

						var x = new _RXCode() { code = code, regex = rx, flags = flags };
						_list.AddFirst(x);
						if(_list.Count > c_maxCount) _list.RemoveLast();
						//note: now cannot free the PCRE code, because another thread may be using it. GC will do it safely.

						return code;
					}
				}
			}
		}
	}

	#endregion
}
