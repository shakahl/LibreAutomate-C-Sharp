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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Miscellaneous string functions. Parsing etc.
	/// </summary>
	public static class StringMisc
	{
		/// <summary>
		/// Parses a function parameter that can optionally have a "***name " prefix, like "***id 100".
		/// Returns: 0 - s does not start with "***"; i+1 - s starts with "***names[i] "; -1 - s is invalid.
		/// </summary>
		/// <param name="s">Parameter. If starts with "***" and is valid, receives the 'value' part; else unchanged. Can be null.</param>
		/// <param name="names">List of supported 'name'.</param>
		/// <remarks>
		/// Used to parse parameters like <i>name</i> of <see cref="Wnd.Child"/>.
		/// </remarks>
		internal static int ParseParam3Stars(ref string s, params string[] names)
		{
			if(s == null || !s.StartsWith_("***")) return 0;
			for(int i = 0; i < names.Length; i++) {
				var ni = names[i];
				if(s.Length - 3 <= ni.Length || !s.EqualsAt_(3, ni)) continue;
				int j = 3 + ni.Length;
				char c = s[j]; if(c != ' ') break;
				s=s.Substring(j + 1);
				return i + 1;
			}
			return -1;
		}

		/// <summary>
		/// Removes '&amp;' characters from string.
		/// Replaces "&amp;&amp;" with "&amp;".
		/// Returns new string if s has '&amp;' characters, else returns s.
		/// </summary>
		/// <remarks>
		/// Character '&amp;' is used to underline next character in displayed text of dialog controls and menu items. Two '&amp;' are used to display single '&amp;'.
		/// The underline is displayed when using the keyboard (eg Alt key) to select dialog controls and menu items.
		/// </remarks>
		public static string RemoveUnderlineAmpersand(string s)
		{
			if(!Empty(s)) {
				for(int i = 0; i < s.Length; i++) if(s[i] == '&') goto g1;
				return s;
				g1:
				var b = Util.Buffers.LibChar(s.Length);
				int j = 0;
				for(int i = 0; i < s.Length; i++) {
					if(s[i] == '&') {
						if(i < s.Length - 1 && s[i + 1] == '&') i++;
						else continue;
					}
					b.A[j++] = s[i];
				}
				s = b.LibToStringCached(j);
			}
			return s;
		}
	}
}
