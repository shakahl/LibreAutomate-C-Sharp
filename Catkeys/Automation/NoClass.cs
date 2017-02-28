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

#pragma warning disable 1591 //XML doc. //TODO

namespace Catkeys.Automation
{
	public static class NoClass
	{
		public static void SendKeys(params string[] keys_text_keys_text_andSoOn) { Input.Keys(keys_text_keys_text_andSoOn); }

		/// <summary>
		/// Suspends this thread for the specified amount of time.
		/// Alias of <see cref="Time.Wait"/>.
		/// </summary>
		/// <param name="timeS">
		/// The number of seconds to wait.
		/// The smallest value is 0.001 (1 ms).
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">timeS is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		public static void Wait(double timeS) { Time.Wait(timeS); }
	}
}
