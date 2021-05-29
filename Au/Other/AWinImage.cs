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
using System.Drawing;
using System.Drawing.Imaging;
//using System.Linq;

using Au.Types;
using Au.Util;

//FUTURE: test OpenCV - an open source library for computer vision.

namespace Au
{
	/// <summary>
	/// Captures, finds and clicks images and colors in windows.
	/// </summary>
	/// <remarks>
	/// An image is any visible rectangular part of a window. A color is any visible pixel (the same as image of size 1x1).
	/// An <b>AWinImage</b> variable holds results of <see cref="Find"/> and similar functions (rectangle etc).
	/// </remarks>
	public partial class AWinImage
	{
		#region load, save

		/// <summary>
		/// Loads image from file, resource or string.
		/// </summary>
		/// <param name="image">See <see cref="WIImage"/>.</param>
		/// <exception cref="FileNotFoundException">Cannot find image file or resource.</exception>
		/// <exception cref="ArgumentException">Bad image or string format.</exception>
		/// <exception cref="Exception">Depending on <i>image</i> string format, exceptions of <see cref="Image.FromFile(string)"/>, <see cref="Bitmap(Stream)"/>, etc.</exception>
		/// <remarks>
		/// Calls <see cref="AImageUtil.LoadGdipBitmapFromFileOrResourceOrString"/>.
		/// </remarks>
		public static Bitmap LoadImage(string image)
			=> AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(image);

		#endregion

		#region results

		readonly WIArea _area;

		///// <summary>
		///// <i>area</i> parameter of the function.
		///// </summary>
		//public WIArea Area => _area;

		AWinImage(WIArea area)
		{
			_area = area;
		}

		AWinImage(AWinImage copy)
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

		/// <summary>
		/// Gets location of the found image, relative to the search area.
		/// </summary>
		/// <remarks>
		/// Relative to the window/control client area (if area type is AWnd), accessible object (if AAcc), image (if Bitmap) or screen (if RECT).
		/// More info: <see cref="Find"/>.
		/// </remarks>
		public RECT Rect { get; private set; }

		/// <summary>
		/// Gets location of the found image in screen coordinates.
		/// </summary>
		/// <remarks>
		/// Slower than <see cref="Rect"/>.
		/// </remarks>
		public RECT RectInScreen {
			get {
				RECT r;
				switch(_area.Type) {
				case WIArea.AreaType.Wnd:
					r = Rect;
					_area.W.MapClientToScreen(ref r);
					return r;
				case WIArea.AreaType.Acc:
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
		internal void MouseAction_(MButton button, Coord x, Coord y)
		{
			if(_area.Type == WIArea.AreaType.Bitmap) throw new InvalidOperationException();

			Debug.Assert(!Rect.NoArea);
			if(Rect.NoArea) return;

			//rejected: Click will activate it. Don't activate if just Move.
			//if(0 != (_f._flags & WIFlags.WindowDC)) {
			//	if(_area.W.IsCloaked) _area.W.ActivateL();
			//}

			var p = Coord.NormalizeInRect(x, y, Rect, centerIfEmpty: true);

			if(_area.Type == WIArea.AreaType.Screen) {
				if(button == 0) AMouse.Move(p);
				else AMouse.ClickEx(button, p);
			} else {
				var w = _area.W;
				if(_area.Type == WIArea.AreaType.Acc) {
					if(!_area.A.GetRect(out var r, w)) throw new AuException(0, "*get rectangle");
					p.x += r.left; p.y += r.top;
				}
				if(button == 0) AMouse.Move(w, p.x, p.y);
				else AMouse.ClickEx(button, w, p.x, p.y);
			}
		}

		/// <summary>
		/// Returns the same value if it is not null. Else throws <see cref="NotFoundException"/>.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = +AWnd.Find("Example");
		/// var wi = +AWinImage.Find(w, ...);
		/// ]]></code>
		/// </example>
		public static AWinImage operator +(AWinImage wi) => wi ?? throw new NotFoundException("Not found (AWinImage).");

		///
		public override string ToString() => $"{ListIndex.ToString()}, {MatchIndex.ToString()}, {Rect.ToString()}";

		#endregion

		/// <summary>
		/// Finds image(s) or color(s) displayed in a window or other area.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="AWinImage"/> object that contains the rectangle of the found image and can click it etc.
		/// Returns null if not found. See example.
		/// </returns>
		/// <param name="area">
		/// Where to search:
		/// - <see cref="AWnd"/> - window or control. The search area is its client area.
		/// - <see cref="AAcc"/> - accessible object.
		/// - <see cref="Bitmap"/> - another image.
		/// - <see cref="RECT"/> - a rectangle area in screen.
		/// - <see cref="WIArea"/> - can contain AWnd, AAcc or Bitmap. Also allows to specify a rectangle in it, which makes the search area smaller and the function faster. Example: <c>AWinImage.Find((w, (left, top, width, height)), "image.png");</c>.
		/// </param>
		/// <param name="image">Image or color to find. Or array of them. More info: <see cref="WIImage"/>.</param>
		/// <param name="flags"></param>
		/// <param name="colorDiff">Maximal allowed color difference. Use to to find images that have slightly different colors than the specified image. Can be 0 - 250, but should be as small as possible. Applied to each color component (red, green, blue) of each pixel.</param>
		/// <param name="also">
		/// Callback function. Called for each found image instance and receives its rectangle, match index and list index. Can return one of <see cref="WIAlso"/> values.
		/// 
		/// Examples:
		/// - Skip some matching images if some condition if false: <c>also: o => condition ? WIAlso.OkReturn : WIAlso.FindOther</c>
		/// - Skip n matching images: <c>also: o => o.Skip(n)</c>
		/// - Get rectangles etc of all matching images: <c>also: o => { list.Add(o); return false; }</c>. Don't use this code in 'wait' functions.
		/// - Get rectangles etc of all matching images and stop waiting: <c>also: o => { list.Add(o); o.Found = true; return false; }</c>
		/// - Do different actions depending on which list images found: <c>var found = new BitArray(images.Length); AWinImage.Find(w, images, also: o => { found[o.ListIndex] = true; return WIAlso.OkFindMoreOfList; }); if(found[0]) AOutput.Write(0); if(found[1]) AOutput.Write(1);</c>
		/// </param>
		/// <exception cref="AuWndException">Invalid window handle (the <i>area</i> argument).</exception>
		/// <exception cref="ArgumentException">An argument is/contains a null/invalid value.</exception>
		/// <exception cref="FileNotFoundException">Image file does not exist.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="LoadImage"/>.</exception>
		/// <exception cref="AuException">Something failed.</exception>
		/// <remarks>
		/// To create code for this function, use dialog "Find image or color in window".
		/// 
		/// The speed mostly depends on:
		/// 1. The size of the search area. Use the smallest possible area (control or accessible object or rectangle in window like <c>(w, rectangle)</c>).
		/// 2. Flags <see cref="WIFlags.WindowDC"/> (makes faster), <see cref="WIFlags.PrintWindow"/>. The speed depends on window.
		/// 3. Video driver. Can be much slower if incorrect, generic or virtual PC driver is used. The above flags should help.
		/// 4. <i>colorDiff</i>. Should be as small as possible.
		/// 
		/// If flag <see cref="WIFlags.WindowDC"/> or <see cref="WIFlags.PrintWindow"/> not used, the search area must be visible on the screen, because this function then gets pixels from the screen.
		/// 
		/// Can find only images that exactly match the specified image. With <i>colorDiff</i> can find images with slightly different colors and brightness. Cannot find images with different shapes.
		/// 
		/// Transparent and partially transparent pixels are ignored. For example, when you capture a non-rectangular area image, the image actually is rectangular, but pixels outside of its captured area are transparent and therefore not compared. Also you can draw transparent areas with an image editor that supports it, for example Paint.NET.
		/// 
		/// This function is not the best way to find objects when the script is intended for long use or for use on multiple computers or must be very reliable. Because it may fail to find the image after are changed some settings - system theme, application theme, text size (DPI), font smoothing (if the image contains text), etc. Also are possible various unexpected temporary conditions that may distort or hide the image, for example adjacent window shadow, a tooltip or some temporary window. If possible, in such scripts instead use other functions, eg find control or accessible object.
		/// 
		/// Flags <see cref="WIFlags.WindowDC"/> and <see cref="WIFlags.PrintWindow"/> cannot be used if <i>area</i> is <b>Bitmap</b> or <b>RECT</b>.
		/// </remarks>
		/// <example>
		/// Code created with dialog "Find image or color in window".
		/// <code><![CDATA[
		/// var w = +AWnd.Find("Window Name");
		/// string image = "image:iVBORw0KGgoAAAANSUhEUgAAABYAAAANCAYAAACtpZ5jAAAAAXNSR0IArs4c...";
		/// var wi = +AWinImage.Find(w, image);
		/// wi.Click();
		/// ]]></code>
		/// </example>
		public static AWinImage Find(WIArea area, WIImage image, WIFlags flags = 0, int colorDiff = 0, Func<AWinImage, WIAlso> also = null)
		{
			var f = new Finder(image, flags, colorDiff, also);
			if(!f.Find(area)) return null;
			return f.Result;
		}

		internal enum _Action { Find, Wait, WaitNot, WaitChanged }

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Finds image(s) or color(s) displayed in a window or other area. Waits until found.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <returns>Returns <see cref="AWinImage"/> object containing the rectangle of the found image. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		public static AWinImage Wait(double secondsTimeout, WIArea area, WIImage image, WIFlags flags = 0, int colorDiff = 0, Func<AWinImage, WIAlso> also = null)
		{
			var f = new Finder(image, flags, colorDiff, also);
			return f.Wait_(_Action.Wait, secondsTimeout, area) ? f.Result : null;
		}

		/// <summary>
		/// Waits until image(s) or color(s) is not displayed in a window or other area.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		public static bool WaitNot(double secondsTimeout, WIArea area, WIImage image, WIFlags flags = 0, int colorDiff = 0, Func<AWinImage, WIAlso> also = null)
		{
			var f = new Finder(image, flags, colorDiff, also);
			return f.Wait_(_Action.WaitNot, secondsTimeout, area);
		}

		/// <summary>
		/// Waits until something visually changes in a window or other area.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// The same as <see cref="WaitNot"/>, but instead of <i>image</i> parameter this function captures the area image at the beginning.
		/// </remarks>
		public static bool WaitChanged(double secondsTimeout, WIArea area, WIFlags flags = 0, int colorDiff = 0)
		{
			var f = new Finder(default, flags, colorDiff, null);
			return f.Wait_(_Action.WaitChanged, secondsTimeout, area);
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		/// <summary>
		/// Contains data and parameters of image(s) or color(s) to find. Finds them in a window or other area.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="AWinImage.Find"/>.
		/// </remarks>
		public unsafe class Finder
		{
			class _Image
			{
				public uint[] pixels;
				public int width, height;
				public _OptimizationData opt;

				public _Image(string file)
				{
					using var b = AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(file);
					_BitmapToData(b);
				}

				public _Image(Bitmap b)
				{
					b = b ?? throw new ArgumentException("null Bitmap");
					_BitmapToData(b);
				}

				void _BitmapToData(Bitmap b)
				{
					var z = b.Size;
					width = z.Width; height = z.Height;
					pixels = new uint[width * height];
					fixed(uint* p = pixels) {
						var d = new BitmapData { Scan0 = (IntPtr)p, Height = height, Width = width, Stride = width * 4, PixelFormat = PixelFormat.Format32bppArgb };
						d = b.LockBits(new Rectangle(default, z), ImageLockMode.ReadOnly | ImageLockMode.UserInputBuffer, PixelFormat.Format32bppArgb, d);
						b.UnlockBits(d);
						if(d.Stride < 0) throw new ArgumentException("bottom-up Bitmap"); //Image.FromHbitmap used to create bottom-up bitmap (stride<0) from compatible bitmap. Now cannot reproduce.
					}
				}

				public _Image(ColorInt color)
				{
					width = height = 1;
					pixels = new uint[1] { (uint)color.argb | 0xff000000 };
				}

				public _Image() { }
			}

			//large unmanaged memory etc reused while waiting, to make slightly faster
			internal unsafe struct _AreaData
			{
				public AMemoryBitmap mb;
				public int width, height, memSize;
				public uint* pixels;

				public void Free()
				{
					if(mb != null) { //null if WIArea.AreaType.Bitmap
						mb.Dispose();
						AVirtualMemory.Free(pixels);
					}
				}
			}

			//ctor parameters
			readonly List<_Image> _images; //support multiple images
			readonly WIFlags _flags;
			readonly uint _colorDiff;
			readonly Func<AWinImage, WIAlso> _also;

			_Action _action;
			WIArea _area;
			_AreaData _ad;
			POINT _resultOffset; //to map the found rectangle from the captured area coordinates to the specified area coordinates

			/// <summary>
			/// Returns <see cref="AWinImage"/> object that contains the rectangle of the found image and can click it etc.
			/// </summary>
			public AWinImage Result { get; private set; }

			/// <summary>
			/// Stores image/color data and search settings in this object. Loads images if need. See <see cref="AWinImage.Find"/>.
			/// </summary>
			/// <exception cref="ArgumentException">An argument is/contains a null/invalid value.</exception>
			/// <exception cref="FileNotFoundException">Image file does not exist.</exception>
			/// <exception cref="Exception">Exceptions of <see cref="LoadImage"/>.</exception>
			public Finder(WIImage image, WIFlags flags = 0, int colorDiff = 0, Func<AWinImage, WIAlso> also = null)
			{
				_flags = flags;
				_colorDiff = (uint)colorDiff; if(_colorDiff > 250) throw new ArgumentOutOfRangeException("colorDiff range: 0 - 250");
				_also = also;

				_images = new List<_Image>();
				if(image.Value != null) _AddImage(image);

				void _AddImage(WIImage image)
				{
					switch(image.Value) {
					case string s:
						_images.Add(new _Image(s));
						break;
					case Bitmap b:
						_images.Add(new _Image(b));
						break;
					case ColorInt c:
						_images.Add(new _Image(c));
						break;
					case WIImage[] a:
						foreach(var v in a) _AddImage(v);
						break;
					case null: throw new ArgumentNullException();
					}
				}
			}

			/// <summary>
			/// Finds the image displayed in the specified window or other area. See <see cref="AWinImage.Find"/>.
			/// </summary>
			/// <returns>Returns true if found. Then use <see cref="Result"/>.</returns>
			/// <param name="area">See <see cref="AWinImage.Find"/>.</param>
			/// <exception cref="AuWndException">Invalid window handle.</exception>
			/// <exception cref="ArgumentException">An argument of this function or of constructor is invalid.</exception>
			/// <exception cref="AuException">Something failed.</exception>
			public bool Find(WIArea area)
			{
				_Before(area, _Action.Find);
				return Find_();
			}

			/// <summary>
			/// See <see cref="AWinImage.Wait"/>.
			/// </summary>
			/// <exception cref="Exception">Exceptions of <see cref="AWinImage.Wait"/>, except those of the constructor.</exception>
			public bool Wait(double secondsTimeout, WIArea area)
				=> Wait_(_Action.Wait, secondsTimeout, area);

			/// <summary>
			/// See <see cref="AWinImage.WaitNot"/>.
			/// </summary>
			/// <exception cref="Exception">Exceptions of <see cref="AWinImage.WaitNot"/>, except those of the constructor.</exception>
			public bool WaitNot(double secondsTimeout, WIArea area)
				=> Wait_(_Action.WaitNot, secondsTimeout, area);

			internal bool Wait_(_Action action, double secondsTimeout, WIArea area)
			{
				_Before(area, action);
				try { return AWaitFor.Condition(secondsTimeout, () => Find_() ^ (action > _Action.Wait)); }
				finally { _After(); }

				//tested: does not create garbage while waiting.
			}

			//called at the start of Find_ and Wait_
			void _Before(WIArea area, _Action action)
			{
				_area = area ?? throw new ArgumentNullException();
				_action = action;

				if(_action == _Action.WaitChanged) {
					Debug.Assert(_images.Count == 0 && _also == null); //the first Find_ will capture the area and add to _images
				} else {
					if(_images.Count == 0) throw new ArgumentException("no image");
				}

				WIFlags badFlags = 0;
				switch(_area.Type) {
				case WIArea.AreaType.Screen:
					badFlags = WIFlags.WindowDC | WIFlags.PrintWindow;
					break;
				case WIArea.AreaType.Wnd:
					_area.W.ThrowIfInvalid();
					break;
				case WIArea.AreaType.Acc:
					if(_area.A == null) throw new ArgumentNullException(nameof(area));
					_area.W = _area.A.WndContainer;
					goto case WIArea.AreaType.Wnd;
				case WIArea.AreaType.Bitmap:
					if(_action != _Action.Find) throw new ArgumentException("wait", "image");
					if(_area.B == null) throw new ArgumentNullException(nameof(area));
					badFlags = WIFlags.WindowDC | WIFlags.PrintWindow;
					break;
				}
				if(_flags.HasAny(badFlags)) throw new ArgumentException("Invalid flags for this area type: " + badFlags);

				Result = new AWinImage(_area);
			}

			//called at the end of Find_ (if not waiting) and Wait_
			void _After()
			{
				_ad.Free(); _ad = default;
				_area = null;
				if(_action == _Action.WaitChanged) _images.Clear();
			}

			internal bool Find_()
			{
				//using var p1 = APerf.Create();
				Result._Clear();

				bool inScreen = !_flags.HasAny(WIFlags.WindowDC | WIFlags.PrintWindow);
				bool failedGetRect = false;

				var w = _area.W;
				if(!w.Is0) {
					if(!w.IsVisible || w.IsMinimized) return false;
					if(inScreen && w.IsCloaked) return false;
				}

				//Get area rectangle.
				RECT r;
				_resultOffset = default;
				switch(_area.Type) {
				case WIArea.AreaType.Wnd:
					failedGetRect = !w.GetClientRect(out r, inScreen);
					break;
				case WIArea.AreaType.Acc:
					failedGetRect = !(inScreen ? _area.A.GetRect(out r) : _area.A.GetRect(out r, w));
					break;
				case WIArea.AreaType.Bitmap:
					r = new RECT(0, 0, _area.B.Width, _area.B.Height);
					break;
				default: //Screen
					r = _area.R;
					if(!AScreen.IsInAnyScreen(r)) r = default;
					_area.HasRect = false;
					_resultOffset.x = r.left; _resultOffset.y = r.top;
					break;
				}
				if(failedGetRect) {
					w.ThrowIfInvalid();
					throw new AuException("*get rectangle");
				}
				//FUTURE: DPI

				//r is the area from where to get pixels. If !inScreen, it is relative to the client area.
				//Intermediate results will be relative to r. Then will be added _resultOffset if a limiting rectangle is used.

				if(_area.HasRect) {
					var rr = _area.R;
					_resultOffset.x = rr.left; _resultOffset.y = rr.top;
					rr.Offset(r.left, r.top);
					r.Intersect(rr);
				}

				if(_area.Type == WIArea.AreaType.Acc) {
					//adjust r and _resultOffset,
					//	because object rectangle may be bigger than client area (eg WINDOW object)
					//	or its part is not in client area (eg scrolled web page).
					//	If not adjusted, then may capture part of parent or sibling controls or even other windows...
					//	Never mind: should also adjust control rectangle in ancestors in the same way.
					//		This is not so important because usually whole control is visible (resized, not clipped).
					int x = r.left, y = r.top;
					w.GetClientRect(out var rw, inScreen);
					r.Intersect(rw);
					x -= r.left; y -= r.top;
					_resultOffset.x -= x; _resultOffset.y -= y;
				}
				if(r.NoArea) return false; //never mind: if WaitChanged and this is the first time, immediately returns 'changed'

				//If WaitChanged, first time just get area pixels into _images[0].
				if(_action == _Action.WaitChanged && _images.Count == 0) {
					_GetAreaPixels(r, true);
					return true;
				}

				//Return false if all images are bigger than the search area.
				for(int i = _images.Count; --i >= 0;) {
					var v = _images[i];
					if(v.width <= r.Width && v.height <= r.Height) goto g1;
				}
				return false; g1:

				BitmapData bitmapBD = null;
				try {
					//Get area pixels.
					if(_area.Type == WIArea.AreaType.Bitmap) {
						var pf = (_area.B.PixelFormat == PixelFormat.Format32bppArgb) ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb; //if possible, use PixelFormat of _area, to avoid conversion/copying. Both these formats are ok, we don't use alpha.
						bitmapBD = _area.B.LockBits(r, ImageLockMode.ReadOnly, pf);
						if(bitmapBD.Stride < 0) throw new ArgumentException("bottom-up Bitmap");
						_ad.pixels = (uint*)bitmapBD.Scan0;
						_ad.width = bitmapBD.Width; _ad.height = bitmapBD.Height;
					} else {
						_GetAreaPixels(r);
					}
					//p1.Next();

					//Find image(s) in area.
					AWinImage alsoResult = null;
					for(int i = 0, n = _images.Count; i < n; i++) {
						Result.ListIndex = i;
						Result.MatchIndex = 0;
						if(_FindImage(_images[i], out var alsoAction, ref alsoResult)) return true;
						if(alsoAction == WIAlso.NotFound || alsoAction == WIAlso.FindOtherOfThis || alsoAction == WIAlso.OkFindMoreOfThis) break;
					}

					if(alsoResult != null) {
						Result = alsoResult;
						return true;
					}
					Result._Clear();
					return false;
				}
				finally {
					if(bitmapBD != null) _area.B.UnlockBits(bitmapBD);
					if(_action == _Action.Find) _After();
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveOptimization)]
			bool _FindImage(_Image image, out WIAlso alsoAction, ref AWinImage alsoResult)
			{
				alsoAction = WIAlso.FindOtherOfList;

				int imageWidth = image.width, imageHeight = image.height;
				if(_ad.width < imageWidth || _ad.height < imageHeight) return false;
				fixed(uint* imagePixels = image.pixels) {
					uint* imagePixelsTo = imagePixels + imageWidth * imageHeight;
					uint* areaPixels = _ad.pixels;

					//rejected. Does not make faster, just adds more code.
					//if image is of same size as area, simply compare. For example, when action is WaitChanged.
					//if(imageWidth == _areaWidth && imageHeight == _areaHeight) {
					//	//AOutput.Write("same size");
					//	if(_skip > 0) return false;
					//	if(!_CompareSameSize(areaPixels, imagePixels, imagePixelsTo, _colorDiff)) return false;
					//	_tempResults ??= new List<RECT>();
					//	_tempResults.Add(new RECT(0, 0, imageWidth, imageHeight));
					//	return true;
					//}
					//else if(imagePixelCount == 1) { ... } //eg when image is color

					if(!image.opt.Init(image, _ad.width)) return false;
					var opt = image.opt; //copy struct, size = 9*int
					int o_pos0 = opt.v0.pos;
					var o_a1 = &opt.v1; var o_an = o_a1 + (opt.N - 1);

					//find first pixel. This part is very important for speed.
					//int nTimesFound = 0; //debug

					var areaWidthMinusImage = _ad.width - imageWidth;
					var pFirst = areaPixels + o_pos0;
					var pLast = pFirst + _ad.width * (_ad.height - imageHeight) + areaWidthMinusImage;

					//this is a workaround for compiler not using registers for variables in fast loops (part 1)
					var f = new _FindData {
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
					//APerf.First();
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
					//APerf.NW();
					//AOutput.Write(nTimesFound);

					#endregion

					if(_action != _Action.WaitChanged) {
						int iFound = (int)(f.p - o_pos0 - areaPixels);
						var r = new RECT(iFound % _ad.width, iFound / _ad.width, imageWidth, imageHeight);
						r.Offset(_resultOffset.x, _resultOffset.y);
						Result.Rect = r;

						if(_also != null) {
							var wi = new AWinImage(Result); //create new AWinImage object because the callback may add it to a list etc
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
				} //fixed

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

			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
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
				internal struct POSCOLOR
				{
					public int pos; //the position in area (not in image) from which to start searching. Depends on where in the image is the color.
					public uint color;
				};

#pragma warning disable 649 //never assigned
				public POSCOLOR v0, v1, v2, v3; //POSCOLOR[] would be slower
#pragma warning restore 649
				public int N; //A valid count

				public bool Init(_Image image, int areaWidth)
				{
					if(N != 0) return N > 0;

					int imageWidth = image.width, imageHeight = image.height;
					int imagePixelCount = imageWidth * imageHeight;
					var imagePixels = image.pixels;
					int i;

#if WI_TEST_NO_OPTIMIZATION
					_Add(image, 0, areaWidth);
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

					_Add(image, i, areaWidth);
					if(!singleColor) {
						//find second different pixel
						int i0 = i;
						i = _FindDifferentPixel(i);
						if(i >= 0) {
							_Add(image, i, areaWidth);
							//find other different pixels
							fixed(POSCOLOR* p = &v0) {
								while(N < 4) {
									for(++i; i < imagePixelCount; i++) {
										var c = imagePixels[i];
										if(_IsTransparent(c)) continue;
										int j = N - 1;
										for(; j >= 0; j--) if(c == p[j].color) break; //find new color
										if(j < 0) break; //found
									}
									if(i >= imagePixelCount) break;
									_Add(image, i, areaWidth);
								}
							}
						} else {
							for(i = imagePixelCount - 1; i > i0; i--) if(!_IsTransparent(imagePixels[i])) break;
							_Add(image, i, areaWidth);
						}
					}

					//fixed (POSCOLOR* o_pc = &v0) for(int j = 0; j < N; j++) AOutput.Write($"{o_pc[j].pos} 0x{o_pc[j].color:X}");
#endif
					return true;

					int _FindDifferentPixel(int iCurrent)
					{
						int m = iCurrent, n = imagePixelCount;
						uint notColor = imagePixels[m++];
						for(; m < n; m++) {
							var c = imagePixels[m];
							if(c == notColor || _IsTransparent(c)) continue;
							return m;
						}
						return -1;
					}
				}

				void _Add(_Image image, int i, int areaWidth)
				{
					fixed(POSCOLOR* p0 = &v0) {
						var p = p0 + N++;
						p->color = image.pixels[i];
						int w = image.width, x = i % w, y = i / w;
						p->pos = y * areaWidth + x;
					}
				}
			}

			void _GetAreaPixels(RECT r, bool toImage0 = false)
			{
				//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.
				//This is the slowest part of Find, especially BitBlt.
				//Speed depends on computer, driver, OS version, theme, size.
				//For example, with Aero theme 2-15 times slower (on Windows 8/10 cannot disable Aero).
				//With incorrect/generic video driver can be 10 times slower. Eg on vmware virtual PC.
				//Much faster when using window DC. Then same speed as without Aero.

				int areaWidth = r.Width, areaHeight = r.Height;
				//_Debug("start", 1);
				//create memory bitmap. When waiting, we reuse _ad.mb, it makes slightly faster.
				if(_ad.mb == null || areaWidth != _ad.width || areaHeight != _ad.height) {
					if(_ad.mb != null) { _ad.mb.Dispose(); _ad.mb = null; }
					_ad.mb = new AMemoryBitmap(_ad.width = areaWidth, _ad.height = areaHeight);
					//_Debug("created MemBmp");
				}

				//copy from screen/window to memory bitmap
				if(0 != (_flags & WIFlags.PrintWindow) && Api.PrintWindow(_area.W, _ad.mb.Hdc, Api.PW_CLIENTONLY | (AVersion.MinWin8_1 ? Api.PW_RENDERFULLCONTENT : 0))) {
					//PW_RENDERFULLCONTENT is new in Win8.1. Undocumented in MSDN, but defined in h. Then can capture windows like Chrome, winstore.
					//AOutput.Write("PrintWindow OK");
				} else {
					//get DC of screen or window
					bool windowDC = 0 != (_flags & WIFlags.WindowDC);
					AWnd w = windowDC ? _area.W : default;
					using var dc = new WindowDC_(w);
					if(dc.Is0) w.ThrowNoNative("Failed");
					//_Debug("get DC");
					//copy from screen/window DC to memory bitmap
					uint rop = windowDC ? Api.SRCCOPY : Api.SRCCOPY | Api.CAPTUREBLT;
					bool bbOK = Api.BitBlt(_ad.mb.Hdc, 0, 0, areaWidth, areaHeight, dc, r.left, r.top, rop);
					if(!bbOK) throw new AuException("BitBlt"); //the API fails only if a HDC is invalid
				}

				//_Debug("captured to MemBmp");
				//get pixels
				var h = new Api.BITMAPINFOHEADER {
					biSize = sizeof(Api.BITMAPINFOHEADER),
					biWidth = areaWidth, biHeight = -areaHeight,
					biPlanes = 1, biBitCount = 32,
					//biCompression = 0, //BI_RGB
				};
				int memSize = areaWidth * areaHeight * 4; //7.5 MB for a max window in 1920*1080 monitor
				if(toImage0) {
					var im = new _Image { width = areaWidth, height = areaHeight, pixels = new uint[areaWidth * areaHeight] };
					fixed(uint* p = im.pixels) _GetBits(p);
					_images.Add(im);
				} else {
					if(memSize > _ad.memSize) { //while waiting, we reuse the memory, it makes slightly faster.
						AVirtualMemory.Free(_ad.pixels);
						_ad.pixels = (uint*)AVirtualMemory.Alloc(memSize);
						_ad.memSize = memSize;
					}
					_GetBits(_ad.pixels);
				}
				//_Debug("_GetBitmapBits", 3);

				void _GetBits(uint* pixels)
				{
					var h = new Api.BITMAPINFOHEADER {
						biSize = sizeof(Api.BITMAPINFOHEADER),
						biWidth = areaWidth, biHeight = -areaHeight,
						biPlanes = 1, biBitCount = 32,
						//biCompression = 0, //BI_RGB
					};
					if(areaHeight != Api.GetDIBits(_ad.mb.Hdc, _ad.mb.Hbitmap, 0, areaHeight, pixels, &h, 0)) //DIB_RGB_COLORS
						throw new AuException("GetDIBits");

					//remove alpha. Don't need for area.
					if(toImage0) {
						byte* b = (byte*)pixels, be = b + memSize;
						for(b += 3; b < be; b += 4) *b = 0xff;
					}

					//var testFile = AFolders.Temp + @"AWinImage\" + s_test++ + ".png";
					//AFile.CreateDirectoryFor(testFile);
					//using(var areaBmp = new Bitmap(areaWidth, areaHeight, areaWidth * 4, PixelFormat.Format32bppRgb, (IntPtr)p)) {
					//	areaBmp.Save(testFile);
					//}
					////ARun.Run(testFile);
				}
			}
		}
		//static int s_test;

		//[Conditional("WI_DEBUG_PERF")]
		//static void _Debug(string s, int perfAction = 2)
		//{
		//	//MessageBox.Show(s);
		//	switch(perfAction) {
		//	case 1: APerf.First(); break;
		//	case 2: APerf.Next(); break;
		//	case 3: APerf.NW(); break;
		//	}
		//}
	}
}

namespace Au.Types
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>
	/// Defines the search area for <see cref="AWinImage.Find"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// It can be a window/control, accessible object, another image or a rectangle in screen.
	/// Also allows to specify a rectangle in it, which makes the search area smaller and the function faster.
	/// Has implicit conversions from AWnd, AAcc, Bitmap, RECT (rectangle in screen), tuple (AWnd, RECT), tuple (Acc, RECT).
	/// Example: <c>AWinImage.Find((w, (left, top, width, height)), image);</c>.
	/// </remarks>
	public class WIArea
	{
		internal enum AreaType : byte { Screen, Wnd, Acc, Bitmap }

		internal readonly AreaType Type;
		internal bool HasRect;
		internal AWnd W;
		internal AAcc A;
		internal Bitmap B;
		internal RECT R;

		WIArea(AreaType t) { Type = t; }

		public static implicit operator WIArea(AWnd w) => new WIArea(AreaType.Wnd) { W = w };
		public static implicit operator WIArea(AAcc a) => new WIArea(AreaType.Acc) { A = a };
		public static implicit operator WIArea(Bitmap b) => new WIArea(AreaType.Bitmap) { B = b };
		public static implicit operator WIArea(RECT r) => new WIArea(AreaType.Screen) { R = r };
		public static implicit operator WIArea((AWnd w, RECT r) t) => new WIArea(AreaType.Wnd) { W = t.w, R = t.r, HasRect = true };
		public static implicit operator WIArea((AAcc a, RECT r) t) => new WIArea(AreaType.Acc) { A = t.a, R = t.r, HasRect = true };
		public static implicit operator WIArea((Bitmap b, RECT r) t) => new WIArea(AreaType.Bitmap) { B = t.b, R = t.r, HasRect = true };
	}

	/// <summary>
	/// Image(s) or color(s) for <see cref="AWinImage.Find"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// Has implicit conversions from:
	/// - string - path of .png or .bmp file. If not full path, uses <see cref="AFolders.ThisAppImages"/>.
	/// - string that starts with "resources/" or has prefix <c>"resource:"</c> - resource name; see <see cref="AResources.GetGdipBitmap"/>.
	/// - string with prefix <c>"image:"</c> - Base64-encoded .png image embedded in script text.
	/// <br/>Can be created with dialog "Find image or color in window" or with function <b>Au.Controls.ImageUtil.ImageToString</b> (in Au.Controls.dll).
	/// - <see cref="ColorInt"/>, <b>int</b> in 0xRRGGBB color format, <b>Color</b> - color. Alpha isn't used.
	/// - <see cref="Bitmap"/> - image object.
	/// - <b>WIImage[]</b> - multiple images or/and colors. Action - find any. To create a different action can be used callback function (parameter <i>also</i>).
	/// 
	/// Icons are not supported directly; you can use <see cref="AIcon"/> to get icon and convert to bitmap.
	/// </remarks>
	public struct WIImage
	{
		readonly object _o;
		WIImage(object o) { _o = o; }

		public static implicit operator WIImage(string pathEtc) => new WIImage(pathEtc);
		public static implicit operator WIImage(Bitmap image) => new WIImage(image);
		public static implicit operator WIImage(ColorInt color) => new WIImage(color);
		public static implicit operator WIImage(int color) => new WIImage((ColorInt)color);
		public static implicit operator WIImage(Color color) => new WIImage((ColorInt)color);
		public static implicit operator WIImage(System.Windows.Media.Color color) => new WIImage((ColorInt)color);
		//public static implicit operator WIImage(IEnumerable<WIImage> list) => new WIImage(list); //error: cannot convert from interfaces
		public static implicit operator WIImage(WIImage[] list) => new WIImage(list);
		//public static implicit operator WIImage(List<WIImage> list) => new WIImage(list); //rare, can use ToArray()

		/// <summary>
		/// Gets the raw value stored in this variable. Can be string, Bitmap, ColorInt, WIImage[], null.
		/// </summary>
		public object Value => _o;
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// Flags for <see cref="AWinImage.Find"/> and similar functions.
	/// </summary>
	[Flags]
	public enum WIFlags
	{
		/// <summary>
		/// Get pixels from the device context (DC) of the window client area, not from screen DC. Usually much faster.
		/// Can get pixels from window parts that are covered by other windows or offscreen. But not from hidden and minimized windows.
		/// Does not work on Windows 7 if Aero theme is turned off. Then this flag is ignored.
		/// Cannot find images in some windows (including Windows Store apps), and in some window parts (glass). All pixels captured from these windows/parts are black.
		/// If the window is DPI-scaled, the image must be captured from its non-scaled version.
		/// </summary>
		WindowDC = 1,

		/// <summary>
		/// Use API <msdn>PrintWindow</msdn> to get window pixels.
		/// Like <b>WindowDC</b>, works with background windows, etc. Differences:
		/// - On Windows 8.1 and later works with all windows and all window parts.
		/// - Works without Aero theme too.
		/// - Slower than with WindowDC, although usually faster than without these flags.
		/// - Some windows may flicker.
		/// - Does not work with windows of higher UAC integrity level. Then this flag is ignored.
		/// </summary>
		PrintWindow = 2,

		//rejected: this was used in QM2. Now can use png alpha instead, and CaptureUI allows to capture it.
		///// <summary>
		///// Use the top-left pixel color of the image as transparent color (don't compare pixels that have this color).
		///// </summary>
		//MakeTransparent = ,
	}

	/// <summary>
	/// Used with <see cref="AWinImage.Find"/> and <see cref="AWinImage.Wait"/>. Its callback function (parameter <i>also</i>) can return one of these values.
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
