using Au.Types;
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
//using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Controls;

namespace Au.Util
{
	/// <summary>
	/// Loads WPF and GDI+/winforms images from file, resource or string.
	/// </summary>
	/// <seealso cref="AResources"/>
	public static class AImageUtil
	{
		/// <summary>
		/// Returns true if string starts with "image:" or "~:".
		/// </summary>
		public static bool HasImageStringPrefix(string s) => s.Starts("image:") || s.Starts("~:");

		/// <summary>
		/// Returns true if string starts with "resource:" or "resources/" or "imagefile:" or "image:" or "~:".
		/// </summary>
		public static bool HasImageOrResourcePrefix(string s) => AResources.HasResourcePrefix(s) || 0 != s.Starts(false, "imagefile:", "image:", "~:");

		/// <summary>
		/// Loads image as stream from Base-64 string that starts with "image:" (png) or "~:" (zipped bmp).
		/// </summary>
		/// <param name="s">Base-64 string with prefix "image:" or "~:".</param>
		/// <exception cref="ArgumentException">String does not start with "image:"/"~:" or is invalid Base-64.</exception>
		/// <exception cref="Exception"><see cref="AConvert.Decompress"/> exceptions (when prefix "~:").</exception>
		public static MemoryStream LoadImageStreamFromString(string s)
		{
			if(!HasImageStringPrefix(s)) throw new ArgumentException("String must start with \"image:\" or \"~:\".");
			bool compressed = s[0] == '~';
			int start = compressed ? 2 : 6, n = (int)((s.Length - start) * 3L / 4);
			var b = new byte[n];
			if(!Convert.TryFromBase64Chars(s.AsSpan(start), b, out n)) throw new ArgumentException("Invalid Base64 string");
			var stream = compressed ? new MemoryStream() : new MemoryStream(b, 0, n, false);
			if(compressed) AConvert.Decompress(stream, b, 0, n);
			return stream;
			//size and speed of "image:" and "~:": "image:" usually is bigger by 10-20% and faster by ~25%
		}

		/// <summary>
		/// Loads GDI+ image from Base-64 string that starts with "image:" (png) or "~:" (zipped bmp).
		/// </summary>
		/// <param name="s">Base-64 string with prefix "image:" or "~:".</param>
		/// <exception cref="ArgumentException">String does not start with "image:"/"~:" or is invalid Base-64.</exception>
		/// <exception cref="Exception"></exception>
		public static System.Drawing.Bitmap LoadGdipBitmapFromString(string s)
			=> new System.Drawing.Bitmap(LoadImageStreamFromString(s));

		/// <summary>
		/// Loads WPF image from Base-64 string that starts with "image:" (png) or "~:" (zipped bmp).
		/// </summary>
		/// <param name="s">Base-64 string with prefix "image:" or "~:".</param>
		/// <exception cref="ArgumentException">String does not start with "image:"/"~:" or is invalid Base-64.</exception>
		/// <exception cref="Exception"></exception>
		public static BitmapFrame LoadWpfImageFromString(string s)
			=> BitmapFrame.Create(LoadImageStreamFromString(s));

		/// <summary>
		/// Calls <see cref="LoadGdipBitmapFromString"/> and handles exceptions. On exception returns null and optionally writes warning to the output.
		/// </summary>
		public static System.Drawing.Bitmap TryLoadGdipBitmapFromString(string s, bool warning)
		{
			try { return LoadGdipBitmapFromString(s); }
			catch(Exception ex) { if(warning) AWarning.Write(ex.ToStringWithoutStack()); }
			return null;
		}

		/// <summary>
		/// Calls <see cref="LoadWpfImageFromString"/> and handles exceptions. On exception returns null and optionally writes warning to the output.
		/// </summary>
		public static BitmapFrame TryLoadWpfImageFromString(string s, bool warning)
		{
			try { return LoadWpfImageFromString(s); }
			catch(Exception ex) { if(warning) AWarning.Write(ex.ToStringWithoutStack()); }
			return null;
		}

		/// <summary>
		/// Loads GDI+ image from file, resource or string.
		/// </summary>
		/// <param name="image">
		/// Can be:
		/// - file path. Can have prefix "imagefile:".
		/// - resource path that starts with "resources/" or has prefix "resource:" (<see cref="AResources.GetGdipBitmap"/>)
		/// - Base-64 image with prefix "image:" (<see cref="LoadGdipBitmapFromString"/>).
		/// </param>
		/// <param name="xaml">If not null, supports XAML images. See <see cref="LoadGdipBitmapFromXaml"/>.</param>
		/// <exception cref="Exception"></exception>
		public static System.Drawing.Bitmap LoadGdipBitmapFromFileOrResourceOrString(string image, (SIZE size, int dpi)? xaml = null) {
			if (HasImageStringPrefix(image))
				return LoadGdipBitmapFromString(image);
			if (xaml != null && (image.Ends(".xaml", true) || image.Starts('<')))
				return LoadGdipBitmapFromXaml(image, xaml.Value.size, xaml.Value.dpi);
			if (AResources.HasResourcePrefix(image))
				return AResources.GetGdipBitmap(image);
			if (image.Starts("imagefile:")) image = image[10..];
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return System.Drawing.Image.FromFile(image) as System.Drawing.Bitmap ?? throw new ArgumentException("Bad image format.");
		}

		/// <summary>
		/// Loads WPF image or icon from file, resource or string.
		/// </summary>
		/// <param name="image">
		/// Can be:
		/// - file path. Can have prefix "imagefile:".
		/// - resource path that starts with "resources/" or has prefix "resource:" (<see cref="AResources.GetWpfImage"/>)
		/// - Base-64 image with prefix "image:" (<see cref="LoadWpfImageFromString"/>).
		/// </param>
		/// <exception cref="Exception"></exception>
		public static BitmapFrame LoadWpfImageFromFileOrResourceOrString(string image) {
			if (HasImageStringPrefix(image)) return LoadWpfImageFromString(image);
			if (AResources.HasResourcePrefix(image)) return AResources.GetWpfImage(image);
			if (image.Starts("imagefile:")) image = image[10..];
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return BitmapFrame.Create(new Uri(image));
		}

		/// <summary>
		/// Loads WPF image element from file, resource or string. Supports xaml, png and other image formats supported by WPF.
		/// </summary>
		/// <param name="image">
		/// Can be:
		/// - file path; can be .xaml, .png etc; supports environment variables etc, see <see cref="APath.ExpandEnvVar"/>; can have prefix "imagefile:".
		/// - resource path that starts with "resources/" or has prefix "resource:"; uses <see cref="AResources.GetXamlObject"/> if ends with ".xaml", else <see cref="AResources.GetWpfImage"/>.
		/// - Base-64 image with prefix "image:"; uses<see cref="LoadWpfImageFromString"/>.
		/// - XAML string that starts with "&lt;".
		/// </param>
		/// <exception cref="Exception">Failed.</exception>
		/// <remarks>
		/// If <i>image</i> starts with "&lt;" or ends with ".xaml" (case-insensitive), returns object created from XAML root element. Else returns <see cref="Image"/> with <b>Source</b> = <b>BitmapFrame</b>.
		/// </remarks>
		public static UIElement LoadWpfImageElementFromFileOrResourceOrString(string image) {
			if (image.Starts('<')) return (UIElement)XamlReader.Parse(image);
			if(image.Ends(".xaml", true)) {
				if (AResources.HasResourcePrefix(image)) return (UIElement)AResources.GetXamlObject(image);
				if (image.Starts("imagefile:")) image = image[10..];
				using var stream = AFile.LoadStream(image);
				return (UIElement)XamlReader.Load(stream);
			} else {
				var bf = LoadWpfImageFromFileOrResourceOrString(image);
				return new Image { Source = bf };
			}
			//Could set UseLayoutRounding=true as a workaround for blurry images, but often it does not work and have to be set on parent element.
			//	Then does not work even if wrapped eg in a Border with UseLayoutRounding.
		}

		/// <summary>
		/// Loads GDI+ image from WPF XAML file or string.
		/// </summary>
		/// <param name="image">XAML file, resource or string. See <see cref="LoadWpfImageElementFromFileOrResourceOrString"/>.</param>
		/// <param name="size">Final image size. Use logical pixels, ie not DPI-scaled.</param>
		/// <param name="dpi">DPI of window that will display the image.</param>
		/// <remarks>
		/// Calls <see cref="LoadWpfImageElementFromFileOrResourceOrString"/> and converts to GDI+ image.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static System.Drawing.Bitmap LoadGdipBitmapFromXaml(string image, SIZE size, int dpi) {
			var e = AImageUtil.LoadWpfImageElementFromFileOrResourceOrString(image);
			e.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
			e.Arrange(new System.Windows.Rect(e.DesiredSize));
			var (wid, hei) = ADpi.Scale(size, dpi);
			var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(wid, hei, dpi, dpi, System.Windows.Media.PixelFormats.Pbgra32);
			rtb.Render(e);
			int stride = wid * 4;
			int msize = hei * stride;
			var m = new _BitmapMemory(msize);
			rtb.CopyPixels(new System.Windows.Int32Rect(0, 0, wid, hei), m.pixels, msize, stride);
			var b = new System.Drawing.Bitmap(wid, hei, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, m.pixels) { Tag = m }; //only this Bitmap creation method preserves alpha
			b.SetResolution(dpi, dpi);
			return b;
		}

		//Holds memory of System.Drawing.Bitmap created with the scan ctor. Such Bitmap does not own/free the memory. We attach _BitmapMemory to Bitmap.Tag, let GC dispose it.
		unsafe class _BitmapMemory
		{
			public readonly IntPtr pixels;
			public _BitmapMemory(int size) { pixels = (IntPtr)AMemory.Alloc(size); }
			~_BitmapMemory() { AMemory.Free((void*)pixels); }
		}
	}
}
