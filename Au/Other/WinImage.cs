//#define WI_DEBUG_PERF
//#define WI_SIMPLE
//#define WI_TEST_NO_OPTIMIZATION

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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

//FUTURE: test OpenCV - an open source library for computer vision.

namespace Au
{
	/// <summary>
	/// Captures, finds and clicks images and colors in windows.
	/// </summary>
	/// <remarks>
	/// An image is any visible rectangular part of a window. A color is any visible pixel (the same as image of size 1x1).
	/// A <b>WinImage</b> variable holds results of <see cref="Find"/> and similar functions (rectangle etc).
	/// </remarks>
	public partial class WinImage
	{
		#region load, save

		/// <summary>
		/// Loads image from file, string or resource.
		/// </summary>
		/// <param name="image"><inheritdoc cref="Find"/></param>
		/// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
		/// <exception cref="Exception">Depending on <paramref name="image"/> string format, exceptions of <see cref="Image.FromFile(string)"/>, <see cref="Bitmap(Stream)"/>, <see cref="Convert_.Decompress"/>.</exception>
		/// <exception cref="ArgumentException">Bad image format (the image cannot be loaded as Bitmap).</exception>
		/// <remarks>
		/// <see cref="Find"/> uses this function when <i>image</i> argument type is string. More info there.
		/// </remarks>
		public static unsafe Bitmap LoadImage(string image)
		{
			Bitmap R = null;
			object o = null;
			if(image.StartsWith_("image:") || image.StartsWith_("~:")) { //Base64-encoded image. Prefix: "image:" png, "~:" zipped bmp.
				bool compressed = image[0] == '~';
				int start = compressed ? 2 : 6, len = image.Length - start, n = (int)(len * 3L / 4);
				var b = new byte[n];
				fixed (byte* pb = b) {
					fixed (char* ps = image) n = Convert_.Base64Decode(ps + start, len, pb, n);
				}
				using(var stream = compressed ? new MemoryStream() : new MemoryStream(b, 0, n, false)) {
					if(compressed) Convert_.Decompress(stream, b, 0, n);
					R = new Bitmap(stream);
				}
				//size and speed of "image:" and "~:": "image:" usually is bigger by 10-20% and faster by ~25%
			} else {
				image = Path_.Normalize(image, Folders.ThisAppImages);
				if(!File_.ExistsAsFile(image, true))
					o = Util.Resources_.GetAppResource(Path_.GetFileName(image, true));
				if(o == null) o = Image.FromFile(image);
				R = o as Bitmap;
				if(R == null) throw new ArgumentException("Bad image format."); //Image but not Bitmap
			}
			return R;
		}

		#endregion

		#region results

		WIArea _area;

		///// <summary>
		///// <i>area</i> parameter of the function.
		///// </summary>
		//public WIArea Area => _area;

		WinImage(WIArea area)
		{
			_area = area;
		}

		WinImage(WinImage copy)
		{
			_area = copy._area;
			Rect = copy.Rect;
			MatchIndex = copy.MatchIndex;
			ListIndex = copy.ListIndex;
		}

		void _Clear()
		{
			Rect = default;
			ListIndex = 0;
			MatchIndex = 0;
		}

		//rejected. Use OrThrow.
		///// <summary>
		///// If x is not null, returns x, else throws <see cref="NotFoundException"/>.
		///// Alternatively you can use <see cref="ExtensionMethods.OrThrow(WinImage)" r=""/>.
		///// </summary>
		///// <exception cref="NotFoundException">x is null.</exception>
		///// <example><inheritdoc cref="ExtensionMethods.OrThrow(WinImage)"/></example>
		//public static WinImage operator +(WinImage x) => x ?? throw new NotFoundException("Not found (WinImage).");

		/// <summary>
		/// Gets location of the found image, relative to the search area.
		/// </summary>
		/// <remarks>
		/// Relative to the window/control client area (if area type is Wnd), accessible object (if Acc), image (if Bitmap) or screen (if RECT).
		/// More info: <see cref="Find"/>.
		/// </remarks>
		public RECT Rect { get; private set; }

		/// <summary>
		/// Gets location of the found image in screen coordinates.
		/// </summary>
		/// <remarks>
		/// Slower than <see cref="Rect"/>.
		/// </remarks>
		public RECT RectInScreen
		{
			get
			{
				RECT r;
				switch(_area.Type) {
				case WIArea.AType.Wnd:
					r = Rect;
					_area.W.MapClientToScreen(ref r);
					return r;
				case WIArea.AType.Acc:
					if(!_area.A.GetRect(out var rr)) return default;
					r = Rect;
					r.Offset(rr.left, rr.top);
					return r;
				}
				return Rect; //screen or bitmap
			}
		}

		/// <summary>
		/// Gets 0-based index of current matching image instance.
		/// </summary>
		/// <remarks>
		/// Can be useful in <i>also</i> callback functions.
		/// When the <i>image</i> argument is a list of images, <b>MatchIndex</b> starts from 0 for each list image.
		/// </remarks>
		public int MatchIndex { get; private set; }

		/// <summary>
		/// When the <i>image</i> argument is a list of images, gets 0-based index of the list image.
		/// </summary>
		public int ListIndex { get; private set; }

		/// <summary>
		/// Can be used in <i>also</i> callback function to skip n matching images. Example: <c>also: o => o.Skip(n)</c>.
		/// </summary>
		/// <param name="n">How many matching images to skip.</param>
		public WIAlso Skip(int n) => MatchIndex == n ? WIAlso.OkReturn : (MatchIndex < n ? WIAlso.FindOther : WIAlso.FindOtherOfList);

		//Called by extension methods.
		internal void LibMouseAction(MButton button, Coord x, Coord y)
		{
			if(_area.Type == WIArea.AType.Bitmap) throw new InvalidOperationException();

			Debug.Assert(!Rect.IsEmpty);
			if(Rect.IsEmpty) return;

			//rejected: Click will activate it. Don't activate if just Move.
			//if(0 != (_f._flags & WIFlags.WindowDC)) {
			//	if(_area.W.IsCloaked) _area.W.ActivateLL();
			//}

			var p = Coord.NormalizeInRect(x, y, Rect, centerIfEmpty: true);

			if(_area.Type == WIArea.AType.Screen) {
				if(button == 0) Mouse.Move(p);
				else Mouse.ClickEx(button, p);
			} else {
				var w = _area.W;
				if(_area.Type == WIArea.AType.Acc) {
					if(!_area.A.GetRect(out var r, w)) throw new AuException(0, "*get rectangle");
					p.x += r.left; p.y += r.top;
				}
				if(button == 0) Mouse.Move(w, p.x, p.y);
				else Mouse.ClickEx(button, w, p.x, p.y);
			}
		}

		///
		public override string ToString() => $"{ListIndex.ToString()}, {MatchIndex.ToString()}, {Rect.ToString()}";

		#endregion

		/// <summary>
		/// Finds image(s) or color(s) displayed in window or other area.
		/// </summary>
		/// <returns>
		/// Returns <see cref="WinImage"/> object containing the rectangle of the found image.
		/// Returns null if not found. See example.
		/// </returns>
		/// <param name="area">
		/// Where to search. Can be a window/control, accessible object, another image or a rectangle in screen.
		/// <list type="bullet">
		/// <item><see cref="Wnd"/> - window or control. The search area is its client area.</item>
		/// <item><see cref="Acc"/> - accessible object.</item>
		/// <item><see cref="Bitmap"/> - another image. These flags are invalid: <see cref="WIFlags.WindowDC"/>.</item>
		/// <item><see cref="RECT"/> - a rectangle area in screen. These flags are invalid: <see cref="WIFlags.WindowDC"/>.</item>
		/// <item><see cref="WIArea"/> - can contain Wnd, Acc or Bitmap. Also allows to specify a rectangle in it, which makes the search area smaller and the function faster. Example: <c>WinImage.Find((w, (left, top, width, height)), "image.png");</c>.</item>
		/// </list>
		/// </param>
		/// <param name="image">
		/// Image or color to find. Can be of type:
		/// <list type="bullet">
		/// <item>
		/// string - path of .png or .bmp file.
		/// If not full path, uses <see cref="Folders.ThisAppImages"/>.
		/// If the file does not exist, looks in resources of apdomain's entry assembly. For example, looks for Project.Properties.Resources.X if file @"C:\X.png" not found.
		/// </item>
		/// <item>
		/// string that starts with "image:" or "~:" - Base64-encoded .png or .bmp image embedded in script.
		/// If "image:", it is .png file data, else it is compressed .bmp file data.
		/// Can be created with function Au.Controls.ImageUtil.ImageToString (in Au.Controls.dll).
		/// </item>
		/// <item>int, ColorInt or Color - color. Int must be in 0xRRGGBB format. Alpha is not used.</item>
		/// <item>Bitmap - image object in memory.</item>
		/// <item>IEnumerable of string, int/ColorInt/Color, Bitmap or object - multiple images or colors. Action - find any. To create a different action can be used callback function (parameter <paramref name="also"/>).</item>
		/// </list>
		/// Icons are not supported directly, but you can use <see cref="Icon_.GetFileIconImage"/> or <see cref="Icon_.HandleToImage"/>.
		/// Transparent and partially transparent pixels are not compared. For example, when you capture a non-rectangular area image, the image actually is rectangular, but pixels outside of its captured area are transparent and therefore not compared. Also you can draw transparent areas with an image editor that supports it, for example Paint.NET.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="colorDiff">Maximal allowed color difference. Use to to find images that have slightly different colors than the specified image. Can be 0 - 250, but should be as small as possible. Applied to each color component (red, green, blue) of each pixel.</param>
		/// <param name="also">
		/// Callback function. Called for each found image instance and receives its rectangle, match index and list index. Can return one of <see cref="WIAlso"/> values.
		/// Callback functions can be used to get rectangles of multiple found images, create custom behaviors/actions, etc. Examples:
		/// <list type="bullet">
		/// <item>Skip some matching images if some condition if false: <c>also: o => condition ? WIAlso.OkReturn : WIAlso.FindOther</c></item>
		/// <item>Skip n matching images: <c>also: o => o.Skip(n)</c></item>
		/// <item>Get rectangles etc of all matching images: <c>also: o => { list.Add(o); return false; }</c>. Don't use this code in 'wait' functions.</item>
		/// <item>Get rectangles etc of all matching images and stop waiting: <c>also: o => { list.Add(o); o.Found = true; return false; }</c></item>
		/// <item>Do different actions depending on which list images found: <c>var found = new BitArray(images.Length); WinImage.Find(w, images, also: o => { found[o.ListIndex] = true; return WIAlso.OkFindMoreOfList; }); if(found[0]) Print(0); if(found[1]) Print(1);</c></item>
		/// </list>
		/// </param>
		/// <exception cref="WndException">Invalid window handle (the <paramref name="area"/> argument).</exception>
		/// <exception cref="ArgumentException">
		/// An argument is of unsupported type or is/contains a null/invalid value.
		/// Image or area is a bottom-up Bitmap object (see <see cref="BitmapData.Stride"/>). Such bitmaps are unusual in .NET (GDI+), but can be created by <b>Image.FromHbitmap</b> (instead use <see cref="BitmapFromHbitmap"/>).
		/// </exception>
		/// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
		/// <exception cref="Exception">Depending on <paramref name="image"/> string format, exceptions of <see cref="Image.FromFile(string)"/>, <see cref="Bitmap(Stream)"/>, <see cref="Convert_.Decompress"/>.</exception>
		/// <exception cref="AuException">Something failed.</exception>
		/// <remarks>
		/// To create code for this function, use dialog "Find image or color in window". It is form <b>Au.Tools.Form_WinImage</b> in Au.Tools.dll.
		/// 
		/// The speed mostly depends on:
		/// 1. The size of the search area. Use the smallest possible area (control or accessible object or rectangle in window like <c>(w, rectangle)</c>).
		/// 2. Flag <see cref="WIFlags.WindowDC"/>. Usually makes several times faster. With this flag the speed depends on window.
		/// 3. Video driver. Can be much slower if incorrect, generic or virtual PC driver is used. Flag <see cref="WIFlags.WindowDC"/> should help.
		/// 4. <paramref name="colorDiff"/>. Should be as small as possible.
		/// 
		/// If flag <see cref="WIFlags.WindowDC"/> is not used, the search area must be visible on the screen. If it is covered by other windows, the function will search in these windows.
		/// 
		/// The function can only find images that exactly match the specified image. With <paramref name="colorDiff"/> it can find images with slightly different colors and brightness. It cannot find images with different shapes.
		/// 
		/// This function is not the best way to find objects when the script is intended for long use or for use on multiple computers or must be very reliable. Because it may fail to find the image after are changed some settings - system theme, application theme, text size (DPI), font smoothing (if the image contains text), etc. Also are possible various unexpected temporary conditions that may distort or hide the image, for example adjacent window shadow, a tooltip or some temporary window. If possible, in such scripts instead use other functions, eg find control or accessible object.
		/// </remarks>
		/// <example>
		/// Code created with dialog "Find image or color in window".
		/// <code><![CDATA[
		/// var w = Wnd.Find("Window Name").OrThrow();
		/// string image = "image:iVBORw0KGgoAAAANSUhEUgAAABYAAAANCAYAAACtpZ5jAAAAAXNSR0IArs4c...";
		/// var wi = WinImage.Find(w, image).OrThrow();
		/// wi.Click();
		/// ]]></code>
		/// </example>
		public static WinImage Find(WIArea area, object image, WIFlags flags = 0, int colorDiff = 0, Func<WinImage, WIAlso> also = null)
		{
			using(var f = new _Finder(_Action.Find, area, image, flags, colorDiff, also)) {
				if(!f.Find()) return null;
				return f.Result;
			}
		}

		internal enum _Action { Find, Wait, WaitNot, WaitChanged }

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <inheritdoc cref="Find"/>
		/// <summary>
		/// Finds image(s) or color(s) displayed in window or other area. Waits until found.
		/// </summary>
		/// <returns>Returns <see cref="WinImage"/> object containing the rectangle of the found image. On timeout returns null if <paramref name="secondsTimeout"/> is negative; else exception.</returns>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits infinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns null.
		/// </param>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="WndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		public static WinImage Wait(double secondsTimeout, WIArea area, object image, WIFlags flags = 0, int colorDiff = 0, Func<WinImage, WIAlso> also = null)
		{
			var r = _Wait(_Action.Wait, secondsTimeout, area, image, flags, colorDiff, also);
			return r.ok ? r.result : null;

			//tested: does not create garbage while waiting.
		}

		/// <inheritdoc cref="Find"/>
		/// <summary>
		/// Waits until image(s) or color(s) is not displayed in window or other area.
		/// </summary>
		/// <param name="secondsTimeout"><inheritdoc cref="WaitFor.Condition"/></param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="WndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		public static bool WaitNot(double secondsTimeout, WIArea area, object image, WIFlags flags = 0, int colorDiff = 0, Func<WinImage, WIAlso> also = null)
		{
			return _Wait(_Action.WaitNot, secondsTimeout, area, image, flags, colorDiff, also).ok;
		}

		/// <inheritdoc cref="Find"/>
		/// <summary>
		/// Waits until something visually changes in window or other area.
		/// </summary>
		/// <param name="secondsTimeout"><inheritdoc cref="WaitFor.Condition"/></param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="WndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <remarks>
		/// The same as <see cref="WaitNot"/>, but instead of <i>image</i> parameter this function captures the area image at the beginning.
		/// </remarks>
		public static bool WaitChanged(double secondsTimeout, WIArea area, WIFlags flags = 0, int colorDiff = 0)
		{
			return _Wait(_Action.WaitChanged, secondsTimeout, area, null, flags, colorDiff, null).ok;
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		static (bool ok, WinImage result) _Wait(_Action action, double secondsTimeout, WIArea area, object image, WIFlags flags, int colorDiff, Func<WinImage, WIAlso> also)
		{
			using(var f = new _Finder(action, area, image, flags, colorDiff, also)) {
				bool ok = WaitFor.Condition(secondsTimeout, () => f.Find() ^ (action > _Action.Wait));
				return (ok, f.Result);
			}
		}

		internal unsafe class _Finder :IDisposable
		{
			class _Image
			{
				Bitmap _b;
				internal BitmapData data;
				internal _OptimizationData opt;
				bool _dispose;

				public _Image(string file)
				{
					_b = LoadImage(file);
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

					//speed: quite fast, even if need conversion. Much faster than Clone.
				}

				public _Image(ColorInt color)
				{
					var p = (int*)Util.NativeHeap.Alloc(4);
					*p = (int)color | unchecked((int)0xff000000);
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
						if(_b == null) Util.NativeHeap.Free((void*)data.Scan0); //when color or _action==WaitChanged
						else _b.UnlockBits(data);
						data = null;
					}
					if(_b != null) {
						if(_dispose) _b.Dispose();
						_b = null;
					}
				}
			}

			struct _AreaData
			{
				public Util.MemoryBitmap mb; //reuse while waiting, it makes slightly faster
				public int width, height, memSize; //_areaMB width and height, use for the same purpose
				public uint* pixels; //the same purpose. Allocating/freeing large memory is somehow slow.
				public BitmapData bmpData; //of _bmp. Could be local, because we don't wait, but better do this way.
			}

			//input
			_Action _action;
			WIArea _area { get; }
			List<_Image> _images; //support multiple images
			WIFlags _flags;
			uint _colorDiff;
			Func<WinImage, WIAlso> _also;

			//output
			internal WinImage Result { get; private set; }
			POINT _resultOffset; //to map the found rectangle from the captured area coordinates to the specified area coordinates

			//area data
			_AreaData _ad;

			public void Dispose()
			{
				if(_area.Type == WIArea.AType.Bitmap) {
					if(_ad.bmpData != null) _area.B.UnlockBits(_ad.bmpData);
				} else {
					Util.NativeHeap.Free(_ad.pixels);
					_ad.mb?.Dispose();
				}
				_DisposeInputImages();
			}

			void _DisposeInputImages()
			{
				if(_images != null) foreach(var v in _images) v.Dispose();
			}

			internal _Finder(_Action action, WIArea area, object image, WIFlags flags, int colorDiff, Func<WinImage, WIAlso> also)
			{
				bool waitChanged = action == _Action.WaitChanged;
				if((!waitChanged && image == null) || area == null) throw new ArgumentNullException();

				_action = action;
				_area = area;
				_flags = flags;
				_colorDiff = (uint)colorDiff; if(_colorDiff > 250) throw new ArgumentOutOfRangeException("colorDiff range: 0 - 250");
				_also = also;

				WIFlags badFlags = 0; string sBadFlags = null;

				switch(_area.Type) {
				case WIArea.AType.Screen:
					badFlags = WIFlags.WindowDC;
					break;
				case WIArea.AType.Wnd:
					_area.W.ThrowIfInvalid();
					break;
				case WIArea.AType.Acc:
					if(_area.A == null) throw new ArgumentNullException(nameof(area));
					_area.W = _area.A.WndContainer;
					goto case WIArea.AType.Wnd;
				case WIArea.AType.Bitmap:
					badFlags = WIFlags.WindowDC;
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

				Result = new WinImage(_area);
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
				case ColorInt color:
					_AddColor(color);
					break;
				case int color:
					_AddColor(color);
					break;
				case Color color:
					_AddColor(color);
					break;
				case System.Windows.Media.Color color:
					_AddColor(color);
					break;
				case IEnumerable<object> e: //string, Bitmap or boxed color
					foreach(var v in e) _AddImage(v);
					break;
				case IEnumerable<ColorInt> e:
					foreach(var color in e) _AddColor(color);
					break;
				case IEnumerable<int> e:
					foreach(var color in e) _AddColor(color);
					break;
				case IEnumerable<Color> e:
					foreach(var color in e) _AddColor(color);
					break;
				case IEnumerable<System.Windows.Media.Color> e:
					foreach(var color in e) _AddColor(color);
					break;
				default: throw new ArgumentException("Unsupported object type.", nameof(image));
				}

				void _AddColor(ColorInt color) => _images.Add(new _Image(color));
			}

			public bool Find()
			{
				//Perf.Next();
				Result._Clear();

				bool windowDC = 0 != (_flags & WIFlags.WindowDC);
				bool failedGetRect = false;

				//Get area rectangle.
				RECT r;
				_resultOffset = default;
				switch(_area.Type) {
				case WIArea.AType.Wnd:
					failedGetRect = !_area.W.GetClientRect(out r, !windowDC);
					break;
				case WIArea.AType.Acc:
					failedGetRect = !(windowDC ? _area.A.GetRect(out r, _area.W) : _area.A.GetRect(out r));
					break;
				case WIArea.AType.Bitmap:
					r = (0, 0, _area.B.Width, _area.B.Height, false);
					break;
				default: //Screen
					r = _area.R;
					if(!Screen_.IsInAnyScreen(r)) r = default;
					_area.HasRect = false;
					_resultOffset.x = r.left; _resultOffset.y = r.top;
					break;
				}
				if(failedGetRect) {
					_area.W.ThrowIfInvalid();
					throw new AuException("*get rectangle");
				}
				//FUTURE: DPI

				//r is the area from where to get pixels. If windowDC, it is relative to the client area.
				//Intermediate results will be relative to r. Then will be added _resultOffset if a limiting lectangle is used.

				if(_area.HasRect) {
					var rr = _area.R;
					_resultOffset.x = rr.left; _resultOffset.y = rr.top;
					rr.Offset(r.left, r.top);
					r.Intersect(rr);
				}

				if(_area.Type == WIArea.AType.Acc) {
					//adjust r and _resultOffset,
					//	because object rectangle may be bigger than client area (eg WINDOW object)
					//	or its part is not in client area (eg scrolled web page).
					//	If not adjusted, then may capture part of parent or sibling controls or even other windows...
					//	Never mind: should also adjust control rectangle in ancestors in the same way.
					//		This is not so important because usually whole control is visible (resized, not clipped).
					int x = r.left, y = r.top;
					_area.W.GetClientRect(out var rw, !windowDC);
					r.Intersect(rw);
					x -= r.left; y -= r.top;
					_resultOffset.x -= x; _resultOffset.y -= y;
				}
				if(r.IsEmpty) return false; //never mind: if WaitChanged and this is the first time, immediately returns 'changed'

				//If WaitChanged, first time just get area pixels into _images[0].
				if(_action == _Action.WaitChanged && _images.Count == 0) {
					_GetAreaPixels(r);
					var data = new BitmapData() { Width = _ad.width, Height = _ad.height, Scan0 = (IntPtr)_ad.pixels };
					_ad.pixels = null; _ad.memSize = 0;
					_images.Add(new _Image(data));
					return true;
				}

				//Return false immediately if all images are bigger than the search area.
				int nGood = 0;
				for(int i = _images.Count - 1; i >= 0; i--) {
					var v = _images[i].data;
					if(v.Width <= r.Width && v.Height <= r.Height) nGood++;
				}
				if(nGood == 0) return false;

				//Get area pixels.
				if(_area.Type == WIArea.AType.Bitmap) {
					if(_ad.bmpData == null) {
						var pf = (_area.B.PixelFormat == PixelFormat.Format32bppArgb) ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb; //if possible, use PixelFormat of _bmp, to avoid conversion/copying. Both these formats are ok, we don't use alpha.
						_ad.bmpData = _area.B.LockBits(r, ImageLockMode.ReadOnly, pf);
						if(_ad.bmpData.Stride < 0) throw new ArgumentException("bottom-up Bitmap");
					}
					_ad.pixels = (uint*)_ad.bmpData.Scan0;
					_ad.width = _ad.bmpData.Width; _ad.height = _ad.bmpData.Height;
				} else {
					_GetAreaPixels(r);
				}
				//Perf.Next();

				//Find image(s) in area.
				WinImage alsoResult = null;
				for(int i = 0, n = _images.Count; i < n; i++) {
					Result.ListIndex = i;
					Result.MatchIndex = 0;
					if(_FindImage(_images[i], out var alsoAction, ref alsoResult)) return true;
					if(alsoAction == WIAlso.NotFound || alsoAction == WIAlso.FindOtherOfThis || alsoAction == WIAlso.OkFindMoreOfThis) break;
				}
				//Perf.Next();
				if(alsoResult != null) {
					Result = alsoResult;
					return true;
				}
				Result._Clear();
				return false;
			}

			bool _FindImage(_Image image, out WIAlso alsoAction, ref WinImage alsoResult)
			{
				alsoAction = WIAlso.FindOtherOfList;

				BitmapData bdata = image.data;
				int imageWidth = bdata.Width, imageHeight = bdata.Height;
				if(_ad.width < imageWidth || _ad.height < imageHeight) return false;
				uint* imagePixels = (uint*)bdata.Scan0, imagePixelsTo = imagePixels + imageWidth * imageHeight;
				uint* areaPixels = _ad.pixels;

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

				if(!image.opt.Init(bdata, _ad.width)) return false;
				var opt = image.opt; //copy struct, size = 9*int
				int o_pos0 = opt.v0.pos;
				var o_a1 = &opt.v1; var o_an = o_a1 + (opt.N - 1);

				//find first pixel. This part is very important for speed.
				//int nTimesFound = 0; //debug

				var areaWidthMinusImage = _ad.width - imageWidth;
				var pFirst = areaPixels + o_pos0;
				var pLast = pFirst + _ad.width * (_ad.height - imageHeight) + areaWidthMinusImage;

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
					RECT r = (iFound % _ad.width, iFound / _ad.width, imageWidth, imageHeight);
					r.Offset(_resultOffset.x, _resultOffset.y);
					Result.Rect = r;

					if(_also != null) {
						var wi = new WinImage(Result); //create new WinImage object because the callback may add it to a list etc
						switch(alsoAction = _also(wi)) {
						case WIAlso.OkReturn:
							alsoResult = null;
							break;
						case WIAlso.OkFindMore:
						case WIAlso.OkFindMoreOfThis:
							alsoResult = wi;
							goto case WIAlso.FindOther;
						case WIAlso.OkFindMoreOfList:
							alsoResult = wi;
							goto gNotFound;
						case WIAlso.NotFound:
						case WIAlso.FindOtherOfList:
							goto gNotFound;
						case WIAlso.FindOther:
						case WIAlso.FindOtherOfThis:
							Result.MatchIndex++;
							goto gContinue;
						default: throw new InvalidEnumArgumentException();
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

			static bool _IsTransparent(uint color) => color < 0xff000000;

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

#if WI_TEST_NO_OPTIMIZATION
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
				if(_ad.mb == null || areaWidth != _ad.width || areaHeight != _ad.height) {
					if(_ad.mb != null) { _ad.mb.Dispose(); _ad.mb = null; }
					_ad.mb = new Util.MemoryBitmap(_ad.width = areaWidth, _ad.height = areaHeight);
					//_Debug("created MemBmp");
				}
				//get DC of screen or window
				bool windowDC = 0 != (_flags & WIFlags.WindowDC);
				Wnd w = windowDC ? _area.W : default;
				using(var dc = new Util.LibWindowDC(w)) { //quite fast, when compared with other parts
					if(dc.Is0 && windowDC) w.ThrowNoNative("Failed");
					//_Debug("get DC");
					//copy from screen/window DC to memory bitmap
					uint rop = windowDC ? Api.SRCCOPY : Api.SRCCOPY | Api.CAPTUREBLT;
					bool bbOK = Api.BitBlt(_ad.mb.Hdc, 0, 0, areaWidth, areaHeight, dc, r.left, r.top, rop);
					if(!bbOK) throw new AuException("BitBlt"); //the API fails only if a HDC is invalid
				}
				//_Debug("captured to MemBmp");
				//get pixels
				int memSize = areaWidth * areaHeight * 4; //7.5 MB for a max window in 1920*1080 monitor
				if(memSize > _ad.memSize) { //while waiting, we reuse the memory, it makes slightly faster.
					_ad.pixels = (uint*)Util.NativeHeap.ReAlloc(_ad.pixels, memSize);
					_ad.memSize = memSize;
				}
				var h = new Api.BITMAPINFOHEADER() {
					biSize = sizeof(Api.BITMAPINFOHEADER),
					biWidth = areaWidth, biHeight = -areaHeight,
					biPlanes = 1, biBitCount = 32,
					//biCompression = 0, //BI_RGB
				};
				if(Api.GetDIBits(_ad.mb.Hdc, _ad.mb.Hbitmap, 0, areaHeight, _ad.pixels, &h, 0) //DIB_RGB_COLORS
					!= areaHeight) throw new AuException("GetDIBits");
				//_Debug("_GetBitmapBits", 3);

				//remove alpha (why it is here?). Currently don't need.
				////Perf.First();
				//byte* p = (byte*)_areaPixels, pe = p + memSize;
				//for(p += 3; p < pe; p += 4) *p = 0xff;
				////Perf.NW(); //1100 for max window

				//see what we have
				//var testFile = Folders.Temp + "WinImage.png";
				//using(var areaBmp = new Bitmap(areaWidth, areaHeight, areaWidth * 4, PixelFormat.Format32bppRgb, (IntPtr)_areaPixels)) {
				//	areaBmp.Save(testFile);
				//}
				//Shell.Run(testFile);
			}
		}

		//[Conditional("WI_DEBUG_PERF")]
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

namespace Au.Types
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>
	/// Defines the search area for <see cref="WinImage.Find"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// It can be a window/control, accessible object, another image or a rectangle in screen.
	/// Also allows to specify a rectangle in it, which makes the search area smaller and the function faster.
	/// Has implicit conversions from Wnd, Acc, Bitmap, RECT (rectangle in screen), tuple (Wnd, RECT), tuple (Acc, RECT).
	/// Example: <c>WinImage.Find((w, (left, top, width, height)), "image.png");</c>.
	/// </remarks>
	public class WIArea
	{
		internal enum AType :byte { Screen, Wnd, Acc, Bitmap }

		internal AType Type;
		internal bool HasRect;
		internal Wnd W;
		internal Acc A;
		internal Bitmap B;
		internal RECT R;

		WIArea() { }
		public WIArea(Wnd w, RECT r) { W = w; Type = AType.Wnd; SetRect(r); }
		public WIArea(Acc a, RECT r) { A = a; Type = AType.Acc; SetRect(r); }
		public void SetRect(RECT r) { R = r; HasRect = true; }

		public static implicit operator WIArea(Wnd w) => new WIArea() { W = w, Type = AType.Wnd };
		public static implicit operator WIArea(Acc a) => new WIArea() { A = a, Type = AType.Acc };
		public static implicit operator WIArea(Bitmap b) => new WIArea() { B = b, Type = AType.Bitmap };
		public static implicit operator WIArea(RECT r) => new WIArea() { R = r, Type = AType.Screen };
		public static implicit operator WIArea((Wnd w, RECT r) t) => new WIArea(t.w, t.r);
		public static implicit operator WIArea((Acc a, RECT r) t) => new WIArea(t.a, t.r);
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	//rejected: now using object. Maybe in the future.
	///// <summary>
	///// Image or color for <see cref="WinImage.Find"/> and similar functions.
	///// </summary>
	//public struct WIImage
	//{
	//}

	/// <summary>
	/// Flags for <see cref="WinImage.Find"/> and similar functions.
	/// </summary>
	[Flags]
	public enum WIFlags
	{
		/// <summary>
		/// Get pixels from the device context (DC) of the window client area, not from screen DC.
		/// Not used when area is Bitmap.
		/// Notes:
		/// Usually much faster.
		/// Can get pixels from window parts that are covered by other windows or offscreen. But not from hidden and minimized windows.
		/// Does not work on Windows 7 if Aero theme is turned off. Then this flag is ignored.
		/// If the window is DPI-scaled, the image must be captured from its non-scaled version.
		/// Cannot find images in some windows (including Windows Store apps), and in some window parts (glass). All pixels captured from these windows/parts are black.
		/// </summary>
		WindowDC = 1,

		//rejected: this was used in QM2. Now can use png alpha instead, and CaptureUI allows to capture it.
		///// <summary>
		///// Use the top-left pixel color of the image as transparent color (don't compare pixels that have this color).
		///// </summary>
		//MakeTransparent = ,
	}

	/// <summary>
	/// Used with <see cref="WinImage.Find"/> and <see cref="WinImage.Wait"/>. Its callback function (parameter <i>also</i>) can return one of these values.
	/// </summary>
	public enum WIAlso
	{
		/// <summary>
		/// Stop searching.
		/// Let the main function (<b>Find</b> or <b>Wait</b>) return current result.
		/// </summary>
		OkReturn,

		/// <summary>
		/// Find more instances of current image. If used list of images, also search for other images.
		/// Then let the main function return current result.
		/// </summary>
		OkFindMore,

		/// <summary>
		/// Find more instances of current image. When used list of images, don't search for other images.
		/// Then let the main function return current result.
		/// </summary>
		OkFindMoreOfThis,

		/// <summary>
		/// If used list of images, search for other images. Don't search for more instances of current image.
		/// Then let the main function return current result.
		/// </summary>
		OkFindMoreOfList,

		/// <summary>
		/// Stop searching.
		/// Let <b>Find</b> return null. Let <b>Wait</b> continue waiting. But if a <b>Find...Return</b> value used previously, return that result.
		/// </summary>
		NotFound,

		/// <summary>
		/// Find more instances of current image. If used list of images, also search for other images.
		/// If not found, let <b>Find</b> return null; let <b>Wait</b> continue waiting; but if a <b>Find...Return</b> value used previously, return that result.
		/// </summary>
		FindOther,

		/// <summary>
		/// Find more instances of current image. When used list of images, don't search for other images.
		/// If not found, let <b>Find</b> return null; let <b>Wait</b> continue waiting; but if a <b>Find...Return</b> value used previously, return that result.
		/// </summary>
		FindOtherOfThis,

		/// <summary>
		/// If used list of images, search for other images. Don't search for more instances of current image.
		/// If not found, let <b>Find</b> return null; let <b>Wait</b> continue waiting; but if a <b>Find...Return</b> value used previously, return that result.
		/// </summary>
		FindOtherOfList,
	}
}
