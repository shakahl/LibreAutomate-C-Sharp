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
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		static class _Util
		{
			public static GridLength GridLengthFromString(string s) {
				double w = 0; var u = GridUnitType.Star;
				if (s == null) u = GridUnitType.Auto;
				else if (s.Ends("*")) w = s.Length > 1 ? s.ToNumber(..^1) : 1.0;
				else { w = s.ToNumber(); u = GridUnitType.Pixel; }
				return new GridLength(w, u);
				//also tested GridLengthConverter
			}

			public static string GridLengthToString(GridLength k) {
				if (k.IsAuto) return null;
				var s = Math.Round(k.Value, 10).ToStringInvariant();
				return k.IsStar ? s + "*" : s;
				//GridLength.ToString is almost same, but: for Auto returns "Auto"; can return long string like "425.79999999999995" instead of "425.8".
			}
		}
	}
}
