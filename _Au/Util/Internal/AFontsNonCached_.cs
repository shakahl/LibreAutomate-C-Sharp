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
using System.Drawing;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Provides various versions of standard font.
	/// </summary>
	/// <remarks>
	/// Extends <see cref="SystemFonts"/>. Much faster.
	/// The properties return non-cached <b>Font</b> objects. It's safe to dispose them. It's OK to not dispose (GC will do; GDI+ fonts don't use much unmanaged memory). They are cloned from a single cached <see cref="SystemFonts.MessageBoxFont"/>.
	/// </remarks>
	internal static class AFontsNonCached_
	{
		//info: we don't return cached Font objects, because we cannot protect them from disposing. The Font class is sealed.
		//	SystemFonts too, always creates new object.
		//	But eg Brushes and SystemBrushes use cached object. It is not protected from disposing (would be exception later).

		//static AFontsNonCached_()
		//{
		//	SystemEvents.UserPreferenceChanged += (_, _) => _regularCached = SystemFonts.MessageBoxFont; //never mind
		//}

		/// <summary>
		/// The cached font from which are cloned other fonts.
		/// Use carefully, to avoid accidental disposing.
		/// </summary>
		static Font _regularCached = SystemFonts.MessageBoxFont; //>200 mcs

		/// <summary>
		/// Standard font used by most windows and controls.
		/// On Windows 10 it is "Segoe UI" 9 by default.
		/// </summary>
		public static Font Regular => _regularCached.Clone() as Font; //5-22 mcs

		/// <summary>
		/// Bold version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Bold => new Font(_regularCached, FontStyle.Bold);

		/// <summary>
		/// Different size version of <see cref="Regular"/> font.
		/// </summary>
		public static Font OfSize(int size) => new Font(_regularCached.FontFamily, size);

		/// <summary>
		/// Different style version of <see cref="Regular"/> font.
		/// </summary>
		public static Font OfStyle(FontStyle style) => new Font(_regularCached, style);

		/// <summary>
		/// Different style and size version of <see cref="Regular"/> font.
		/// </summary>
		public static Font OfSizeStyle(int size, FontStyle style) => new Font(_regularCached.FontFamily, size, style);
	}

	/// <summary>
	/// Provides cached various versions of standard font.
	/// Use carefully, to avoid accidental disposing.
	/// </summary>
	internal static class AFontsCached_
	{
		static Font _regularCached = SystemFonts.MessageBoxFont; //>200 mcs
		static Font _boldCached;

		/// <summary>
		/// Standard font used by most windows and controls.
		/// On Windows 10 it is "Segoe UI" 9 by default.
		/// </summary>
		public static Font Regular => _regularCached;

		/// <summary>
		/// Bold version of <see cref="Regular"/> font.
		/// </summary>
		public static Font Bold => _boldCached ??= new Font(_regularCached, FontStyle.Bold);
	}
}
