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



	//public class DocBase
	//{
	//	/// <summary>
	//	/// This is base.
	//	/// Line2.
	//	/// Token: <token>poken</token> etc.
	//	/// </summary>
	//	/// <param name="i">Parrrram.</param>
	//	public virtual void Meth(int i)
	//	{

	//	}
	//}

	//public class DocInher :DocBase
	//{
	//	/// <inheritdoc />
	//	public override void Meth(int i)
	//	{

	//	}
	//}
}

#if false
namespace Help
{
	/** <summary>
A Catkeys wildcard expression is a text string that can be compared with other string as wildcard text, regular expression, multi-part, etc.
Like a regular expression, but in most cases easier to use, faster and more lightweight.
Typically used for string parameters of find-item functions, like <see cref="Wnd.Find">Wnd.Find</see>.
<h3>Wildcard expression reference</h3>
By default case-insensitive. Always culture-insensitive.
By default, if contains *? characters, is compared as wildcard pattern (see <see cref="String_.Like_(string, string, bool)">Like_</see>), else as simple text.
		
Wildcard characters:
<list type="bullet">
<item/>* - zero or more of any characters.
<item/>? - any character.
</list>
		
Can start with **options|, like <c>"**tc|text"</c>. Options:
<list type="bullet">
<item>t - simple text, not wildcard. Example: <c>"**t|text"</c>.</item>
<item>r - regular expression. See <see cref="System.Text.RegularExpressions.Regex"/>.</item>
<item>c - case-sensitive. Example: <c>"**rc|regex, match case"</c>. Info: case-sensitive regular expression is about 4 times faster.</item>
<item>n - must not match.</item>
<item>m - multi-part. Use separator []. Example: <c>"**m|findThis[]orThis[]**r|orThisRegex[]**n|butNotThis[]**nr|andNotThisRegex".</c> Example 2: <c>"**mn|notThis[]andNotThis"</c>.</item>
</list>
		
Pattern "" matches only "". Pattern null usually means 'match any'.
Tip: to avoid many nulls in code, you can omit optional parameters, for example instaed of <c>Wnd.Find(null, null, "notepad")</c> use <c>Wnd.Find(program:"notepad")</c> .
		
Exception <see cref="ArgumentException"/> if invalid **options| or regular expression.
		
Example for users of such find-item functions.
<code><![CDATA[
//Find item whose name is "example" (case-insensitive), date starts with "2017-" and some third property matches a case-sensitive regular expression.
var item = x.FindItem("example", "2017-*", "**rc|regex");
]]></code>
	</summary>*/
	public class Wildcard_expression { }

	/** <summary>
	Test help.
	Etc.
	</summary>*/
	public struct Astruct { }

	/** <summary>
	Test help.
	Etc.
	</summary>*/
	public enum Aenum { }

	/** <summary>
	Test help.
	Etc.
	</summary>*/
	public interface Ainterface { }

	/** <summary>
	Test help.
	Etc.
	</summary>*/
	public delegate void Adelegate();

	/** <summary>
	Test help.
	Etc.
	</summary>*/
	public struct Astruct2
	{
	/** <summary>
	Test help.
	Etc.
	</summary>*/
		public int field1;

	/** <summary>
	Test help.
	Etc.
	</summary>*/
		public int field2;
	}
}
#endif
