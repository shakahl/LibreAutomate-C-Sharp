using Au.Types;
using Au.More;
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//using System.Linq;

#pragma warning disable 649

namespace Au.Controls
{
	public static unsafe class KImageUtil
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

		internal struct BitmapFileInfo_
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
		internal static bool GetBitmapFileInfo_(byte[] mem, out BitmapFileInfo_ x) {
			x = new BitmapFileInfo_();
			fixed (byte* bp = mem) {
				BITMAPFILEHEADER* f = (BITMAPFILEHEADER*)bp;
				int minHS = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPCOREHEADER);
				if (mem.Length <= minHS || f->bfType != (((byte)'M' << 8) | (byte)'B') || f->bfOffBits >= mem.Length || f->bfOffBits < minHS)
					return false;

				Api.BITMAPINFOHEADER* h = (Api.BITMAPINFOHEADER*)(f + 1);
				int siz = h->biSize;
				if (siz >= sizeof(Api.BITMAPINFOHEADER) && siz <= sizeof(BITMAPV5HEADER) * 2) {
					x.width = h->biWidth;
					x.height = h->biHeight;
					x.bitCount = h->biBitCount;
					x.isCompressed = h->biCompression != 0;
				} else if (siz == sizeof(BITMAPCOREHEADER)) {
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
		/// Image type as detected by <see cref="ImageTypeFromString(bool, out int, string)"/>.
		/// </summary>
		public enum ImageType
		{
			/// <summary>The string isn't image.</summary>
			None,

			/// <summary>XAML image.</summary>
			Xaml,

			/// <summary>Compressed and Base64-encoded .bmp file data with "~:" prefix. See <see cref="ImageToString(string)"/>.</summary>
			Base64CompressedBmp,

			/// <summary>Base64-encoded .png/gif/jpg file data with "image:" prefix.</summary>
			Base64PngGifJpg,

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
			ShellIcon,

			//rejected. Where it used, we cannot know the assembly; maybe it is script's assembly and in editor we cannot get its resources or it is difficult (need to use meta).
			///// <summary>"resource:name". An image resource name from managed resources of the entry assembly. In Visual Studio use build action "Resource".</summary>
			//Resource,
		}

		/// <summary>
		/// Gets image type from string.
		/// </summary>
		/// <param name="anyFile">When the string is valid but not of any image type, return ShellIcon instead of None.</param>
		/// <param name="prefixLength">Length of prefix "imagefile:" or "image:" or "~:".</param>
		/// <param name="s">File path etc. See <see cref="ImageType"/>.</param>
		/// <param name="length">If -1, calls CharPtr_.Length(s).</param>
		internal static ImageType ImageTypeFromString(bool anyFile, out int prefixLength, byte* s, int length = -1) {
			prefixLength = 0;
			if (length < 0) length = BytePtr_.Length(s);
			if (length < (anyFile ? 2 : 8)) return default; //C:\x.bmp or .h
			char c1 = (char)s[0], c2 = (char)s[1];

			//special strings
			switch (c1) {
			case '~':
				if (c2 != ':') return default;
				prefixLength = 2;
				return ImageType.Base64CompressedBmp;
			case 'i':
				if (BytePtr_.AsciiStarts(s, "image:")) {
					if (length < 10) return default;
					prefixLength = 6;
					return ImageType.Base64PngGifJpg;
				}
				if (BytePtr_.AsciiStarts(s, "imagefile:")) {
					s += 10; length -= 10;
					if (length < 8) return default;
					prefixLength = 10;
					c1 = (char)s[0]; c2 = (char)s[1];
				}
				break;
			//case 'r': if(BytePtr_.AsciiStarts(s, "resource:")) return ImageType.Resource; break;
			case '<':
				if (BytePtr_.AsciiFindString(s, length, "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'") > 0) {
					if (BytePtr_.AsciiFindString(s, length, "<Path ") >= 0) return ImageType.Xaml;
					if (BytePtr_.AsciiFindString(s, length, "<GeometryDrawing ") >= 0) return ImageType.Xaml;
				}
				return default;
			}

			//file path
			if (length >= 8 && (c1 == '%' || (c2 == ':' && c1.IsAsciiAlpha()) || (c1 == '\\' && c2 == '\\'))) { //is image file path?
				byte* ext = s + length - 3;
				if (ext[-1] == '.') {
					if (BytePtr_.AsciiStartsi(ext, "bmp")) return ImageType.Bmp;
					if (BytePtr_.AsciiStartsi(ext, "png")) return ImageType.PngGifJpg;
					if (BytePtr_.AsciiStartsi(ext, "gif")) return ImageType.PngGifJpg;
					if (BytePtr_.AsciiStartsi(ext, "jpg")) return ImageType.PngGifJpg;
					if (BytePtr_.AsciiStartsi(ext, "ico")) return ImageType.Ico;
					if (BytePtr_.AsciiStartsi(ext, "cur")) return ImageType.Cur;
					if (BytePtr_.AsciiStartsi(ext, "ani")) return ImageType.Cur;
				} else if (((char)ext[2]).IsAsciiDigit()) { //can be like C:\x.dll,10
					byte* k = ext + 1, k2 = s + 8;
					for (; k > k2; k--) if (!((char)*k).IsAsciiDigit()) break;
					if (*k == '-') k--;
					if (*k == ',' && k[-4] == '.' && ((char)k[-1]).IsAsciiAlpha()) return ImageType.IconLib;
				}
			}

			if (anyFile) return ImageType.ShellIcon; //can be other file type, URL, .ext, :: ITEMIDLIST, ::{CLSID}
			return default;
		}

		/// <summary>
		/// Gets image type from string.
		/// </summary>
		/// <param name="anyFile">When the string is valid but not of any image type, return ShellIcon instead of None.</param>
		/// <param name="prefixLength">Length of prefix "imagefile:" or "image:" or "~:".</param>
		/// <param name="s">File path etc. See <see cref="ImageType"/>.</param>
		public static ImageType ImageTypeFromString(bool anyFile, out int prefixLength, string s) {
			var b = Convert2.ToUtf8(s);
			fixed (byte* p = b) return ImageTypeFromString(true, out prefixLength, p, b.Length - 1);
		}

		/// <summary>
		/// Loads image and returns its data in .bmp file format.
		/// Returns null if fails, for example file not found or invalid Base64 string.
		/// </summary>
		/// <param name="s">Depends on t. File path or resource name without prefix or Base64 image data without prefix.</param>
		/// <param name="t">Image type and string format.</param>
		/// <param name="searchPath">Use <see cref="filesystem.searchPath"/></param>
		/// <param name="xaml">If not null, supports XAML images. See <see cref="ImageUtil.LoadGdipBitmapFromXaml"/>.</param>
		/// <remarks>Supports environment variables etc. If not full path, searches in <see cref="folders.ThisAppImages"/>.</remarks>
		public static byte[] BmpFileDataFromString(string s, ImageType t, bool searchPath = false, (int dpi, SIZE? size)? xaml = null) {
			//print.it(t, s);
			try {
				switch (t) {
				case ImageType.Bmp:
				case ImageType.PngGifJpg:
				case ImageType.Cur:
					if (searchPath) {
						s = filesystem.searchPath(s, folders.ThisAppImages);
						if (s == null) return null;
					} else {
						if (!pathname.isFullPathExpand(ref s)) return null;
						s = pathname.normalize(s, folders.ThisAppImages);
						if (!filesystem.exists(s).isFile) return null;
					}
					break;
				}

				switch (t) {
				case ImageType.Base64CompressedBmp:
					return Convert2.Decompress(Convert.FromBase64String(s));
				case ImageType.Base64PngGifJpg:
					using (var stream = new MemoryStream(Convert.FromBase64String(s), false)) {
						return _ImageToBytes(Image.FromStream(stream));
					}
				//case ImageType.Resource:
				//	return _ImageToBytes(ResourceUtil.GetGdipBitmap(s));
				case ImageType.Bmp:
					return File.ReadAllBytes(s);
				case ImageType.PngGifJpg:
					return _ImageToBytes(Image.FromFile(s));
				case ImageType.Ico:
				case ImageType.IconLib:
				case ImageType.ShellIcon:
				case ImageType.Cur:
					return _IconToBytes(s, t == ImageType.Cur, searchPath);
				case ImageType.Xaml when xaml != null:
					var xv = xaml.Value;
					return _ImageToBytes(ImageUtil.LoadGdipBitmapFromXaml(s, xv.dpi, xv.size));
				}
			}
			catch (Exception ex) { Debug_.Print(ex.Message + "    " + s); }
			return null;
		}

		static byte[] _ImageToBytes(Image im) {
			if (im == null) return null;
			try {
				//workaround for the black alpha problem. Does not make slower.
				var flags = (ImageFlags)im.Flags;
				if (0 != (flags & ImageFlags.HasAlpha)) { //most png have it
					var t = new Bitmap(im.Width, im.Height);
					t.SetResolution(im.HorizontalResolution, im.VerticalResolution);

					using (var g = Graphics.FromImage(t)) {
						g.Clear(Color.White);
						g.DrawImageUnscaled(im, 0, 0);
					}

					var old = im; im = t; old.Dispose();
				}

				var m = new MemoryStream();
				im.Save(m, ImageFormat.Bmp);
				return m.ToArray();
			}
			finally { im.Dispose(); }
		}

		static byte[] _IconToBytes(string s, bool isCursor, bool searchPath) {
			//info: would be much easier with Icon.ToBitmap(), but it creates bitmap that is displayed incorrectly when we draw it in Scintilla.
			//	Mask or alpha areas then are black.
			//	Another way - draw it like in the png case. Not tested. Maybe would not work with cursors. Probably slower.

			IntPtr hi; int siz;
			if (isCursor) {
				hi = Api.LoadImage(default, s, Api.IMAGE_CURSOR, 0, 0, Api.LR_LOADFROMFILE | Api.LR_DEFAULTSIZE);
				siz = Api.GetSystemMetrics(Api.SM_CXCURSOR);
				//note: if LR_DEFAULTSIZE, uses SM_CXCURSOR, normally 32. It may be not what Explorer displays eg in Cursors folder. But without it gets the first cursor, which often is large, eg 128.
			} else {
				hi = icon.of(s, 16, searchPath ? 0 : IconGetFlags.DontSearch)?.Detach() ?? default;
				siz = 16;
			}
			if (hi == default) return null;
			try {
				using var m = new MemoryBitmap(siz, siz);
				RECT r = new RECT(0, 0, siz, siz);
				Api.FillRect(m.Hdc, r, Api.GetStockObject(0)); //WHITE_BRUSH
				if (!Api.DrawIconEx(m.Hdc, 0, 0, hi, siz, siz)) return null;

				int headersSize = sizeof(BITMAPFILEHEADER) + sizeof(Api.BITMAPINFOHEADER);
				var a = new byte[headersSize + siz * siz * 3];
				fixed (byte* p = a) {
					BITMAPFILEHEADER* f = (BITMAPFILEHEADER*)p;
					Api.BITMAPINFOHEADER* h = (Api.BITMAPINFOHEADER*)(f + 1);
					for (int i = 0; i < headersSize; i++) p[i] = 0;
					byte* bits = p + headersSize;
					h->biSize = sizeof(Api.BITMAPINFOHEADER); h->biBitCount = 24; h->biWidth = siz; h->biHeight = siz; h->biPlanes = 1;
					if (0 == Api.GetDIBits(m.Hdc, m.Hbitmap, 0, siz, bits, h, 0)) return null; //DIB_RGB_COLORS
					f->bfType = ((byte)'M' << 8) | (byte)'B'; f->bfOffBits = headersSize; f->bfSize = a.Length;
				}
				return a;
			}
			finally {
				Api.DestroyIcon(hi);
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
			print.it(b.bmWidth, b.bmHeight, b.bmBits);
			return b.bmWidth;
		}
		finally {
			Api.DeleteObject(ii.hbmColor);
			Api.DeleteObject(ii.hbmMask);
		}
	}
#endif

		/// <summary>
		/// Compresses .bmp file data (<see cref="Convert2.Compress"/>) and Base64-encodes.
		/// Returns string with "~:" prefix.
		/// </summary>
		public static string BmpFileDataToString(byte[] bmpFileData) {
			if (bmpFileData == null) return null;
			return "~:" + Convert.ToBase64String(Convert2.Compress(bmpFileData));
		}

		/// <summary>
		/// Converts image file data to string that can be used in source code instead of file path. It is supported by some functions of this library, for example <see cref="uiimage.find"/>.
		/// Returns string with prefix "image:" (Base-64 encoded .png/gif/jpg file data) or "~:" (Base-64 encoded compressed .bmp file data).
		/// Supports all <see cref="ImageType"/> formats. For non-image files gets icon. Converts icons to bitmap.
		/// Returns null if path is not a valid image string or the file does not exist or failed to load.
		/// </summary>
		/// <remarks>Supports environment variables etc. If not full path, searches in <see cref="folders.ThisAppImages"/> and standard directories.</remarks>
		public static string ImageToString(string path) {
			var t = ImageTypeFromString(true, out _, path);
			switch (t) {
			case ImageType.None: return null;
			case ImageType.Base64CompressedBmp:
			case ImageType.Base64PngGifJpg:
			//case ImageType.Resource:
			//	return path;
			case ImageType.PngGifJpg:
				path = filesystem.searchPath(path, folders.ThisAppImages); if (path == null) return null;
				try { return "image:" + Convert.ToBase64String(filesystem.loadBytes(path)); }
				catch (Exception ex) { Debug_.Print(ex.Message); return null; }
			}
			return BmpFileDataToString(BmpFileDataFromString(path, t, true));
		}

		//currently not used.
		//	When tried to display the WPF image in Image control, was blurry when high DPI, although size is correct (because bitmap's DPI is correct).
		//	UseLayoutRounding did not help. It seems WPF scales/unscales the image don't know how many times.
		///// <summary>
		///// Converts GDI+ bitmap to WPF bitmap image.
		///// </summary>
		//public static System.Windows.Media.Imaging.BitmapSource WinformsBitmapToWpf(Bitmap bmp, bool dispose, int? dpi = null) {
		//	var size = bmp.Size;
		//	var bd = bmp.LockBits(new Rectangle(default, size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//	try {
		//		return System.Windows.Media.Imaging.BitmapSource.Create(
		//			size.Width, size.Height,
		//			dpi ?? bmp.HorizontalResolution.ToInt(),
		//			dpi ?? bmp.VerticalResolution.ToInt(),
		//			pixelFormat: System.Windows.Media.PixelFormats.Bgra32,
		//			null, bd.Scan0, bd.Height * bd.Stride, bd.Stride);
		//	}
		//	finally {
		//		bmp.UnlockBits(bd);
		//		if (dispose) bmp.Dispose();
		//	}
		//}

		///// <summary>
		///// If <i>dpi</i> &gt; 96 (100%) and image resolution is different, returns scaled image.Size. Else returns image.Size.
		///// </summary>
		//public static SIZE ImageSize(Image image, int dpi) {
		//	if (image == null) return default;
		//	SIZE z = image.Size;
		//	if (dpi > 96) {
		//		z.width = Math2.MulDiv(z.width, dpi, image.HorizontalResolution.ToInt());
		//		z.height = Math2.MulDiv(z.height, dpi, image.VerticalResolution.ToInt());
		//	}
		//	return z;
		//}

		///// <summary>
		///// If <i>dpi</i> &gt; 96 (100%) and image resolution is different, returns scaled copy of <i>image</i>. Else returns <i>image</i>.
		///// </summary>
		///// <param name="image"></param>
		///// <param name="dpi"></param>
		///// <param name="disposeOld">If performed scaling (it means created new image), dispose old image.</param>
		///// <remarks>
		///// Unlike <see cref="System.Windows.Forms.Control.ScaleBitmapLogicalToDevice"/>, returns same object if don't need scaling.
		///// </remarks>
		//public static Image ScaleImage(Image image, int dpi, bool disposeOld) {
		//	if (image != null && dpi > 96) {
		//		int xRes = image.HorizontalResolution.ToInt(), yRes = image.VerticalResolution.ToInt();
		//		//print.it(xRes, yRes, dpi);
		//		if (xRes != dpi || yRes != dpi) {
		//			var z = image.Size;
		//			var r = _ScaleBitmap(image, Math2.MulDiv(z.Width, dpi, xRes), Math2.MulDiv(z.Height, dpi, yRes), z);
		//			if (disposeOld) image.Dispose();
		//			image = r;
		//		}
		//	}
		//	return image;
		//}

		////From .NET DpiHelper.ScaleBitmapToSize, which is used by Control.ScaleBitmapLogicalToDevice.
		//private static Bitmap _ScaleBitmap(Image oldImage, int width, int height, Size oldSize) {
		//	//note: could simply return new Bitmap(oldImage, width, height). It uses similar code, but lower quality.

		//	var r = new Bitmap(width, height, oldImage.PixelFormat);

		//	using var graphics = Graphics.FromImage(r);
		//	var mode = InterpolationMode.HighQualityBicubic;
		//	//if(width % oldSize.Width == 0 && height % oldSize.Height == 0) mode = InterpolationMode.NearestNeighbor; //DpiHelper does it, but maybe it isn't a good idea
		//	graphics.InterpolationMode = mode;
		//	graphics.CompositingQuality = CompositingQuality.HighQuality;

		//	var sourceRect = new RectangleF(-0.5f, -0.5f, oldSize.Width, oldSize.Height);
		//	var destRect = new RectangleF(0, 0, width, height);

		//	graphics.DrawImage(oldImage, destRect, sourceRect, GraphicsUnit.Pixel);

		//	return r;
		//}
	}
}
