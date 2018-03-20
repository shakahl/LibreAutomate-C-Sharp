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
using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	using static Sci;

	/// <summary>
	/// Functions to work with Scintilla control text, code, etc.
	/// A SciText object is created by AuScintilla which is the SC property.
	/// </summary>
	public unsafe partial class SciText
	{
		/// <summary>
		/// The host AuScintilla.
		/// </summary>
		public AuScintilla SC { get; private set; }

		[ThreadStatic] static WeakReference<byte[]> t_byte;

		internal static byte[] LibByte(int n) { return Au.Util.Buffers.Get(n, ref t_byte); }
		//these currently not used
		//internal static Au.Util.Buffers.ByteBuffer LibByte(ref int n) { var r = Au.Util.Buffers.Get(n, ref t_byte); n = r.Length - 1; return r; }
		//internal static Au.Util.Buffers.ByteBuffer LibByte(int n, out int nHave) { var r = Au.Util.Buffers.Get(n, ref t_byte); nHave = r.Length - 1; return r; }


		internal SciText(AuScintilla sc)
		{
			SC = sc;
		}

		#region util

		/// <summary>
		/// Calls a Scintilla message to the control.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage, LPARAM wParam, LPARAM lParam)
		{
			return SC.Call(sciMessage, wParam, lParam);
		}

		/// <summary>
		/// Calls a Scintilla message.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage, LPARAM wParam)
		{
			return SC.Call(sciMessage, wParam);
		}

		/// <summary>
		/// Calls a Scintilla message.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage)
		{
			return SC.Call(sciMessage);
		}

		/// <summary>
		/// Calls a Scintilla message that sets a string which is passed using lParam.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM SetString(int sciMessage, LPARAM wParam, string lParam)
		{
			fixed (byte* s = _ToUtf8(lParam)) {
				return SC.Call(sciMessage, wParam, s);
			}
		}

		/// <summary>
		/// Calls a Scintilla message and passes two strings using wParam and lParam.
		/// wParam0lParam must be like "WPARAM\0LPARAM". Asserts if no '\0'.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM SetStringString(int sciMessage, string wParam0lParam)
		{
			int len;
			fixed (byte* s = _ToUtf8(wParam0lParam, &len)) {
				int i = Au.Util.LibCharPtr.Length(s);
				Debug.Assert(i < len);
				return SC.Call(sciMessage, s, s + i + 1);
			}
		}

		/// <summary>
		/// Calls a Scintilla message that gets a string.
		/// Don't call this function from another thread.
		/// </summary>
		/// <param name="sciMessage"></param>
		/// <param name="wParam"></param>
		/// <param name="bufferSize">
		/// How much bytes to allocate for Scintilla to store the text.
		/// If -1 (default), at first calls sciMessage with lParam=0 (null buffer), let it return required buffer size. Then it can get binary string (with '\0' characters).
		/// If 0, returns "" and does not call the message.
		/// If positive, it can be either known or max expected text length, without the terminating '\0' character. The function will find length of the retrieved string (finds '\0'). Then it cannot get binary string (with '\0' characters).
		/// The function allocates bufferSize+1 bytes and sets that last byte = 0. If Scintilla overwrites it, asserts and calls Environment.FailFast.
		/// </param>
		public string GetString(int sciMessage, LPARAM wParam, int bufferSize = -1)
		{
			if(bufferSize < 0) return GetStringOfLength(sciMessage, wParam, Call(sciMessage, wParam));
			if(bufferSize == 0) return "";
			fixed (byte* b = LibByte(bufferSize)) {
				b[bufferSize] = 0;
				Call(sciMessage, wParam, b);
				Debug.Assert(b[bufferSize] == 0); if(b[bufferSize] != 0) Environment.FailFast("SciText.CallS");
				int len = Au.Util.LibCharPtr.Length(b, bufferSize);
				return _FromUtf8(b, len);
			}
		}

		/// <summary>
		/// The same as <see cref="GetString(int, LPARAM, int)"/>, but always uses utf8Length bytes of the result (does not find length).
		/// </summary>
		/// <param name="sciMessage"></param>
		/// <param name="wParam"></param>
		/// <param name="utf8Length">
		/// Known length (bytes) of the result UTF8 string, without the terminating '\0' character.
		/// If 0, returns "" and does not call the message.
		/// Cannot be negative.
		/// </param>
		/// <remarks>
		/// This function can get binary string (with '\0' characters).
		/// </remarks>
		public string GetStringOfLength(int sciMessage, LPARAM wParam, int utf8Length)
		{
			if(utf8Length == 0) return "";
			fixed (byte* b = LibByte(utf8Length)) {
				b[utf8Length] = 0;
				Call(sciMessage, wParam, b);
				Debug.Assert(b[utf8Length] == 0); if(b[utf8Length] != 0) Environment.FailFast("SciText.CallS");
				return _FromUtf8(b, utf8Length);
			}
		}

		string _FromUtf8(byte* b, int n = -1)
		{
			return Convert_.Utf8ToString(b, n);
		}

		byte[] _ToUtf8(string s, int* utf8Length = null)
		{
			return Convert_.LibUtf8FromString(s, ref t_byte, utf8Length);
		}

		#endregion

		bool _CanParseTags(string s)
		{
			if(Empty(s)) return false;
			switch(SC.InitTagsStyle) {
			case AuScintilla.TagsStyle.AutoAlways:
				return s.IndexOf('<') >= 0;
			case AuScintilla.TagsStyle.AutoWithPrefix:
				return s.StartsWith_("<>");
			}
			return false;
		}

		/// <summary>
		/// Removes all text.
		/// Uses SCI_CLEARALL.
		/// </summary>
		public void ClearText()
		{
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
			Call(SCI_CLEARALL);
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
		}

		/// <summary>
		/// Replaces all text.
		/// </summary>
		/// <param name="s">Text.</param>
		/// <param name="ignoreTags">Don't parse tags, regardless of SC.TagsStyle.</param>
		public void SetText(string s, bool ignoreTags = false)
		{
			if(!ignoreTags && _CanParseTags(s)) {
				ClearText();
				SC.Tags.AddText(s, false, SC.InitTagsStyle == AuScintilla.TagsStyle.AutoWithPrefix);
			} else {
				if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
				SetString(SCI_SETTEXT, 0, s);
				if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
			}
		}

		/// <summary>
		/// Appends text and optionally "\r\n".
		/// Optionally scrolls and moves current position to the end (SCI_GOTOPOS).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="andRN">Also append "\r\n". Ignored if parses tags; then appends.</param>
		/// <param name="scroll">Move current position and scroll to the end. Ignored if parses tags; then moves/scrolls.</param>
		/// <param name="ignoreTags">Don't parse tags, regardless of SC.TagsStyle.</param>
		public void AppendText(string s, bool andRN, bool scroll, bool ignoreTags = false)
		{
			if(!ignoreTags && _CanParseTags(s)) {
				SC.Tags.AddText(s, true, SC.InitTagsStyle == AuScintilla.TagsStyle.AutoWithPrefix);
				return;
			}

			int n = Convert_.Utf8LengthFromString(s);
			fixed (byte* b = LibByte(n + 2)) {
				Convert_.Utf8FromString(s, b, n + 1);
				if(andRN) { b[n++] = (byte)'\r'; b[n++] = (byte)'\n'; }

				if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
				Call(SCI_APPENDTEXT, n, b);
				if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
			}
			if(scroll) Call(SCI_GOTOPOS, TextLengthBytes);
		}

		/// <summary>
		/// Appends UTF8 text of specified length.
		/// Does not append newline (s should contain it). Does not parse tags. Moves current position and scrolls to the end.
		/// </summary>
		internal void LibAddText(bool append, byte* s, int lenToAppend)
		{
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
			if(append) Call(SCI_APPENDTEXT, lenToAppend, s);
			else Call(SCI_SETTEXT, 0, s);
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);

			if(append) Call(SCI_GOTOPOS, TextLengthBytes);
		}

		//not used now
		///// <summary>
		///// Appends styled UTF8 text of specified length.
		///// Does not append newline (s should contain it). Does not parse tags. Moves current position and scrolls to the end.
		///// Uses SCI_ADDSTYLEDTEXT. Caller does not have to move cursor to the end.
		///// lenToAppend is length in bytes, not in cells.
		///// </summary>
		//internal void LibAddStyledText(bool append, byte* s, int lenBytes)
		//{
		//	if(append) Call(SCI_SETEMPTYSELECTION, TextLengthBytes);

		//	if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
		//	if(!append) Call(SCI_SETTEXT);
		//	Call(SCI_ADDSTYLEDTEXT, lenBytes, s);
		//	if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);

		//	if(append) Call(SCI_GOTOPOS, TextLengthBytes);
		//}

		/// <summary>
		/// Sets UTF8 text.
		/// Does not pare tags etc, just calls SCI_SETTEXT and SCI_SETREADONLY if need.
		/// </summary>
		internal void LibSetText(byte* s)
		{
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
			Call(SCI_SETTEXT, 0, s);
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
		}

		/// <summary>
		/// Gets all text.
		/// </summary>
		public string GetText()
		{
			int n = TextLengthBytes;
			return GetStringOfLength(SCI_GETTEXT, n + 1, n);
		}

		/// <summary>
		/// Gets text length. It is the number of bytes, not characters.
		/// </summary>
		public int TextLengthBytes { get => Call(SCI_GETTEXTLENGTH); }

		/// <summary>
		/// Gets line index from character position.
		/// </summary>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, returns the last line.</param>
		public int LineIndexFromPosition(int pos)
		{
			if(pos <= 0) return 0;
			return Call(SCI_LINEFROMPOSITION, pos);
		}

		/// <summary>
		/// Gets line start position from line index.
		/// </summary>
		/// <param name="line">0-based line index. If negative, returns 0. If greater than line count, returns text length.</param>
		public int LineStart(int line)
		{
			if(line <= 0) return 0;
			int r = Call(SCI_POSITIONFROMLINE, line);
			if(r < 0) r = TextLengthBytes;
			return r;
		}

		/// <summary>
		/// Gets line end position from line index.
		/// </summary>
		/// <param name="line">0-based line index. If negative, returns 0. If greater than line count, returns text length.</param>
		/// <param name="withRN">Include \r\n.</param>
		public int LineEnd(int line, bool withRN = false)
		{
			if(line < 0) return 0;
			if(withRN) return LineStart(line) + Call(SCI_LINELENGTH, line);
			return Call(SCI_GETLINEENDPOSITION, line);
		}

		/// <summary>
		/// Gets line start position from any position.
		/// </summary>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, uses the last line.</param>
		public int LineStartFromPosition(int pos)
		{
			return LineStart(LineIndexFromPosition(pos));
		}

		/// <summary>
		/// Gets line end position from any position.
		/// </summary>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, uses the last line.</param>
		/// <param name="withRN">Include \r\n.</param>
		/// <param name="lineStartIsLineEnd">If pos is at a line start (0 or after '\n' character), return pos.</param>
		public int LineEndFromPosition(int pos, bool withRN = false, bool lineStartIsLineEnd = false)
		{
			if(pos < 0) pos = 0;
			if(lineStartIsLineEnd) {
				if(pos == 0 || Call(SCI_GETCHARAT, pos - 1) == (int)'\n') return pos;
			}
			return LineEnd(LineIndexFromPosition(pos), withRN);
		}

		/// <summary>
		/// Gets line text.
		/// </summary>
		/// <param name="line">0-based line index. If invalid, returns "".</param>
		public string LineText(int line)
		{
			return RangeText(LineStart(line), LineEnd(line));
		}

		/// <summary>
		/// Gets range text.
		/// </summary>
		/// <param name="from">If less than 0, uses 0.</param>
		/// <param name="to">If less than 0, uses TextLengthBytes.</param>
		public string RangeText(int from, int to)
		{
			if(from < 0) from = 0;
			if(to < 0) to = TextLengthBytes;
			Debug.Assert(to >= from);
			int n = to - from; if(n <= 0) return "";
			fixed (byte* b = LibByte(n)) {
				var tr = new Sci_TextRange() { chrg = new Sci_CharacterRange() { cpMin = from, cpMax = to }, lpstrText = b };
				int r = Call(SCI_GETTEXTRANGE, 0, &tr);
				Debug.Assert(r == n);
				return _FromUtf8(b, r);
			}
		}

		/// <summary>
		/// Gets line height.
		/// Currently all lines are the same height.
		/// </summary>
		public int LineHeight(int line)
		{
			return Call(SCI_TEXTHEIGHT, line);
		}

		/// <summary>
		/// Gets the number of lines.
		/// </summary>
		public int LineCount { get => Call(SCI_GETLINECOUNT); }

		/// <summary>
		/// Gets annotation text of line.
		/// Returns "" if the line does not contain annotation or is invalid line index.
		/// </summary>
		public string AnnotationText(int line)
		{
			if(SC.Images != null) return SC.Images.LibAnnotationText(line);
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
			if(SC.Images != null) SC.Images.LibAnnotationText(line, s);
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
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="withRN">Include "\r\n".</param>
		public void RangeToFullLines(ref int from, ref int to, bool withRN = false)
		{
			Debug.Assert(from <= to);
			from = LineStartFromPosition(from);
			to = LineEndFromPosition(to, withRN, true);
		}

		/// <summary>
		/// SCI_DELETERANGE.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="length"></param>
		public void DeleteRange(int pos, int length)
		{
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
			Call(SCI_DELETERANGE, pos, length);
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
		}

		/// <summary>
		/// SCI_INSERTTEXT.
		/// Does not parse tags.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="s"></param>
		public void InsertText(int pos, string s)
		{
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 0);
			SetString(SCI_INSERTTEXT, pos, s);
			if(SC.InitReadOnlyAlways) Call(SCI_SETREADONLY, 1);
		}
	}
}
