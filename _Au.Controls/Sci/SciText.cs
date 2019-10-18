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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.AStatic;
using Au.Util;

//rejected: throw exception if invalid position. Often difficult to detect it. And this library is not public.
//	Only few methods throw. They are used for long time and never threw.

namespace Au.Controls
{
	using static Sci;

	/// <summary>
	/// Functions to work with Scintilla control text, code, etc.
	/// A SciText object is created by AuScintilla which is the C property.
	/// </summary>
	public unsafe partial class SciText
	{
		/// <summary>
		/// The host AuScintilla control.
		/// </summary>
		public AuScintilla C { get; private set; }

		[ThreadStatic] static WeakReference<byte[]> t_byte;

		internal static byte[] LibByte(int n) => AMemoryArray.Get(n, ref t_byte);
		//these currently not used
		//internal static AMemoryArray.ByteBuffer LibByte(ref int n) { var r = AMemoryArray.Get(n, ref t_byte); n = r.Length - 1; return r; }
		//internal static AMemoryArray.ByteBuffer LibByte(int n, out int nHave) { var r = AMemoryArray.Get(n, ref t_byte); nHave = r.Length - 1; return r; }


		internal SciText(AuScintilla sc)
		{
			C = sc;
		}

		#region low level

		/// <summary>
		/// Sends a Scintilla message to the control and returns LPARAM.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public LPARAM CallRetPtr(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) => C.CallRetPtr(sciMessage, wParam, lParam);

		/// <summary>
		/// Sends a Scintilla message to the control and returns int.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public int Call(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) => C.Call(sciMessage, wParam, lParam);

		/// <summary>
		/// Calls a Scintilla message that sets a string which is passed using lParam.
		/// The string can be null if the Scintilla message allows it.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public int SetString(int sciMessage, LPARAM wParam, string lParam, bool useUtf8LengthForWparam = false)
		{
			fixed(byte* s = _ToUtf8(lParam, out var len)) {
				if(useUtf8LengthForWparam) wParam = len;
				return C.Call(sciMessage, wParam, s);
			}
		}

		/// <summary>
		/// Calls a Scintilla message that sets a string which is passed using wParam.
		/// The string can be null if the Scintilla message allows it.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public int SetString(int sciMessage, string wParam, LPARAM lParam)
		{
			fixed(byte* s = _ToUtf8(wParam)) {
				return C.Call(sciMessage, lParam, s);
			}
		}

		/// <summary>
		/// Calls a Scintilla message and passes two strings using wParam and lParam.
		/// wParam0lParam must be like "WPARAM\0LPARAM". Asserts if no '\0'.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public int SetStringString(int sciMessage, string wParam0lParam)
		{
			fixed(byte* s = _ToUtf8(wParam0lParam, out var len)) {
				int i = LibCharPtr.Length(s);
				Debug.Assert(i < len);
				return C.Call(sciMessage, s, s + i + 1);
			}
		}

		/// <summary>
		/// Calls a Scintilla message that gets a string.
		/// Don't call this function from another thread.
		/// </summary>
		/// <param name="sciMessage"></param>
		/// <param name="wParam"></param>
		/// <param name="bufferSize">
		/// How much UTF-8 bytes to allocate for Scintilla to store the text.
		/// If -1 (default), at first calls sciMessage with lParam=0 (null buffer), let it return required buffer size. Then it can get binary string (with '\0' characters).
		/// If 0, returns "" and does not call the message.
		/// If positive, it can be either known or max expected text length, without the terminating '\0' character. The function will find length of the retrieved string (finds '\0'). Then it cannot get binary string (with '\0' characters).
		/// The function allocates bufferSize+1 bytes and sets that last byte = 0. If Scintilla overwrites it, asserts and calls Environment.FailFast.
		/// </param>
		public string GetString(int sciMessage, LPARAM wParam, int bufferSize = -1)
		{
			if(bufferSize < 0) return GetStringOfLength(sciMessage, wParam, Call(sciMessage, wParam));
			if(bufferSize == 0) return "";
			fixed(byte* b = LibByte(bufferSize)) {
				b[bufferSize] = 0;
				Call(sciMessage, wParam, b);
				Debug.Assert(b[bufferSize] == 0);
				int len = LibCharPtr.Length(b, bufferSize);
				return _FromUtf8(b, len);
			}
		}

		/// <summary>
		/// The same as <see cref="GetString(int, LPARAM, int)"/>, but always uses utf8Length bytes of the result (does not find length).
		/// </summary>
		/// <param name="sciMessage"></param>
		/// <param name="wParam"></param>
		/// <param name="utf8Length">
		/// Known length (bytes) of the result UTF-8 string, without the terminating '\0' character.
		/// If 0, returns "" and does not call the message.
		/// Cannot be negative.
		/// </param>
		/// <remarks>
		/// This function can get binary string (with '\0' characters).
		/// </remarks>
		public string GetStringOfLength(int sciMessage, LPARAM wParam, int utf8Length)
		{
			if(utf8Length == 0) return "";
			fixed(byte* b = LibByte(utf8Length)) {
				b[utf8Length] = 0;
				Call(sciMessage, wParam, b);
				Debug.Assert(b[utf8Length] == 0);
				return _FromUtf8(b, utf8Length);
			}
		}

		string _FromUtf8(byte* b, int n = -1)
		{
			return AConvert.Utf8ToString(b, n);
		}

		byte[] _ToUtf8(string s)
		{
			return AConvert.Utf8FromString(s);
		}

		byte[] _ToUtf8(string s, out int utf8Length)
		{
			var r = AConvert.Utf8FromString(s);
			utf8Length = r == null ? 0 : r.Length - 1;
			return r;
		}

		/// <summary>
		/// Converts <i>from</i> and <i>to</i> from characters to UTF-8 bytes and/or <i>to</i> from length to absolute position.
		/// </summary>
		/// <param name="utf16">Input parameters are UTF-16.</param>
		/// <param name="from"></param>
		/// <param name="to">If -1, and !<i>toIsLength</i>, uses <see cref="TextLength8"/>.</param>
		/// <param name="toIsLength">to is length (position relative to from). Converts to absolute position.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void NormalizeRange(bool utf16, ref int from, ref int to, bool toIsLength = false)
		{
			if(from < 0 || to < (toIsLength ? 0 : -1) || (!toIsLength && to < from)) throw new ArgumentOutOfRangeException();
			if(utf16) from = CountBytesFromChars(from);
			if(to < 0) {
				to = TextLength8;
			} else if(utf16) {
				if(toIsLength) to = CountBytesFromChars(from, to);
				else to = CountBytesFromChars(to);
			} else {
				if(toIsLength) to += from;
			}
		}

		/// <summary>
		/// Converts position and length from UTF-16 to UTF-8.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="len"></param>
		void _NormalizeFromLen(ref int from, ref int len)
		{
			from = CountBytesFromChars(from);
			len = CountBytesFromChars(from, len) - from;
		}

		/// <summary>
		/// => utf16 ? CountBytesFromChars(pos) : pos;
		/// </summary>
		int _ParamPos(bool utf16, int pos) => utf16 ? CountBytesFromChars(pos) : pos;

		/// <summary>
		/// => utf16 ? CountBytesToChars(pos) : pos;
		/// </summary>
		int _ReturnPos(bool utf16, int pos) => utf16 ? CountBytesToChars(pos) : pos;

#if true
		/// <summary>
		/// Converts UTF-16 position to UTF-8 position.
		/// </summary>
		public int CountBytesFromChars(int charsPos) => charsPos > 0 ? Call(SCI_POSITIONRELATIVECODEUNITS, 0, charsPos) : 0;

		/// <summary>
		/// Counts a number of UTF-16 characters before or after the UTF-8 position and return that UTF-8 position.
		/// charsRelative can be negative.
		/// </summary>
		public int CountBytesFromChars(int bytesPos, int charsRelative) => Call(SCI_POSITIONRELATIVECODEUNITS, bytesPos, charsRelative);

		/// <summary>
		/// Converts UTF-8 position to UTF-16 position.
		/// </summary>
		public int CountBytesToChars(int bytesPos) => Call(SCI_COUNTCODEUNITS, 0, bytesPos);

		/// <summary>
		/// Returns the number of UTF-16 characters between two UTF-8 positions.
		/// </summary>
		public int CountBytesToChars(int bytesStart, int bytesEnd) => Call(SCI_COUNTCODEUNITS, bytesStart, bytesEnd);
#else //rejected: throw exception if invalid parameters. In many cases cannot know it because Scintilla does not return eg -1. And this library is not public.
		/// <summary>
		/// Converts UTF-16 position to UTF-8 position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int CountBytesFromChars(int charsPos)
		{
			int r = charsPos;
			if(charsPos > 0) {
				r = Call(SCI_POSITIONRELATIVECODEUNITS, 0, charsPos);
				if(r == 0) r = -1; //> text length
			}
			return r >= 0 ? r : throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// Counts the number of UTF-16 characters before or after the UTF-8 position and return that UTF-8 position.
		/// charsRelative can be negative.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int CountBytesFromChars(int bytesPos, int charsRelative)
		{
			int r = -1;
			if(bytesPos >= 0) {
				r = Call(SCI_POSITIONRELATIVECODEUNITS, bytesPos, charsRelative);
				if(r == 0 && charsRelative > 0) r = -1; //> text length. If charsRelative<0, we don't know if failed. Returns 0 if fails.
			}
			return r >= 0 ? r : throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// Converts UTF-8 position to UTF-16 position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int CountBytesToChars(int bytesPos)
		{
			if(bytesPos < 0) throw new ArgumentOutOfRangeException();
			return Call(SCI_COUNTCODEUNITS, 0, bytesPos); //if invalid, returns text length
		}

		/// <summary>
		/// Returns the number of UTF-16 characters between two UTF-8 positions.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int CountBytesToChars(int bytesStart, int bytesEnd)
		{
			int r = -1;
			if(bytesStart >= 0 && bytesEnd >= bytesStart) { //SCI_COUNTCODEUNITS returns 0 if bytesEnd<bytesStart
				r = Call(SCI_COUNTCODEUNITS, bytesStart, bytesEnd);
				if(r == 0 && bytesEnd > bytesStart) r = -1;
			}
			return r >= 0 ? r : throw new ArgumentOutOfRangeException();
		}
#endif

		struct _NoReadonly : IDisposable
		{
			SciText _t;
			bool _ro;

			public _NoReadonly(SciText t)
			{
				_t = t;
				_ro = _t.C.ZInitReadOnlyAlways || _t.IsReadonly;
				if(_ro) _t.C.Call(SCI_SETREADONLY, 0);
			}

			public void Dispose()
			{
				if(_ro) _t.C.Call(SCI_SETREADONLY, 1);
			}
		}

		struct _NoUndoNotif : IDisposable
		{
			SciText _t;
			bool _noUndo, _noNotif;

			public _NoUndoNotif(SciText t, SciSetTextFlags flags)
			{
				if(t.C.ZInitReadOnlyAlways) flags = 0;
				_t = t;
				_noUndo = flags.Has(SciSetTextFlags.NoUndo) && 0 != _t.Call(SCI_GETUNDOCOLLECTION);
				_noNotif = flags.Has(SciSetTextFlags.NoNotify);
				if(_noNotif) _t.C.ZDisableModifiedNotifications = true;
				if(_noUndo) _t.Call(SCI_SETUNDOCOLLECTION, false);
			}

			public void Dispose()
			{
				if(_noUndo) {
					_t.Call(SCI_EMPTYUNDOBUFFER);
					_t.Call(SCI_SETUNDOCOLLECTION, true);
				}
				if(_noNotif) _t.C.ZDisableModifiedNotifications = false;
			}
		}

		#endregion

		/// <summary>
		/// Removes all text (SCI_CLEARALL).
		/// </summary>
		/// <param name="flags"></param>
		public void ClearText(SciSetTextFlags flags = 0)
		{
			using(new _NoUndoNotif(this, flags))
			using(new _NoReadonly(this))
				Call(SCI_CLEARALL);
		}

		/// <summary>
		/// Replaces all text.
		/// </summary>
		/// <param name="s">Text.</param>
		/// <param name="flags"></param>
		/// <param name="ignoreTags">Don't parse tags, regardless of C.ZInitTagsStyle.</param>
		public void SetText(string s, SciSetTextFlags flags = 0, bool ignoreTags = false)
		{
			using(new _NoUndoNotif(this, flags)) {
				if(!ignoreTags && _CanParseTags(s)) {
					ClearText();
					C.ZTags.AddText(s, false, C.ZInitTagsStyle == AuScintilla.ZTagsStyle.AutoWithPrefix);
				} else {
					using(new _NoReadonly(this))
						SetString(SCI_SETTEXT, 0, s ?? "");
				}
			}
		}

		bool _CanParseTags(string s)
		{
			if(Empty(s)) return false;
			switch(C.ZInitTagsStyle) {
			case AuScintilla.ZTagsStyle.AutoAlways:
				return s.IndexOf('<') >= 0;
			case AuScintilla.ZTagsStyle.AutoWithPrefix:
				return s.Starts("<>");
			}
			return false;
		}

		///// <summary>
		///// Replaces all text.
		///// Does not parse tags.
		///// </summary>
		///// <param name="s">Text.</param>
		///// <param name="startIndex"></param>
		///// <param name="flags"></param>
		//public void SetTextUtf8(byte[] s, int startIndex = 0, SciSetTextFlags flags = 0)
		//{
		//	using(new _NoUndoNotif(this, flags)) {
		//		LibSetText(s, startIndex);
		//	}
		//}

		/// <summary>
		/// Sets UTF-8 text.
		/// Does not parse tags etc, just calls SCI_SETTEXT and SCI_SETREADONLY if need.
		/// s must end with 0. Asserts.
		/// </summary>
		internal void LibSetText(byte[] s, int startIndex)
		{
			Debug.Assert(s.Length > 0 && s[s.Length - 1] == 0);
			Debug.Assert((uint)startIndex < s.Length);
			fixed(byte* p = s) LibSetText(p + startIndex);
		}

		/// <summary>
		/// Sets UTF-8 text.
		/// Does not pare tags etc, just calls SCI_SETTEXT and SCI_SETREADONLY if need.
		/// </summary>
		internal void LibSetText(byte* s)
		{
			using(new _NoReadonly(this))
				Call(SCI_SETTEXT, 0, s);
		}

		/// <summary>
		/// Appends text and optionally "\r\n".
		/// Optionally scrolls and moves current position to the end (SCI_GOTOPOS).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="andRN">Also append "\r\n". Ignored if parses tags; then appends.</param>
		/// <param name="scroll">Move current position and scroll to the end. Ignored if parses tags; then moves/scrolls.</param>
		/// <param name="ignoreTags">Don't parse tags, regardless of C.TagsStyle.</param>
		public void AppendText(string s, bool andRN, bool scroll, bool ignoreTags = false)
		{
			if(!ignoreTags && _CanParseTags(s)) {
				C.ZTags.AddText(s, true, C.ZInitTagsStyle == AuScintilla.ZTagsStyle.AutoWithPrefix);
				return;
			}

			int n = AConvert.Utf8LengthFromString(s);
			fixed(byte* b = LibByte(n + 2)) {
				AConvert.Utf8FromString(s, b, n + 1);
				if(andRN) { b[n++] = (byte)'\r'; b[n++] = (byte)'\n'; }

				using(new _NoReadonly(this))
					Call(SCI_APPENDTEXT, n, b);
			}
			if(scroll) Call(SCI_GOTOPOS, TextLength8);
		}

		/// <summary>
		/// Appends UTF-8 text of specified length.
		/// Does not append newline (s should contain it). Does not parse tags. Moves current position and scrolls to the end.
		/// </summary>
		internal void LibAddText(bool append, byte* s, int lenToAppend)
		{
			using(new _NoReadonly(this))
				if(append) Call(SCI_APPENDTEXT, lenToAppend, s);
				else Call(SCI_SETTEXT, 0, s);

			if(append) Call(SCI_GOTOPOS, TextLength8);
		}

		//not used now
		///// <summary>
		///// Appends styled UTF-8 text of specified length.
		///// Does not append newline (s should contain it). Does not parse tags. Moves current position and scrolls to the end.
		///// Uses SCI_ADDSTYLEDTEXT. Caller does not have to move cursor to the end.
		///// lenToAppend is length in bytes, not in cells.
		///// </summary>
		//internal void LibAddStyledText(bool append, byte* s, int lenBytes)
		//{
		//	if(append) Call(SCI_SETEMPTYSELECTION, TextLengthBytes);

		//	using(new _NoReadonly(this))
		//	if(!append) Call(SCI_SETTEXT);
		//	Call(SCI_ADDSTYLEDTEXT, lenBytes, s);

		//	if(append) Call(SCI_GOTOPOS, TextLengthBytes);
		//}

		/// <summary>
		/// Gets all text.
		/// </summary>
		public string AllText()
		{
			int n = TextLength8;
			return GetStringOfLength(SCI_GETTEXT, n + 1, n);
		}

		/// <summary>
		/// Gets text length (SCI_GETTEXTLENGTH) in UTF-8 bytes.
		/// </summary>
		public int TextLength8 => Call(SCI_GETTEXTLENGTH);

		/// <summary>
		/// Gets text length (SCI_GETTEXTLENGTH) in UTF-16 bytes.
		/// </summary>
		public int TextLength16 => CountBytesToChars(TextLength8);

		/// <summary>
		/// Gets (SCI_GETCURRENTPOS) or sets (SCI_SETEMPTYSELECTION) current caret position in UTF-8 bytes.
		/// The 'set' function makes empty selection; does not scroll and does not make visible like GoToPos.
		/// </summary>
		public int CurrentPos8 { get => Call(SCI_GETCURRENTPOS); set => Call(SCI_SETEMPTYSELECTION, value); }

		/// <summary>
		/// Gets (SCI_GETCURRENTPOS) or sets (SCI_SETEMPTYSELECTION) current caret position in UTF-16 chars.
		/// The 'set' function makes empty selection; does not scroll and does not make visible like GoToPos.
		/// </summary>
		public int CurrentPos16 { get => CountBytesToChars(CurrentPos8); set => Call(SCI_SETEMPTYSELECTION, CountBytesFromChars(value)); }

		/// <summary>
		/// SCI_GETSELECTIONSTART UTF-8.
		/// </summary>
		public int SelectionStart8 => Call(SCI_GETSELECTIONSTART);

		/// <summary>
		/// SCI_GETSELECTIONSTART UTF-16.
		/// </summary>
		public int SelectionStar16 => CountBytesToChars(SelectionStart8);

		/// <summary>
		/// SCI_GETSELECTIONEND UTF-8.
		/// Always greater or equal than SelectionStart.
		/// </summary>
		public int SelectionEnd8 => Call(SCI_GETSELECTIONEND);

		/// <summary>
		/// SCI_GETSELECTIONEND UTF-16.
		/// Always greater or equal than SelectionStartChars.
		/// </summary>
		public int SelectionEnd16 => CountBytesToChars(SelectionEnd8);

		/// <summary>
		/// true if there is selected text.
		/// </summary>
		public bool IsSelection => 0 == Call(SCI_GETSELECTIONEMPTY);

		/// <summary>
		/// Gets line index from character position.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, returns the last line.</param>
		public int LineIndexFromPos(bool utf16, int pos)
			=> Call(SCI_LINEFROMPOSITION, _ParamPos(utf16, pos));

		/// <summary>
		/// Gets line start position from line index.
		/// </summary>
		/// <param name="utf16">Return UTF-16.</param>
		/// <param name="line">0-based line index.</param>
		public int LineStart(bool utf16, int line)
			=> _ReturnPos(utf16, Call(SCI_POSITIONFROMLINE, line));

		/// <summary>
		/// Gets line end position from line index.
		/// </summary>
		/// <param name="utf16">Return UTF-16.</param>
		/// <param name="line">0-based line index.</param>
		/// <param name="withRN">Include \r\n.</param>
		public int LineEnd(bool utf16, int line, bool withRN = false)
			=> _ReturnPos(utf16, withRN ? Call(SCI_POSITIONFROMLINE, line) + Call(SCI_LINELENGTH, line) : Call(SCI_GETLINEENDPOSITION, line));

		/// <summary>
		/// Gets line start position from any position.
		/// </summary>
		/// <param name="utf16">pos is UTF-16. Return UTF-16.</param>
		/// <param name="pos">A position in document text.</param>
		public int LineStartFromPos(bool utf16, int pos)
			=> LineStart(utf16, LineIndexFromPos(utf16, pos));

		/// <summary>
		/// Gets line start position from any position and gets line index.
		/// Returns start position.
		/// </summary>
		/// <param name="utf16">pos is UTF-16. Return UTF-16.</param>
		/// <param name="pos">A position in document text.</param>
		/// <param name="line">Receives line index.</param>
		public int LineStartFromPos(bool utf16, int pos, out int line)
			=> Call(SCI_POSITIONFROMLINE, line = Call(SCI_LINEFROMPOSITION, _ParamPos(utf16, pos)));

		/// <summary>
		/// Gets line end position from any position.
		/// </summary>
		/// <param name="utf16">pos is UTF-16. Return UTF-16.</param>
		/// <param name="pos">A position in document text.</param>
		/// <param name="withRN">Include \r\n.</param>
		/// <param name="lineStartIsLineEnd">If pos is at a line start (0 or after '\n' character), return pos.</param>
		public int LineEndFromPos(bool utf16, int pos, bool withRN = false, bool lineStartIsLineEnd = false)
		{
			int pos0 = pos;
			pos = _ParamPos(utf16, pos);
			if(lineStartIsLineEnd) {
				if(pos == 0 || Call(SCI_GETCHARAT, pos - 1) == '\n') return pos0;
			}
			return LineEnd(utf16, LineIndexFromPos(false, pos), withRN);
		}

		/// <summary>
		/// Gets line index, start and end positions from position.
		/// </summary>
		/// <param name="utf16">pos is UTF-16. Return UTF-16.</param>
		/// <param name="pos">A position in document text.</param>
		/// <param name="withRN">Include \r\n.</param>
		public (int line, int start, int end) LineStartEndFromPos(bool utf16, int pos, bool withRN = false)
		{
			int startPos = LineStartFromPos(false, _ParamPos(utf16, pos), out int line);
			int endPos = withRN ? startPos + Call(SCI_LINELENGTH, line) : Call(SCI_GETLINEENDPOSITION, line);
			return (line, _ReturnPos(utf16, startPos), _ReturnPos(utf16, endPos));
		}

		/// <summary>
		/// Gets line text.
		/// </summary>
		/// <param name="line">0-based line index. If invalid, returns "".</param>
		/// <param name="withRN">Include \r\n.</param>
		public string LineText(int line, bool withRN = false)
		{
			return RangeText(false, LineStart(false, line), LineEnd(false, line, withRN));
		}

		/// <summary>
		/// Gets line height.
		/// Currently all lines are the same height.
		/// </summary>
		public int LineHeight(int line = 0)
		{
			return Call(SCI_TEXTHEIGHT, line);
		}

		/// <summary>
		/// Gets the number of lines.
		/// </summary>
		public int LineCount => Call(SCI_GETLINECOUNT);

		/// <summary>
		/// Gets the number of tabs + spaces/4 at the start of the line that contains the specified position.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="pos">A position in document text.</param>
		/// <param name="extraSpaces">Receives the number of extra spaces, 0 to 3.</param>
		public int LineIndentationFromPos(bool utf16, int pos, out int extraSpaces)
		{
			int line = LineIndexFromPos(utf16, pos);
			int i = Call(SCI_GETLINEINDENTATION, line), r = i / 4;
			extraSpaces = i - r;
			return r;
		}

		/// <summary>
		/// Gets the number of tabs + spaces/4 at the start of the line that contains the specified position.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="pos">A position in document text.</param>
		public int LineIndentationFromPos(bool utf16, int pos) => LineIndentationFromPos(utf16, pos, out _);

		/// <summary>
		/// Gets position from point.
		/// </summary>
		/// <param name="utf16">Return UTF-16.</param>
		/// <param name="p">Point in client area.</param>
		/// <param name="minusOneIfFar">Return -1 if p is not in text characters.</param>
		public int PosFromXY(bool utf16, POINT p, bool minusOneIfFar)
			=> _ReturnPos(utf16, Call(minusOneIfFar ? SCI_POSITIONFROMPOINTCLOSE : SCI_POSITIONFROMPOINT, p.x, p.y));

		/// <summary>
		/// Gets annotation text of line.
		/// Returns "" if the line does not contain annotation or is invalid line index.
		/// </summary>
		public string AnnotationText(int line)
		{
			if(C.ZImages != null) return C.ZImages.LibAnnotationText(line);
			return LibAnnotationText(line);
		}

		/// <summary>
		/// Gets raw annotation text which can contain image info.
		/// AnnotationText gets text without image info.
		/// Returns "" if the line does not contain annotation or is invalid line index.
		/// </summary>
		public string LibAnnotationText(int line)
		{
			return GetString(SCI_ANNOTATIONGETTEXT, line);
		}

		/// <summary>
		/// Sets annotation text of line.
		/// Does nothing if invalid line index.
		/// If s is null or "", removes annotation.
		/// Preserves existing image info.
		/// </summary>
		public void AnnotationText(int line, string s)
		{
			if(C.ZImages != null) C.ZImages.LibAnnotationText(line, s);
			else LibAnnotationText(line, s);
		}

		/// <summary>
		/// Sets raw annotation text which can contain image info.
		/// If s is null or "", removes annotation.
		/// </summary>
		internal void LibAnnotationText(int line, string s)
		{
			if(Empty(s)) s = null;
			SetString(SCI_ANNOTATIONSETTEXT, line, s);
		}

		/// <summary>
		/// Moves 'from' to the start of its line, and 'to' to the end of its line.
		/// Does not change 'to' if it is at a line start.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="from">Start index.</param>
		/// <param name="to">End index.</param>
		/// <param name="withRN">Include "\r\n".</param>
		public void RangeToFullLines(bool utf16, ref int from, ref int to, bool withRN = false)
		{
			Debug.Assert(from <= to);
			from = _ReturnPos(utf16, LineStartFromPos(utf16, from));
			to = _ReturnPos(utf16, LineEndFromPos(utf16, to, withRN, true));
		}

		/// <summary>
		/// SCI_INSERTTEXT.
		/// Does not parse tags.
		/// Does not change current selection; for it use <see cref="ReplaceSel"/>.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="pos">Start index. If -1, uses current position.</param>
		/// <param name="s">Text to insert. Can be null.</param>
		public void InsertText(bool utf16, int pos, string s)
		{
			using(new _NoReadonly(this))
				SetString(SCI_INSERTTEXT, _ParamPos(utf16, pos), s ?? "");
		}

		/// <summary>
		/// SCI_DELETERANGE.
		/// Does not parse tags.
		/// Does not change current selection, unless it is in the from-to range (including to); for it use <see cref="ReplaceSel"/> or <see cref="ReplaceRange"/> with empty string and <i>finalCurrentPos</i>.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="from">Start index.</param>
		/// <param name="to">End index. If -1, uses control text length.</param>
		/// <param name="toIsLength"></param>
		public void DeleteRange(bool utf16, int from, int to, bool toIsLength = false)
		{
			using(new _NoReadonly(this)) {
				NormalizeRange(utf16, ref from, ref to, toIsLength);
				Call(SCI_DELETERANGE, from, to - from);
			}
		}

		/// <summary>
		/// Replaces text range.
		/// Does not parse tags.
		/// By default does not change current selection, unless it is in the from-to range (including to).
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="from">Start index.</param>
		/// <param name="to">End index. If -1, uses control text length.</param>
		/// <param name="s">Replacement text. Can be null.</param>
		/// <param name="toIsLength"></param>
		/// <param name="finalCurrentPos">
		/// If AtStart or AtEnd, after replacing sets curent position at the start or end of the replacement.
		/// Else if current position was in the from-to range (including to), sets at from.
		/// Else does not change current position and selection.
		/// Alternatively use <see cref="ReplaceSel"/>; it sets current position at the end of the replacement.
		/// </param>
		public void ReplaceRange(bool utf16, int from, int to, string s, bool toIsLength = false, SciFinalCurrentPos finalCurrentPos = 0)
		{
			using(new _NoReadonly(this)) {
				NormalizeRange(utf16, ref from, ref to, toIsLength);
				Call(SCI_SETTARGETRANGE, from, to);
				SetString(SCI_REPLACETARGET, 0, s ??= "", true);
				if(finalCurrentPos != default) {
					CurrentPos8 = finalCurrentPos == SciFinalCurrentPos.AtStart ? from : CountBytesFromChars(from, s.Length);
				}
			}
		}

		/// <summary>
		/// Gets range text.
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="from">Start index.</param>
		/// <param name="to">End index. If -1, uses control text length.</param>
		/// <param name="toIsLength"></param>
		public string RangeText(bool utf16, int from, int to, bool toIsLength = false)
		{
			NormalizeRange(utf16, ref from, ref to, toIsLength);
			int n = to - from; if(n == 0) return "";
			fixed(byte* b = LibByte(n)) {
				var tr = new Sci_TextRange() { chrg = new Sci_CharacterRange() { cpMin = from, cpMax = to }, lpstrText = b };
				int r = Call(SCI_GETTEXTRANGE, 0, &tr);
				Debug.Assert(r == n);
				return _FromUtf8(b, r);
			}
		}

		/// <summary>
		/// SCI_REPLACESEL.
		/// Does not parse tags.
		/// If read-only, asserts and fails (unlike most other functions that change text).
		/// </summary>
		/// <param name="s">Replacement text. Can be null.</param>
		public void ReplaceSel(string s)
		{
			Debug.Assert(!IsReadonly);
			SetString(SCI_REPLACESEL, 0, s ?? "");
		}

		/// <summary>
		/// GoToPos and SCI_REPLACESEL.
		/// Does not parse tags.
		/// If read-only, asserts and fails (unlike most other functions that change text).
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="s">Replacement text. Can be null.</param>
		/// <param name="pos">Start index.</param>
		public void ReplaceSel(bool utf16, int pos, string s)
		{
			Debug.Assert(!IsReadonly);
			GoToPos(utf16, pos);
			SetString(SCI_REPLACESEL, 0, s ?? "");
		}

		/// <summary>
		/// Sets selection (SCI_SETSEL) and replaces with new text (SCI_REPLACESEL).
		/// Does not parse tags.
		/// If read-only, asserts and fails (unlike most other functions that change text).
		/// </summary>
		/// <param name="utf16"></param>
		/// <param name="from">Start index.</param>
		/// <param name="to">End index. If -1, uses control text length.</param>
		/// <param name="s">Replacement text. Can be null.</param>
		/// <param name="toIsLength"></param>
		public void SetAndReplaceSel(bool utf16, int from, int to, string s, bool toIsLength = false)
		{
			Debug.Assert(!IsReadonly);
			NormalizeRange(utf16, ref from, ref to, toIsLength);
			Call(SCI_SETSEL, from, to);
			SetString(SCI_REPLACESEL, 0, s ?? "");
		}

		/// <summary>
		/// SCI_GOTOPOS and ensures visible.
		/// </summary>
		public void GoToPos(bool utf16, int pos)
		{
			pos = _ParamPos(utf16, pos);
			int line = Call(SCI_LINEFROMPOSITION, pos);
			Call(SCI_ENSUREVISIBLEENFORCEPOLICY, line);
			Call(SCI_GOTOPOS, pos);
		}

		/// <summary>
		/// SCI_GOTOLINE and ensures visible.
		/// </summary>
		public void GoToLine(int line)
		{
			Call(SCI_ENSUREVISIBLEENFORCEPOLICY, line);
			Call(SCI_GOTOLINE, line);
		}

		/// <summary>
		/// SCI_SETSEL and ensures visible.
		/// </summary>
		public void SelectAndMakeVisible(bool utf16, int from, int to, bool toIsLength = false)
		{
			NormalizeRange(utf16, ref from, ref to, toIsLength);
			GoToPos(false, from); //ensures line visible and selection visible (without it in some cases selection to the left of the caret may be invisible)
			Call(SCI_SETSEL, from, to);
		}

		/// <summary>
		/// SCI_GETREADONLY.
		/// </summary>
		public bool IsReadonly => 0 != Call(SCI_GETREADONLY);

		public struct FileLoaderSaver
		{
			_Encoding _enc;

			public bool IsBinary => _enc == _Encoding.Binary;

			/// <summary>
			/// Loads file as UTF-8.
			/// Returns byte[] that must be passed to <see cref="SetText"/>.
			/// </summary>
			/// <param name="file">To pass to File.OpenRead.</param>
			/// <exception cref="Exception">Exceptions of File.OpenRead, File.Read, Encoding.Convert.</exception>
			/// <remarks>
			/// Supports any encoding (UTF-8, UTF-16, etc), BOM. Remembers it for Save.
			/// If UTF-8 with BOM, the returned array contains BOM (to avoid copying), and <b>SetText</b> knows it.
			/// If file data is binary or file size is more than 100_000_000, the returned text shows error message or image. Then <b>SetText</b> makes the control read-only; <b>Save</b> throws exception.
			/// </remarks>
			public byte[] Load(string file)
			{
				_enc = _Encoding.Binary;
				if(0 != file.Ends(true, ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".ico", ".cur", ".ani")) {
					if(!AFile.ExistsAsFile(file)) throw new FileNotFoundException($"Could not find file '{file}'.");
					return Encoding.UTF8.GetBytes($"//Image file @\"{file}\"\0");
				}

				using var fr = AFile.WaitIfLocked(() => File.OpenRead(file));
				var fileSize = fr.Length;
				if(fileSize > 100_000_000) return Encoding.UTF8.GetBytes("//Cannot edit. The file is too big, more than 100_000_000 bytes.\0");
				int trySize = (int)Math.Min(fileSize, 65_000);
				var b = new byte[trySize + 4];
				trySize = fr.Read(b, 0, trySize);
				fixed(byte* p = b) _enc = _DetectEncoding(p, trySize);
				if(_enc == _Encoding.Binary) return Encoding.UTF8.GetBytes("//Cannot edit. The file is binary, not text.\0");
				int bomLength = (int)_enc >> 4;
				//Print(_enc, bomLength, fileSize);

				if(fileSize > trySize) {
					var old = b; b = new byte[fileSize + 4]; Array.Copy(old, b, trySize);
					fr.Read(b, trySize, (int)fileSize - trySize);
				}

				Encoding e = _NetEncoding();
				if(e != null) b = Encoding.Convert(e, Encoding.UTF8, b, bomLength, (int)fileSize - bomLength + e.GetByteCount("\0"));
				return b;
			}

			Encoding _NetEncoding()
			{
				switch(_enc) {
				case _Encoding.Ansi: return Encoding.Default;
				case _Encoding.Utf16BOM: case _Encoding.Utf16NoBOM: return Encoding.Unicode;
				case _Encoding.Utf16BE: return Encoding.BigEndianUnicode;
				case _Encoding.Utf32BOM: return Encoding.UTF32;
				case _Encoding.Utf32BE: return new UTF32Encoding(true, false);
				}
				return null;
			}

			static unsafe _Encoding _DetectEncoding(byte* s, int len)
			{
				if(len == 0) return _Encoding.Utf8NoBOM;
				if(len == 1) return s[0] == 0 ? _Encoding.Binary : (s[0] < 128 ? _Encoding.Utf8NoBOM : _Encoding.Ansi);
				if(len >= 3 && s[0] == 0xEF && s[1] == 0xBB && s[2] == 0xBF) return _Encoding.Utf8BOM;
				//bool canBe16 = 0 == (fileSize & 1), canBe32 = 0 == (fileSize & 3); //rejected. .NET ignores it too.
				if(s[0] == 0xFF && s[1] == 0xFE) {
					if(len >= 4 && s[2] == 0 && s[3] == 0) return _Encoding.Utf32BOM;
					return _Encoding.Utf16BOM;
				}
				if(s[0] == 0xFE && s[1] == 0xFF) return _Encoding.Utf16BE;
				if(len >= 4 && *(uint*)s == 0xFFFE0000) return _Encoding.Utf32BE;
				int zeroAt = LibCharPtr.Length(s, len);
				if(zeroAt == len - 1) len--; //WordPad saves .rtf files with '\0' at the end
				if(zeroAt == len) { //no '\0'
					byte* p = s, pe = s + len; for(; p < pe; p++) if(*p >= 128) break; //is ASCII?
					if(p < pe && 0 == Api.MultiByteToWideChar(Api.CP_UTF8, Api.MB_ERR_INVALID_CHARS, s, len, null, 0)) return _Encoding.Ansi;
					return _Encoding.Utf8NoBOM;
				}
				var u = (char*)s; len /= 2;
				if(LibCharPtr.Length(u, len) == len) //no '\0'
					if(0 != Api.WideCharToMultiByte(Api.CP_UTF8, Api.WC_ERR_INVALID_CHARS, u, len, null, 0, default, null)) return _Encoding.Utf16NoBOM;
				return _Encoding.Binary;
			}

			enum _Encoding : byte
			{
				/// <summary>Not a text file, or loading failed, or not initialized.</summary>
				Binary = 0, //must be 0

				/// <summary>ASCII or UTF-8 without BOM.</summary>
				Utf8NoBOM = 1,

				/// <summary>UTF-8 with BOM (3 bytes).</summary>
				Utf8BOM = 1 | (3 << 4),

				/// <summary>ANSI containing non-ASCII characters, unknown code page.</summary>
				Ansi = 2,

				/// <summary>UTF-16 without BOM.</summary>
				Utf16NoBOM = 3,

				/// <summary>UTF-16 with BOM (2 bytes).</summary>
				Utf16BOM = 3 | (2 << 4),

				/// <summary>UTF-16 with big endian BOM (2 bytes).</summary>
				Utf16BE = 4 | (2 << 4),

				/// <summary>UTF-32 with BOM (4 bytes).</summary>
				Utf32BOM = 5 | (4 << 4),

				/// <summary>UTF-32 with big endian BOM (4 bytes).</summary>
				Utf32BE = 6 | (4 << 4),

				//rejected. .NET does not save/load with UTF-7 BOM, so we too. Several different BOM of different length.
				///// <summary>UTF-7 with BOM.</summary>
				//Utf7BOM,
			}

			/// <summary>
			/// Sets control text.
			/// If the file is binary or too big, shows error message or image, makes the control read-only, and returns false. Else returns true.
			/// Uses <see cref="SciSetTextFlags"/> NoUndo and NoNotify.
			/// </summary>
			/// <param name="z">Control's Z.</param>
			/// <param name="text">Returned by <b>Load</b>.</param>
			public unsafe bool SetText(SciText z, byte[] text)
			{
				using(new _NoUndoNotif(z, SciSetTextFlags.NoUndoNoNotify)) {
					z.LibSetText(text, _enc == _Encoding.Utf8BOM ? 3 : 0);
				}
				if(_enc != _Encoding.Binary) return true;
				z.Call(SCI_SETREADONLY, 1);
				return false;
			}

			/// <summary>
			/// Saves control text with the same encoding/BOM as loaded. Uses <see cref="AFile.Save"/>.
			/// </summary>
			/// <param name="z">Control's Z.</param>
			/// <param name="file">To pass to AFile.Save.</param>
			/// <param name="tempDirectory">To pass to AFile.Save.</param>
			/// <exception cref="Exception">Exceptions of AFile.Save.</exception>
			/// <exception cref="InvalidOperationException">The file is binary (then <b>SetText</b> made the control read-only), or <b>Load</b> not called.</exception>
			public unsafe void Save(SciText z, string file, string tempDirectory = null)
			{
				if(_enc == _Encoding.Binary) throw new InvalidOperationException();

				//_enc = _Encoding.Utf32BOM; //test

				int len = z.TextLength8;
				int bom = (int)_enc >> 4;
				if(bom == 2 || bom == 4) bom = 1; //1 UTF16 or UTF32 character
				var b = LibByte(len + bom);

				fixed(byte* p = b) z.Call(SCI_GETTEXT, len + 1, p + bom);

				Encoding e = _NetEncoding();
				if(e != null) {
					if(bom != 0) b[0] = 0; //clear BOM placeholder
					b = Encoding.Convert(Encoding.UTF8, e, b, 0, len + bom);
					len = b.Length;
					switch(_enc) {
					case _Encoding.Utf16BOM: case _Encoding.Utf32BOM: b[0] = 0xFF; b[1] = 0xFE; break;
					case _Encoding.Utf16BE: b[0] = 0xFE; b[1] = 0xFF; break;
					case _Encoding.Utf32BE: b[2] = 0xFE; b[3] = 0xFF; break;
					}
				} else if(bom == 3) {
					len += 3;
					b[0] = 0xEF; b[1] = 0xBB; b[2] = 0xBF;
				} //else bom 0

				//for(int i = 0; i < len; i++) Print(b[i]); return; //test

				AFile.Save(file, temp => { using var fs = File.OpenWrite(temp); fs.Write(b, 0, len); }, tempDirectory: tempDirectory);
				//using(var fs = File.OpenWrite(file)) fs.Write(b, 0, len); //not much faster
			}
		}

		public int MarginFromPoint(POINT p, bool screenCoord)
		{
			if(screenCoord) p = C.PointToClient(p);
			if(C.ClientRectangle.Contains(p)) {
				for(int i = 0, w = 0; i < 5; i++) { w += Call(SCI_GETMARGINWIDTHN, i); if(w >= p.x) return i; }
			}
			return -1;
		}

		/// <summary>
		/// Gets text and offsets of lines containing selection.
		/// Returns true. If <i>ifFullLines</i> is true, may return false.
		/// </summary>
		/// <param name="x">Receives results. The positions are UTF-8.</param>
		/// <param name="ifFullLines">Fail (return false) if selection length is 0 or selection start is not at a line start.</param>
		/// <param name="oneMore">Get +1 line if selection ends at a line start, except if selection length is 0.</param>
		public bool GetSelectionLines(out SelectionLines x, bool ifFullLines = false, bool oneMore = false)
		{
			x = default;
			x.selStart = SelectionStart8; x.selEnd = SelectionEnd8;
			if(ifFullLines && x.selEnd == x.selStart) return false;
			var (_, start, end) = LineStartEndFromPos(false, x.selStart);
			if(ifFullLines && start != x.selStart) return false;
			x.linesStart = start;

			if(x.selEnd > x.selStart) {
				(_, start, end) = LineStartEndFromPos(false, x.selEnd);
				if(!oneMore && start == x.selEnd) end = start; //selection end is at line start. We need the line only if oneMore.
				if(ifFullLines && x.selEnd < end) return false;
			}

			x.linesEnd = end;
			x.text = this.RangeText(false, x.linesStart, end);
			return true;
		}

		public struct SelectionLines
		{
			public int selStart, selEnd, linesStart, linesEnd;
			public string text;
		}

		public string SelectedText()
		{
			int i = SelectionStart8, j = SelectionEnd8;
			return j > i ? RangeText(false, i, j) : "";
		}

		public void IndicatorClear(int indic) => IndicatorClear(false, indic, 0, TextLength8);

		public void IndicatorClear(bool utf16, int indic, int from, int length)
		{
			if(utf16) _NormalizeFromLen(ref from, ref length);
			Call(SCI_SETINDICATORCURRENT, indic);
			Call(SCI_INDICATORCLEARRANGE, from, length);
		}

		public void IndicatorAdd(bool utf16, int indic, int from, int length)
		{
			if(utf16) _NormalizeFromLen(ref from, ref length);
			Call(SCI_SETINDICATORCURRENT, indic);
			Call(SCI_INDICATORFILLRANGE, from, length);
		}

		public void IndicatorAdd(bool utf16, int indic, int from, int length, int _value)
		{
			if(utf16) _NormalizeFromLen(ref from, ref length);
			Call(SCI_SETINDICATORCURRENT, indic);
			Call(SCI_SETINDICATORVALUE, _value);
			Call(SCI_INDICATORFILLRANGE, from, length);
		}

	}

	[Flags]
	public enum SciFinalCurrentPos
	{
		AtStart = 1,
		AtEnd = 2,
	}

	/// <summary>
	/// Flags for 'set text', 'clear text' and similar functions. Eg you can disable Undo collection or 'changed' notifications.
	/// Note: Ignores NoUndo and NoNotify if ZInitReadOnlyAlways, because then Undo and notifications are disabled when creating control.
	/// </summary>
	[Flags]
	public enum SciSetTextFlags
	{
		/// <summary>
		/// Cannot be undone. Clear Undo buffer.
		/// </summary>
		NoUndo = 1,

		/// <summary>
		/// Don't send 'modified' and 'text changed' notifications (don't call overrides and events).
		/// </summary>
		NoNotify = 2,

		/// <summary>
		/// NoUndo | NoNotify.
		/// </summary>
		NoUndoNoNotify = 3,
	}
}
