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

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Catkeys.Automation
{
	public static class NoClass
	{
		public static void SendKeys(params string[] keys_text_keys_text_andSoOn) { Input.Keys(keys_text_keys_text_andSoOn); }

		public static void Wait(double timeS) { Time.Wait(timeS); }
	}
}
