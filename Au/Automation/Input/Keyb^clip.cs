using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#pragma warning disable 1591 //XML doc. //TODO

namespace Au
{
	public partial class Keyb
	{

		public static void Paste(string text, string keys = null, string format = null)
		{
			var x = new Keyb(Keyb.Options);
			x._Paste(text, keys, format);
		}

		void _Paste(string text, string keys = null, string format = null)
		{

		}

		public static void Paste(Wnd w, string text, string keys = null, string format = null)
		{

		}

		public static string Copy(bool cut = false, string format = null)
		{
			return null;
		}

		public static string Copy(Wnd w, bool cut = false, string format = null)
		{
			return null;
		}
	}

	public static partial class NoClass
	{
		/// <inheritdoc cref="Keyb.Paste(string, string, string)"/>
		public static void Paste(string text, string keys = null) => Keyb.Paste(text, keys);
	}
}
