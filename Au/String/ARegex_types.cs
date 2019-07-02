using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

using System.Text.RegularExpressions; //for XML doc links

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Types
{
	/// <summary>
	/// Regular expression match info.
	/// Used with <see cref="ARegex"/> class functions and String extension methods like <see cref="AExtString.RegexMatch"/>.
	/// </summary>
	/// <remarks>
	/// Contains info about a regular expression match found in the subject string: index, length, substring, etc.
	/// Also contains an array of group matches, as <see cref="RXGroup"/>. Groups are regular expression parts enclosed in (), except (?...).
	/// Group matches can be accessed like array elements. Group 0 is whole match. Group 1 is the first group. See examples.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// var s = "ab cd-45-ef gh";
	/// if(s.RegexMatch(@"\b([a-z]+)-(\d+)\b", out RXMatch m))
	/// 	Print(
	/// 		m.GroupCountPlusOne, //3 (whole match and 2 groups)
	/// 		m.Index, //3, same as m[0].Index
	/// 		m.Value, //"cd-45-ef", same as m[0].Value
	/// 		m[1].Index, //3
	/// 		m[1].Value, //"cd"
	/// 		m[2].Index, //6
	/// 		m[2].Value //"45"
	/// 		);
	/// ]]></code>
	/// A group in the subject string may not exist even if whole match found. Then its Exists property is false, Index -1, Length 0, Value null.
	/// <code><![CDATA[
	/// var s = "ab cd--ef gh";
	/// if(s.RegexMatch(@"\b([a-z]+)-(\d+)?-([a-z]+)\b", out RXMatch m))
	/// 	Print(
	/// 		m.GroupCountPlusOne, //4 (whole match and 3 groups)
	/// 		m[2].Exists, //false
	/// 		m[2].Index, //-1
	/// 		m[2].Length, //0
	/// 		m[2].Value //null
	/// 		);
	/// ]]></code>
	/// </example>
	public unsafe class RXMatch
	{
		internal RXMatch(ARegex regex, string subject, int rc, in Cpp.RegexMatch k)
		{
			if(k.mark != null) Mark = new string(k.mark, 0, k.mark[-1]);
			if(rc < 0) return;
			Exists = true;
			IsPartial = rc == 0;
			IndexNoK = k.indexNoK;
			_regex = regex;
			//_subject = subject;

			var g = _groups = new RXGroup[k.vecCount];
			var v = k.vec;
			for(int i = 0; i < g.Length; i++) {
				g[i] = new RXGroup(subject, v[i]);
			}
		}

		ARegex _regex;
		//string _subject;
		RXGroup[] _groups;

		/// <summary>
		/// The number of groups in the regular expression, + 1 for the whole match.
		/// </summary>
		public int GroupCountPlusOne => _groups.Length;

		/// <summary>
		/// Start offset of the match in the subject string. The same as that of group 0.
		/// </summary>
		public int Index => _groups[0].Index;

		/// <summary>
		/// <see cref="Index"/> + <see cref="Length"/>. The same as that of group 0.
		/// </summary>
		public int EndIndex => _groups[0].EndIndex;

		/// <summary>
		/// Length of the match in the subject string. The same as that of group 0.
		/// </summary>
		public int Length => _groups[0].Length;

		/// <summary>
		/// The match (substring) in the subject string. The same as that of group 0.
		/// </summary>
		public string Value => _groups[0].Value;

		/// <summary>
		/// The subject string in which this match was found.
		/// </summary>
		public string Subject => _groups[0].LibSubject;

		/// <summary>
		/// Returns <see cref="Value"/>.
		/// </summary>
		public override string ToString() => Value;

		/// <summary>
		/// Start offset of whole match regardless of \K.
		/// When the regular expression contains \K, this is different (less) than <see cref="Index"/>.
		/// </summary>
		public int IndexNoK { get; private set; }

		/// <summary>
		/// The name of a found mark, or null.
		/// Marks can be inserted in regular expression pattern like (*MARK:name) or (*:name).
		/// After a full successful match, it is the last mark encountered on the matching path through the pattern. After a "no match" or a partial match, it is the last encountered mark. For example, consider this pattern: "^(*MARK:A)((*MARK:B)a|b)c". When it matches "bc", the mark is A. The B mark is "seen" in the first branch of the group, but it is not on the matching path. On the other hand, when this pattern fails to match "bx", the mark is B.
		/// </summary>
		public string Mark { get; private set; }

		/// <summary>
		/// Gets the return value of the <see cref="ARegex.Match"/> call.
		/// Can be false only when the function returned false but a mark is available (see <see cref="Mark"/>). Otherwise, when the function returns flase, it returns null instead of a RXMatch object.
		/// When false, all properties except Exists and Mark have undefined values or throw exception.
		/// </summary>
		public bool Exists { get; private set; }

		/// <summary>
		/// Returns true if this match is partial.
		/// Partial match is possible if used a PARTIAL_ flag.
		/// </summary>
		public bool IsPartial { get; private set; }

		/// <summary>
		/// Gets group info. Index 0 is whole match. Index 1 is the first group.
		/// </summary>
		/// <param name="group">1-based group index, or 0 for whole match.</param>
		/// <exception cref="IndexOutOfRangeException">The group index is &lt; 0 or &gt;= <see cref="GroupCountPlusOne"/>.</exception>
		public ref RXGroup this[int group] => ref _groups[group];

		/// <summary>
		/// Gets group info of a named group.
		/// </summary>
		/// <param name="groupName">
		/// Group name.
		/// In regular expression, to set name of group <c>(text)</c>, use <c>(?&lt;NAME&gt;text)</c>.
		/// </param>
		/// <exception cref="ArgumentException">Unknown group name.</exception>
		/// <remarks>
		/// If more than 1 group have this name, prefers the first group that matched (<see cref="RXGroup.Exists"/> is true).
		/// </remarks>
		public ref RXGroup this[string groupName]
		{
			get
			{
				int i = GroupNumberFromName(groupName);
				if(i < 0) throw new ArgumentException("Unknown group name.");
				return ref _groups[i];
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
		/// <remarks>
		/// If more than 1 group have this name, prefers the first group that matched (<see cref="RXGroup.Exists"/> is true).
		/// </remarks>
		/// <seealso cref="ARegex.GroupNumberFromName"/>
		public int GroupNumberFromName(string groupName)
		{
			if(groupName == null) throw new ArgumentNullException();
			fixed (char* p = groupName) return LibGroupNumberFromName(p, groupName.Length, out _);
		}

		/// <summary>
		/// Finds a named group and returns its 1-based index.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="groupName">
		/// Group name.
		/// In regular expression, to set name of group <c>(text)</c>, use <c>(?&lt;NAME&gt;text)</c>.
		/// </param>
		/// <param name="notUnique">Receives true if more than 1 group have this name.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <remarks>
		/// If more than 1 group have this name, prefers the first group that matched (<see cref="RXGroup.Exists"/> is true).
		/// </remarks>
		/// <seealso cref="ARegex.GroupNumberFromName"/>
		public int GroupNumberFromName(string groupName, out bool notUnique)
		{
			if(groupName == null) throw new ArgumentNullException();
			fixed (char* p = groupName) return LibGroupNumberFromName(p, groupName.Length, out notUnique);
		}

		//Used by ARegex.ReplaceAll to avoid repl.Substring.
		internal int LibGroupNumberFromName(char* s, int len, out bool notUnique)
		{
			notUnique = false;
			if(len > 32 || len < 1) return -1;
			int step; ushort* first, last;
			if(s[len] == '\0') {
				step = Cpp.pcre2_substring_nametable_scan(_regex._CodeHR, s, &first, &last);
			} else {
				var p = stackalloc char[33];
				int i; for(i = 0; i < len; i++) p[i] = s[i]; p[i] = '\0';
				step = Cpp.pcre2_substring_nametable_scan(_regex._CodeHR, p, &first, &last);
			}
			if(step <= 0) return -1;
			int R = 0;
			notUnique = last > first;
			for(; first <= last; first += step) {
				int r = *first;
				if(_groups[r].Index >= 0) return r; //return the first that is set
				if(R == 0) R = r; //if none is set, return the first
			}
			return R;
		}

		/// <summary>
		/// Returns the expanded version of the specified replacement pattern.
		/// </summary>
		/// <param name="repl">
		/// Replacement pattern.
		/// Can consist of any combination of literal text and substitutions like $1.
		/// Supports .NET regular expression substitution syntax. See <see cref="Regex.Replace(string, string, int)"/>. Also replaces $* with the name of the last encountered mark.
		/// </param>
		/// <remarks>
		/// Works like <see cref="Match.Result"/>.
		/// See also: <see cref="ARegex.Replace(string, string, int, RXMore)"/>.
		/// </remarks>
		public string ExpandReplacement(string repl)
		{
			if(Empty(repl)) return repl;
			using(new Util.LibStringBuilder(out var b)) {
				ARegex.LibExpandReplacement(this, repl, b);
				return b.ToString();
			}
		}
	}

	/// <summary>
	/// Regular expression group match info.
	/// Used with <see cref="RXMatch"/>, <see cref="ARegex"/> and some String extension methods.
	/// </summary>
	/// <remarks>
	/// Groups are regular expression parts enclosed in (). Except non-capturing parts, like (?:...) and (?options). A RXGroup variable contains info about a group found in the subject string: index, length, substring.
	/// 
	/// Some groups specified in regular expression may not exist in the subject string even if it matches the regular expression. For example, regular expression "A(\d+)?B" matches string "AB", but group (\d+) does not exist. Then <see cref="Exists"/> is false, <see cref="Index"/> -1, <see cref="Length"/> 0, <see cref="Value"/> null.
	/// 
	/// When a group matches multiple times, the RXGroup variable contains only the last instance. For example, if subject is <c>"begin 12 345 67 end"</c> and regular expression is <c>(\d+ )+</c>, value of group 1 is <c>"67"</c>. If you need all instances (<c>"12"</c>, <c>"345"</c>, <c>"67"</c>), instead use .NET <see cref="Regex"/> and <see cref="Group.Captures"/>. Also you can get all instances with <see cref="ARegex.Callout"/>.
	/// 
	/// Examples and more info: <see cref="RXMatch"/>, <see cref="ARegex"/>.
	/// </remarks>
	public struct RXGroup
	{
		readonly string _subject;
		readonly int _index; //offset in _subject, or -1 if this group does not exist
		readonly int _len; //length, or 0 if this group does not exist

		internal RXGroup(string subject, int start, int end)
		{
			_subject = subject;
			_index = start;
			_len = end - start; //note: can be <0 if (?=...\K). It's OK.
		}

		internal RXGroup(string subject, POINT p)
		{
			_subject = subject;
			_index = p.x;
			_len = p.y - p.x; //note: can be <0 if (?=...\K). It's OK.
		}

		/// <summary>
		/// Start offset of the group match in the subject string.
		/// </summary>
		public int Index => _index;

		/// <summary>
		/// <see cref="Index"/> + <see cref="Length"/>.
		/// </summary>
		public int EndIndex => _index + _len;

		/// <summary>
		/// Length of the group match in the subject string.
		/// </summary>
		public int Length => _len;

		/// <summary>
		/// String value of the group match in the subject string.
		/// </summary>
		public string Value
		{
			get
			{
				if(_len > 0) return _subject.Substring(_index, _len);
				return _index < 0 ? null : "";
			}
		}

		internal string LibSubject => _subject;

		/// <summary>
		/// Returns true if the group exists in the subject string, false if does not exist.
		/// More info in <see cref="RXGroup"/> topic. Example in <see cref="RXMatch"/> topic.
		/// </summary>
		/// <remarks>
		/// Other ways to detect it: if a group does not exist, its Index is -1 and Value is null.
		/// </remarks>
		public bool Exists => _index >= 0;

		/// <summary>
		/// Returns <see cref="Value"/>.
		/// </summary>
		public override string ToString() => Value;
	}

	/// <summary>
	/// Rarely used parameters for <see cref="ARegex"/> class functions.
	/// </summary>
	/// <remarks>
	/// The constructor allows you to set initial values of fields. You can modify them later if need. ARegex functions don't modify them.
	/// 
	/// The start/end fields can be used to specify part of subject string. When a function has parameter <i>group</i>, the start/end fields don't depend on it; they are used to specify where to search for whole match.
	/// </remarks>
	public class RXMore
	{
		/// <summary>
		/// The start index (offset) in the subject string.
		/// Default 0. Valid values are from 0 to (including) subject length.
		/// The subject part before it is not ignored if regular expression starts with a lookbehind assertion or anchor, eg <c>^</c> or <c>\b</c> or <c>(?&lt;=...)</c>. Instead of <c>^</c> you can use <c>\G</c>. More info in PCRE documentation topic <see href="https://www.pcre.org/current/doc/html/pcre2api.html">pcre2api</see>, chapter "The string to be matched by pcre2_match()".
		/// </summary>
		public int start;

		/// <summary>
		/// The end index (offset) in the subject string. As if the string ends here.
		/// If negative (default -1), is used subject string length. Else valid values are from <see cref="start"/> to (including) subject length.
		/// </summary>
		public int end;

		/// <summary>
		/// Options.
		/// The same options also can be set when calling ARegex constructor. Constructor's flags and matchFlags are added, which means that matchFlags cannot unset flags set when calling constructor.
		/// </summary>
		public RXMatchFlags matchFlags;

		/// <summary>
		/// Sets field values.
		/// If <i>end</i> is -1 (default), will be used subject string length.
		/// </summary>
		public RXMore(int start = 0, int end = -1, RXMatchFlags matchFlags = 0)
		{
			this.start = start; this.end = end; this.matchFlags = matchFlags;
		}

		private RXMore() { }
	}

	#region callout

	/// <summary>
	/// Delegate type of callout callback function.
	/// See <see cref="ARegex.Callout"/>.
	/// </summary>
	public delegate void RXCalloutFunc(RXCalloutData b);

	/// <summary>
	/// Managed version of PCRE API struct pcre2_callout_block.
	/// When you set <see cref="ARegex.Callout"/>, your callout function's parameter is of this type.
	/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
	/// Most properties are pcre2_callout_block fields as documented in PCRE help. Other properties and methods are easier/safer versions of unsafe fields like offset_vector.
	/// </summary>
	public unsafe struct RXCalloutData
	{
#pragma warning disable 649 //field never assigned
		struct pcre2_callout_block
		{
			public int version;
			public readonly int callout_number, capture_top, capture_last;
			public readonly LPARAM* vec;
			public readonly char* mark, subject;
			public readonly LPARAM subject_length;
			public readonly LPARAM start_match;
			public readonly LPARAM current_position;
			public readonly LPARAM pattern_position;
			public readonly LPARAM next_item_length;
			public readonly LPARAM callout_string_offset;
			public readonly LPARAM callout_string_length;
			public readonly char* callout_string;
			public readonly int callout_flags;
		}
#pragma warning restore 649

		//We use pointer instead of adding pcre2_callout_block fields to this struct. Other ways are not good:
		//	Passing whole block to the final callback by value is slow (104 bytes, tested speed). Also then cannot have Result like now.
		//	With 'in' fast, but then users have to declare lambda parameters like 'in RXCalloutData d'. Now just 'd'.
		pcre2_callout_block* _p;

		/// <summary>
		/// Sets the return value of the callout function, as documented in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// Default 0.
		/// If 1, matching fails at the current point, but the testing of other matching possibilities goes ahead, just as if a lookahead assertion had failed.
		/// If -1 (PCRE2_ERROR_NOMATCH), the match function returns false (no match). Values less tan -2 are PCRE error codes and cause exception.
		/// </summary>
		public int Result { set => _p->version = value; internal get => _p->version; }

		internal RXCalloutData(void* calloutBlock)
		{
			_p = (pcre2_callout_block*)calloutBlock;
			Result = 0;
		}

		/// <summary>
		/// Callout number, eg 5 for "(?C5)".
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int callout_number => _p->callout_number;

		/// <summary>
		/// One more than the number of the highest numbered captured group so far.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int capture_top => _p->capture_top;

		/// <summary>
		/// The number of the most recently captured group.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int capture_last => _p->capture_last;

		/// <summary>
		/// Flags.
		/// 1 PCRE2_CALLOUT_STARTMATCH, 2 PCRE2_CALLOUT_BACKTRACK.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int callout_flags => _p->callout_flags;

		/// <summary>
		/// The offset within the subject string at which the current match attempt started. But depends on \K etc.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int start_match => (int)_p->start_match;

		/// <summary>
		/// The current offset within the subject string.
		/// </summary>
		public int current_position => (int)_p->current_position;

		/// <summary>
		/// The offset in the regular expression to the next item to be matched.
		/// </summary>
		public int pattern_position => (int)_p->pattern_position;

		/// <summary>
		/// The length of the next item to be processed in the regular expression.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int next_item_length => (int)_p->next_item_length;

		/// <summary>
		/// The callout string offset in the regular expression. Used with callouts like "(?C'calloutString')".
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public int callout_string_offset => (int)_p->callout_string_offset;

		/// <summary>
		/// The callout string, eg "xyz" for "(?C'xyz')".
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public string callout_string => _p->callout_string == null ? null : Util.AStringCache.LibAdd(_p->callout_string, (int)_p->callout_string_length);

		/// <summary>
		/// The most recently passed (*MARK), (*PRUNE), or (*THEN) item in the match, or null if no such items have been passed.
		/// More info in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2callout.html">pcre2callout</see>.
		/// </summary>
		public string mark => _p->mark == null ? null : Util.AStringCache.LibAdd(_p->mark);

		/// <summary>
		/// Gets the start index and length of the specified group in the subject string.
		/// </summary>
		/// <param name="group">Group number (1-based index).</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>group</i> must be &gt; 0 and &lt; capture_top.</exception>
		public (int index, int length) Group(int group)
		{
			if(group <= 0 || group >= _p->capture_top) throw new ArgumentOutOfRangeException(nameof(group), "Must be > 0 and < capture_top.");
			var v = _p->vec;
			int i = (int)v[group *= 2];
			return (i, (int)v[group + 1] - i);
		}

		/// <summary>
		/// Gets the value (substring) of the specified group.
		/// </summary>
		/// <param name="group">Group number (1-based index).</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>group</i> must be &gt; 0 and &lt; capture_top.</exception>
		public string GroupValue(int group)
		{
			var (i, len) = Group(group);
			if(i < 0) return null;
			if(len == 0) return "";
			return new string(_p->subject, i, len);
		}

		/// <summary>
		/// Gets the start index and length of the most recently captured group in the subject string.
		/// </summary>
		public (int index, int length) LastGroup => Group(_p->capture_last);

		/// <summary>
		/// Gets the value (substring) of the most recently captured group.
		/// </summary>
		public string LastGroupValue => GroupValue(_p->capture_last);
	}

	#endregion

#pragma warning disable 1591 //no XML doc
	/// <summary>
	/// Flags for <see cref="ARegex"/> constructor.
	/// Documented in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2api.html">pcre2api</see>.
	/// </summary>
	/// <remarks>
	/// Many options also can be specified in regular expression (RE):
	/// - These can be anywhere in RE: (?i) CASELESS, (?m) MULTILINE, (?s) DOTALL, (?n) NO_AUTO_CAPTURE, (?x) EXTENDED, (?xx) EXTENDED_MORE, (?J) DUPNAMES, (?U) UNGREEDY. Can be multiple, like (?ms). Can be unset, like (?-i). RE "\Qtext\E" is like RE "text" with flag LITERAL.
	/// - Instead of ANCHORED can be used \A or \G at the start of RE. Or ^, except in multiline mode.
	/// - Instead of ENDANCHORED can be used \z at the end of RE. Or $, except in multiline mode.
	/// - Flag UTF is implicitly added if RE contains non-ASCII characters and there is no flag NEVER_UTF.
	/// - These must be at the very start and are named like flags: (*UTF), (*UCP), (*NOTEMPTY), (*NOTEMPTY_ATSTART), (*NO_AUTO_POSSESS), (*NO_DOTSTAR_ANCHOR), (*NO_START_OPT).
	/// - More info in <see href="https://www.pcre.org/current/doc/html/pcre2pattern.html">PCRE syntax reference</see>.
	/// 
	/// Some of RXFlags flags also exist in <see cref="RXMatchFlags"/>. You can set them either when calling ARegex constructor or when calling ARegex functions that have parameter <i>more</i>. You can use different flags for each function call with the same ARegex variable.
	/// </remarks>
	[Flags]
	public enum RXFlags :ulong
	{
		ANCHORED = 0x80000000,
		ENDANCHORED = 0x20000000,
		NO_UTF_CHECK = 0x40000000,

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

		/// <summary>
		/// Fully support Unicode text (case-insensitivity etc). More info in PCRE documentation topic <see href="https://www.pcre.org/current/doc/html/pcre2unicode.html">pcre2unicode</see>.
		/// This flag is implicitly added if regular expression contains non-ASCII characters and there is no flag NEVER_UTF.
		/// </summary>
		UTF = 0x00080000,

		NEVER_BACKSLASH_C = 0x00100000,
		ALT_CIRCUMFLEX = 0x00200000,
		ALT_VERBNAMES = 0x00400000,
		//USE_OFFSET_LIMIT = 0x00800000, //used with pcre2_set_offset_limit(), but currently we don't support it
		EXTENDED_MORE = 0x01000000,
		LITERAL = 0x02000000,

		//PCRE2_EXTRA_ flags.

		//ALLOW_SURROGATE_ESCAPES = 0x1L << 32, //not used with UTF-16
		//BAD_ESCAPE_IS_LITERAL = 0x2L << 32, //dangerous
		MATCH_WORD = 0x4L << 32,
		MATCH_LINE = 0x8L << 32,

		//Match API flags. ARegex ctor moves them to a field that later is combined with RXMatchFlags when calling the match API.

		NOTBOL = 0x00000001L << 56, //hi byte of long
		NOTEOL = 0x00000002L << 56,
		NOTEMPTY = 0x00000004L << 56,
		NOTEMPTY_ATSTART = 0x00000008L << 56,
		PARTIAL_SOFT = 0x00000010L << 56,
		PARTIAL_HARD = 0x00000020L << 56,
	}

	/// <summary>
	/// Flags for <see cref="ARegex.Match"/> and other <see cref="ARegex"/> class functions.
	/// Documented in PCRE help topic <see href="https://www.pcre.org/current/doc/html/pcre2api.html">pcre2api</see>.
	/// </summary>
	/// <remarks>
	/// These flags also exist in <see cref="RXFlags"/> (ARegex constructor flags). You can set them either when calling constructor or when calling other functions.
	/// </remarks>
	[Flags]
	public enum RXMatchFlags :uint
	{
		//These are the same as in RXFlags, and can be used either when compiling or when matching.

		ANCHORED = 0x80000000,
		ENDANCHORED = 0x20000000,
		NO_UTF_CHECK = 0x40000000,

		//These are only for matching. Also added to the hi int of RXFlags.

		NOTBOL = 0x00000001,
		NOTEOL = 0x00000002,
		NOTEMPTY = 0x00000004,
		NOTEMPTY_ATSTART = 0x00000008,
		PARTIAL_SOFT = 0x00000010,
		PARTIAL_HARD = 0x00000020,
	}
#pragma warning restore 1591

	internal static unsafe partial class Cpp
	{
		/// <summary>This and related API are documented in the C++ dll project.</summary>
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern LPARAM Cpp_RegexCompile(string rx, LPARAM len, RXFlags flags, out int codeSize, out BSTR errStr);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_RegexDtor(IntPtr code);

		/// <summary>This and related API are documented in the C++ dll project.</summary>
		internal struct RegexMatch //note: 'ref struct' crashes VS
		{
			public POINT* vec;
			public int vecCount;
			public int indexNoK;
			public char* mark;
		}

		internal unsafe delegate int PcreCalloutT(void* calloutBlock, void* param);

		/// <summary>This and related API are documented in the C++ dll project.</summary>
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_RegexMatch(HandleRef code, string s, LPARAM len, LPARAM start, RXMatchFlags flags,
			PcreCalloutT callout, out RegexMatch m, out BSTR errStr);
		//note: don't use [MarshalAs(UnmanagedType.BStr)] out string errStr, it makes much slower.

		//This overload allows to pass m null. Using 2 overloads makes programming simpler.
		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_RegexMatch(HandleRef code, string s, LPARAM len, LPARAM start, RXMatchFlags flags,
			PcreCalloutT callout, RegexMatch* m, out BSTR errStr);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Cpp_RegexSubstitute(HandleRef code, string s, LPARAM len, LPARAM start, PCRE2_SUBSTITUTE_ flags,
			string repl, LPARAM rlen, [Out] char[] outputbuffer, ref LPARAM outlen,
			PcreCalloutT callout, out BSTR errStr);

		#region PCRE API

		//internal enum PCRE2_ERROR_
		//{
		//	PARTIAL = 0, //note: the PCRE API value is -2, but our C++ dll API then returns 0
		//	NOMATCH = -1,
		//	CALLOUT = -37,
		//	NOMEMORY = -48,
		//	NOUNIQUESUBSTRING = -50,
		//	//others not useful
		//}

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int pcre2_pattern_info(HandleRef code, PCRE2_INFO_ what, void* where);

		[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int pcre2_substring_nametable_scan(HandleRef code, char* name, ushort** first, ushort** last);

		internal enum PCRE2_SUBSTITUTE_
		{
			EXTENDED = 0x00000200,
			GLOBAL = 0x00000100,
			OVERFLOW_LENGTH = 0x00001000,
			UNKNOWN_UNSET = 0x00000800,
			UNSET_EMPTY = 0x00000400,
		}

		internal enum PCRE2_INFO_
		{
			ALLOPTIONS = 0,
			//ARGOPTIONS = 1,
			//BACKREFMAX = 2,
			//BSR = 3,
			//CAPTURECOUNT = 4,
			//FIRSTCODEUNIT = 5,
			//FIRSTCODETYPE = 6,
			//FIRSTBITMAP = 7,
			//HASCRORLF = 8,
			//JCHANGED = 9,
			//JITSIZE = 10,
			//LASTCODEUNIT = 11,
			//LASTCODETYPE = 12,
			//MATCHEMPTY = 13,
			//MATCHLIMIT = 14,
			//MAXLOOKBEHIND = 15,
			//MINLENGTH = 16,
			//NAMECOUNT = 17,
			//NAMEENTRYSIZE = 18,
			//NAMETABLE = 19,
			//NEWLINE = 20,
			//DEPTHLIMIT = 21,
			//SIZE = 22,
			//HASBACKSLASHC = 23,
			//FRAMESIZE = 24,
			//HEAPLIMIT = 25,
		}

		#endregion
	}
}
