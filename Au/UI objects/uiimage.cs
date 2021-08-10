using System.Drawing;

//FUTURE: test OpenCV - an open source library for computer vision.

namespace Au
{
	/// <summary>
	/// Captures, finds and clicks images and colors in windows.
	/// </summary>
	/// <remarks>
	/// An image is any visible rectangular part of a window. A color is any visible pixel (the same as image of size 1x1).
	/// A <b>uiimage</b> variable holds results of <see cref="find"/> and similar functions (rectangle etc).
	/// </remarks>
	public partial class uiimage
	{
		#region results

		readonly IFArea _area;

		///// <summary>
		///// <i>area</i> parameter of the function.
		///// </summary>
		//public IFArea Area => _area;

		internal uiimage(IFArea area) {
			_area = area;
		}

		internal uiimage(uiimage copy) {
			_area = copy._area;
			Rect = copy.Rect;
			MatchIndex = copy.MatchIndex;
			ListIndex = copy.ListIndex;
		}

		internal void Clear_() {
			Rect = default;
			ListIndex = 0;
			MatchIndex = 0;
		}

		/// <summary>
		/// Gets location of the found image, relative to the search area.
		/// </summary>
		/// <remarks>
		/// Relative to the window/control client area (if area type is <b>wnd</b>), UI element (if <b>elm</b>), image (if <b>Bitmap</b>) or screen (if <b>RECT</b>).
		/// More info: <see cref="find"/>.
		/// </remarks>
		public RECT Rect { get; internal set; }

		/// <summary>
		/// Gets location of the found image in screen coordinates.
		/// </summary>
		/// <remarks>
		/// Slower than <see cref="Rect"/>.
		/// </remarks>
		public RECT RectInScreen {
			get {
				RECT r;
				switch (_area.Type) {
				case IFArea.AreaType.Wnd:
					r = Rect;
					_area.W.MapClientToScreen(ref r);
					return r;
				case IFArea.AreaType.Elm:
					if (!_area.A.GetRect(out var rr)) return default;
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
		public int MatchIndex { get; internal set; }

		/// <summary>
		/// When the <i>image</i> argument is a list of images, gets 0-based index of the list image.
		/// </summary>
		public int ListIndex { get; internal set; }

		/// <summary>
		/// Can be used in <i>also</i> callback function to skip n matching images. Example: <c>also: o => o.Skip(n)</c>.
		/// </summary>
		/// <param name="n">How many matching images to skip.</param>
		public IFAlso Skip(int n) => MatchIndex == n ? IFAlso.OkReturn : (MatchIndex < n ? IFAlso.FindOther : IFAlso.FindOtherOfList);

		//Called by extension methods.
		internal void MouseAction_(MButton button, Coord x, Coord y) {
			if (_area.Type == IFArea.AreaType.Bitmap) throw new InvalidOperationException();

			Debug.Assert(!Rect.NoArea);
			if (Rect.NoArea) return;

			//rejected: Click will activate it. Don't activate if just Move.
			//if(0 != (_f._flags & IFFlags.WindowDC)) {
			//	if(_area.W.IsCloaked) _area.W.ActivateL();
			//}

			var p = Coord.NormalizeInRect(x, y, Rect, centerIfEmpty: true);

			if (_area.Type == IFArea.AreaType.Screen) {
				if (button == 0) mouse.move(p);
				else mouse.clickEx(button, p);
			} else {
				var w = _area.W;
				if (_area.Type == IFArea.AreaType.Elm) {
					if (!_area.A.GetRect(out var r, w)) throw new AuException(0, "*get rectangle");
					p.x += r.left; p.y += r.top;
				}
				if (button == 0) mouse.move(w, p.x, p.y);
				else mouse.clickEx(button, w, p.x, p.y);
			}
		}

		/// <summary>
		/// Returns the same value if it is not null. Else throws <see cref="NotFoundException"/>.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = +wnd.find("Example");
		/// var wi = +uiimage.find(w, ...);
		/// ]]></code>
		/// </example>
		public static uiimage operator +(uiimage wi) => wi ?? throw new NotFoundException("Not found (uiimage).");

		///
		public override string ToString() => $"{ListIndex.ToString()}, {MatchIndex.ToString()}, {Rect.ToString()}";

		#endregion

		/// <summary>
		/// Finds image(s) or color(s) displayed in a window or other area.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="uiimage"/> object that contains the rectangle of the found image and can click it etc.
		/// Returns null if not found. See example.
		/// </returns>
		/// <param name="area">
		/// Where to search:
		/// - <see cref="wnd"/> - window or control. The search area is its client area.
		/// - <see cref="elm"/> - UI element.
		/// - <see cref="Bitmap"/> - another image.
		/// - <see cref="RECT"/> - a rectangle area in screen.
		/// - <see cref="IFArea"/> - can contain wnd, elm or Bitmap. Also allows to specify a rectangle in it, which makes the search area smaller and the function faster. Example: <c>uiimage.find((w, (left, top, width, height)), "image.png");</c>.
		/// </param>
		/// <param name="image">Image or color to find. Or array of them. More info: <see cref="IFImage"/>.</param>
		/// <param name="flags"></param>
		/// <param name="colorDiff">Maximal allowed color difference. Use to to find images that have slightly different colors than the specified image. Can be 0 - 250, but should be as small as possible. Applied to each color component (red, green, blue) of each pixel.</param>
		/// <param name="also">
		/// Callback function. Called for each found image instance and receives its rectangle, match index and list index. Can return one of <see cref="IFAlso"/> values.
		/// 
		/// Examples:
		/// - Skip some matching images if some condition if false: <c>also: o => condition ? IFAlso.OkReturn : IFAlso.FindOther</c>
		/// - Skip n matching images: <c>also: o => o.Skip(n)</c>
		/// - Get rectangles etc of all matching images: <c>also: o => { list.Add(o); return false; }</c>. Don't use this code in 'wait' functions.
		/// - Get rectangles etc of all matching images and stop waiting: <c>also: o => { list.Add(o); o.Found = true; return false; }</c>
		/// - Do different actions depending on which list images found: <c>var found = new BitArray(images.Length); uiimage.find(w, images, also: o => { found[o.ListIndex] = true; return IFAlso.OkFindMoreOfList; }); if(found[0]) print.it(0); if(found[1]) print.it(1);</c>
		/// </param>
		/// <exception cref="AuWndException">Invalid window handle (the <i>area</i> argument).</exception>
		/// <exception cref="ArgumentException">An argument is/contains a null/invalid value.</exception>
		/// <exception cref="FileNotFoundException">Image file does not exist.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="ImageUtil.LoadGdipBitmap"/>.</exception>
		/// <exception cref="AuException">Something failed.</exception>
		/// <remarks>
		/// To create code for this function, use dialog "Find image or color in window".
		/// 
		/// The speed mostly depends on:
		/// 1. The size of the search area. Use the smallest possible area (control or UI element or rectangle in window like <c>(w, rectangle)</c>).
		/// 2. Flags <see cref="IFFlags.WindowDC"/> (makes faster), <see cref="IFFlags.PrintWindow"/>. The speed depends on window.
		/// 3. Video driver. Can be much slower if incorrect, generic or virtual PC driver is used. The above flags should help.
		/// 4. <i>colorDiff</i>. Should be as small as possible.
		/// 
		/// If flag <see cref="IFFlags.WindowDC"/> or <see cref="IFFlags.PrintWindow"/> not used, the search area must be visible on the screen, because this function then gets pixels from the screen.
		/// 
		/// Can find only images that exactly match the specified image. With <i>colorDiff</i> can find images with slightly different colors and brightness. Cannot find images with different shapes.
		/// 
		/// Transparent and partially transparent pixels are ignored. For example, when you capture a non-rectangular area image, the image actually is rectangular, but pixels outside of its captured area are transparent and therefore not compared. Also you can draw transparent areas with an image editor that supports it, for example Paint.NET.
		/// 
		/// This function is not the best way to find objects when the script is intended for long use or for use on multiple computers or must be very reliable. Because it may fail to find the image after are changed some settings - system theme, application theme, text size (DPI), font smoothing (if the image contains text), etc. Also are possible various unexpected temporary conditions that may distort or hide the image, for example adjacent window shadow, a tooltip or some temporary window. If possible, in such scripts instead use other functions, eg find control or UI element.
		/// 
		/// Flags <see cref="IFFlags.WindowDC"/> and <see cref="IFFlags.PrintWindow"/> cannot be used if <i>area</i> is <b>Bitmap</b> or <b>RECT</b>.
		/// </remarks>
		/// <example>
		/// Code created with dialog "Find image or color in window".
		/// <code><![CDATA[
		/// var w = +wnd.find("Window Name");
		/// string image = "image:iVBORw0KGgoAAAANSUhEUgAAABYAAAANCAYAAACtpZ5jAAAAAXNSR0IArs4c...";
		/// var wi = +uiimage.find(w, image);
		/// wi.Click();
		/// ]]></code>
		/// </example>
		public static uiimage find(IFArea area, IFImage image, IFFlags flags = 0, int colorDiff = 0, Func<uiimage, IFAlso> also = null) {
			var f = new uiimageFinder(image, flags, colorDiff, also);
			if (!f.Find(area)) return null;
			return f.Result;
		}

		internal enum Action_ { Find, Wait, WaitNot, WaitChanged }

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Finds image(s) or color(s) displayed in a window or other area. Waits until found.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <returns>Returns <see cref="uiimage"/> object containing the rectangle of the found image. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="find"/>.</exception>
		public static uiimage wait(double secondsTimeout, IFArea area, IFImage image, IFFlags flags = 0, int colorDiff = 0, Func<uiimage, IFAlso> also = null) {
			var f = new uiimageFinder(image, flags, colorDiff, also);
			return f.Wait_(Action_.Wait, secondsTimeout, area) ? f.Result : null;
		}

		/// <summary>
		/// Waits until image(s) or color(s) is not displayed in a window or other area.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="find"/>.</exception>
		public static bool waitNot(double secondsTimeout, IFArea area, IFImage image, IFFlags flags = 0, int colorDiff = 0, Func<uiimage, IFAlso> also = null) {
			var f = new uiimageFinder(image, flags, colorDiff, also);
			return f.Wait_(Action_.WaitNot, secondsTimeout, area);
		}

		/// <summary>
		/// Waits until something visually changes in a window or other area.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">Invalid window handle (the area argument), or the window closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="find"/>.</exception>
		/// <remarks>
		/// The same as <see cref="waitNot"/>, but instead of <i>image</i> parameter this function captures the area image at the beginning.
		/// </remarks>
		public static bool waitChanged(double secondsTimeout, IFArea area, IFFlags flags = 0, int colorDiff = 0) {
			var f = new uiimageFinder(default, flags, colorDiff, null);
			return f.Wait_(Action_.WaitChanged, secondsTimeout, area);
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
	}
}

namespace Au.Types
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>
	/// Defines the search area for <see cref="uiimage.find"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// It can be a window/control, UI element, another image or a rectangle in screen.
	/// Also allows to specify a rectangle in it, which makes the search area smaller and the function faster.
	/// Has implicit conversions from wnd, elm, Bitmap, RECT (rectangle in screen), tuple (wnd, RECT), tuple (Acc, RECT).
	/// Example: <c>uiimage.find((w, (left, top, width, height)), image);</c>.
	/// </remarks>
	public class IFArea
	{
		internal enum AreaType : byte { Screen, Wnd, Elm, Bitmap }

		internal readonly AreaType Type;
		internal bool HasRect;
		internal wnd W;
		internal elm A;
		internal Bitmap B;
		internal RECT R;

		IFArea(AreaType t) { Type = t; }

		public static implicit operator IFArea(wnd w) => new(AreaType.Wnd) { W = w };
		public static implicit operator IFArea(elm e) => new(AreaType.Elm) { A = e };
		public static implicit operator IFArea(Bitmap b) => new(AreaType.Bitmap) { B = b };
		public static implicit operator IFArea(RECT r) => new(AreaType.Screen) { R = r };
		public static implicit operator IFArea((wnd w, RECT r) t) => new(AreaType.Wnd) { W = t.w, R = t.r, HasRect = true };
		public static implicit operator IFArea((elm e, RECT r) t) => new(AreaType.Elm) { A = t.e, R = t.r, HasRect = true };
		public static implicit operator IFArea((Bitmap b, RECT r) t) => new(AreaType.Bitmap) { B = t.b, R = t.r, HasRect = true };
	}

	/// <summary>
	/// Image(s) or color(s) for <see cref="uiimage.find"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// Has implicit conversions from:
	/// - string - path of .png or .bmp file. If not full path, uses <see cref="folders.ThisAppImages"/>.
	/// - string that starts with "resources/" or has prefix <c>"resource:"</c> - resource name; see <see cref="ResourceUtil.GetGdipBitmap"/>.
	/// - string with prefix <c>"image:"</c> - Base64 encoded .png image.
	/// <br/>Can be created with dialog "Find image or color in window" or with function <b>Au.Controls.KImageUtil.ImageToString</b> (in Au.Controls.dll).
	/// - <see cref="ColorInt"/>, <b>int</b> in 0xRRGGBB color format, <b>Color</b> - color. Alpha isn't used.
	/// - <see cref="Bitmap"/> - image object.
	/// - <b>IFImage[]</b> - multiple images or/and colors. Action - find any. To create a different action can be used callback function (parameter <i>also</i>).
	/// 
	/// Icons are not supported directly; you can use <see cref="icon"/> to get icon and convert to bitmap.
	/// </remarks>
	public struct IFImage
	{
		readonly object _o;
		IFImage(object o) { _o = o; }

		public static implicit operator IFImage(string pathEtc) => new(pathEtc);
		public static implicit operator IFImage(Bitmap image) => new(image);
		public static implicit operator IFImage(ColorInt color) => new(color);
		public static implicit operator IFImage(int color) => new((ColorInt)color);
		public static implicit operator IFImage(Color color) => new((ColorInt)color);
		public static implicit operator IFImage(System.Windows.Media.Color color) => new((ColorInt)color);
		//public static implicit operator IFImage(IEnumerable<IFImage> list) => new(list); //error: cannot convert from interfaces
		public static implicit operator IFImage(IFImage[] list) => new(list);
		//public static implicit operator IFImage(List<IFImage> list) => new(list); //rare, can use ToArray()

		/// <summary>
		/// Gets the raw value stored in this variable. Can be string, Bitmap, ColorInt, IFImage[], null.
		/// </summary>
		public object Value => _o;
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// Flags for <see cref="uiimage.find"/> and similar functions.
	/// </summary>
	[Flags]
	public enum IFFlags
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
	/// Used with <see cref="uiimage.find"/> and <see cref="uiimage.wait"/>. Its callback function (parameter <i>also</i>) can return one of these values.
	/// </summary>
	public enum IFAlso
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
