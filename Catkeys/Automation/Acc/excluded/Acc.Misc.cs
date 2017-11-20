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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	public unsafe partial class Acc
	{
		/// <summary>
		/// Contains miscellaneous static Acc-related functions, rarely used or useful only for programmers.
		/// </summary>
		public static class Misc
		{
			//now this file is excluded. This function moved to Acc.
			/// <summary>
			/// When searching for an accessible object, if an intermediate object has more than this number of direct children, skip them and their descendants. This property is used by Acc.Find, Acc.Children, Acc.EnumChildren and similar functions.
			/// Valid values are 1000 to 1000000. Default 10000. Getting and comparing such large number of children is very slow. In extreme cases your app may hang or crash if there was no limit. For example OpenOffice Calc TABLE has one billion children.
			/// This property is thread-specific.
			/// </summary>
			public static int MaxChildren
			{
				get { int n = t_maxChildren; if(n == 0) t_maxChildren = n = 10_000; return n; }
				set { t_maxChildren = Math_.MinMax(value, 1000, 1_000_000); }
			}
			[ThreadStatic] static int t_maxChildren;
		}
	}
}
