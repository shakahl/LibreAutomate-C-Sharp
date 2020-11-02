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
	/// Loads WPF and winforms images from file, resource or string.
	/// </summary>
	/// <seealso cref="AResources"/>
	public static class AImageUtil
	{
		/// <summary>
		/// Returns true if string starts with "image:" or "~:".
		/// </summary>
		public static bool HasImageStringPrefix(string s) => s.Starts("image:") || s.Starts("~:");

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
		/// Loads winforms image from Base-64 string that starts with "image:" (png) or "~:" (zipped bmp).
		/// </summary>
		/// <param name="s">Base-64 string with prefix "image:" or "~:".</param>
		/// <exception cref="ArgumentException">String does not start with "image:"/"~:" or is invalid Base-64.</exception>
		/// <exception cref="Exception"></exception>
		public static System.Drawing.Bitmap LoadWinformsImageFromString(string s)
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
		/// Calls <see cref="LoadWinformsImageFromString"/> and handles exceptions. On exception returns null and optionally writes warning to the output.
		/// </summary>
		public static System.Drawing.Bitmap TryLoadWinformsImageFromString(string s, bool warning)
		{
			try { return LoadWinformsImageFromString(s); }
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
		/// Loads winforms image or icon from file, resource or string.
		/// </summary>
		/// <param name="image">File path, or string with prefix "resource:" (<see cref="AResources.GetWinformsImage"/>) or "image:"/"~:" (<see cref="LoadWinformsImageFromString"/>).</param>
		/// <exception cref="Exception"></exception>
		public static System.Drawing.Bitmap LoadWinformsImageFromFileOrResourceOrString(string image) {
			if (HasImageStringPrefix(image))
				return LoadWinformsImageFromString(image);
			if (AResources.HasResourcePrefix(image))
				return AResources.GetWinformsImage(image);
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return System.Drawing.Image.FromFile(image) as System.Drawing.Bitmap ?? throw new ArgumentException("Bad image format.");
		}

		/// <summary>
		/// Loads WPF image or icon from file, resource or string.
		/// </summary>
		/// <param name="image">File path, or string with prefix "resource:" (<see cref="AResources.GetWpfImage"/>) or "image:"/"~:" (<see cref="LoadWpfImageFromString"/>).</param>
		/// <exception cref="Exception"></exception>
		public static BitmapFrame LoadWpfImageFromFileOrResourceOrString(string image) {
			if (HasImageStringPrefix(image)) return LoadWpfImageFromString(image);
			if (AResources.HasResourcePrefix(image)) return AResources.GetWpfImage(image);
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return BitmapFrame.Create(new Uri(image));
		}

		/// <summary>
		/// Loads WPF image element from file, resource or string. Supports xaml, png and other image formats supported by WPF.
		/// </summary>
		/// <param name="image">
		/// Can be:
		/// - full path of image file (.xaml, .png etc, supports environment variables etc, see <see cref="APath.ExpandEnvVar"/>);
		/// - string with prefix "resource:" (uses <see cref="AResources.GetXamlObject"/> if ends with ".xaml", else <see cref="AResources.GetWpfImage"/>);
		/// - "image:" (<see cref="LoadWpfImageFromString"/>);
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
				using var stream = AFile.LoadStream(image);
				return (UIElement)XamlReader.Load(stream);
			} else {
				var bf = LoadWpfImageFromFileOrResourceOrString(image);
				return new Image { Source = bf };
			}
			//Could set UseLayoutRounding=true as a workaround for blurry images, but often it does not work and have to be set on parent element.
			//	Then does not work even if wrapped eg in a Border with UseLayoutRounding.
		}
	}
}
