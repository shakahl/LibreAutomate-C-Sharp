//#define SI_DEBUG_PERF
//#define SI_SIMPLE
//#define SI_TEST_NO_OPTIMIZATION
//#define SI_CAPTURE_BOTTOM_UP

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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;
using System.Drawing.Imaging;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Captures and finds images on screen.
	/// </summary>
	public static class ScreenImage
	{
		#region capture

		/// <summary>
		/// Copies a rectangle of screen pixels to a new Bitmap object.
		/// </summary>
		/// <param name="r">A rectangle in screen coordinates.</param>
		/// <exception cref="CatException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		/// <exception cref="Exception">Exceptions of Image.FromHbitmap.</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var file = Folders.Temp + "notepad.png";
		/// Wnd w = Wnd.Find("* Notepad");
		/// w.Activate();
		/// using(var b = ScreenImage.Capture(w.Rect)) { b.Save(file); }
		/// Shell.Run(file);
		/// ]]></code>
		/// </example>
		public static Bitmap Capture(RECT r)
		{
			return _Capture(r);
		}

		/// <summary>
		/// Copies a rectangle of window client area pixels to a new Bitmap object.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="r">A rectangle in w client area coordinates. Use <c>w.ClientRect</c> to get whole client area.</param>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="CatException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		/// <exception cref="Exception">Exceptions of Image.FromHbitmap.</exception>
		/// <remarks>
		/// How this is different from <see cref="Capture(RECT)"/>:
		/// 1. Gets pixels from window's device context (DC), not from screen DC, unless the Aero theme is turned off (on Windows 7). The window can be under other windows. 
		/// 2. If the window is partially or completely transparent, gets non-transparent image.
		/// 3. Does not work with Windows Store app windows (creates black image) and possibly with some other windows.
		/// 4. If the window is DPI-scaled, captures its non-scaled view. And r must contain non-scaled coordinates.
		/// </remarks>
		public static Bitmap Capture(Wnd w, RECT r)
		{
			w.ThrowIfInvalid();
			return _Capture(r, w);
		}

		static unsafe Bitmap _Capture(RECT r, Wnd w = default(Wnd))
		{
			//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.

			using(var mb = new Util.MemoryBitmap(r.Width, r.Height)) {
				//IntPtr dc = includeNonClient ? Api.GetWindowDC(w) : Api.GetDC(w); //window DC - nothing good: if in background, captures incorrect caption etc. If need nonclient part, better activate window and capture window rect from screen.
				IntPtr dc = Api.GetDC(w);
				if(dc == Zero && !w.Is0) w.ThrowNoNative("Failed");
				bool ok = Api.BitBlt(mb.Hdc, 0, 0, r.Width, r.Height, dc, r.left, r.top, 0xCC0020); //SRCCOPY
				Api.ReleaseDC(w, dc);
				Debug.Assert(ok); //fails only if a dc is invalid
				_Debug("captured to MemBmp");
#if SI_CAPTURE_BOTTOM_UP
				//The bitmap is bottom-up, unlike bitmaps created/loaded/captured by GDI+.
				//Such bitmap cannot be passed directly to Find. Or Find would have to convert or adapt to it.
				//When saved/loaded, it is top-down.
				//Slightly faster.

				var R = Image.FromHbitmap(mb.Hbitmap);
				//Normally display colors is 32-bit (True Color). Then PixelFormat is Format32bppRgb.
				//On older OS it's possible to set 16-bit display colors (High Color). Then PixelFormat is Format16bppRgb565.
				Debug.Assert(R.PixelFormat == PixelFormat.Format32bppRgb); //asserts when testing 16-bit display colors
				if(R.PixelFormat != PixelFormat.Format32bppRgb) {
					var t = R;
					try { R = t.Clone(new Rectangle(0, 0, t.Width, t.Height), PixelFormat.Format32bppRgb); }
					finally { t.Dispose(); }
				}
				_Debug("Image.FromHbitmap");
				return R;

				//Convert PixelFormat from 32 to 24 bit, it's more compact, we don't need alpha.
				//No, 32 bit is easier to search, and png size is not much bigger.
				//using(var k = Image.FromHbitmap(b.Hbitmap)) {
				//	return k.Clone(new Rectangle(0, 0, k.Width, k.Height), PixelFormat.Format24bppRgb);
				//}
#else
				var R = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppRgb);
				try {
					var h = new Api.BITMAPINFOHEADER() {
						biSize = sizeof(Api.BITMAPINFOHEADER),
						biWidth = r.Width, biHeight = -r.Height, //use -height for top-down
						biPlanes = 1, biBitCount = 32,
						//biCompression = 0, //BI_RGB
					};
					var d = R.LockBits(new Rectangle(0, 0, r.Width, r.Height), ImageLockMode.ReadWrite, R.PixelFormat); //tested: fast, no copy
					try {
						var apiResult = Api.GetDIBits(mb.Hdc, mb.Hbitmap, 0, r.Height, (void*)d.Scan0, &h, 0); //DIB_RGB_COLORS
						if(apiResult != r.Height) throw new CatException("GetDIBits");

						//remove alpha (why it is here?). Will compress better.
						//Perf.First();
						byte* p = (byte*)d.Scan0, pe = p + r.Width * r.Height * 4;
						for(p += 3; p < pe; p += 4) *p = 0xff;
						//int n = 0; for(p += 3; p < pe; p += 4) if(*p != 0) n++; Print(n);
						//Perf.NW(); //1100 for max window
					}
					finally { R.UnlockBits(d); } //tested: fast, no copy
					return R;
				}
				catch { R.Dispose(); throw; }
#endif
			}
		}

		//Creates Format32bppRgb top-down bitmap. Unlike Image.FromHbitmap, which usually creates bottom-up bitmap which is incompatible with ScreenImage functions.
		//static unsafe Bitmap BitmapFromHbitmap(IntPtr hbitmap)
		//{
		//	var bh = new Api.BITMAPINFOHEADER() { biSize = sizeof(Api.BITMAPINFOHEADER) };
		//	var hdc = Api.GetDC(Wnd0);
		//	try {

		//	}
		//	finally { Api.ReleaseDC(Wnd0, hdc); }
		//	if(Api.GetDIBits())
		//}

		//FUTURE
		//public static bool CaptureUI()
		//{

		//}

		#endregion

		/// <summary>
		/// <see cref="Find"/> flags.
		/// </summary>
		[Flags]
		public enum FindFlags
		{
			/// <summary>
			/// Move the mouse to the image.
			/// Not used with WaitNot, WaitChanged and when wndEtc is Bitmap.
			/// </summary>
			MoveMouse = 1,

			/// <summary>
			/// When found, wait until mouse buttons released.
			/// It is especially useful when waiting and using flag MoveMouse.
			/// Not when wndEtc is Bitmap.
			/// </summary>
			WaitMouseReleased = 2,

			/// <summary>
			/// Throw CatException exception if not found.
			/// Not used when waiting.
			/// </summary>
			ThrowIfNotFound = 4,

			/// <summary>
			/// Get pixels from window's device context (DC), not from screen DC.
			/// Notes:
			/// Usually significantly faster.
			/// Can get pixels from window parts that are covered by other windows or offscreen. But not from hidden and minimized windows.
			/// Does not work on Windows 7 if Aero theme is turned off. Then this flag is ignored.
			/// If the window is DPI-scaled, the specified image must be taken from its non-scaled version.
			/// Cannot find images in some windows (including Windows Store apps), and in some window parts (glass). All pixels taken from these windows/parts are black.
			/// Not used when wndEtc is Bitmap.
			/// </summary>
			WindowDC = 8,

			/// <summary>
			/// Find all instances of the specified image. Store rectangles in <see cref="FindResult.a"/> or <see cref="FindResult.a2"/>; uses a2 if multiple images are specified.
			/// Not used with WaitNot, WaitChanged.
			/// </summary>
			AllInstances = 16,

			/// <summary>
			/// When the image argument specifies multiple images, all they must exist, else result is "not found".
			/// </summary>
			AllMustExist = 32,

			/// <summary>
			/// Get result rectangles in screen coordinates.
			/// Without this flag the rectangles are in window client area coordinates. When searching in accessible object - in object coordinates.
			/// Not used with WaitNot, WaitChanged and when wndEtc is Bitmap.
			/// </summary>
			ResultInScreen = 64,

			//this was used in QM2. Now using png alpha channel instead.
			///// <summary>
			///// Use the top-left pixel color of the image as transparent color (don't compare pixels that have this color).
			///// </summary>
			//MakeTransparent = 64,
		}

		/// <summary>
		/// <see cref="Find"/> results.
		/// </summary>
		public class FindResult
		{
			/// <summary>
			/// Screen coordinates of the found image.
			/// Used in these cases:
			/// 1. The image argument specifies single image and is not used flag AllInstances.
			/// 2. The image argument specifies multiple images and are not used flags AllInstances or AllMustExist.
			/// </summary>
			public RECT r;

			/// <summary>
			/// Screen coordinates of found images.
			/// Used in these cases:
			/// 1. The image argument specifies single image and is used flag AllInstances. Then the array contains all found instances of the image.
			/// 2. The image argument specifies multiple images and is used flag AllMustExist and not AllInstances. Then the array contains the first found instance of each image.
			/// </summary>
			public RECT[] a;

			/// <summary>
			/// Screen coordinates of found images.
			/// Used when the image argument specifies multiple images and is used flag AllInstances.
			/// It is array of arrays. The main array matches the specified images. Each indice - array containing rectangles of all found instances of that image, or null if not found.
			/// </summary>
			public RECT[][] a2;

			/// <summary>
			/// 0 if not found, 1 or greater if found.
			/// When the image argument specifies multiple images, and flag AllMustExist not used, this is the 1-based index of the first found image.
			/// </summary>
			public int index;

			/// <summary>
			/// true if the image was found.
			/// </summary>
			public bool Found { get => index > 0; }

			/// <summary>
			/// true if the image was found.
			/// </summary>
			public static implicit operator bool(FindResult r) => r.Found;

			internal void Clear()
			{
				r.SetEmpty();
				a = null;
				a2 = null;
				index = 0;
			}
		}

		/// <summary>
		/// Finds the specified image(s) or color(s) on the screen.
		/// Returns <see cref="FindResult"/> object containing screen rectangle(s) of found image(s) etc. It also can be used like <c>if(!ScreenImage.Find(...)) Print("not found");</c>.
		/// </summary>
		/// <param name="image">
		/// One or more images or colors to find. Can be:
		/// string - path of .png or .bmp file. If not full path, uses <see cref="Folders.ThisAppImages"/>.
		/// Bitmap - image object in memory.
		/// int - color in 0xRRGGBB format. Alpha is not used.
		/// IEnumerable of string, Bitmap, int or object - multiple images or colors. Default action - find any. If flag AllMustExist - find all.
		/// 
		/// Some pixels in png images can be transparent or partially transparent. These pixels are not compared.
		/// </param>
		/// <param name="wndEtc">
		/// Where to search. Can be:
		/// Wnd - window or control. The function searches only in the client area.
		/// Acc - accessible object. The function searches in the intersection rectangle of the object's reactangle and the client area of the object's direct container control or window.
		/// Bitmap - another image. These flags are not used: MoveMouse, WaitMouseReleased, WindowDC, ResultInScreen.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="r">A rectangle in wndEtc where to search. If null (default), searches in whole wndEtc rectangle.</param>
		/// <param name="colorDiff">Maximal color difference. Allows to find on-screen images that have slightly different colors than the specified image. Can be 0 - 250, but should be as small as possible. Applied to each color component (red, green, blue) of each pixel.</param>
		/// <param name="skip">Number of matching image instances to skip. Use when there are several matching image instances in the search area and you need not the first one.</param>
		/// <exception cref="ArgumentException">An argument is of unsupported type or is/contains a null/empty/invalid string/collection/Bitmap/etc.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Image.FromFile(string)"/>, Bitmap.LockBits.</exception>
		/// <exception cref="WndException">Invalid window (the wndEtc argument).</exception>
		/// <exception cref="CatException">Something failed. A Bitmap object uses bottom-up format.</exception>
		/// <exception cref="NotFoundException">Not found, when used flag ThrowIfNotFound.</exception>
		public static FindResult Find(object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
		{
			using(var f = new _Finder(_Action.Find, image, wndEtc, flags, r, colorDiff, skip)) {
				//f.Find();
				while(f.Find()) { if(_Finally(f, flags)) break; }
				return f.Result;
			}
		}

		enum _Action { Find, Wait, WaitNot, WaitChanged }

		public static FindResult Wait(double timeS, object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
		{
			return _Wait(_Action.Wait, timeS, image, wndEtc, flags, r, colorDiff, skip);
		}

		public static bool WaitNot(double timeS, object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
		{
			return !_Wait(_Action.WaitNot, timeS, image, wndEtc, flags, r, colorDiff, skip);
		}

		public static bool WaitChanged(double timeS, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
		{
			return !_Wait(_Action.WaitChanged, timeS, null, wndEtc, flags, r, colorDiff, skip);
		}

		static FindResult _Wait(_Action action, double timeS, object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
		{
			using(var f = new _Finder(action, image, wndEtc, flags, r, colorDiff, skip)) {
				while(WaitFor.Condition(timeS, v => (v as _Finder).Find_ApplyNot(), f, 50, 1000)) {
					if(_Finally(f, flags)) break;
					//CONSIDER: adjust timeS, if not 0. Would need to subtract elapsed time and add maybe 5 s.
				}
				return f.Result;
			}
		}

		//TODO
		internal static void Test(object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0)
		{
			using(var f = new _Finder(_Action.Find, image, wndEtc, flags, r, colorDiff, 0)) {
				for(int i = 0; i < 1; i++) f.Find();
			}
		}

		static bool _Finally(_Finder f, FindFlags flags)
		{
			if(0 != (flags & FindFlags.WaitMouseReleased)) {
				bool pressed = false;
				while(Input.IsMouseLeft || Input.IsMouseRight || Input.IsMouseMiddle) { pressed = true; Time.Sleep(20); }
				if(pressed) return false; //need to search again because eg the window with the image may be moved or resized
			}

			if(0 != (flags & FindFlags.MoveMouse)) {
				//TODO
			}

			return true;
		}

		unsafe class _Finder :IDisposable
		{
			class _Image
			{
				Bitmap _b;
				internal BitmapData data;
				bool _dispose;
				internal _OptimizationData opt;

				public _Image(string file)
				{
					file = Path_.Normalize(file, Folders.ThisAppImages);
					_b = Image.FromFile(file) as Bitmap;
					_dispose = true;
					_InitBitmap();

					//CONSIDER: support resources and Base64 image data strings.
				}

				public _Image(Bitmap bmp)
				{
					_b = bmp ?? throw new ArgumentException("null Bitmap");
					_InitBitmap();
				}

				void _InitBitmap()
				{
					data = _b.LockBits(new Rectangle(0, 0, _b.Width, _b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					if(data.Stride < 0) {
						_b.UnlockBits(data); data = null;
						throw new CatException("bottom-up Bitmap");
					}

					//speed: Clone is much slower than LockBits, which is often quite fast even if need conversion.
				}

				public _Image(int color)
				{
					var p = (int*)Util.NativeHeap.Alloc(4);
					*p = color |= unchecked((int)0xff000000);
					data = new BitmapData() { Width = 1, Height = 1, Scan0 = (IntPtr)p };
				}

				//when _action is WaitChanged
				public _Image(BitmapData d)
				{
					data = d;
				}

				public void Dispose()
				{
					if(data != null) {
						if(_b == null) Util.NativeHeap.Free((void*)data.Scan0); //when color or _action is WaitChanged
						else _b.UnlockBits(data);
						data = null;
					}
					if(_b != null) {
						if(_dispose) _b.Dispose();
						_b = null;
					}
				}
			}

			//input
			List<_Image> _images; //support multiple images
			Wnd _w;
			Acc _acc;
			Bitmap _bmp;
			RECT? _r;
			uint _colorDiff;
			int _skip;
			FindFlags _flags;
			_Action _action;
			//output
			FindResult _result;
			List<RECT> _tempResults;
			//area data
			Util.MemoryBitmap _areaMB; //reuse while waiting, it makes slightly faster
			int _areaWidth, _areaHeight, _areaMemSize; //_areaMB width and height, use for the same purpose
			uint* _areaPixels; //the same purpose. Allocating/freeing large memory is somehow slow.
			BitmapData _areaData; //of _bmp. Could be local, because we don't wait, but better do this way.

			public void Dispose()
			{
				if(_bmp != null) {
					if(_areaData != null) _bmp.UnlockBits(_areaData);
				} else {
					Util.NativeHeap.Free(_areaPixels);
					_areaMB?.Dispose();
				}
				_DisposeInputImages();
			}

			void _DisposeInputImages()
			{
				if(_images != null) foreach(var v in _images) v.Dispose();
			}

			public FindResult Result { get => _result ?? (_result = new FindResult()); }

			public _Finder(_Action action, object image, Types<Wnd, Acc, Bitmap> wndEtc, FindFlags flags = 0, RECT? r = null, int colorDiff = 0, int skip = 0)
			{
				_action = action;
				_flags = flags;
				_colorDiff = (uint)colorDiff; if(_colorDiff > 250) throw new ArgumentException("colorDiff can be 0 - 250");
				_r = r;
				_skip = Math.Max(skip, 0);

				switch(wndEtc.type) {
				case 1:
					wndEtc.v1.ThrowIfInvalid();
					_w = wndEtc.v1;
					break;
				case 2:
					if(wndEtc.v2.a == null) throw new ArgumentException("null object", nameof(wndEtc));
					CatException.ThrowIfFailed(Api.WindowFromAccessibleObject(wndEtc.v2.a, out _w), "*get object's window");
					_acc = wndEtc.v2;
					break;
				case 3 when action == _Action.Find: //throw if action is 'wait'. There is no sense to wait for some changes in Bitmap.
					_bmp = wndEtc.v3;
					if(_bmp == null) throw new ArgumentException("null object", nameof(wndEtc));
					break;
				default: throw new ArgumentException();
				}

				_images = new List<_Image>();
				if(action != _Action.WaitChanged) {
					try { _AddImage(image); }
					catch { _DisposeInputImages(); throw; } //Dispose() will not be called because we are in ctor
					if(_images.Count == 0) throw new ArgumentException("Empty.", nameof(image));
				} //else the first Find will add the area to _images

				if(action < _Action.WaitNot) _result = new FindResult(); //else don't need results

#if DEBUG
				if(_bmp != null) {
					FindFlags bad = FindFlags.MoveMouse | FindFlags.ResultInScreen | FindFlags.WaitMouseReleased | FindFlags.WindowDC;
					if(0 != (flags & bad)) Output.Warning("These flags are not used when wndEtc is Bitmap: " + bad);
					flags &= ~bad;
				}
				if(action != _Action.Find) {
					FindFlags bad = FindFlags.ThrowIfNotFound;
					if(action >= _Action.WaitNot) {
						bad |= FindFlags.AllInstances | FindFlags.MoveMouse | FindFlags.ResultInScreen;
					}
					if(0 != (flags & bad)) Output.Warning("These flags are not used with this function: " + bad);
					flags &= ~bad;
				}
#endif
			}

			void _AddImage(object image)
			{
				switch(image) {
				case string file:
					_images.Add(new _Image(file));
					break;
				case Bitmap bitmap:
					_images.Add(new _Image(bitmap));
					break;
				case int color:
					_images.Add(new _Image(color));
					break;
				case IEnumerable<object> e:
					foreach(var v in e) _AddImage(v);
					break;
				default: throw new ArgumentException("Bad type.", nameof(image));
				}
			}

			public bool Find()
			{
				Perf.Next(); //TODO
				_result?.Clear();

				bool allMustExist = 0 != (_flags & FindFlags.AllMustExist);
				bool windowDC = 0 != (_flags & FindFlags.WindowDC);

				//Get area rect.
				var r = new RECT();
				var offset = new POINT();
				bool resultInScreen = 0 != (_flags & FindFlags.ResultInScreen);
				if(_bmp != null) {
					r.right = _bmp.Width;
					r.bottom = _bmp.Height;
				} else {
					if(_acc != null) {
						CatException.ThrowIfFailed(_acc.a.accLocation(out var x, out var y, out var cx, out var cy, _acc.elem));
						r.Set(x, y, cx, cy, true);
						if(resultInScreen) { offset.x = r.left; offset.y = r.top; }
						if(windowDC) _w.MapScreenToClient(ref r);
					} else {
						r = _w.ClientRectInScreen;
						if(resultInScreen) { offset.x = r.left; offset.y = r.top; }
						if(windowDC) r.Offset(-r.left, -r.top);
					}
					//TODO: DPI
				}
				if(_r != null) {
					var rr = _r.GetValueOrDefault();
					offset.x += rr.left; offset.y += rr.top;
					rr.Offset(r.left, r.top);
					r.Intersect(rr);
				}
				if(_acc != null) {
					//may need to adjust r and offset,
					//	because object rect may be bigger than client area (eg WINDOW object)
					//	or its part is not in client area (eg scrolled web page).
					//	If not adjusted, then may capture part of parent or sibling controls or even other windows...
					//	Never mind: should also adjust control rect in ancestors in the same way.
					//		This is not so important because usually whole control is visible (resized, not clipped).
					//		More often there are sibling controls on top. Difficult to exclude them.
					int x = r.left, y = r.top;
					r.Intersect(windowDC ? _w.ClientRect : _w.ClientRectInScreen);
					x -= r.left; y -= r.top;
					offset.x -= x; offset.y -= y;
				}
				//Print(r);
				if(r.IsEmpty) return false; //never mind: if WaitChanged and this is the first time, immediately returns 'changed'

				//If WaitChanged, first time just get area pixels into _images[0].
				if(_action == _Action.WaitChanged && _images.Count == 0) {
					_GetAreaPixels(r);
					var data = new BitmapData() { Width = _areaWidth, Height = _areaHeight, Scan0 = (IntPtr)_areaPixels };
					_areaPixels = null; _areaMemSize = 0;
					_images.Add(new _Image(data));
					return true;
				}

				//Return false immediately if all (or one, if AllMustExist) images are bigger than the search area.
				int n = 0;
				foreach(var v in _images) if(v.data.Width <= r.Width && v.data.Height <= r.Height) n++;
				if(n == 0 || (allMustExist && n < _images.Count)) return false;

				//Get area pixels.
				if(_bmp != null) {
					if(_areaData == null) {
						var pf = (_bmp.PixelFormat == PixelFormat.Format32bppArgb) ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb; //if possible, use PixelFormat of _bmp, to avoid conversion/copying. Both these formats are ok, we don't use alpha.
						_areaData = _bmp.LockBits(r, ImageLockMode.ReadOnly, pf);
						if(_areaData.Stride < 0) throw new CatException("bottom-up Bitmap");
					}
					_areaPixels = (uint*)_areaData.Scan0;
					_areaWidth = _areaData.Width; _areaHeight = _areaData.Height;
				} else {
					_GetAreaPixels(r);
				}
				Perf.Next();

				//Find image(s) in area.
				bool found = false;
				bool allInst = 0 != (_flags & FindFlags.AllInstances), multipleImages = _images.Count > 1;
				for(int i = 0; i < _images.Count; i++) {
					if(_FindImage(_images[i])) {
						found = true;
						if(_result == null) continue; //don't need results (WaitNot, WaitChanged)
						if(_result.index == 0) _result.index = i + 1;

						RECT R = _tempResults[0]; RECT[] A = null;
						if(allInst) {
							A = _tempResults.ToArray();
							for(int j = 0; j < A.Length; j++) A[j].Offset(offset.x, offset.y);
						} else {
							R.Offset(offset.x, offset.y);
						}

						if(multipleImages) {
							if(allInst) {
								if(_result.a2 == null) _result.a2 = new RECT[_images.Count][];
								_result.a2[i] = A;
							} else if(allMustExist) {
								if(_result.a == null) _result.a = new RECT[_images.Count];
								_result.a[i] = R;
							} else {
								_result.r = R;
								break;
							}
						} else {
							if(allInst) _result.a = A;
							else _result.r = R;
						}
					} else {
						if(allMustExist) {
							found = false;
							break;
						}
					}
				}
				Perf.Next();
				if(found) return true;
				_result?.Clear();
				return false;
			}

			bool _FindImage(_Image image)
			{
				_tempResults?.Clear();
				bool found = false;

				BitmapData bdata = image.data;
				int imageWidth = bdata.Width, imageHeight = bdata.Height;
				if(_areaWidth < imageWidth || _areaHeight < imageHeight) return false;
				uint* imagePixels = (uint*)bdata.Scan0, imagePixelsTo = imagePixels + imageWidth * imageHeight;
				uint* areaPixels = _areaPixels;

				//rejected. Does not make faster, just adds more code.
				//if image is of same size as area, simply compare. For example, when action is WaitChanged.
				//if(imageWidth == _areaWidth && imageHeight == _areaHeight) {
				//	//Print("same size");
				//	if(_skip > 0) return false;
				//	if(!_CompareSameSize(areaPixels, imagePixels, imagePixelsTo, _colorDiff)) return false;
				//	if(_tempResults == null) _tempResults = new List<RECT>();
				//	_tempResults.Add(new RECT(0, 0, imageWidth, imageHeight, true));
				//	return true;
				//}
				//else if(imagePixelCount == 1) { ... } //eg when image is color

				if(!image.opt.Init(bdata, _areaWidth)) return false;
				var opt = image.opt; //copy struct, size = 9*int
				int o_pos0 = opt.v0.pos;
				var o_a1 = &opt.v1; var o_an = o_a1 + (opt.N - 1);

				int skip = _skip;

				//find first pixel. This part is very important for speed.
				//int nTimesFound = 0;

				var areaWidthMinusImage = _areaWidth - imageWidth;
				var pFirst = areaPixels + o_pos0;
				var pLast = pFirst + _areaWidth * (_areaHeight - imageHeight) + areaWidthMinusImage;

				//this is a workaround for compiler not using registers for variables in fast loops (part 1)
				var f = new _FindData() {
					color = (opt.v0.color & 0xffffff) | (_colorDiff << 24),
					p = pFirst - 1,
					pLineLast = pFirst + areaWidthMinusImage
				};

				#region fast_code

				//Print($"_areaWidth={_areaWidth} imageWidth={imageWidth} o_pos0={o_pos0} lineLast={f.pLineLast- areaPixels}");
				//TODO: finally remove the debug comment lines

				gContinue:
				{
					var f_ = &f; //part 2 of the workaround
					var p_ = f_->p + 1; //register
					var color_ = f_->color; //register
					var pLineLast_ = f_->pLineLast; //register
					for(;;) { //lines
						if(color_ < 0x1000000) {
							for(; p_ <= pLineLast_; p_++) {
								if(color_ == (*p_ & 0xffffff)) goto gPixelFound;
							}
						} else {
							//all variables except f.pLineLast are in registers
							//	It is very sensitive to other code. Compiler can take some registers for other code and not use here.
							//	Then still not significantly slower, but I like to have full speed.
							//	Code above fast_code region should not contain variables that are used in loops below this block.
							//	Also don't use class members in fast_code region, because then compiler may take a register for 'this' pointer.
							//	Here we use f.pLineLast instead of pLineLast_, else d2_ would be in memory (it is used 3 times).
							var d_ = color_ >> 24; //register
							var d2_ = d_ * 2; //register
							for(; p_ <= f.pLineLast; p_++) {
								if((color_ & 0xff) - ((byte*)p_)[0] + d_ > d2_) continue;
								if((color_ >> 8 & 0xff) - ((byte*)p_)[1] + d_ > d2_) continue;
								if((color_ >> 16 & 0xff) - ((byte*)p_)[2] + d_ > d2_) continue;
								goto gPixelFound;
							}
						}
						if(p_ > pLast) goto gNotFound;
						p_--; p_ += imageWidth;
						f.pLineLast = pLineLast_ = p_ + areaWidthMinusImage;
						//Print($"from={(p_ - areaPixels) % _areaWidth} to={(pLineLast_ - areaPixels) % _areaWidth}");
					}
					gPixelFound:
					f.p = p_;
				}

				//nTimesFound++;
				var ap = f.p - o_pos0; //first area pixel of the top-left of the image

				//int deb_p = (int)(ap - areaPixels); Print($"x={deb_p % _areaWidth} y={deb_p / _areaWidth}");

				//compare other 0-3 selected pixels
				for(var op = o_a1; op < o_an; op++) {
					uint aPix = ap[op->pos], iPix = op->color;
					var colorDiff = f.color >> 24;
					if(colorDiff == 0) {
						if(!_MatchPixelExact(aPix, iPix)) goto gContinue;
					} else {
						if(!_MatchPixelDiff(aPix, iPix, colorDiff)) goto gContinue;
					}
				}

				//now compare all pixels of the image
				//Perf.First();
				uint* ip = imagePixels, ipLineTo = ip + imageWidth;
				for(;;) { //lines
					if(f.color < 0x1000000) {
						do {
							if(!_MatchPixelExact(*ap, *ip)) goto gContinue;
							ap++;
						}
						while(++ip < ipLineTo);
					} else {
						var colorDiff = f.color >> 24;
						do {
							if(!_MatchPixelDiff(*ap, *ip, colorDiff)) goto gContinue;
							ap++;
						}
						while(++ip < ipLineTo);
					}
					if(ip == imagePixelsTo) break;
					ap += areaWidthMinusImage;
					ipLineTo += imageWidth;
				}
				//Perf.NW();

				#endregion

				if(--skip >= 0) goto gContinue;
				//Print(nTimesFound);

				found = true;
				if(_result != null) { //else don't need results (WaitNot, WaitChanged)
					if(_tempResults == null) _tempResults = new List<RECT>(); //else cleared at the beginning of this func
					int iFound = (int)(f.p - o_pos0 - areaPixels);
					_tempResults.Add(new RECT(iFound % _areaWidth, iFound / _areaWidth, imageWidth, imageHeight, true));
				}

				if(0 != (_flags & FindFlags.AllInstances)) {
					skip = 1;
					goto gContinue;
				}

				return true;
				gNotFound:
				return found;
			}

			struct _FindData
			{
				public uint color;
				public uint* p, pLineLast;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool _MatchPixelExact(uint ap, uint ip)
			{
				if(ip == (ap | 0xff000000)) return true;
				return ip < 0xff000000; //transparent?
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			static bool _MatchPixelDiff(uint ap, uint ip, uint colorDiff)
			{
				//info: optimized. Don't modify.
				//	All variables are in registers.
				//	Only 3.5 times slower than _MatchPixelExact (when all pixels match), which is inline.

				if(ip >= 0xff000000) { //else transparent
					uint d = colorDiff, d2 = d * 2;
					if(((ip & 0xff) - (ap & 0xff) + d) > d2) goto gFalse;
					if(((ip >> 8 & 0xff) - (ap >> 8 & 0xff) + d) > d2) goto gFalse;
					if(((ip >> 16 & 0xff) - (ap >> 16 & 0xff) + d) > d2) goto gFalse;
				}
				return true;
				gFalse:
				return false;
			}

			//bool _CompareSameSize(uint* area, uint* image, uint* imageTo, uint colorDiff)
			//{
			//	if(colorDiff == 0) {
			//		do {
			//			if(!_MatchPixelExact(*area, *image)) break;
			//			area++;
			//		} while(++image < imageTo);
			//	} else {
			//		do {
			//			if(!_MatchPixelDiff(*area, *image, colorDiff)) break;
			//			area++;
			//		} while(++image < imageTo);
			//	}
			//	return image == imageTo;
			//}

			static bool _IsTransparent(uint color)
			{
				return color < 0xff000000;
			}

			struct _OptimizationData
			{
				public struct POSCOLOR
				{
					public int pos; //the position in area (not in image) from which to start searching. Depends on where in the image is the color.
					public uint color;
				};

#pragma warning disable 649 //never assigned
				public POSCOLOR v0, v1, v2, v3; //POSCOLOR[] would be slower
#pragma warning restore 649
				public int N; //A valid count

				public bool Init(BitmapData bdata, int areaWidth)
				{
					if(N != 0) return N > 0;

					int imageWidth = bdata.Width, imageHeight = bdata.Height;
					int imagePixelCount = imageWidth * imageHeight;
					uint* imagePixels = (uint*)bdata.Scan0;
					int i;

#if SI_TEST_NO_OPTIMIZATION
					_Add(bdata, 0, areaWidth);
#else

					//Find several unique-color pixels for first-pixel search.
					//This greatly reduces the search time in most cases.

					//find first nontransparent pixel
					for(i = 0; i < imagePixelCount; i++) if(!_IsTransparent(imagePixels[i])) break;
					if(i == imagePixelCount) { N = -1; return false; } //not found because all pixels in image are transparent

					//SHOULDDO:
					//1. Use colorDiff.
					//CONSIDER:
					//1. Start from center.
					//2. Prefer high saturation pixels.
					//3. If large area, find its its dominant color(s) and don't use them. For speed, compare eg every 11-th.
					//4. Create a better algorithm. Maybe just shorter. This code is converted from QM2.

					//find first nonbackground pixel (consider top-left pixel is background)
					bool singleColor = false;
					if(i == 0) {
						i = _FindDifferentPixel(0);
						if(i < 0) { singleColor = true; i = 0; }
					}

					_Add(bdata, i, areaWidth);
					if(!singleColor) {
						//find second different pixel
						int i0 = i;
						i = _FindDifferentPixel(i);
						if(i >= 0) {
							_Add(bdata, i, areaWidth);
							//find other different pixels
							fixed (POSCOLOR* p = &v0) {
								while(N < 4) {
									for(++i; i < imagePixelCount; i++) {
										var c = imagePixels[i];
										if(_IsTransparent(c)) continue;
										int j = N - 1;
										for(; j >= 0; j--) if(c == p[j].color) break; //find new color
										if(j < 0) break; //found
									}
									if(i >= imagePixelCount) break;
									_Add(bdata, i, areaWidth);
								}
							}
						} else {
							for(i = imagePixelCount - 1; i > i0; i--) if(!_IsTransparent(imagePixels[i])) break;
							_Add(bdata, i, areaWidth);
						}
					}

					//fixed (POSCOLOR* o_pc = &v0) for(int j = 0; j < N; j++) Print($"{o_pc[j].pos} 0x{o_pc[j].color:X}");
#endif
					return true;

					int _FindDifferentPixel(int iCurrent)
					{
						int m = iCurrent, n = imagePixelCount;
						var p = imagePixels;
						uint notColor = p[m++];
						for(; m < n; m++) {
							var c = p[m];
							if(c == notColor || _IsTransparent(c)) continue;
							return m;
						}
						return -1;
					}
				}

				void _Add(BitmapData bdata, int i, int areaWidth)
				{
					fixed (POSCOLOR* p0 = &v0) {
						var p = p0 + N++;
						p->color = ((uint*)bdata.Scan0)[i];
						int w = bdata.Width, x = i % w, y = i / w;
						p->pos = y * areaWidth + x;
					}
				}
			}

			void _GetAreaPixels(RECT r)
			{
				//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.
				//This is the slowest part of Find, especially BitBlt.
				//Speed depends on computer, driver, OS version, theme, size.
				//For example, with Aero theme 2-15 times slower (on Windows 8/10 cannot disable Aero).
				//With incorrect/generic video driver can be 10 times slower. Eg on vmware virtual PC.
				//Much faster when using window DC. Then same speed as without Aero.

				int areaWidth = r.Width, areaHeight = r.Height;
				_Debug("start", 1);
				//create memory bitmap. When waiting, we reuse _areaMB, it makes slightly faster.
				if(_areaMB == null || areaWidth != _areaWidth || areaHeight != _areaHeight) {
					if(_areaMB != null) { _areaMB.Dispose(); _areaMB = null; }
					_areaMB = new Util.MemoryBitmap(_areaWidth = areaWidth, _areaHeight = areaHeight);
					_Debug("created MemBmp");
				}
				//get DC of screen or window
				bool windowDC = 0 != (_flags & FindFlags.WindowDC);
				Wnd w = windowDC ? _w : Wnd0;
				IntPtr dc = Api.GetDC(w); //quite fast, when compared with other parts
				if(dc == Zero && windowDC) _w.ThrowNoNative("Failed");
				_Debug("get DC");
				//copy from screen/window DC to memory bitmap
				bool bbOK = Api.BitBlt(_areaMB.Hdc, 0, 0, areaWidth, areaHeight, dc, r.left, r.top, 0xCC0020); //SRCCOPY
				Api.ReleaseDC(w, dc);
				if(!bbOK) throw new CatException("BitBlt"); //fails only if a hdc is invalid
				_Debug("captured to MemBmp");
				//get pixels
				int memSize = areaWidth * areaHeight * 4; //7.5 MB for a max window in 1920*1080 monitor
				if(memSize > _areaMemSize) { //while waiting, we reuse the memory, it makes slightly faster.
					_areaPixels = (uint*)Util.NativeHeap.ReAlloc(_areaPixels, memSize);
					_areaMemSize = memSize;
				}
				var h = new Api.BITMAPINFOHEADER() {
					biSize = sizeof(Api.BITMAPINFOHEADER),
					biWidth = areaWidth, biHeight = -areaHeight,
					biPlanes = 1, biBitCount = 32,
					//biCompression = 0, //BI_RGB
				};
				if(Api.GetDIBits(_areaMB.Hdc, _areaMB.Hbitmap, 0, areaHeight, _areaPixels, &h, 0) //DIB_RGB_COLORS
					!= areaHeight) throw new CatException("GetDIBits");
				_Debug("_GetBitmapBits", 3);

				//remove alpha (why it is here?). Currently don't need.
				////Perf.First();
				//byte* p = (byte*)_areaPixels, pe = p + memSize;
				//for(p += 3; p < pe; p += 4) *p = 0xff;
				////Perf.NW(); //1100 for max window

				//see what we have
				//var testFile = Folders.Temp + "ScreenImage.png";
				//using(var areaBmp = new Bitmap(areaWidth, areaHeight, areaWidth * 4, PixelFormat.Format32bppRgb, (IntPtr)_areaPixels)) {
				//	areaBmp.Save(testFile);
				//}
				//Shell.Run(testFile);
			}

			public bool Find_ApplyNot()
			{
				return Find() ^ (_action > _Action.Wait);
			}
		}

		[Conditional("SI_DEBUG_PERF")]
		static void _Debug(string s, int perfAction = 2)
		{
			//MessageBox.Show(s);
			switch(perfAction) {
			case 1: Perf.First(); break;
			case 2: Perf.Next(); break;
			case 3: Perf.NW(); break;
			}
		}
	}
}
