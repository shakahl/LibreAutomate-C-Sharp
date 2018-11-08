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
//using System.Xml.Linq;

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
		/// Sends a Scintilla message to the control and returns LPARAM.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public LPARAM CallRetPtr(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) => SC.CallRetPtr(sciMessage, wParam, lParam);

		/// <summary>
		/// Sends a Scintilla message to the control and returns int.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public int Call(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) => SC.Call(sciMessage, wParam, lParam);

		/// <summary>
		/// Calls a Scintilla message that sets a string which is passed using lParam.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public int SetString(int sciMessage, LPARAM wParam, string lParam)
		{
			fixed (byte* s = _ToUtf8(lParam)) {
				return SC.Call(sciMessage, wParam, s);
			}
		}

		/// <summary>
		/// Calls a Scintilla message that sets a string which is passed using wParam.
		/// If the message changes control text, this function does not work if the control is read-only. At first make non-readonly temporarily.
		/// Don't call this function from another thread.
		/// </summary>
		public int SetString(int sciMessage, string wParam, LPARAM lParam)
		{
			fixed (byte* s = _ToUtf8(wParam)) {
				return SC.Call(sciMessage, lParam, s);
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
		/// How much UTF8 bytes to allocate for Scintilla to store the text.
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

		struct _NoReadonly :IDisposable
		{
			SciText _t;
			bool _ro;

			public _NoReadonly(SciText t)
			{
				_t = t;
				_ro = _t.SC.InitReadOnlyAlways;
				if(_ro) _t.SC.Call(SCI_SETREADONLY, 0);
			}

			public void Dispose()
			{
				if(_ro) _t.SC.Call(SCI_SETREADONLY, 1);
			}
		}

		struct _NoUndoNotif :IDisposable
		{
			SciText _t;
			bool _noUndo, _noNotif;

			public _NoUndoNotif(SciText t, bool noUndo, bool noNotif)
			{
				if(t.SC.InitReadOnlyAlways) noUndo = noNotif = false;
				_t = t;
				_noUndo = noUndo && 0 != _t.Call(SCI_GETUNDOCOLLECTION);
				_noNotif = noNotif;
				if(_noNotif) _t.SC.DisableModifiedNotifications = true;
				if(_noUndo) _t.Call(SCI_SETUNDOCOLLECTION, false);
			}

			public void Dispose()
			{
				if(_noUndo) {
					_t.Call(SCI_EMPTYUNDOBUFFER);
					_t.Call(SCI_SETUNDOCOLLECTION, true);
				}
				if(_noNotif) _t.SC.DisableModifiedNotifications = false;
			}
		}

		#endregion

		/// <summary>
		/// Removes all text (SCI_CLEARALL).
		/// </summary>
		/// <param name="noUndo">Cannot be undone; clear Undo buffer.</param>
		/// <param name="noNotif">Don't send 'modified' notifications.</param>
		/// <remarks>
		/// Ignores noUndo and noNotif if InitReadOnlyAlways, because then Undo and notifications are disabled when creating control.
		/// </remarks>
		public void ClearText(bool noUndo = false, bool noNotif = false)
		{
			using(new _NoUndoNotif(this, noUndo, noNotif))
			using(new _NoReadonly(this))
				Call(SCI_CLEARALL);
		}

		/// <summary>
		/// Replaces all text.
		/// </summary>
		/// <param name="s">Text.</param>
		/// <param name="noUndo">Cannot be undone; clear Undo buffer.</param>
		/// <param name="noNotif">Don't send 'modified' notifications.</param>
		/// <param name="ignoreTags">Don't parse tags, regardless of SC.TagsStyle.</param>
		/// <remarks>
		/// Ignores noUndo and noNotif if InitReadOnlyAlways, because then Undo and notifications are disabled when creating control.
		/// </remarks>
		public void SetText(string s, bool noUndo = false, bool noNotif = false, bool ignoreTags = false)
		{
			using(new _NoUndoNotif(this, noUndo, noNotif)) {
				if(!ignoreTags && _CanParseTags(s)) {
					ClearText();
					SC.Tags.AddText(s, false, SC.InitTagsStyle == AuScintilla.TagsStyle.AutoWithPrefix);
				} else {
					using(new _NoReadonly(this))
						SetString(SCI_SETTEXT, 0, s);
				}
			}
		}

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

		///// <summary>
		///// Replaces all text.
		///// Does not parse tags.
		///// </summary>
		///// <param name="s">Text.</param>
		///// <param name="startIndex"></param>
		///// <param name="noUndo">Cannot be undone; clear Undo buffer.</param>
		///// <param name="noNotif">Don't send 'modified' notifications.</param>
		///// <remarks>
		///// Ignores noUndo and noNotif if InitReadOnlyAlways, because then Undo and notifications are disabled when creating control.
		///// </remarks>
		//public void SetTextUtf8(byte[] s, int startIndex = 0, bool noUndo = false, bool noNotif = false)
		//{
		//	using(new _NoUndoNotif(this, noUndo, noNotif)) {
		//		LibSetText(s, startIndex);
		//	}
		//}

		/// <summary>
		/// Sets UTF8 text.
		/// Does not pare tags etc, just calls SCI_SETTEXT and SCI_SETREADONLY if need.
		/// </summary>
		internal void LibSetText(byte[] s, int startIndex)
		{
			fixed (byte* p = s) LibSetText(p + startIndex);
		}

		/// <summary>
		/// Sets UTF8 text.
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

				using(new _NoReadonly(this))
					Call(SCI_APPENDTEXT, n, b);
			}
			if(scroll) Call(SCI_GOTOPOS, TextLengthBytes);
		}

		/// <summary>
		/// Appends UTF8 text of specified length.
		/// Does not append newline (s should contain it). Does not parse tags. Moves current position and scrolls to the end.
		/// </summary>
		internal void LibAddText(bool append, byte* s, int lenToAppend)
		{
			using(new _NoReadonly(this))
				if(append) Call(SCI_APPENDTEXT, lenToAppend, s);
				else Call(SCI_SETTEXT, 0, s);

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

		//	using(new _NoReadonly(this))
		//	if(!append) Call(SCI_SETTEXT);
		//	Call(SCI_ADDSTYLEDTEXT, lenBytes, s);

		//	if(append) Call(SCI_GOTOPOS, TextLengthBytes);
		//}

		/// <summary>
		/// Gets all text.
		/// </summary>
		public string GetText()
		{
			int n = TextLengthBytes;
			return GetStringOfLength(SCI_GETTEXT, n + 1, n);
		}

		/// <summary>
		/// Gets text length (SCI_GETTEXTLENGTH) in UTF8 bytes.
		/// </summary>
		public int TextLengthBytes => Call(SCI_GETTEXTLENGTH);

		/// <summary>
		/// Gets or sets current caret position (SCI_GETCURRENTPOS/SCI_SETEMPTYSELECTION) in UTF8 bytes.
		/// The 'set' function makes empty selection; does not scroll and does not make visible like GoToPos.
		/// </summary>
		public int PositionBytes { get => Call(SCI_GETCURRENTPOS); set => Call(SCI_SETEMPTYSELECTION, value); }

		/// <summary>
		/// Gets line index from character position (UTF8 bytes).
		/// </summary>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, returns the last line.</param>
		public int LineIndexFromPosition(int pos)
		{
			if(pos <= 0) return 0;
			return Call(SCI_LINEFROMPOSITION, pos);
		}

		/// <summary>
		/// Gets line start position (UTF8 bytes) from line index.
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
		/// Gets line end position (UTF8 bytes) from line index.
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
		/// Gets line start position from any position. Both are in UTF8 bytes.
		/// </summary>
		/// <param name="pos">A position in document text. If negative, returns 0. If greater than text length, uses the last line.</param>
		public int LineStartFromPosition(int pos)
		{
			return LineStart(LineIndexFromPosition(pos));
		}

		/// <summary>
		/// Gets line end position from any position. Both are in UTF8 bytes.
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
		/// <param name="from">Start index (UTF8 bytes). If less than 0, uses 0.</param>
		/// <param name="to">End index (UTF8 bytes). If less than 0, uses TextLengthBytes.</param>
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
		/// <param name="from">Start index (UTF8 bytes).</param>
		/// <param name="to">End index (UTF8 bytes).</param>
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
		/// <param name="pos">Start index (UTF8 bytes).</param>
		/// <param name="length">Length (UTF8 bytes).</param>
		public void DeleteRange(int pos, int length)
		{
			using(new _NoReadonly(this))
				Call(SCI_DELETERANGE, pos, length);
		}

		/// <summary>
		/// SCI_INSERTTEXT.
		/// Does not parse tags.
		/// </summary>
		/// <param name="pos">Start index (UTF8 bytes).</param>
		/// <param name="s"></param>
		public void InsertText(int pos, string s)
		{
			using(new _NoReadonly(this))
				SetString(SCI_INSERTTEXT, pos, s);
		}

		/// <summary>
		/// SCI_GOTOPOS and ensures visible.
		/// </summary>
		public void GoToPos(int pos)
		{
			Call(SCI_GOTOPOS, pos);
			int line = Call(SCI_LINEFROMPOSITION, pos);
			Call(SCI_ENSUREVISIBLEENFORCEPOLICY, line);
		}

		/// <summary>
		/// SCI_GOTOLINE and ensures visible.
		/// </summary>
		public void GoToLine(int line)
		{
			Call(SCI_GOTOLINE, line);
			Call(SCI_ENSUREVISIBLEENFORCEPOLICY, line);
		}

		public struct FileLoaderSaver
		{
			_Encoding _enc;

			public bool IsBinary => _enc == _Encoding.Binary;

			/// <summary>
			/// Loads file as UTF-8.
			/// Returns byte[] that must be pased to <see cref="SetText"/>.
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
				if(0 != Path_.GetExtension(file).Equals_(true, ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".ico", ".cur", ".ani"))
					return Encoding.UTF8.GetBytes($"//Image file @\"{file}\"");

				using(var fr = File_.WaitIfLocked(() => File.OpenRead(file))) {
					var fileSize = fr.Length;
					if(fileSize > 100_000_000) return Encoding.UTF8.GetBytes("//Cannot edit. The file is too big, more than 100_000_000 bytes.");
					int trySize = (int)Math.Min(fileSize, 65_000);
					var b = new byte[trySize];
					trySize = fr.Read(b, 0, trySize);
					fixed (byte* p = b) _enc = _DetectEncoding(p, trySize);
					//Print(_enc);
					if(_enc == _Encoding.Binary) return Encoding.UTF8.GetBytes("//Cannot edit. The file is binary, not text.");
					int bomLength = (int)_enc >> 4;

					if(fileSize > trySize) {
						var old = b; b = new byte[fileSize]; Array.Copy(old, b, trySize);
						fr.Read(b, trySize, (int)fileSize - trySize);
					}

					Encoding e = _NetEncoding();
					if(e != null) b = Encoding.Convert(e, Encoding.UTF8, b, bomLength, (int)fileSize - bomLength);
					return b;
				}
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
				if(Au.Util.LibCharPtr.Length(s, len) == len) { //no '\0'
					byte* p = s, pe = s + len; for(; p < pe; p++) if(*p >= 128) break; //is ASCII?
					if(p < pe && 0 == Api.MultiByteToWideChar(Api.CP_UTF8, Api.MB_ERR_INVALID_CHARS, s, len, null, 0)) return _Encoding.Ansi;
					return _Encoding.Utf8NoBOM;
				}
				var u = (char*)s; len /= 2;
				if(Au.Util.LibCharPtr.Length(u, len) == len) //no '\0'
					if(0 != Api.WideCharToMultiByte(Api.CP_UTF8, Api.WC_ERR_INVALID_CHARS, u, len, null, 0, default, null)) return _Encoding.Utf16NoBOM;
				return _Encoding.Binary;
			}

			enum _Encoding :byte
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
			/// </summary>
			/// <param name="sci">Control's ST.</param>
			/// <param name="text">Returned by <b>Load</b>.</param>
			public unsafe bool SetText(SciText sci, byte[] text)
			{
				using(new _NoUndoNotif(sci, true, true)) {
					sci.LibSetText(text, _enc == _Encoding.Utf8BOM ? 3 : 0);
				}
				if(_enc != _Encoding.Binary) return true;
				sci.Call(SCI_SETREADONLY, 1);
				return false;
			}

			/// <summary>
			/// Saves control text with the same encoding/BOM as loaded. Uses <see cref="File_.Save"/>.
			/// </summary>
			/// <param name="sci">Control's ST.</param>
			/// <param name="file">To pass to File.OpenRead.</param>
			/// <exception cref="Exception">Exceptions of File.OpenRead, File.Read, Encoding.Convert.</exception>
			/// <exception cref="InvalidOperationException">The file is binary (then <b>SetText</b> made the control read-only), or <b>Load</b> not called.</exception>
			public unsafe void Save(SciText sci, string file)
			{
				if(_enc == _Encoding.Binary) throw new InvalidOperationException();

				//_enc = _Encoding.Utf32BOM; //test

				int len = sci.TextLengthBytes;
				int bom = (int)_enc >> 4;
				if(bom == 2 || bom == 4) bom = 1; //1 UTF16 or UTF32 character
				var b = LibByte(len + bom);

				fixed (byte* p = b) sci.Call(SCI_GETTEXT, len + 1, p + bom);

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

				File_.Save(file, temp => { using(var fs = File.OpenWrite(temp)) { fs.Write(b, 0, len); } });
			}
		}
	}
}
