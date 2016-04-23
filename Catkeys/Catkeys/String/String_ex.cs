using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public static partial class String_
	{
		public static string PathExpand(this string t)
		{
			return null;
		}

		public static string PathUnexpand(this string t)
		{
			return null;
		}

		public static string PathAppend(this string t, string s)
		{
			s = Path.Combine(t, s); //TODO: if s empty, use $app$
			return Path.GetFullPath(s); //TODO: only if contains \.
			//return s;
		}

		public static string PathPrepend(this string t)
		{
			return null;
		}
	}
}
