//#define SUPPORT_RAW_HANDLE

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
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Sets or gets clipboard data in multiple formats.
	/// </summary>
	/// <remarks>
	/// The <b>AddX</b> functions add data to the variable (not to the clipboard). Then <see cref="SetClipboard"/> copies the added data to the clipboard. Also you can use the variable with <see cref="AClipboard.PasteData"/>.
	/// The static <b>GetX</b> functions get data directly from the clipboard.
	/// </remarks>
	/// <example>
	/// Get bitmap image from clipboard.
	/// <code><![CDATA[
	/// var image = AClipboardData.GetImage();
	/// if(image == null) Print("no image in clipboard"); else Print(image.Size);
	/// ]]></code>
	/// Set clipboard data of two formats: text and image.
	/// <code><![CDATA[
	/// new AClipboardData().AddText("text").AddImage(Image.FromFile(@"q:\file.png")).SetClipboard();
	/// ]]></code>
	/// Paste data of two formats: HTML and text.
	/// <code><![CDATA[
	/// AClipboard.PasteData(new AClipboardData().AddHtml("<b>text</b>").AddText("text"));
	/// ]]></code>
	/// Copy data of two formats: HTML and text.
	/// <code><![CDATA[
	/// string html = null, text = null;
	/// AClipboard.CopyData(() => { html = AClipboardData.GetHtml(); text = AClipboardData.GetText(); });
	/// Print(html); Print(text);
	/// ]]></code>
	/// </example>
	public class AClipboardData
	{
		struct _Data { public object data; public int format; }
		List<_Data> _a = new List<_Data>();

		#region add

		static void _CheckFormat(int format, bool minimalCheckFormat = false)
		{
			bool badFormat = false;
			if(format <= 0 || format > 0xffff) badFormat = true;
			else if(format < 0xC000 && !minimalCheckFormat) {
				if(format >= Api.CF_MAX) badFormat = true; //rare. Most are either not GlobalAlloc'ed or not auto-freed.
				else badFormat = format == Api.CF_BITMAP || format == Api.CF_PALETTE || format == Api.CF_METAFILEPICT || format == Api.CF_ENHMETAFILE; //not GlobalAlloc'ed
			}
			if(badFormat) throw new ArgumentException("Invalid format id.");
		}

		AClipboardData _Add(object data, int format, bool minimalCheckFormat = false)
		{
			if(data == null) throw new ArgumentNullException();
			_CheckFormat(format, minimalCheckFormat);

			_a.Add(new _Data() { data = data, format = format });
			return this;
		}

		/// <summary>
		/// Adds text.
		/// Returns this.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="format">
		/// Clipboard format id. Default: <see cref="ClipFormats.Text"/> (CF_UNICODETEXT).
		/// Text encoding (UTF-16, ANSI, etc) depends on format; default UTF-16. See <see cref="ClipFormats.Register"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">Invalid format.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Encoding.GetBytes(string)"/>, which is called if encoding is not UTF-16.</exception>
		public AClipboardData AddText(string text, int format = ClipFormats.Text)
		{
			Encoding enc = ClipFormats.LibGetTextEncoding(format, out bool unknown);
			if(enc == null) return _Add(text, format == 0 ? Api.CF_UNICODETEXT : format);
			return _Add(enc.GetBytes(text + "\0"), format);
		}

		/// <summary>
		/// Adds data of any format as byte[].
		/// Returns this.
		/// </summary>
		/// <param name="data">byte[] containing data.</param>
		/// <param name="format">Clipboard format id. See <see cref="ClipFormats.Register"/>.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">Invalid format. Supported are all registered formats and standard formats &lt;CF_MAX except GDI handles.</exception>
		public AClipboardData AddBinary(byte[] data, int format)
		{
			return _Add(data, format);
		}

		//rejected: rarely used, difficult to use, creates problems. If somebody needs it, can use API.
#if SUPPORT_RAW_HANDLE
			/// <summary>
			/// Adds data of any format as raw clipboard object handle.
			/// Returns this.
			/// </summary>
			/// <param name="handle">Any handle supported by API <msdn>SetClipboardData</msdn>. The type depends on format. For most formats, after setting clipboard data the handle is owned and freed by Windows.</param>
			/// <param name="format">Clipboard format id. See <see cref="RegisterClipboardFormat"/>.</param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException">Invalid format.</exception>
			/// <remarks>
			/// The same handle cannot be added to the clipboard twice. To avoid it, 'set clipboard' functions remove handles from the variable.
			/// </remarks>
			public Data AddHandle(IntPtr handle, int format)
			{
				return _Add(handle != default ? (object)handle : null, format, minimalCheckFormat: true);
			}
#endif

		/// <summary>
		/// Adds image. Uses clipboard format <see cref="ClipFormats.Image"/> (CF_BITMAP).
		/// Returns this.
		/// </summary>
		/// <param name="image">Image. Must be <see cref="Bitmap"/>, else exception.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public AClipboardData AddImage(Image image)
		{
			return _Add(image as Bitmap, Api.CF_BITMAP, minimalCheckFormat: true);
		}

		/// <summary>
		/// Adds HTML text. Uses clipboard format <see cref="ClipFormats.Html"/> ("HTML Format").
		/// Returns this.
		/// </summary>
		/// <param name="html">Full HTML or HTML fragment. If full HTML, a fragment in it can be optionally specified. See examples.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// d.AddHtml("<i>italy</i>");
		/// d.AddHtml("<html><body><i>italy</i></body></html>");
		/// d.AddHtml("<html><body><!--StartFragment--><i>italy</i><!--EndFragment--></body></html>");
		/// ]]></code>
		/// </example>
		public AClipboardData AddHtml(string html)
		{
			return AddBinary(LibCreateHtmlFormatData(html), ClipFormats.Html);
			//note: don't support UTF-16 string of HTML format (starts with "Version:"). UTF8 conversion problems.
		}

		/// <summary>
		/// Adds rich text (RTF). Uses clipboard format <see cref="ClipFormats.Rtf"/> ("Rich Text Format").
		/// Returns this.
		/// </summary>
		/// <param name="rtf">Rich text. Simplest example: <c>@"{\rtf1 text\par}"</c>.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public AClipboardData AddRtf(string rtf)
		{
			return AddText(rtf, ClipFormats.Rtf);
		}

		/// <summary>
		/// Adds list of files to copy/paste. Uses clipboard format <see cref="ClipFormats.Files"/> (CF_HDROP).
		/// Returns this.
		/// </summary>
		/// <param name="files">One or more file paths.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public AClipboardData AddFiles(params string[] files)
		{
			if(files == null) throw new ArgumentNullException();
			var b = new StringBuilder("\x14\0\0\0\0\0\0\0\x1\0"); //struct DROPFILES
			foreach(var s in files) { b.Append(s); b.Append('\0'); }
			return _Add(b.ToString(), Api.CF_HDROP, false);
		}

		/// <summary>
		/// Copies the added data of all formats to the clipboard.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry) or set clipboard data.</exception>
		/// <exception cref="OutOfMemoryException">Failed to allocate memory for clipboard data.</exception>
		/// <remarks>
		/// Calls API <msdn>OpenClipboard</msdn>, <msdn>EmptyClipboard</msdn>, <msdn>SetClipboardData</msdn> and <msdn>CloseClipboard</msdn>.
		/// </remarks>
		public void SetClipboard()
		{
			using(new AClipboard.LibOpenClipboard(true)) {
				AClipboard.LibEmptyClipboard();
				SetOpenClipboard();
			}
		}

		/// <summary>
		/// Copies the added data to the clipboard.
		/// </summary>
		/// <param name="renderLater">Call API <msdn>SetClipboardData</msdn>(format, default). When/if some app will try to get clipboard data, the first time your clipboard owner window will receive <msdn>WM_RENDERFORMAT</msdn> message and should call <c>SetOpenClipboard(false);</c>.</param>
		/// <param name="format">Copy data only of this format. If 0 (default), of all formats.</param>
		/// <exception cref="OutOfMemoryException">Failed to allocate memory for clipboard data.</exception>
		/// <exception cref="AuException">Failed to set clipboard data.</exception>
		/// <remarks>
		/// This function is similar to <see cref="SetClipboard"/>. It calls API <msdn>SetClipboardData</msdn> and does not call <b>OpenClipboard</b>, <b>EmptyClipboard</b>, <b>CloseClipboard</b>. The clipboard must be open and owned by a window of this thread.
		/// </remarks>
		public void SetOpenClipboard(bool renderLater = false, int format = 0)
		{
			for(int i = 0; i < _a.Count; i++) {
				var v = _a[i];
				if(format != 0 && v.format != format) continue;
				if(renderLater) {
					ALastError.Clear();
					Api.SetClipboardData(v.format, default);
					int ec = ALastError.Code; if(ec != 0) throw new AuException(ec, "*set clipboard data");
				} else _SetClipboard(v.format, v.data);
			}
#if SUPPORT_RAW_HANDLE
				//remove caller-added handles, to avoid using the same handle twice
				if(renderLater) return;
				for(int i = _a.Count - 1; i >= 0; i--) {
					var v = _a[i];
					if(format != 0 && v.format != format) continue;
					if(v.data is IntPtr) _a.RemoveAt(i);
				}
#endif
		}

		static unsafe void _SetClipboard(int format, object data)
		{
			IntPtr h = default;
			switch(data) {
			case string s:
				fixed (char* p = s) h = _CopyToHmem(p, (s.Length + 1) * 2);
				break;
			case byte[] b:
				fixed (byte* p = b) h = _CopyToHmem(p, b.Length);
				break;
#if SUPPORT_RAW_HANDLE
				case IntPtr ip:
					h = ip;
					break;
#endif
			case Bitmap bmp:
				h = bmp.GetHbitmap();
				var h2 = Api.CopyImage(h, 0, 0, 0, Api.LR_COPYDELETEORG); //DIB to compatible bitmap
				if(h2 == default) goto ge;
				h = h2;
				break;
			}
			Debug.Assert(h != default);
			if(default != Api.SetClipboardData(format, h)) return;
			ge:
			int ec = ALastError.Code;
			if(data is Bitmap) Api.DeleteObject(h); else Api.GlobalFree(h);
			throw new AuException(ec, "*set clipboard data");
		}

		static unsafe IntPtr _CopyToHmem(void* p, int size)
		{
			var h = Api.GlobalAlloc(Api.GMEM_MOVEABLE, size); if(h == default) goto ge;
			var v = (byte*)Api.GlobalLock(h); if(v == null) { Api.GlobalFree(h); goto ge; }
			try { Buffer.MemoryCopy(p, v, size, size); } finally { Api.GlobalUnlock(h); }
			return h;
			ge: throw new OutOfMemoryException();
		}

		/// <summary>
		/// Copies Unicode text to the clipboard without open/empty/close.
		/// </summary>
		internal static void LibSetText(string text)
		{
			Debug.Assert(text != null);
			_SetClipboard(Api.CF_UNICODETEXT, text);
		}

		/// <summary>
		/// Converts HTML string to byte[] containing data in clipboard format "HTML Format".
		/// </summary>
		/// <param name="html">Full HTML or HTML fragment. If full HTML, a fragment in it can be optionally specified. See examples.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <example>
		/// HTML examples.
		/// <code><![CDATA[
		/// "<i>italy</i>"
		/// "<html><body><i>italy</i></body></html>"
		/// "<html><body><!--StartFragment--><i>italy</i><!--EndFragment--></body></html>"
		/// ]]></code>
		/// </example>
		internal static unsafe byte[] LibCreateHtmlFormatData(string html)
		{
			if(html == null) throw new ArgumentNullException();
			var b = new StringBuilder(c_headerTemplate);
			//find "<body>...</body>" and "<!--StartFragment-->...<!--EndFragment-->" in it
			int isb = -1, ieb = -1, isf = -1, ief = -1; //start/end of inner body and fragment
			if(html.RegexMatch(@"<body\b.*?>", 0, out RXGroup body) && (ieb = html.Find("</body>", body.EndIndex)) >= 0) {
				isb = body.EndIndex;
				isf = html.Find(c_startFragment, isb..ieb, true);
				if(isf >= 0) {
					isf += c_startFragment.Length;
					ief = html.Find(c_endFragment, isf..ieb, true);
				}
			}
			//Print($"{isb} {ieb}  {isf} {ief}");
			if(ieb < 0) { //no "<body>...</body>"
				b.Append("<html><body>").Append(c_startFragment).Append(html).Append(c_endFragment).Append("</body></html>");
				isf = 12 + c_startFragment.Length;
				ief = isf + Encoding.UTF8.GetByteCount(html);
			} else {
				if(ief < 0) { //"...<body>...</body>..."
					b.Append(html, 0, isb).Append(c_startFragment).Append(html, isb, ieb - isb)
						.Append(c_endFragment).Append(html, ieb, html.Length - ieb);
					isf = isb + c_startFragment.Length;
					ief = ieb + c_startFragment.Length;
				} else { //"...<body>...<!--StartFragment-->...<!--EndFragment-->...</body>..."
					b.Append(html);
					isb = isf; ieb = ief; //reuse these vars to calc UTF8 lengths
				}
				//correct isf/ief if html part lenghts are different in UTF8
				if(!Util.AStringUtil.IsAscii(html)) {
					fixed (char* p = html) {
						int lenDiff1 = Encoding.UTF8.GetByteCount(p, isb) - isb;
						int lenDiff2 = Encoding.UTF8.GetByteCount(p + isb, ieb - isb) - (ieb - isb);
						isf += lenDiff1;
						ief += lenDiff1 + lenDiff2;
					}
				}
			}
			//Print($"{isf} {ief}");
			isf += c_headerTemplate.Length; ief += c_headerTemplate.Length;

			b.Append('\0');
			var a = Encoding.UTF8.GetBytes(b.ToString());
			_SetNum(a.Length - 1, 53);
			_SetNum(isf, 79);
			_SetNum(ief, 103);

			//Print(Encoding.UTF8.GetString(a));
			return a;

			void _SetNum(int num, int i)
			{
				for(; num != 0; num /= 10) a[--i] = (byte)('0' + num % 10);
			}
		}
		const string c_startFragment = "<!--StartFragment-->";
		const string c_endFragment = "<!--EndFragment-->";
		const string c_headerTemplate = @"Version:0.9
StartHTML:0000000105
EndHTML:0000000000
StartFragment:0000000000
EndFragment:0000000000
";

		#endregion

		#region get

		struct _GlobalLock : IDisposable
		{
			IntPtr _hmem;

			public _GlobalLock(IntPtr hmem, out IntPtr mem, out int size)
			{
				mem = Api.GlobalLock(hmem);
				if(mem == default) { _hmem = default; size = 0; return; }
				size = (int)Api.GlobalSize(_hmem = hmem);
			}

			public void Dispose()
			{
				Api.GlobalUnlock(_hmem);
			}
		}

		/// <summary>
		/// Gets clipboard text without open/close.
		/// If format is 0, tries CF_UNICODETEXT and CF_HDROP.
		/// </summary>
		internal static unsafe string LibGetText(int format)
		{
			IntPtr h = default;
			if(format == 0) {
				h = Api.GetClipboardData(Api.CF_UNICODETEXT);
				if(h == default) format = Api.CF_HDROP;
			}
			if(format == 0) format = Api.CF_UNICODETEXT;
			else {
				h = Api.GetClipboardData(format); if(h == default) return null;
				if(format == Api.CF_HDROP) return string.Join("\r\n", _HdropToFiles(h));
			}

			using(new _GlobalLock(h, out var mem, out int len)) {
				if(mem == default) return null;
				var s = (char*)mem; var b = (byte*)s;

				Encoding enc = ClipFormats.LibGetTextEncoding(format, out bool unknown);
				if(unknown) {
					if((len & 1) != 0 || Util.LibCharPtr.Length(b, len) > len - 2) enc = Encoding.Default; //autodetect
				}

				if(enc == null) {
					len /= 2; while(len > 0 && s[len - 1] == '\0') len--;
					return new string(s, 0, len);
				} else {
					//most apps add single '\0' at the end. Some don't add. Some add many, eg Dreamweaver. Trim all.
					int charLen = enc.GetByteCount("\0");
					switch(charLen) {
					case 1:
						while(len > 0 && b[len - 1] == '\0') len--;
						break;
					case 2:
						for(int k = len / 2; k > 0 && s[k - 1] == '\0'; k--) len -= 2;
						break;
					case 4:
						var ip = (int*)s; for(int k = len / 4; k > 0 && ip[k - 1] == '\0'; k--) len -= 4;
						break;
					}
					return enc.GetString(b, len);
					//note: don't parse HTML format here. Let caller use GetHtml or parse itself.
				}
			}
		}

		/// <summary>
		/// Gets text from the clipboard.
		/// Returns null if there is no text.
		/// </summary>
		/// <param name="format">
		/// Clipboard format id. Default: <see cref="ClipFormats.Text"/> (CF_UNICODETEXT).
		/// If 0, tries to get text (<see cref="ClipFormats.Text"/>) or file paths (<see cref="ClipFormats.Files"/>; returns multiline text).
		/// Text encoding (UTF-16, ANSI, etc) depends on format; default UTF-16. See <see cref="ClipFormats.Register"/>.
		/// </param>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static string GetText(int format = ClipFormats.Text)
		{
			using(new AClipboard.LibOpenClipboard(false)) {
				return LibGetText(format);
			}
		}

		/// <summary>
		/// Gets clipboard data of any format as byte[].
		/// Returns null if there is no data of the specified format.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid format. Supported are all registered formats and standard formats &lt;CF_MAX except GDI handles.</exception>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static byte[] GetBinary(int format)
		{
			_CheckFormat(format);
			using(new AClipboard.LibOpenClipboard(false)) {
				var h = Api.GetClipboardData(format); if(h == default) return null;
				using(new _GlobalLock(h, out var mem, out int len)) {
					if(mem == default) return null;
					var b = new byte[len];
					Marshal.Copy(mem, b, 0, len);
					return b;
				}
			}
		}

#if SUPPORT_RAW_HANDLE
			public static IntPtr GetHandle(int format)
			{
				_CheckFormat(format, minimalCheckFormat: true);
				using(new LibOpenClipboard(false)) {
					return Api.GetClipboardData(format);
				}
			}
#endif

		/// <summary>
		/// Gets image from the clipboard. Uses clipboard format <see cref="ClipFormats.Image"/> (CF_BITMAP).
		/// Returns null if there is no data of this format.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Image.FromHbitmap"/>.</exception>
		public static Bitmap GetImage()
		{
			using(new AClipboard.LibOpenClipboard(false)) {
				var h = Api.GetClipboardData(Api.CF_BITMAP); if(h == default) return null;
				return Image.FromHbitmap(h, Api.GetClipboardData(Api.CF_PALETTE));
			}
		}

		/// <summary>
		/// Gets HTML text from the clipboard. Uses clipboard format <see cref="ClipFormats.Html"/> ("HTML Format").
		/// Returns null if there is no data of this format or if failed to parse it.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static string GetHtml() => GetHtml(out _, out _, out _);

		/// <summary>
		/// Gets HTML text from the clipboard. Uses clipboard format <see cref="ClipFormats.Html"/> ("HTML Format").
		/// Returns null if there is no data of this format or if failed to parse it.
		/// </summary>
		/// <param name="fragmentStart">Fragment start index in the returned string.</param>
		/// <param name="fragmentLength">Fragment length.</param>
		/// <param name="sourceURL">Source URL, or null if unavailable.</param>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static string GetHtml(out int fragmentStart, out int fragmentLength, out string sourceURL)
		{
			return LibParseHtmlFormatData(GetBinary(ClipFormats.Html), out fragmentStart, out fragmentLength, out sourceURL);
		}

		internal static string LibParseHtmlFormatData(byte[] b, out int fragmentStart, out int fragmentLength, out string sourceURL)
		{
			//Print(s);
			fragmentStart = fragmentLength = 0; sourceURL = null;
			if(b == null) return null;
			string s = Encoding.UTF8.GetString(b);

			int ish = s.Find("StartHTML:", true);
			int ieh = s.Find("EndHTML:", true);
			int isf = s.Find("StartFragment:", true);
			int ief = s.Find("EndFragment:", true);
			if(ish < 0 || ieh < 0 || isf < 0 || ief < 0) return null;
			isf = s.ToInt(isf + 14); if(isf < 0) return null;
			ief = s.ToInt(ief + 12); if(ief < isf) return null;
			ish = s.ToInt(ish + 10); if(ish < 0) ish = isf; else if(ish > isf) return null;
			ieh = s.ToInt(ieh + 8); if(ieh < 0) ieh = ief; else if(ieh < ief) return null;

			if(s.Length != b.Length) {
				if(ieh > b.Length) return null;
				_CorrectOffset(ref isf);
				_CorrectOffset(ref ief);
				_CorrectOffset(ref ish);
				_CorrectOffset(ref ieh);
			} else if(ieh > s.Length) return null;
			//Print(ish, ieh, isf, ief);

			int isu = s.Find("SourceURL:", true), ieu;
			if(isu >= 0 && (ieu = s.FindAny("\r\n", isu += 10)) >= 0) sourceURL = s.Substring(isu, ieu - isu);

			fragmentStart = isf - ish; fragmentLength = ief - isf;
			return s.Substring(ish, ieh - ish);

			void _CorrectOffset(ref int i)
			{
				i = Encoding.UTF8.GetCharCount(b, 0, i);
			}
		}

		/// <summary>
		/// Gets rich text (RTF) from the clipboard. Uses clipboard format <see cref="ClipFormats.Rtf"/> ("Rich Text Format").
		/// Returns null if there is no data of this format.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static string GetRtf() => GetText(ClipFormats.Rtf);

		//FUTURE: GetCsvTable

		/// <summary>
		/// Gets file paths from the clipboard. Uses clipboard format <see cref="ClipFormats.Files"/> (CF_HDROP).
		/// Returns null if there is no data of this format.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static string[] GetFiles()
		{
			using(new AClipboard.LibOpenClipboard(false)) {
				var h = Api.GetClipboardData(Api.CF_HDROP); if(h == default) return null;
				return _HdropToFiles(h);
			}
		}

		/// <summary>
		/// Gets file paths from HDROP.
		/// Returns array of 0 or more non-null elements.
		/// </summary>
		static unsafe string[] _HdropToFiles(IntPtr hdrop)
		{
			int n = Api.DragQueryFile(hdrop, -1, null, 0);
			var a = new string[n];
			var b = stackalloc char[500];
			for(int i = 0; i < n; i++) {
				int len = Api.DragQueryFile(hdrop, i, b, 500);
				a[i] = new string(b, 0, len);
			}
			return a;
		}

		#endregion

		#region contains

		/// <summary>
		/// Returns true if the clipboard contains data of the specified format.
		/// </summary>
		/// <param name="format">Clipboard format id. See <see cref="ClipFormats"/>.</param>
		/// <remarks>Calls API <msdn>IsClipboardFormatAvailable</msdn>.</remarks>
		public static bool Contains(int format)
		{
			return Api.IsClipboardFormatAvailable(format);
		}

		/// <summary>
		/// Returns the first of the specified formats that is in the clipboard.
		/// Returns 0 if the clipboard is empty. Returns -1 if the clipboard contains data but not in any of the specified formats.
		/// </summary>
		/// <param name="formats">Clipboard format ids. See <see cref="ClipFormats"/>.</param>
		/// <remarks>Calls API <msdn>GetPriorityClipboardFormat</msdn>.</remarks>
		public static int Contains(params int[] formats)
		{
			return Api.GetPriorityClipboardFormat(formats, formats.Length);
		}

		#endregion

		//CONSIDER: EnumFormats, OnClipboardChanged
	}
}

namespace Au.Types
{
	/// <summary>
	/// Some clipboard format ids.
	/// These and other standard and registered format ids can be used with <see cref="AClipboardData"/> class functions.
	/// </summary>
	public static class ClipFormats
	{
		/// <summary>The text format. Standard, API constant CF_UNICODETEXT. The default format of <see cref="AClipboardData"/> add/get text functions.</summary>
		public const int Text = Api.CF_UNICODETEXT;

		/// <summary>The image format. Standard, API constant CF_BITMAP. Used by <see cref="AClipboardData"/> add/get image functions.</summary>
		public const int Image = Api.CF_BITMAP;

		/// <summary>The file-list format. Standard, API constant CF_HDROP. Used by <see cref="AClipboardData"/> add/get files functions.</summary>
		public const int Files = Api.CF_HDROP;

		/// <summary>The HTML format. Registered, name "HTML Format". Used by <see cref="AClipboardData"/> add/get HTML functions.</summary>
		public static int Html { get; } = Api.RegisterClipboardFormat("HTML Format");

		/// <summary>The RTF format. Registered, name "Rich Text Format". Used by <see cref="AClipboardData"/> add/get RTF functions.</summary>
		public static int Rtf { get; } = Api.RegisterClipboardFormat("Rich Text Format");

		/// <summary>
		/// The "Clipboard Viewer Ignore" registered format.
		/// </summary>
		/// <remarks>
		/// Some clipboard viewer/manager programs don't try to get clipboard data if this format is present. For example Ditto, Clipdiary.
		/// The copy/paste functions of this library add this format to the clipboard to avoid displaying the temporary text/data in these programs, which also could make the paste function slower and less reliable.
		/// </remarks>
		public static int ClipboardViewerIgnore { get; } = Api.RegisterClipboardFormat("Clipboard Viewer Ignore");

		/// <summary>
		/// Registers a clipboard format and returns its id. If already registered, just returns id.
		/// </summary>
		/// <param name="name">Format name.</param>
		/// <param name="textEncoding">Text encoding, if it's a text format. Used by <see cref="AClipboardData.GetText"/>, <see cref="AClipboardData.AddText"/> and functions that call them. For example <see cref="Encoding.UTF8"/> or <see cref="Encoding.Default"/> (ANSI). If null, text of unknown formats is considered Unicode UTF-16 (no encoding/decoding needed).</param>
		/// <remarks>Calls API <msdn>RegisterClipboardFormat</msdn>.</remarks>
		public static int Register(string name, Encoding textEncoding = null)
		{
			var R = Api.RegisterClipboardFormat(name);
			if(textEncoding != null && R != 0 && R != Html && R != Rtf) s_textEncoding[R] = textEncoding;
			return R;
		}

		static readonly ConcurrentDictionary<int, Encoding> s_textEncoding = new ConcurrentDictionary<int, Encoding>();

		/// <summary>
		/// Gets text encoding for format.
		/// Returns null if UTF-16 or if the format is unknown and not in s_textEncoding.
		/// </summary>
		internal static Encoding LibGetTextEncoding(int format, out bool unknown)
		{
			unknown = false;
			if(format == 0 || format == Api.CF_UNICODETEXT || format == Api.CF_HDROP) return null;
			if(format == Rtf || format < Api.CF_MAX) return Encoding.Default;
			if(format == Html) return Encoding.UTF8;
			if(s_textEncoding.TryGetValue(format, out var enc)) return enc == Encoding.Unicode ? null : enc;
			unknown = true;
			return null;
		}
	}
}
