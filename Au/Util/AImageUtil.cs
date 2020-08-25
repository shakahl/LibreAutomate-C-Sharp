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
//using System.Linq;

using Au.Types;
using System.Windows.Media.Imaging;

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
		public static Bitmap LoadWinformsImageFromString(string s)
			=> new Bitmap(LoadImageStreamFromString(s));

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
		public static Bitmap TryLoadWinformsImageFromString(string s, bool warning)
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
		public static Bitmap LoadWinformsImageFromFileOrResourceOrString(string image) {
			if (HasImageStringPrefix(image))
				return LoadWinformsImageFromString(image);
			if (AResources.HasResourcePrefix_(image))
				return AResources.GetWinformsImage(image);
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return Image.FromFile(image) as Bitmap ?? throw new ArgumentException("Bad image format.");
		}

		/// <summary>
		/// Loads WPF image or icon from file, resource or string.
		/// </summary>
		/// <param name="image">File path, or string with prefix "resource:" (<see cref="AResources.GetWpfImage"/>) or "image:"/"~:" (<see cref="LoadWpfImageFromString"/>).</param>
		/// <exception cref="Exception"></exception>
		public static BitmapFrame LoadWpfImageFromFileOrResourceOrString(string image) {
			if (HasImageStringPrefix(image))
				return LoadWpfImageFromString(image);
			if (AResources.HasResourcePrefix_(image))
				return AResources.GetWpfImage(image);
			image = APath.Normalize(image, AFolders.ThisAppImages, flags: PNFlags.CanBeUrlOrShell); //CanBeUrlOrShell: support "pack:"
			return BitmapFrame.Create(new Uri(image));
		}
	}
}
