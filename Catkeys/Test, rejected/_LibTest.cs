//Testing internal library classes.

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
using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

using static Catkeys.Automation.NoClass;

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field
#pragma warning disable 1591 //XML doc

namespace Catkeys
{
	internal static class LibTest
	{
		#region old_test_functions

		//public static unsafe void TestLibMemory()
		//{

		//	Perf.First();
		//	//string x = InterDomain.GetVariable("hhh", () => "ggg");
		//	//IntPtr x = InterDomain.GetVariable("hhh", () => (IntPtr)5);
		//	//Perf.Next();
		//	//IntPtr m = Zero;
		//	for(int i = 0; i < 1; i++) {
		//		//Perf.First();

		//		//m = Util.Misc.GetProcMem(); //(env var + mutex) 75 -> 20

		//		//m = Util.Misc.GetProcMem3(); //(shared memory) 70 -> 41

		//		//m = Util.Misc.GetProcMem4(); //(win class) 65 -> 15

		//		var m = Util.LibProcessMemory.Ptr;

		//		Perf.Next();
		//		//Perf.NW();
		//		Print((IntPtr)m);
		//		//int* k = (int*)m;
		//		//Print(*k);
		//		//if(*k == 0) *k = 7;
		//		Print(m->test);
		//		if(m->test == 0) m->test = 7;
		//	}
		//	Perf.Write();
		//	//Print(m);

		//	//GC.Collect();
		//}

		#endregion

		#region enum files

		public static IEnumerable<string> EnumerateFiles(string path, string pattern, bool recursive)
		{
			IEnumerable<string> files = _EnumerateFileSystemEntries(path, pattern);

			foreach(string file in files) {
				yield return file;
			}

			if(recursive) {
				foreach(string directory in _EnumerateDirectories(path)) {
					files = EnumerateFiles(directory, pattern, true);

					foreach(string file in files) {
						yield return file;
					}
				}
			}
		}

		/// <summary>
		/// Gets full paths of subdirectories within the parent directory.
		/// </summary>
		/// <param name="path">Parent directory.</param>
		static IEnumerable<string> _EnumerateDirectories(string path)
		{
			IEnumerable<string> directories = null;

			try {
				directories = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
			}
			catch(UnauthorizedAccessException) {
			}
			catch(IOException) {
				// Path not found
			}

			if(directories == null) {
				yield break;
			}

			foreach(string directory in directories) {
				yield return directory;
			}
		}

		/// <summary>
		/// Gets all files matching a certain wildcard pattern inside the parent directory.
		/// </summary>
		/// <param name="path">The directory containing files to match.</param>
		/// <param name="pattern">The file system pattern (such as *.txt or *.*).</param>
		/// <remarks>This is not a recursive search.</remarks>
		static IEnumerable<string> _EnumerateFileSystemEntries(string path, string pattern)
		{
			IEnumerable<string> files = null;

			try {
				files = Directory.EnumerateFileSystemEntries(path, pattern, SearchOption.TopDirectoryOnly);
			}
			catch(UnauthorizedAccessException) {
			}
			catch(IOException) {
				// Path not found
			}

			if(files == null) {
				yield break;
			}

			// Check exists otherwise this could return matching directories
			foreach(string file in files.Where(File.Exists)) {
				yield return file;
			}
		}

		#endregion


		public static void TestFuture()
		{

		}
	}
}
