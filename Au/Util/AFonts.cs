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
//using System.Linq;
using System.Drawing;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// Provides various versions of standard font.
	/// </summary>
	/// <remarks>
	/// Extends <see cref="SystemFonts"/>. Much faster.
	/// The properties return non-cached <b>Font</b> objects. It's safe to dispose them. They are cloned from a single cached <see cref="SystemFonts.MessageBoxFont"/>.
	/// </remarks>
	public static class AFonts
	{
		//info: we don't return cached Font objects, because we cannot protect them from disposing. The Font class is sealed.
		//	SystemFonts too, always creates new object.
		//	But eg Brushes and SystemBrushes use cached object. It is not protected from disposing (would be exception later).

		//static AFonts()
		//{
		//	SystemEvents.UserPreferenceChanged += (unu, sed) => LibRegularCached = SystemFonts.MessageBoxFont; //never mind
		//}

		/// <summary>
		/// The cached font from which are cloned other fonts.
		/// Use carefully, to avoid accidental disposing; don't use for Font objects that can be accessed by any code.
		/// </summary>
		internal static Font LibRegularCached = SystemFonts.MessageBoxFont; //>200 mcs

		/// <summary>
		/// Standard font used by most windows and controls.
		/// On Windows 10 it is "Segoe UI" 9 by default.
		/// </summary>
		public static Font Regular => LibRegularCached.Clone() as Font; //5-22 mcs

		/// <summary>
		/// Bold version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Bold => new Font(LibRegularCached, FontStyle.Bold); //slightly slower than Clone

		/// <summary>
		/// Italic version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Italic => new Font(LibRegularCached, FontStyle.Italic);

		/// <summary>
		/// Underlined version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Underline => new Font(LibRegularCached, FontStyle.Underline);

		/// <summary>
		/// Strikeout version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Strikeout => new Font(LibRegularCached, FontStyle.Strikeout);
	}
}
