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
using System.Drawing;
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	static class Image_
	{
		public static bool IsImageStringPrefix(string s) => s.Starts("image:") || s.Starts("~:");

		/// <summary>
		/// Loads image from Base-64 string that starts with "image:" (png) or "~:" (zipped bmp).
		/// Exception if fails.
		/// </summary>
		/// <param name="s">Must start with "image:" or "~:". Asserts. Use <see cref="IsImageStringPrefix"/>.</param>
		public static Bitmap LoadImageFromString(string s)
		{
			Debug.Assert(IsImageStringPrefix(s));
			bool compressed = s[0] == '~';
			int start = compressed ? 2 : 6, n = (int)((s.Length - start) * 3L / 4);
			var b = new byte[n];
			if(!Convert.TryFromBase64Chars(s.AsSpan(start), b, out n)) throw new ArgumentException("Invalid Base64 string");
			using var stream = compressed ? new MemoryStream() : new MemoryStream(b, 0, n, false);
			if(compressed) AConvert.Decompress(stream, b, 0, n);
			return new Bitmap(stream);
			//size and speed of "image:" and "~:": "image:" usually is bigger by 10-20% and faster by ~25%
		}

		/// <summary>
		/// Calls <see cref="LoadImageFromString"/> and handles exceptions. On exception returns null and optionally writes warning to the output.
		/// </summary>
		public static Bitmap TryLoadImageFromString(string s, bool warning)
		{
			try { return LoadImageFromString(s); }
			catch(Exception ex) { if(warning) AWarning.Write(ex.ToStringWithoutStack()); }
			return null;
		}
	}
}
