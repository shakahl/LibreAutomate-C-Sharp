//#define SI_DEBUG_PERF
//#define SI_SIMPLE
//#define SI_TEST_NO_OPTIMIZATION

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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Captures and finds images on screen.
	/// </summary>
	public static class ScreenImage
	{
		/// <summary>
		/// Copies a rectangle of screen pixels to a new Bitmap object.
		/// </summary>
		/// <param name="rect">A rectangle in screen coordinates.</param>
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
		public static Bitmap Capture(RECT rect)
		{
			return _Capture(rect);
		}

		/// <summary>
		/// Copies a rectangle of window client area pixels to a new Bitmap object.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="rect">A rectangle in w client area coordinates. Use <c>w.ClientRect</c> to get whole client area.</param>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="CatException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		/// <exception cref="Exception">Exceptions of Image.FromHbitmap.</exception>
		/// <remarks>
		/// How this is different from <see cref="Capture(RECT)"/>:
		/// 1. Gets pixels from window's device context (DC), not from screen DC, unless the Aero theme is turned off (on Windows 7). The window can be under other windows. 
		/// 2. If the window is partially or completely transparent, gets non-transparent image.
		/// 3. Does not work with Windows Store app windows (creates black image) and possibly with some other windows.
		/// 4. If the window is DPI-scaled, captures its non-scaled view. And rect must contain non-scaled coordinates.
		/// </remarks>
		public static Bitmap Capture(Wnd w, RECT rect)
		{
			w.ThrowIfInvalid();
			return _Capture(rect, w);
		}

		static unsafe Bitmap _Capture(RECT r, Wnd w = default)
		{
			//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.

			using(var mb = new Util.MemoryBitmap(r.Width, r.Height)) {
				//IntPtr dc = includeNonClient ? Api.GetWindowDC(w) : Api.GetDC(w); //window DC - nothing good: if in background, captures incorrect caption etc. If need nonclient part, better activate window and capture window rectangle from screen.
				IntPtr dc = Api.GetDC(w);
				if(dc == Zero && !w.Is0) w.ThrowNoNative("Failed");
				bool ok = Api.BitBlt(mb.Hdc, 0, 0, r.Width, r.Height, dc, r.left, r.top, 0xCC0020); //SRCCOPY
				Api.ReleaseDC(w, dc);
				Debug.Assert(ok); //fails only if a dc is invalid
								  //_Debug("captured to MemBmp");
				var R = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppRgb);
				try {
					var bh = new Api.BITMAPINFOHEADER() {
						biSize = sizeof(Api.BITMAPINFOHEADER),
						biWidth = r.Width, biHeight = -r.Height, //use -height for top-down
						biPlanes = 1, biBitCount = 32,
						//biCompression = 0, //BI_RGB
					};
					var d = R.LockBits(new Rectangle(0, 0, r.Width, r.Height), ImageLockMode.ReadWrite, R.PixelFormat); //tested: fast, no copy
					try {
						var apiResult = Api.GetDIBits(mb.Hdc, mb.Hbitmap, 0, r.Height, (void*)d.Scan0, &bh, 0); //DIB_RGB_COLORS
						if(apiResult != r.Height) throw new CatException("GetDIBits");

						//remove alpha (why it is here?). Will compress better.
						//Perf.First();
						byte* p = (byte*)d.Scan0, pe = p + r.Width * r.Height * 4;
						for(p += 3; p < pe; p += 4) *p = 0xff;
						//Perf.NW(); //1100 for max window
					}
					finally { R.UnlockBits(d); } //tested: fast, no copy
					return R;
				}
				catch { R.Dispose(); throw; }
			}
		}

		/// <summary>
		/// Creates Bitmap from a GDI bitmap.
		/// </summary>
		/// <param name="hbitmap">GDI bitmap handle. This function makes its copy.</param>
		/// <remarks>
		/// How this function is different from Image.FromHbitmap:
		/// 1. Image.FromHbitmap usually creates bottom-up bitmap, which is incompatible with ScreenImage.Find and similar functions. This function creates normal top-down bitmap, like <c>new Bitmap(...)</c>, <c>Bitmap.FromFile(...)</c> etc do.
		/// 2. This function always creates bitmap of Format32bppRgb PixelFormat.
		/// </remarks>
		/// <exception cref="CatException">Failed. For example hbitmap is Zero.</exception>
		/// <exception cref="Exception">Exceptions of Bitmap(int, int, PixelFormat) constructor.</exception>
		public static unsafe Bitmap BitmapFromHbitmap(IntPtr hbitmap)
		{
			var bh = new Api.BITMAPINFOHEADER() { biSize = sizeof(Api.BITMAPINFOHEADER) };
			var hdc = Api.GetDC(default);
			try {
				if(0 == Api.GetDIBits(hdc, hbitmap, 0, 0, null, &bh, 0)) goto ge;
				int wid = bh.biWidth, hei = bh.biHeight;
				if(hei > 0) bh.biHeight = -bh.biHeight; else hei = -hei;
				bh.biBitCount = 32;

				var R = new Bitmap(wid, hei, PixelFormat.Format32bppRgb);
				var d = R.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadWrite, R.PixelFormat);
				bool ok = hei == Api.GetDIBits(hdc, hbitmap, 0, hei, (void*)d.Scan0, &bh, 0);
				R.UnlockBits(d);
				if(!ok) { R.Dispose(); goto ge; }
				return R;
			}
			finally { Api.ReleaseDC(default, hdc); }
			ge:
			throw new CatException();
		}

		//FUTURE
		//public static bool CaptureUI()
		//{

		//}

		/// <summary>
		/// Finds the specified image(s) or color(s) on the screen.
		/// Returns <see cref="SIResult"/> object containing the rectangle of the found image.
		/// Returns null if not found. You can use <see cref="ExtensionMethods.OrThrow{T}"/>: <c>var r = ScreenImage.Find(...).OrThrow();</c>
		/// </summary>
		/// <param name="image">
		/// Image or color to find. Can be:
		/// string - path of .png or .bmp file. If not full path, uses <see cref="Folders.ThisAppImages"/>. Also can use resources and embedded images; read in Remarks.
		/// Bitmap - image object in memory.
		/// int - color in 0xRRGGBB format. Alpha is not used.
		/// IEnumerable of string, Bitmap, int or object - multiple images or colors. Default action - find any. If flag AllMustExist - must find all.
		/// </param>
		/// <param name="area">
		/// Where to search. Can be a window/control, accessible object, Bitmap or a rectangle in screen.
		/// <list type="bullet">
		/// <item>Wnd - window or control. The search area is its client area.</item>
		/// <item>Acc - accessible object.</item>
		/// <item>Bitmap - another image. These flags are invalid: WindowDC.</item>
		/// <item>RECT - a rectangle area in screen. These flags are invalid: WindowDC.</item>
		/// <item><see cref="SIArea"/> - can contain Wnd, Acc or Bitmap. Also allows to specify a rectangle in it, which makes the search area smaller and the function faster. Example: <c>ScreenImage.Find("image.png", new SIArea(w, 100, 100, 100, 100));</c>.</item>
		/// </list>
		/// </param>
		/// <param name="flags"></param>
		/// <param name="colorDiff">Maximal allowed color difference. Use to to find images that have slightly different colors than the specified image. Can be 0 - 250, but should be as small as possible. Applied to each color component (red, green, blue) of each pixel.</param>
		/// <param name="also">
		/// A callback function to call for each found image until it returns true.
		/// Can be used to create actions like "skip n matching images" (like <c>also: t => t.MatchIndex == 1</c>), "click all matching images" (<c>also: t => { t.MouseClick(); Wait(0.5); return false; }</c>), "get rectangles of all matching images" (<c>also: t => { rectList.Add(t.Rect); return false; }</c>), "ignore images that are or aren't in some custom areas", etc.
		/// When the callback function returns true, Find() returns result "found". Else Find() tries to find more matching images (towards the right and bottom) and calls the callback function again when found. If the callback function returns false every time, Find() returns result "not found".
		/// </param>
		/// <exception cref="WndException">Invalid window handle (the area argument).</exception>
		/// <exception cref="ArgumentException">An argument is of unsupported type or is/contains a null/invalid value.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Image.FromFile(string)"/>, Bitmap.LockBits. For example when the image file does not exist.</exception>
		/// <exception cref="CatException">Something failed.</exception>
		/// <remarks>
		/// If <paramref name="image"/> is file path, and the file does not exist, looks in resources of apdomain's entry assembly. For example, looks for Project.Properties.Resources.X if file "C:\\X.png" not found. Alternatively you can use code like <c>using(var b = Project.Properties.Resources.X) ScreenImage.Find(b, w);</c>.
		/// 
		/// <paramref name="image"/> can be string containing Base-64 encoded .png image file data with prefix "image:".
		/// 
		/// Some pixels in image can be transparent or partially transparent (AA of 0xAARRGGBB is not 255). These pixels are not compared.
		/// 
		/// The speed mostly depends on:
		/// 1. The size of the search area. Use the smallest possible area (control or accessible object or rectangle in window like <c>new SIArea(w, r)</c>).
		/// 2. Flag WindowDC (usually makes several times faster). With this flag the speed depends on window.
		/// 3. Video driver. Can be eg 10 times slower if incorrect or generic driver is used, for example on a virtual PC. Flag WindowDC should help.
		/// 4. colorDiff. Should be as small as possible.
		/// 
		/// If flag WindowDC is not used, the search area must be visible on the screen. If it is covered by other windows, the function will search in these windows.
		/// 
		/// The function can only find images that exactly match the specified image. With colorDiff it can find images with slightly different colors and brightness. It cannot find images with different shapes.
		/// 
		/// This function is not the best way to find objects when the script is intended for long use or for use on multiple computers or must be very reliable. Because it may fail to find the image after are changed some settings - system theme, application theme, text size (DPI), font smoothing (if the image contains text), etc. Also are possible various unexpected temporary conditions that may distort or hide the image, for example adjacent window shadow, a tooltip or some temporary window. If possible, in such scripts instead use other functions, eg find control or accessible object.
		/// 
		/// Throws ArgumentException if image or area is a bottom-up Bitmap object (see <see cref="BitmapData.Stride"/>). Such bitmaps are unusual in .NET (GDI+), but can be created by Image.FromHbitmap; instead use <see cref="BitmapFromHbitmap"/>.
		/// </remarks>
		public static SIResult Find(object image, SIArea area, SIFlags flags = 0, int colorDiff = 0, Func<SIResult, bool> also = null)
		{
			using(var f = new _Finder(_Action.Find, image, area, flags, colorDiff, also)) {
				if(!f.Find()) return null;
				return f.Result;
			}
		}
		//TODO: colorDiff should be double 0 to 1, not int 0 to 255.
		//TODO: (if not already done): an easier way to find window/control/Acc/etc that contains image.
		//TODO: should have a public Finder, like Wnd and Acc.
		//FUTURE: test OpenCV - an open source library for computer vision.

		internal enum _Action { Find, Wait, WaitNot, WaitChanged }

		/// <summary>
		/// Waits for the specified image(s) or color(s) on the screen.
		/// Returns <see cref="SIResult"/> object containing the rectangle of the found image.
		/// On timeout returns false if secondsTimeout is negative (else exception).
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns null.
		/// </param>
		/// <param name="image"></param>
		/// <param name="area"></param>
		/// <param name="flags"></param>
		/// <param name="colorDiff"></param>
		/// <param name="also"></param>
		/// <exception cref="TimeoutException">Timeout. Thrown only when secondsTimeout is greater than 0.</exception>
		/// <remarks>
		/// Parameters and other info is the same as with <see cref="Find"/>.
		/// </remarks>
		public static SIResult WaitFor(double secondsTimeout, object image, SIArea area, SIFlags flags = 0, int colorDiff = 0, Func<SIResult, bool> also = null)
		{
			var r = _Wait(_Action.Wait, secondsTimeout, image, area, flags, colorDiff, also);
			return r.ok ? r.result : null;

			//tested: does not create garbage while waiting.
		}

		/// <summary>
		/// Waits until the specified image(s) or color(s) is NOT found on the screen.
		/// Returns true. On timeout returns false if secondsTimeout is negative (else exception).
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="image"></param>
		/// <param name="area"></param>
		/// <param name="flags"></param>
		/// <param name="colorDiff"></param>
		/// <param name="also"></param>
		/// <remarks>
		/// Parameters and other info is the same as with <see cref="Find"/>.
		/// </remarks>
		public static bool WaitNot(double secondsTimeout, object image, SIArea area, SIFlags flags = 0, int colorDiff = 0, Func<SIResult, bool> also = null)
		{
			return _Wait(_Action.WaitNot, secondsTimeout, image, area, flags, colorDiff, also).ok;
		}

		/// <summary>
		/// Waits until something visually changes in a screen area.
		/// Returns true. On timeout returns false if secondsTimeout is negative (else exception).
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="area"></param>
		/// <param name="flags"></param>
		/// <param name="colorDiff"></param>
		/// <remarks>
		/// Parameters and other info is the same as with <see cref="WaitNot"/> and <see cref="Find"/>. Instead of <b>image</b> parameter, this function captures the screen area image at the beginning.
		/// </remarks>
		public static bool WaitChanged(double secondsTimeout, SIArea area, SIFlags flags = 0, int colorDiff = 0)
		{
			return _Wait(_Action.WaitChanged, secondsTimeout, null, area, flags, colorDiff, null).ok;
		}

		static (bool ok, SIResult result) _Wait(_Action action, double secondsTimeout, object image, SIArea area, SIFlags flags, int colorDiff, Func<SIResult, bool> also)
		{
			using(var f = new _Finder(action, image, area, flags, colorDiff, also)) {
				var ok = Catkeys.WaitFor.Condition(secondsTimeout, o => (o as _Finder).Find_ApplyNot(), f, 50, 1000);
				return (ok, f.Result);
			}
		}

		//info: this class and some its members ar not private because used by SIResult.
		unsafe internal class _Finder :IDisposable
		{
			class _Image
			{
				Bitmap _b;
				internal BitmapData data;
				bool _dispose;
				internal _OptimizationData opt;

				public _Image(string file)
				{
					object o = null;
					if(file.StartsWith_("image:") || file.StartsWith_("~:")) { //image embedded in script as string
						int n = file.Length * 4 / 3;
						var b = new byte[n];
						bool compressed = file[0] == '~';
						fixed (byte* p = b) n = Convert_.Base64Decode(file, p, n, compressed ? 2 : 6);
						using(var stream = compressed ? new MemoryStream() : new MemoryStream(b, 0, n, false)) {
							if(compressed) Convert_.Decompress(stream, b, 0, n);
							_b = new Bitmap(stream);
						}
						//size and speed: "image:" usually is bigger by 10-20% and faster by ~25%
					} else {
						file = Path_.Normalize(file, Folders.ThisAppImages);
						if(!Files.ExistsAsFile(file))
							o = Util.Resources_.GetAppResource(Path_.GetFileNameWithoutExtension(file));
						if(o == null) o = Image.FromFile(file);
						_b = o as Bitmap;
						if(_b == null) throw new ArgumentException("Bad image format."); //Image but not Bitmap
					}
					_dispose = true;
					_InitBitmap();
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
						throw new ArgumentException("bottom-up Bitmap");
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
			internal _Action _action;
			List<_Image> _images; //support multiple images
			internal SIArea _area;
			internal SIFlags _flags;
			uint _colorDiff;
			Func<SIResult, bool> _also;

			//output
			SIResult _result;
			Point _resultOffset; //to map the found rectangle from the captured area coordinates to the specified area coordinates

			//area data
			Util.MemoryBitmap _areaMB; //reuse while waiting, it makes slightly faster
			int _areaWidth, _areaHeight, _areaMemSize; //_areaMB width and height, use for the same purpose
			uint* _areaPixels; //the same purpose. Allocating/freeing large memory is somehow slow.
			BitmapData _areaData; //of _bmp. Could be local, because we don't wait, but better do this way.

			public void Dispose()
			{
				if(_area.Type == SIArea.AType.Bitmap) {
					if(_areaData != null) _area.B.UnlockBits(_areaData);
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

			public SIResult Result => _result;

			internal _Finder(_Action action, object image, SIArea area, SIFlags flags, int colorDiff, Func<SIResult, bool> also)
			{
				bool waitChanged = action == _Action.WaitChanged;
				if((!waitChanged && image == null) || area == null) throw new ArgumentNullException();

				_action = action;
				_area = area;
				_flags = flags;
				_colorDiff = (uint)colorDiff; if(_colorDiff > 250) throw new ArgumentOutOfRangeException("colorDiff range: 0 - 250");
				_also = also;

				SIFlags badFlags = 0; string sBadFlags = null;

				switch(_area.Type) {
				case SIArea.AType.Screen:
					badFlags = SIFlags.WindowDC;
					break;
				case SIArea.AType.Wnd:
					_area.W.ThrowIfInvalid();
					break;
				case SIArea.AType.Acc:
					if(_area.A == null) throw new ArgumentNullException(nameof(area));
					_area.W = _area.A.WndContainer;
					break;
				case SIArea.AType.Bitmap:
					badFlags = SIFlags.WindowDC;
					if(action != _Action.Find) throw new ArgumentException(); //there is no sense to wait for some changes in Bitmap
					if(_area.B == null) throw new ArgumentNullException(nameof(area));
					break;
				}

				if(0 != (_flags & badFlags)) sBadFlags = "Invalid flags for this area type: " + badFlags;
				if(sBadFlags != null) throw new ArgumentException(sBadFlags);

				_images = new List<_Image>();
				if(!waitChanged) { //else the first Find will add the area to _images
					try { _AddImage(image); }
					catch { _DisposeInputImages(); throw; } //Dispose() will not be called because we are in ctor
					if(_images.Count == 0) throw new ArgumentException("Empty.", nameof(image));
				}

				_result = new SIResult(this);
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
				//Perf.Next();
				_result.LibClear();

				bool windowDC = 0 != (_flags & SIFlags.WindowDC);
				bool allMustExist = 0 != (_flags & SIFlags.AllMustExist);

				//Get area rectangle.
				RECT r;
				_resultOffset = default;
				switch(_area.Type) {
				case SIArea.AType.Wnd:
					r = windowDC ? _area.W.ClientRect : _area.W.ClientRectInScreen;
					//TODO: if failed, probably the window is destroyed, then throw
					break;
				case SIArea.AType.Acc:
					if(!(windowDC ? _area.A.GetRect(out r, _area.W) : _area.A.GetRect(out r))) throw new CatException("*get rectangle");
					break;
				case SIArea.AType.Bitmap:
					r = new RECT(0, 0, _area.B.Width, _area.B.Height, false);
					break;
				default: //Screen
					r = _area.R;
					if(!Screen_.IsInAnyScreen(r)) r = default;
					_area.HasRect = false;
					_resultOffset.X = r.left; _resultOffset.Y = r.top;
					break;
				}
				//FUTURE: DPI

				//r is the area from where to get pixels. If windowDC, it is relative to the client area.
				//Intermediate results will be relative to r. Then will be added _resultOffset if a limiting lectangle is used.

				if(_area.HasRect) {
					var rr = _area.R;
					_resultOffset.X = rr.left; _resultOffset.Y = rr.top;
					rr.Offset(r.left, r.top);
					r.Intersect(rr);
				}

				if(_area.Type == SIArea.AType.Acc) {
					//adjust r and _resultOffset,
					//	because object rectangle may be bigger than client area (eg WINDOW object)
					//	or its part is not in client area (eg scrolled web page).
					//	If not adjusted, then may capture part of parent or sibling controls or even other windows...
					//	Never mind: should also adjust control rectangle in ancestors in the same way.
					//		This is not so important because usually whole control is visible (resized, not clipped).
					int x = r.left, y = r.top;
					r.Intersect(windowDC ? _area.W.ClientRect : _area.W.ClientRectInScreen);
					x -= r.left; y -= r.top;
					_resultOffset.X -= x; _resultOffset.Y -= y;
				}
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
				int nGood = 0;
				for(int i = _images.Count - 1; i >= 0; i--) {
					var v = _images[i].data;
					if(v.Width <= r.Width && v.Height <= r.Height) nGood++;
				}
				if(nGood == 0 || (allMustExist && nGood < _images.Count)) return false;

				//Get area pixels.
				if(_area.Type == SIArea.AType.Bitmap) {
					if(_areaData == null) {
						var pf = (_area.B.PixelFormat == PixelFormat.Format32bppArgb) ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb; //if possible, use PixelFormat of _bmp, to avoid conversion/copying. Both these formats are ok, we don't use alpha.
						_areaData = _area.B.LockBits(r, ImageLockMode.ReadOnly, pf);
						if(_areaData.Stride < 0) throw new ArgumentException("bottom-up Bitmap");
					}
					_areaPixels = (uint*)_areaData.Scan0;
					_areaWidth = _areaData.Width; _areaHeight = _areaData.Height;
				} else {
					_GetAreaPixels(r);
				}
				//Perf.Next();

				//Find image(s) in area.
				bool found = false;
				for(int i = 0, n = _images.Count; i < n; i++) {
					_result.ListIndex = i;
					_result.MatchIndex = 0;
					if(_FindImage(_images[i])) {
						found = true;
						if(!allMustExist) break;
					} else if(allMustExist) break;
				}
				//Perf.Next();
				if(found) return true;
				_result.LibClear();
				return false;
			}

			bool _FindImage(_Image image)
			{
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

				//find first pixel. This part is very important for speed.
				//int nTimesFound = 0; //debug

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

				//This for loop must be as fast as possible.
				//	There are too few 32-bit registers. Must be used a many as possible registers. See comments below.
				//	No problems if 64-bit.

				gContinue:
				{
					var f_ = &f; //part 2 of the workaround
					var p_ = f_->p + 1; //register
					var color_ = f_->color; //register
					var pLineLast_ = f_->pLineLast; //register
					for(; ; ) { //lines
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
					}
					gPixelFound:
					f.p = p_;
				}

				//nTimesFound++;
				var ap = f.p - o_pos0; //the first area pixel of the top-left of the image

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
				for(; ; ) { //lines
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
				//Print(nTimesFound);

				#endregion

				if(_action != _Action.WaitChanged) {
					int iFound = (int)(f.p - o_pos0 - areaPixels);
					RECT r = new RECT(iFound % _areaWidth, iFound / _areaWidth, imageWidth, imageHeight, true);
					r.Offset(_resultOffset.X, _resultOffset.Y);
					_result.Rect = r;

					if(_also != null) {
						if(!_also(_result)) {
							_result.MatchIndex++;
							goto gContinue;
						}
					}
				}

				return true;
				gNotFound:
				return false;
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
				//_Debug("start", 1);
				//create memory bitmap. When waiting, we reuse _areaMB, it makes slightly faster.
				if(_areaMB == null || areaWidth != _areaWidth || areaHeight != _areaHeight) {
					if(_areaMB != null) { _areaMB.Dispose(); _areaMB = null; }
					_areaMB = new Util.MemoryBitmap(_areaWidth = areaWidth, _areaHeight = areaHeight);
					//_Debug("created MemBmp");
				}
				//get DC of screen or window
				bool windowDC = 0 != (_flags & SIFlags.WindowDC);
				Wnd w = windowDC ? _area.W : default;
				IntPtr dc = Api.GetDC(w); //quite fast, when compared with other parts
				if(dc == Zero && windowDC) _area.W.ThrowNoNative("Failed");
				//_Debug("get DC");
				//copy from screen/window DC to memory bitmap
				bool bbOK = Api.BitBlt(_areaMB.Hdc, 0, 0, areaWidth, areaHeight, dc, r.left, r.top, 0xCC0020); //SRCCOPY
				Api.ReleaseDC(w, dc);
				if(!bbOK) throw new CatException("BitBlt"); //fails only if a hdc is invalid
															//_Debug("captured to MemBmp");
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
				//_Debug("_GetBitmapBits", 3);

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

		//[Conditional("SI_DEBUG_PERF")]
		//static void _Debug(string s, int perfAction = 2)
		//{
		//	//MessageBox.Show(s);
		//	switch(perfAction) {
		//	case 1: Perf.First(); break;
		//	case 2: Perf.Next(); break;
		//	case 3: Perf.NW(); break;
		//	}
		//}
	}
}

namespace Catkeys.Types
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>
	/// Defines the search area for <see cref="ScreenImage.Find"/> and similar functions.
	/// It can be a window/control, accessible object, Bitmap or a rectangle in screen.
	/// Also allows to specify a rectangle in it, which makes the search area smaller and the function faster.
	/// Example: <c>ScreenImage.Find("image.png", new SIArea(w, 100, 100, 100, 100));</c>.
	/// </summary>
	public class SIArea
	{
		internal enum AType :byte { Screen, Wnd, Acc, Bitmap }

		internal AType Type;
		internal bool HasRect;
		internal Wnd W;
		internal Acc A;
		internal Bitmap B;
		internal RECT R;

		public static implicit operator SIArea(Wnd w) => new SIArea() { W = w, Type = AType.Wnd };

		public static implicit operator SIArea(Acc a) => new SIArea() { A = a, Type = AType.Acc };

		public static implicit operator SIArea(Bitmap b) => new SIArea() { B = b, Type = AType.Bitmap };

		public static implicit operator SIArea(RECT r) => new SIArea() { R = r, Type = AType.Screen };

		SIArea() { }

		public SIArea(Wnd w, RECT r) { W = w; Type = AType.Wnd; SetRect(r); }

		public SIArea(Wnd w, int x, int y, int width, int height) { W = w; Type = AType.Wnd; SetRect(x, y, width, height); }

		public SIArea(Acc a, RECT r) { A = a; Type = AType.Acc; SetRect(r); }

		public SIArea(Acc a, int x, int y, int width, int height) { A = a; Type = AType.Acc; SetRect(x, y, width, height); }

		public SIArea(int x, int y, int width, int height) { Type = AType.Screen; SetRect(x, y, width, height); }

		public void SetRect(RECT r) { R = r; HasRect = true; }

		public void SetRect(int x, int y, int width, int height) { R = new RECT(x, y, width, height, true); HasRect = true; }
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// Flags for <see cref="ScreenImage.Find"/> and similar functions.
	/// </summary>
	[Flags]
	public enum SIFlags
	{
		/// <summary>
		/// Get pixels from the device context (DC) of the window client area, not from screen DC.
		/// Not used when area is Bitmap.
		/// Notes:
		/// Usually much faster.
		/// Can get pixels from window parts that are covered by other windows or offscreen. But not from hidden and minimized windows.
		/// Does not work on Windows 7 if Aero theme is turned off. Then this flag is ignored.
		/// If the window is DPI-scaled, the specified image must be captured from its non-scaled version.
		/// Cannot find images in some windows (including Windows Store apps), and in some window parts (glass). All pixels captured from these windows/parts are black.
		/// </summary>
		WindowDC = 1,

		/// <summary>
		/// When the image argument specifies multiple images, all they must exist, else result is "not found".
		/// The result rectangle is of the last image. To get all rectangles, use the <b>also</b> parameter: <c>also: t => { rectList.Add(t.Rect); return true; }</c>.
		/// </summary>
		AllMustExist = 2,

		//CONSIDER: this was used in QM2. Now can use png alpha channel instead, but need some work and knowledge. But a code creator tool could do it in one click.
		///// <summary>
		///// Use the top-left pixel color of the image as transparent color (don't compare pixels that have this color).
		///// </summary>
		//MakeTransparent = ,
	}

	/// <summary>
	/// Result of <see cref="ScreenImage.Find"/> and similar functions.
	/// </summary>
	public class SIResult :ISupportOrThrow
	{
		ScreenImage._Finder _f;

		internal SIResult(ScreenImage._Finder f)
		{
			_f = f;
		}

		/// <summary>
		/// Location of the found image.
		/// Relative to the window/control client area (if area type is Wnd), accessible object (if Acc), bitmap (if Bitmap) or screen (if RECT).
		/// More info: <see cref="ScreenImage.Find"/>.
		/// </summary>
		public RECT Rect;

		/// <summary>
		/// When there are multiple matching images on the screen, this is the 0-based index of current matching image.
		/// Can be used in callback function to create action "skip n matching images". Example: <c>also: t => t.MatchIndex==1</c>.
		/// When the image argument specifies multiple images, this will start from 0 for each image.
		/// </summary>
		public int MatchIndex { get; internal set; }

		/// <summary>
		/// When the image argument specifies multiple images, this is the 0-based index of the found image in the list.
		/// </summary>
		public int ListIndex { get; internal set; }

		internal void LibClear()
		{
			Rect = default;
			ListIndex = 0;
			MatchIndex = 0;
		}

		internal void LibMouseAction(MButton button, Coord x, Coord y)
		{
			var area = _f._area;
			if(area.Type == SIArea.AType.Bitmap) throw new InvalidOperationException();

			Debug.Assert(!Rect.IsEmpty);
			if(Rect.IsEmpty) return;

			//rejected: Click will activate it. Don't activate if just Move.
			//if(0 != (_f._flags & SIFlags.WindowDC)) {
			//	if(area.W.IsCloaked) area.W.ActivateLL();
			//}

			var p = Coord.NormalizeInRect(x, y, Rect, centerIfEmpty: true);

			if(area.Type == SIArea.AType.Screen) {
				if(button == 0) Mouse.Move(p.X, p.Y);
				else Mouse.ClickEx(button, p.X, p.Y);
			} else {
				if(area.Type == SIArea.AType.Acc) {
					if(!area.A.GetRect(out var r, area.W)) throw new CatException(0, "*get rectangle");
					p.Offset(r.left, r.top);
				}
				if(button == 0) Mouse.Move(area.W, p.X, p.Y);
				else Mouse.ClickEx(button, area.W, p.X, p.Y);
			}
		}
	}
}
