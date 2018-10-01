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
using System.Drawing.Imaging;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Util;

#pragma warning disable 649

namespace Au.Controls
{
	public static unsafe class ImageUtil
	{
		#region API

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		internal struct BITMAPFILEHEADER
		{
			public ushort bfType;
			public int bfSize;
			public ushort bfReserved1;
			public ushort bfReserved2;
			public int bfOffBits;
		}

		internal struct BITMAPCOREHEADER
		{
			public int bcSize;
			public ushort bcWidth;
			public ushort bcHeight;
			public ushort bcPlanes;
			public ushort bcBitCount;
		}

		internal struct BITMAPV5HEADER
		{
			public int bV5Size;
			public int bV5Width;
			public int bV5Height;
			public ushort bV5Planes;
			public ushort bV5BitCount;
			public int bV5Compression;
			public int bV5SizeImage;
			public int bV5XPelsPerMeter;
			public int bV5YPelsPerMeter;
			public int bV5ClrUsed;
			public int bV5ClrImportant;
			public uint bV5RedMask;
			public uint bV5GreenMask;
			public uint bV5BlueMask;
			public uint bV5AlphaMask;
			public uint bV5CSType;
			public CIEXYZTRIPLE bV5Endpoints;
			public uint bV5GammaRed;
			public uint bV5GammaGreen;
			public uint bV5GammaBlue;
			public uint bV5Intent;
			public uint bV5ProfileData;
			public uint bV5ProfileSize;
			public uint bV5Reserved;
		}

		internal struct CIEXYZTRIPLE
		{
			public CIEXYZ ciexyzRed;
			public CIEXYZ ciexyzGreen;
			public CIEXYZ ciexyzBlue;
		}

		internal struct CIEXYZ
		{
			public int ciexyzX;
			public int ciexyzY;
			public int ciexyzZ;
		}

		#endregion

		internal struct LibBitmapFileInfo
		{
			/// <summary>
			/// Can be BITMAPINFOHEADER/BITMAPV5HEADER or BITMAPCOREHEADER.
			/// </summary>
			public void* biHeader;
			public int width, height, bitCount;
			public bool isCompressed;
		}

		/// <summary>
		/// Gets some info from BITMAPINFOHEADER or BITMAPCOREHEADER.
		/// Checks if it is valid bitmap file header. Returns false if invalid.
		/// </summary>
		internal static bool LibGetBitmapFileInfo(byte[] mem, out LibBitmapFileInfo x)
		{
			x = new LibBitmapFileInfo();
			fixed (byte* bp = mem) {
				BITMAPFILEHEADER* f = (BITMAPFILEHEADER*)bp;
				int minHS = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPCOREHEADER);
				if(mem.Length <= minHS || f->bfType != (((byte)'M' << 8) | (byte)'B') || f->bfOffBits >= mem.Length || f->bfOffBits < minHS)
					return false;

				Api.BITMAPINFOHEADER* h = (Api.BITMAPINFOHEADER*)(f + 1);
				int siz = h->biSize;
				if(siz >= sizeof(Api.BITMAPINFOHEADER) && siz <= sizeof(BITMAPV5HEADER) * 2) {
					x.width = h->biWidth;
					x.height = h->biHeight;
					x.bitCount = h->biBitCount;
					x.isCompressed = h->biCompression != 0;
				} else if(siz == sizeof(BITMAPCOREHEADER)) {
					BITMAPCOREHEADER* ch = (BITMAPCOREHEADER*)h;
					x.width = ch->bcWidth;
					x.height = ch->bcHeight;
					x.bitCount = ch->bcBitCount;
				} else return false;

				x.biHeader = h;
				return true;
				//note: don't check f->bfSize. Sometimes it is incorrect (> or < memSize). All programs open such files. Instead check other data later.
			}
		}

		/// <summary>
		/// Image type as detected by <see cref="ImageTypeFromString(bool, string)"/>.
		/// </summary>
		public enum ImageType
		{
			/// <summary>The string isn't image.</summary>
			None,

			/// <summary>Compressed and Base64-encoded .bmp file data with "~:" prefix. See <see cref="ImageToString(string)"/>.</summary>
			Base64CompressedBmp,

			/// <summary>Base64-encoded .png/gif/jpg file data with "image:" prefix.</summary>
			Base64PngGifJpg,

			/// <summary>"resource:name". An image resource name from managed resources of the entry assembly.</summary>
			Resource,

			/// <summary>.bmp file path.</summary>
			Bmp,

			/// <summary>.png, .gif or .jpg file path.</summary>
			PngGifJpg,

			/// <summary>.ico file path.</summary>
			Ico,

			/// <summary>.cur or .ani file path.</summary>
			Cur,

			/// <summary>Icon from a .dll or other file containing icons, like @"C:\a\b.dll,15".</summary>
			IconLib,

			/// <summary>None of other image types, when <i>anyFile</i> is true.</summary>
			ShellIcon
		}

		/// <summary>
		/// Gets image type from string.
		/// </summary>
		/// <param name="anyFile">When the string is valid but not of any image type, return ShellIcon instead of None.</param>
		/// <param name="s">File path etc. See <see cref="ImageType"/>.</param>
		/// <param name="length">If -1, calls LibCharPtr.Length(s).</param>
		internal static ImageType ImageTypeFromString(bool anyFile, byte* s, int length = -1)
		{
			if(length < 0) length = LibCharPtr.Length(s);
			if(length < (anyFile ? 2 : 8)) return ImageType.None; //C:\x.bmp or .h
			char c1 = (char)s[0], c2 = (char)s[1];

			//special strings
			switch(c1) {
			case '~': return (c2 == ':') ? ImageType.Base64CompressedBmp : ImageType.None;
			case 'i': if(LibCharPtr.AsciiStartsWith(s, "image:")) return ImageType.Base64PngGifJpg; break;
			case 'r': if(LibCharPtr.AsciiStartsWith(s, "resource:")) return ImageType.Resource; break;
			}

			//file path
			if(length >= 8 && (c1 == '%' || (c2 == ':' && Char_.IsAsciiAlpha(c1)) || (c1 == '\\' && c2 == '\\'))) { //is image file path?
				byte* ext = s + length - 3;
				if(ext[-1] == '.') {
					if(LibCharPtr.AsciiStartsWithI(ext, "bmp")) return ImageType.Bmp;
					if(LibCharPtr.AsciiStartsWithI(ext, "png")) return ImageType.PngGifJpg;
					if(LibCharPtr.AsciiStartsWithI(ext, "gif")) return ImageType.PngGifJpg;
					if(LibCharPtr.AsciiStartsWithI(ext, "jpg")) return ImageType.PngGifJpg;
					if(LibCharPtr.AsciiStartsWithI(ext, "ico")) return ImageType.Ico;
					if(LibCharPtr.AsciiStartsWithI(ext, "cur")) return ImageType.Cur;
					if(LibCharPtr.AsciiStartsWithI(ext, "ani")) return ImageType.Cur;
				} else if(Char_.IsAsciiDigit(ext[2])) { //can be like C:\x.dll,10
					byte* k = ext + 1, k2 = s + 8;
					for(; k > k2; k--) if(!Char_.IsAsciiDigit(*k)) break;
					if(*k == '-') k--;
					if(*k == ',' && k[-4] == '.' && Char_.IsAsciiAlpha(k[-1])) return ImageType.IconLib;
				}
			}

			if(anyFile) return ImageType.ShellIcon; //can be other file type, URL, .ext, :: ITEMIDLIST, ::{CLSID}
			return ImageType.None;
		}

		/// <summary>
		/// Gets image type from string.
		/// </summary>
		/// <param name="anyFile">When the string is valid but not of any image type, return ShellIcon instead of None.</param>
		/// <param name="s">File path etc. See <see cref="ImageType"/>.</param>
		public static ImageType ImageTypeFromString(bool anyFile, string s)
		{
			var b = Convert_.Utf8FromString(s);
			fixed (byte* p = b) return ImageTypeFromString(true, p, b.Length - 1);
		}

		/// <summary>
		/// Loads image and returns its data in .bmp file format.
		/// </summary>
		/// <param name="s">Depends on t. File path or resource name without prefix or Base64 image data without prefix.</param>
		/// <param name="t">Image type and string format.</param>
		/// <param name="searchPath">Use <see cref="File_.SearchPath"/></param>
		/// <remarks>Supports environment variables etc. If not full path, searches in <see cref="Folders.ThisAppImages"/>.</remarks>
		public static byte[] BmpFileDataFromString(string s, ImageType t, bool searchPath = false)
		{
			//Print(t, s);
			try {
				switch(t) {
				case ImageType.Bmp:
				case ImageType.PngGifJpg:
				case ImageType.Cur:
					if(searchPath) {
						s = File_.SearchPath(s, Folders.ThisAppImages);
						if(s == null) return null;
					} else {
						if(!Path_.IsFullPathExpandEnvVar(ref s)) return null;
						s = Path_.Normalize(s, Folders.ThisAppImages);
						if(!File_.ExistsAsFile(s)) return null;
					}
					break;
				}

				switch(t) {
				case ImageType.Base64CompressedBmp:
					return Convert_.Decompress(Convert_.Base64Decode(s));
				case ImageType.Base64PngGifJpg:
					using(var stream = new MemoryStream(Convert_.Base64Decode(s), false)) {
						return _ImageToBytes(Image.FromStream(stream));
					}
				case ImageType.Resource:
					return _ImageToBytes(Resources_.GetAppResource(s) as Image);
				case ImageType.Bmp:
					return File.ReadAllBytes(s);
				case ImageType.PngGifJpg:
					return _ImageToBytes(Image.FromFile(s));
				case ImageType.Ico:
				case ImageType.IconLib:
				case ImageType.ShellIcon:
				case ImageType.Cur:
					return _IconToBytes(s, t == ImageType.Cur, searchPath);
				}
			}
			catch(Exception ex) { Debug_.Print(ex.Message + "    " + s); }
			return null;
		}

		static byte[] _ImageToBytes(Image im)
		{
			if(im == null) return null;
			try {
				//workaround for the black alpha problem. Does not make slower.
				var flags = (ImageFlags)im.Flags;
				if(0 != (flags & ImageFlags.HasAlpha)) { //most png have it
					var t = new Bitmap(im.Width, im.Height);
					t.SetResolution(im.HorizontalResolution, im.VerticalResolution);

					using(var g = Graphics.FromImage(t)) {
						g.Clear(Color.White);
						g.DrawImageUnscaled(im, 0, 0);
					}

					var old = im; im = t; old.Dispose();
				}

				using(var m = new MemoryStream()) {
					im.Save(m, ImageFormat.Bmp);
					return m.ToArray();
				}
			}
			finally { im.Dispose(); }
		}

		static byte[] _IconToBytes(string s, bool isCursor, bool searchPath)
		{
			//info: would be much easier with Icon.ToBitmap(), but it creates bitmap that is displayed incorrectly when we draw it in Scintilla.
			//	Mask or alpha areas then are black.
			//	Another way - draw it like in the png case. Not tested. Maybe would not work with cursors. Probably slower.

			IntPtr hi; int siz;
			if(isCursor) {
				hi = Api.LoadImage(default, s, Api.IMAGE_CURSOR, 0, 0, Api.LR_LOADFROMFILE | Api.LR_DEFAULTSIZE);
				siz = Api.GetSystemMetrics(Api.SM_CXCURSOR);
				//note: if LR_DEFAULTSIZE, uses SM_CXCURSOR, normally 32. It may be not what Explorer displays eg in Cursors folder. But without it gets the first cursor, which often is large, eg 128.
			} else {
				hi = Icon_.GetFileIconHandle(s, 16, searchPath ? GIFlags.SearchPath : 0);
				siz = 16;
			}
			if(hi == default) return null;
			try {
				using(var m = new MemoryBitmap(siz, siz)) {
					RECT r = (0, 0, siz, siz, false);
					Api.FillRect(m.Hdc, r, Api.GetStockObject(0)); //WHITE_BRUSH
					if(!Api.DrawIconEx(m.Hdc, 0, 0, hi, siz, siz)) return null;

					int headersSize = sizeof(BITMAPFILEHEADER) + sizeof(Api.BITMAPINFOHEADER);
					var a = new byte[headersSize + siz * siz * 3];
					fixed (byte* p = a) {
						BITMAPFILEHEADER* f = (BITMAPFILEHEADER*)p;
						Api.BITMAPINFOHEADER* h = (Api.BITMAPINFOHEADER*)(f + 1);
						for(int i = 0; i < headersSize; i++) p[i] = 0;
						byte* bits = p + headersSize;
						h->biSize = sizeof(Api.BITMAPINFOHEADER); h->biBitCount = 24; h->biWidth = siz; h->biHeight = siz; h->biPlanes = 1;
						if(0 == Api.GetDIBits(m.Hdc, m.Hbitmap, 0, siz, bits, h, 0)) return null; //DIB_RGB_COLORS
						f->bfType = ((byte)'M' << 8) | (byte)'B'; f->bfOffBits = headersSize; f->bfSize = a.Length;
					}
					return a;
				}
			}
			finally {
				if(isCursor) Api.DestroyCursor(hi); else Api.DestroyIcon(hi);
			}
		}

#if false //currently not used

	/// <summary>
	/// Gets icon or cursor size from its native handle.
	/// Returns 0 if failed.
	/// </summary>
	public static int GetIconSizeFromHandle(IntPtr hi)
	{
		if(!Api.GetIconInfo(hi, out var ii)) return 0;
		try {
			var hb = (ii.hbmColor != default) ? ii.hbmColor : ii.hbmMask;
			Api.BITMAP b;
			if(0 == Api.GetObject(hb, sizeof(BITMAP), &b)) return 0;
			Print(b.bmWidth, b.bmHeight, b.bmBits);
			return b.bmWidth;
		}
		finally {
			Api.DeleteObject(ii.hbmColor);
			Api.DeleteObject(ii.hbmMask);
		}
	}
#endif

		/// <summary>
		/// Compresses .bmp file data (<see cref="Convert_.Compress"/>) and Base64-encodes.
		/// Returns string with "~:" prefix.
		/// </summary>
		public static string BmpFileDataToString(byte[] bmpFileData)
		{
			if(bmpFileData == null) return null;
			return "~:" + Convert.ToBase64String(Convert_.Compress(bmpFileData));
		}

		/// <summary>
		/// Converts image file data to string that can be used in source code instead of file path. It is supported by some functions of this library, for example <see cref="WinImage.Find"/>.
		/// Returns string with prefix "image:" (Base-64 encoded .png/gif/jpg file data) or "~:" (Base-64 encoded compressed .bmp file data).
		/// Supports all <see cref="ImageType"/> formats. For non-image files gets icon. Converts icons to bitmap.
		/// Returns null if path is not a valid image string or the file does not exist or failed to load.
		/// </summary>
		/// <remarks>Supports environment variables etc. If not full path, searches in Folders.ThisAppImages and standard directories.</remarks>
		public static string ImageToString(string path)
		{
			var t = ImageTypeFromString(true, path);
			switch(t) {
			case ImageType.None: return null;
			case ImageType.Base64CompressedBmp:
			case ImageType.Base64PngGifJpg:
			case ImageType.Resource:
				return path;
			case ImageType.PngGifJpg:
				path = File_.SearchPath(path, Folders.ThisAppImages); if(path == null) return null;
				try { return "image:" + Convert.ToBase64String(File_.LoadBytes(path)); }
				catch(Exception ex) { Debug_.Print(ex.Message); return null; }
			}
			return BmpFileDataToString(BmpFileDataFromString(path, t, true));
		}
	}
}
