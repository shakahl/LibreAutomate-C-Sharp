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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

using ScintillaNET;

namespace G.Controls
{
	/// <summary>
	/// Gets image file paths etc from Scintilla control text and displays the images below that lines.
	/// A single variable can be used with multiple Scintilla controls. Benefit - shared cache.
	/// </summary>
	public unsafe class SciImages :IDisposable
	{
		class _Image
		{
			public byte[] data;
			public int nameHash;
			public int nLines, width;
		}

		List<_Image> _a;
		IntPtr _pen;
		Scintilla _c;
		int _lineHeight;
		int _cacheSize;
		StringBuilder _sb;
		bool _isEditor;

		/// <summary>
		/// Use this if want to hide all annotations (annotation text).
		/// When using this class, don't use Csintilla API to show/hide all annotations. This class sets it to show always, because it draws images in annotation areas.
		/// </summary>
		public bool HideAnnotations { get; set; }

		/// <summary>
		/// Prepares this variable and the Scintilla control to display images.
		/// Sets sci.AnnotationVisible to visible. Need it because will draw images in annotation areas.
		/// </summary>
		/// <param name="sci">The control.</param>
		/// <param name="isEditor">Display images that are not in "&lt;image "path etc"&gt; tag. Then does not display icons of files that don't contain images.</param>
		public SciImages(Scintilla sci, bool isEditor = false)
		{
			_c = sci;
			_isEditor = isEditor;
			_sci_AnnotationDrawCallback = _AnnotationDrawCallback_;
			_c.DirectMessage(SciCommon.SCI_SETANNOTATIONDRAWCALLBACK, Zero, Marshal.GetFunctionPointerForDelegate(_sci_AnnotationDrawCallback));
			_c.AnnotationVisible = Annotation.Standard; //keep annotations always visible. Adding annotations while visible is slower, but much faster than getting images from files etc.
		}

		public void Dispose()
		{
			if(_pen != Zero) { Api.DeleteObject(_pen); _pen = Zero; }

			GC.SuppressFinalize(this);
		}

		~SciImages()
		{
			Dispose();
		}

		/// <summary>
		/// Removes all cached images.
		/// Will auto-reload from files etc when need.
		/// </summary>
		public void ClearCache()
		{
			if(_a == null) return;
			_a.Clear();
			_cacheSize = 0;
		}

		/// <summary>
		/// If cache is large (at least MaxCacheSize and 4 images), removes about 3/4 of older cached images.
		/// Will auto-reload from files etc when need.
		/// </summary>
		public void CompactCache()
		{
			if(_a == null) return;
			//Print(_cacheSize);
			if(_cacheSize < MaxCacheSize || _a.Count < 4) return;
			_cacheSize = 0;
			int n = _a.Count, max = MaxCacheSize / 4;
			while(_cacheSize < max && n > 2) _cacheSize += _a[--n].data.Length;
			_a.RemoveRange(0, n);
		}

		/// <summary>
		/// Maximal size of the image cache.
		/// Default 2 MB.
		/// </summary>
		public int MaxCacheSize { get; set; } = 2 * 1024 * 1024;

		//TODO: cache:
		//	Maybe don't cache some: embedded, very large.
		//	Maybe in cache store zipped.

		public void ParseTextAndSetAnnotationsForImages(int iLine, string text, int start = 0, int length = -1)
		{
			if(length < 0) length = text.Length - start;
			Debug.Assert(start >= 0 && start + length <= text.Length);
			if(length < 10) return; //"C:\x.ico"
									//bool annotAdded = false;

			for(int i = start, iTo = i + length; i < iTo; iLine++) {
				int nAnnotLines = 0, totalWidth = 0;
				_Image u; bool isMulti = false;
				while(null != (u = _GetImageInLine(text, ref i, iTo, ref isMulti))) {
					if(nAnnotLines < u.nLines) nAnnotLines = u.nLines;
					if(totalWidth > 0) totalWidth += 30;
					totalWidth += u.width;
				}

				if(nAnnotLines == 0) continue;

				if(_sb == null) _sb = new StringBuilder(); else _sb.Clear();
				_sb.AppendFormat("|{0:x6}|", Calc.MakeUint(Math.Min(totalWidth, 0xffff), nAnnotLines));
				if(nAnnotLines > 1) _sb.Append('\n', nAnnotLines - 1);

				string annot = _c.Lines[iLine].AnnotationText, sNew = null;
				if(annot.Length == _sb.Length || (annot.Length > 0 && !HideAnnotations)) {
					bool skip = false;
					if(HideAnnotations) {
						sNew = _sb.ToString();
						if(sNew == annot) skip = true;
					} else {
						if(annot[0] == '|' && annot.Length >= 8 && annot[7] == '|') skip = true;
						else { _sb.Append('\n'); _sb.Append(annot); sNew = _sb.ToString(); }
					}
					if(skip) { /*Print("SAME");*/ continue; }
				} else sNew = _sb.ToString();

				//Print(sNew);
				_c.Lines[iLine].AnnotationText = sNew;
				//annotAdded = true;
			}

			//never mind: scintilla prints without annotations, therefore without images too.
		}

		_Image _GetImageInLine(string s, ref int iFrom, int iTo, ref bool isMulti)
		{
			g1:
			int i = iFrom;
			if(i >= iTo - 4) { iFrom = iTo; isMulti = false; return null; }
			//find next "string". If not found, return next line or the end of whole string.
			if(!isMulti) //else i is at image2 in "image1|image2"
			{
				i = _FindQuoteInLine(s, i, iTo);
				if(i == iTo || s[i++] != '\"') { iFrom = i; isMulti = false; return null; }
			}
			iFrom = _FindQuoteInLine(s, i, iTo);
			if(iFrom == iTo || s[iFrom++] != '\"') { isMulti = false; return null; }
			int i2 = iFrom - 1;

			//if not editor, skip if not <image "..."
			if(!_isEditor && !isMulti) { if(i < 8 || !s.EqualsPart_(i - 8, "<image ")) goto g1; }

			//support "image1|image2|..."
			int i3 = s.IndexOf('|', i, i2 - i);
			if(i3 >= i) { i2 = i3; iFrom = i3 + 1; isMulti = true; } else isMulti = false;

			//is it an image string?
			var imType = ImageUtil.ImageTypeFromString(!_isEditor, s, i, i2 - i);
			if(imType == ImageUtil.ImageType.None) goto g1;

			if(_a == null) _a = new List<_Image>();

			//is already loaded?
			int hash = Convert_.HashFnv1(s, i, i2 - i);
			for(int j = 0; j < _a.Count; j++) if(_a[j].nameHash == hash) return _a[j];

			//var test = s.Substring(i, i2 - i);
			//PrintList(test, EImageUtil.ImageToString(test));

			switch(imType) {
			case ImageUtil.ImageType.Embedded: i += 2; break; //~:
			case ImageUtil.ImageType.Resource: i += 9; break; //resource:
			}

			string path = s.Substring(i, i2 - i);

			//load
			byte[] b = ImageUtil.BitmapFileDataFromString(path, imType, !_isEditor);
			if(b == null) goto g1;
			if(!ImageUtil.GetBitmapFileInfo(b, out var q)) goto g1;

			//to avoid memory problems when loaded many big images, eg showing all png in Program Files
			CompactCache(); //will auto reload when need, it does not noticeably slow down
			_cacheSize += b.Length;

			//add to _a
			var u = new _Image() {
				data = b,
				nameHash = hash,
				width = q.width
			};
			_a.Add(u);

			//calculate n annotation lines from image height, max 10 lines
			if(0 == _lineHeight) _lineHeight = _c.Lines[0].Height;
			int n = (q.height + IMAGE_MARGIN_TOP + IMAGE_MARGIN_BOTTOM + (_lineHeight - 1)) / _lineHeight;
			u.nLines = Math.Min(n, _isEditor ? 10 : 250);

			return u;
		}

		const int IMAGE_MARGIN_TOP = 2; //frame + 1
		const int IMAGE_MARGIN_BOTTOM = 1; //just for frame. It is minimal margin, in most cases will be more.

		//Searches for '\"' or '\n' until iTo.
		int _FindQuoteInLine(string s, int i, int iTo)
		{
			for(; i < iTo && s[i] != '\n'; i++) if(s[i] == '\"') break;
			return i;
		}

		SciCommon.AnnotationDrawCallback _sci_AnnotationDrawCallback;
		unsafe int _AnnotationDrawCallback_(void* cbParam, ref SciCommon.AnnotationDrawCallbackData c)
		{
			sbyte* s = c.text, s2;
			if(c.textLen < 8 || s[0] != '|' || s[7] != '|') return 0;
			int k = strtoul(s + 1, &s2, 16); if((int)(s2 - s) != 7) return 0;
			int nLines = Calc.HiUshort(k), width = Calc.LoUshort(k);
			//PrintList(nLines, width);

			//just get width?
			if(c.step == 0) return width + 1;
			if(c.annotLine >= nLines) return 0; //an annotation text line below the image lines

			var hdc = c.hdc;
			RECT r = c.rect;

			//Get line text, to find image strings.
			//Cannot store array indices in annotation, because they may change.
			//Also cannot store image strings in annotation, because then boxed annotation would be too wide (depends on text length).

			var text = _c.Lines[c.line].Text; if(Empty(text)) return 1;
			//bool hasImages = false;

			//Perf.First();
			bool isPenSelected = false; IntPtr oldPen = Zero;
			try {
				//handle exceptions because SetDIBitsToDevice may read more than need, like CreateDIBitmap, although never noticed this.
				//for each image string in this code line: find images cached in _a, or load and add to _a; draw image; draw frame.
				_Image u; bool isMulti = false;
				int x = r.left + 1;
				for(int i = 0; null != (u = _GetImageInLine(text, ref i, text.Length, ref isMulti));) {
					//hasImages = true;
					//draw image
					if(!ImageUtil.GetBitmapFileInfo(u.data, out var q)) { Debug.Assert(false); continue; }
					int isFirstLine = (c.annotLine == 0) ? 1 : 0, hLine = r.bottom - r.top;
					int currentTop = c.annotLine * hLine, currentBottom = currentTop + hLine, imageBottom = q.height + IMAGE_MARGIN_TOP;
					int y = r.top + isFirstLine * IMAGE_MARGIN_TOP, yy = Math.Min(currentBottom, imageBottom) - currentTop;

					if(imageBottom > currentTop && q.width > 0 && q.height > 0) {
						fixed (byte* bp = u.data) {
							ImageUtil.BITMAPFILEHEADER* f = (ImageUtil.BITMAPFILEHEADER*)bp;
							byte* pBits = bp + f->bfOffBits;
							int bytesInLine = Calc.AlignUp(q.width * q.bitCount, 32) / 8;
							int sizF = u.data.Length - f->bfOffBits, siz = bytesInLine * q.height;
							if(q.isCompressed) {
								//this is slow with big images. It seems processes current line + all remaining lines. Such bitmaps are rare.
								int yOffs = -c.annotLine * hLine; if(isFirstLine == 0) yOffs += IMAGE_MARGIN_TOP;
								var ok = SetDIBitsToDevice(hdc, x, r.top + isFirstLine * IMAGE_MARGIN_TOP,
									q.width, q.height, 0, yOffs, 0, q.height,
									pBits, q.biHeader, 0); //DIB_RGB_COLORS
								Debug.Assert(ok > 0);
							} else if(siz <= sizF) {
								//this is fast, but cannot use with compressed bitmaps
								int hei = yy - y, bmY = q.height - (currentTop - ((isFirstLine ^ 1) * IMAGE_MARGIN_TOP) + hei);
								var ok = SetDIBitsToDevice(hdc, x, r.top + isFirstLine * IMAGE_MARGIN_TOP,
									q.width, hei, 0, 0, 0, hei,
									pBits + bmY * bytesInLine, q.biHeader, 0); //DIB_RGB_COLORS
								Debug.Assert(ok > 0);
							} else Debug.Assert(false);

							//could use this instead, but very slow with big images. It seems always processes whole bitmap, not just current line.
							//int hei=yy-y, bmY=q.height-(currentTop-((isFirstLine ^ 1)*IMAGE_MARGIN_TOP)+hei);
							//StretchDIBits(hdc,
							//	x, y, q.width, hei,
							//	0, bmY, q.width, hei,
							//	pBits, h, 0, SRCCOPY);
						}
					}

					//draw frame
					if(_pen == Zero) _pen = CreatePen(0, 1, 0x8000);
					oldPen = Api.SelectObject(hdc, _pen); isPenSelected = true;
					int xx = x + q.width;
					if(isFirstLine != 0) y--;
					if(yy > y) {
						MoveToEx(hdc, x - 1, y); LineTo(hdc, x - 1, yy); //left |
						MoveToEx(hdc, xx, y); LineTo(hdc, xx, yy); //right |
						if(isFirstLine != 0) { MoveToEx(hdc, x, y); LineTo(hdc, xx, y); } //top _
					}
					if(yy >= y && yy < hLine) { MoveToEx(hdc, x - 1, yy); LineTo(hdc, xx + 1, yy); } //bottom _

					x += u.width + 30;
				}
			}
			catch(Exception ex) { DebugPrint(ex.Message); }
			finally { if(isPenSelected) Api.SelectObject(hdc, oldPen); }
			//Perf.NW();

			return width + 1;

			//speed: fast. The fastest way. Don't need bitmap handle, memory DC, etc.
			//tested: don't know what ColorUse param of SetDIBitsToDevice does, but DIB_RGB_COLORS works for any h_biBitCount.
			//speed if drawing frame: multiple LineTo is faster than single PolyPolyline.
			//tested: GDI+ much slower, particularly DrawImage().
			//tested: in QM2 was used LZO compression, now ZIP (DeflateStream). ZIP compresses better, but not so much. LZO is faster, but ZIP is fast enough. GIF and JPG in most cases compress less than ZIP and sometimes less than LZO.
			//tested: saving in 8-bit format in most cases does not make much smaller when compressed. For screenshots we reduce colors to 4-bit.
		}

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int strtoul(sbyte* s, sbyte** endPtr, int numberBase = 0);

		[DllImport("gdi32.dll")]
		internal static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, int w, int h, int xSrc, int ySrc, int StartScan, int cLines, byte* lpvBits, void* lpbmi, uint ColorUse); //BITMAPINFO*

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreatePen(int iStyle, int cWidth, uint color);

		[DllImport("gdi32.dll")]
		internal static extern bool MoveToEx(IntPtr hdc, int x, int y, POINT* lppt = null);

		[DllImport("gdi32.dll")]
		internal static extern bool LineTo(IntPtr hdc, int x, int y);

	}
}
