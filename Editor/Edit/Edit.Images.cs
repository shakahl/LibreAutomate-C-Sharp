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

//partial class Edit
//{
unsafe class SciImages :IDisposable
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

	public bool HideAnnotations { get; set; }

	public SciImages(Scintilla sci, bool isEditor = false)
	{
		_isEditor = isEditor;
		_c = sci;
		_sci_AnnotationDrawCallback = _AnnotationDrawCallback_;
		_c.DirectMessage(SciCommon.SCI_SETANNOTATIONDRAWCALLBACK, Zero, Marshal.GetFunctionPointerForDelegate(_sci_AnnotationDrawCallback));
		_c.AnnotationVisible = Annotation.Standard;
		//keep annotations always visible. Adding annotations while visible is slower, but much faster than getting images from DB.
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

	public void ClearCache(bool ifBig = false)
	{
		if(_a == null) return;
		if(ifBig && _cacheSize < 500 * 1024) return;
		_a.Clear();
		_cacheSize = 0;
	}

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
		var imType = EImageUtil.ImageTypeFromString(!_isEditor, s, i, i2 - i);
		if(imType == EImageUtil.ImageType.None) goto g1;

		if(_a == null) _a = new List<_Image>();

		//is already loaded?
		int hash = Convert_.HashFnv1(s, i, i2 - i);
		for(int j = 0; j < _a.Count; j++) if(_a[j].nameHash == hash) return _a[j];

		//var test = s.Substring(i, i2 - i);
		//PrintList(test, EImageUtil.ImageToEmbeddedString(test));

		switch(imType) {
		case EImageUtil.ImageType.Embedded: i += 2; break; //~:
		case EImageUtil.ImageType.Resource: i += 9; break; //resource:
		}

		string path = s.Substring(i, i2 - i);

		//load
		byte[] b = EImageUtil.BitmapFileDataFromString(path, imType, !_isEditor);
		if(b == null) goto g1;
		if(!EImageUtil.GetBitmapFileInfo(b, out var q)) goto g1;

		//to avoid memory problems when loaded many big images, eg showing all png in Program Files
		if(_cacheSize >= 2 * 1024 * 1024 || _a.Count >= 200) ClearCache(); //will auto reload when need, it does not noticeably slow down
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

	SciCommon.Sci_AnnotationDrawCallback _sci_AnnotationDrawCallback;
	unsafe int _AnnotationDrawCallback_(void* cbParam, ref SciCommon.SCAnnotationDrawCallback c)
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

		if(_pen == Zero) _pen = CreatePen(0, 1, 0x8000);
		var oldPen = Api.SelectObject(hdc, _pen);
		try {
			//handle exceptions because SetDIBitsToDevice may read more than need, like CreateDIBitmap, although never noticed this.
			//for each image string in this code line: find images cached in _a, or load and add to _a; draw image; draw frame.
			_Image u; bool isMulti = false;
			int x = r.left + 1;
			for(int i = 0; null != (u = _GetImageInLine(text, ref i, text.Length, ref isMulti));) {
				//hasImages = true;
				//draw image
				if(!EImageUtil.GetBitmapFileInfo(u.data, out var q)) { Debug.Assert(false); continue; }
				fixed (byte* bp = u.data) {
					EImageUtil.BITMAPFILEHEADER* f = (EImageUtil.BITMAPFILEHEADER*)bp;

					int isFirstLine = (c.annotLine == 0) ? 1 : 0, hLine = r.bottom - r.top;
					int currentTop = c.annotLine * hLine, currentBottom = currentTop + hLine, imageBottom = q.height + IMAGE_MARGIN_TOP;
					int y = r.top + isFirstLine * IMAGE_MARGIN_TOP, yy = Math.Min(currentBottom, imageBottom) - currentTop;
					if(imageBottom > currentTop && q.width > 0 && q.height > 0) {
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
			}
		}
		catch(Exception ex) { DebugPrint(ex.Message); }
		finally { Api.SelectObject(hdc, oldPen); }

		return width + 1;
	}

	[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int strtoul(sbyte* s, sbyte** endPtr, int numberBase = 0);

	[DllImport("gdi32.dll")]
	internal static extern IntPtr CreatePen(int iStyle, int cWidth, uint color);

	[DllImport("gdi32.dll")]
	internal static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, int w, int h, int xSrc, int ySrc, int StartScan, int cLines, byte* lpvBits, void* lpbmi, uint ColorUse); //BITMAPINFO*

}
//}
