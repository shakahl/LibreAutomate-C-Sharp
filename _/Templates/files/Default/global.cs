/// Class file "global.cs" is compiled with every script and project, like with /*/ c global.cs /*/.
/// Where don't need it, define NO_GLOBAL in meta comments: /*/ define NO_GLOBAL; /*/.
/// You can edit this file. Add/remove global usings, add classes, add assembly/module attributes,
/// 	add more global class files and library references like /*/ c \file.cs; r Lib.dll; /*/.
/// Note: editing this file affects all other files, not only new files created afterwards.
/// See also: Options -> Code -> Favorite namespaces.


#if !NO_GLOBAL
global using Au; //automation
global using Au.Types; //types of parameters etc
global using System; //.NET types used everywhere
global using System.Collections.Generic; //List, Dictionary and other collections

//less important namespaces
global using System.Linq; //extension methods for collections
global using System.Collections.Concurrent; //thread-safe collections
global using System.Diagnostics; //debug
global using System.Globalization; //culture info
global using System.IO; //file, directory
global using System.IO.Compression; //zip
global using System.Media; //sound
global using System.Runtime.CompilerServices; //some useful attributes, Unsafe
global using System.Runtime.InteropServices; //types for Windows API etc
global using System.Text; //encodings, StringBuilder
global using System.Threading; //threads, synchronization
global using System.Threading.Tasks; //thread pool
global using Microsoft.Win32; //registry, common dialogs
global using Au.More; //rarely used in automation scripts

//global using static Example;


///// <summary>
///// Example of global class and function.
///// </summary>
//static class Example {
//	/// <summary>
//	/// Example.
//	/// </summary>
//	/// <param name="a"></param>
//	/// <param name="b"></param>
//	/// <returns></returns>
//	/// <example>
//	/// <code><![CDATA[
//	/// print.it(Example.Add(2, 7));
//	/// print.it(Add(2, 7)); //if uncommented the 'global using static Example;'
//	/// ]]></code>
//	/// </example>
//	public static int Add(int a, int b) {
//		return a + b;
//	}
//}

#endif
