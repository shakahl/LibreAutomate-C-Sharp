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
//using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys.Util
{
	/// <summary>
	/// Extends the .NET struct Color.
	/// </summary>
	public static class Color_
	{
		/// <summary>
		/// Converts color from ARGB (0xAARRGGBB) to ABGR (0xAABBGGRR) or vice versa (swaps the red and blue bytes).
		/// ARGB is used in .NET, GDI+ and HTML/CSS.
		/// ABGR is used by most Windows native API.
		/// </summary>
		public static uint SwapRedBlue(uint color)
		{
			return (color & 0xff00ff00) | ((color << 16) & 0xff0000) | ((color >> 16) & 0xff);
		}

		/// <summary>
		/// Converts from System.Drawing.Color to native Windows COLORREF with alpha (0xAABBGGRR).
		/// </summary>
		public static uint ColorToABGR(Color color)
		{
			return SwapRedBlue((uint)color.ToArgb());
		}

		/// <summary>
		/// Converts from native Windows COLORREF with alpha (0xAABBGGRR) to System.Drawing.Color.
		/// </summary>
		public static Color ColorFromABGR(uint color)
		{
			return Color.FromArgb((int)SwapRedBlue(color));
		}

		/// <summary>
		/// Converts from native Windows COLORREF without alpha (0x00BBGGRR) to opaque System.Drawing.Color.
		/// The alpha byte of the color argument can be 0 or any other value. Makes it 0xFF.
		/// </summary>
		public static Color ColorFromBGR(uint color)
		{
			return Color.FromArgb((int)(SwapRedBlue(color) | 0xFF000000));
		}

		/// <summary>
		/// Converts from ARGB uint (0xAARRGGBB) to System.Drawing.Color.
		/// Same as <see cref="Color.FromArgb(int)"/> but don't need <c>unchecked((int)0xFF...)</c>.
		/// </summary>
		public static Color ColorFromARGB(uint color)
		{
			return Color.FromArgb((int)color);
		}

		/// <summary>
		/// Converts from RGB uint (0x00RRGGBB) to opaque System.Drawing.Color.
		/// Same as <see cref="Color.FromArgb(int)"/> but don't need to specify alpha. You can replace <c>Color.FromArgb(unchecked((int)0xFF123456))</c> with <c>Color_.ColorFromRGB(0x123456)</c>.
		/// </summary>
		public static Color ColorFromRGB(uint color)
		{
			return Color.FromArgb((int)(color | 0xFF000000));
		}

		/// <summary>
		/// Converts string to opaque System.Drawing.Color.
		/// The string can be a color name (<see cref="Color.FromName(string)"/>) or 0xRRGGBB.
		/// Returns false if invalid name.
		/// </summary>
		public static bool ColorFromNameOrRGB(string s, out Color c)
		{
			if(Empty(s)) goto ge;
			if(s[0] == '0') {
				int i = s.ToInt32_(0, out int len);
				if(len == 0 || (len < s.Length && char.IsLetterOrDigit(s[len]))) goto ge;
				c = ColorFromRGB((uint)i);
			} else c = Color.FromName(s);
			return (c.A != 0);
			ge:
			c = default;
			return false;
		}

		/// <summary>
		/// Calculates color's perceived brightness.
		/// Returns a value in range 0 (brightness of black color) to 1 (brightness of white color).
		/// </summary>
		/// <param name="colorRGB">Color in 0xRRGGBB format. Alpha is ignored.</param>
		/// <remarks>
		/// Unlike Color.GetBrightness, this function gives different weights for red, green and blue components.
		/// </remarks>
		public static float Brightness0to255(uint colorRGB)
		{
			uint R = colorRGB >> 16 & 0xff, G = colorRGB >> 8 & 0xff, B = colorRGB & 0xff;
			return (float)(Math.Sqrt(R * R * .299 + G * G * .587 + B * B * .114) / 255);
		}

		/// <summary>
		/// Calculates color's perceived brightness.
		/// Returns a value in range 0 (brightness of black color) to 1 (brightness of white color).
		/// </summary>
		/// <param name="c">Color. Alpha is ignored.</param>
		/// <remarks>
		/// Unlike Color.GetBrightness, this function gives different weights for red, green and blue components.
		/// </remarks>
		public static float Brightness0to255(Color c)
		{
			return (float)(Math.Sqrt(c.R * c.R * .299 + c.G * c.G * .587 + c.B * c.B * .114) / 255);
		}

		/// <summary>
		/// Changes color's luminance (makes darker or brighter). Hue and saturation are not affected.
		/// Calls API <msdn>ColorAdjustLuma</msdn>.
		/// </summary>
		/// <param name="colorRGB">Color in 0xRRGGBB or 0xBBGGRR format. Alpha is not used and not changed.</param>
		/// <param name="n">The luminance in units of 0.1 percent of the range (which depends on totalRange). Can be from -1000 to 1000.</param>
		/// <param name="totalRange">If true, n is in the whole luminance range (from minimal to maximal possible). If false, n is in the range from current luminance of the color to the maximal (if n positive) or minimal (if n negative) luminance.</param>
		public static uint AdjustLuminance(uint colorRGB, int n, bool totalRange = false)
		{
			return ColorAdjustLuma(colorRGB & 0xffffff, n, !totalRange) | (colorRGB & 0xFF000000);
			//return SwapRedBlue(ColorAdjustLuma(SwapRedBlue(colorRGB & 0xffffff), n, !totalRange)) | (colorRGB & 0xFF000000); //the same
		}

		[DllImport("shlwapi.dll")]
		static extern uint ColorAdjustLuma(uint clrRGB, int n, bool fScale);
	}
}
