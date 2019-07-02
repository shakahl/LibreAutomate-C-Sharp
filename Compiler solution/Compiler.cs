//Algorithm of comiling a script.

//Au.Tasks looks whether compiler is loaded. If not:
//	Creates thread, which executes Au.Compiler.dll in "Au.Compiler" appdomain. Waits until it is finished initializing.
//	Compiler creates a message-only window for communication. Then sets event to notify the main thread to stop waiting.
//Au.Tasks sends message to the compiler window with the cs file, dll file and options.
//Compiler compiles the cs file to the dll file. Returns a success/error code. On error, shows error in editor.

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
//using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
//using System.Reflection.Metadata;
//using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Au.Compiler
{
	public class Compiler
	{
		public static void Main()
		{
		}
	}
}
