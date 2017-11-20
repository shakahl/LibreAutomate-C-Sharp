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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
#if false //currently not used in lib
	/// <summary>
	/// Stores a string[] and has implicit casts from string[], List&lt;string&gt; and string like "one|two|three".
	/// Can be used for function parameters that accept a list of strings of any of these types.
	/// The called function then can get the string[] through the Arr property or implicit cast to string[].
	/// </summary>
	[DebuggerStepThrough]
	public class StringList
	{
		//For speed and memory usage, better would be struct. Then don't need to alloc new object.
		//But then cannot assign null, which makes it more difficult/unclear to use (although could use StringList? parameters).
		//In this case, easy/clear usage is more important than some speed/memory advantage.
		//Could inherit from string[] or List<string>, but C# does not allow it. Other tries to make this better failed too.
		//It's difficult to measure speed because of compiler optimizations etc, but passing a string[] to a function through a parameter of this class is only less than 30% slower than passing directly through a string[] parameter or struct.

		private string[] _a;

		StringList(string[] a) { _a = a; }

		/// <summary>
		/// Stores the string[] in this variable. Does not copy.
		/// </summary>
		public static implicit operator StringList(string[] a) { return a == null ? null : new StringList(a); }
		/// <summary>
		/// Converts List&lt;string&gt; to string[].
		/// </summary>
		public static implicit operator StringList(List<string> a) { return a == null ? null : new StringList(a.ToArray()); }
		/// <summary>
		/// Converts from string like "one|two|three" to string[] {"one", "two", "three"}.
		/// </summary>
		public static implicit operator StringList(string s) { return Empty(s) ? null : new StringList(s.Split_("|")); }

		/// <summary>
		/// Gets string[] stored in StringList.
		/// </summary>
		public static implicit operator string[] (StringList x) { return x?._a; }

		/// <summary>
		/// Gets string[] stored in this StringList.
		/// </summary>
		public string[] Arr => _a;
	}
#endif
}
