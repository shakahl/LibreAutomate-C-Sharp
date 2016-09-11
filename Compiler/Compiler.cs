//Algorithm of comiling a script.

//CatkeysTasks looks whether compiler is loaded. If not:
//	Creates thread, which executes Compiler.dll in "Compiler" appdomain. Waits until it is finished initializing.
//	Compiler creates a message-only window for communication. Then sets event to notify the main thread to stop waiting.
//CatkeysTasks sends message to compiler window with the cs file, dll file and options.
//Compiler compiles the cs file to the dll file. Returns a success/error code. On error, shows error in editor.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Compiler
{
	public class Compiler
	{
		static Wnd.Misc.WndClass _wndClassCompiler = Wnd.Misc.WndClass.Register("Catkeys_Compiler", _WndProcCompiler);

		public static void Main()
		{
			Wnd w = Wnd.Misc.CreateMessageWindow(_wndClassCompiler.Name);

			var dom = AppDomain.CurrentDomain;
			dom.SetData("hwndCompiler", w.Handle);
			Api.SetEvent((IntPtr)dom.GetData("eventInited"));

			//message loop (not Application.Run() because then loads slower etc)
			Api.MSG m;
			while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) { Api.DispatchMessage(ref m); }
		}

		unsafe static LPARAM _WndProcCompiler(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
		{
			switch(msg) {
			case Api.WM_USER:
				return _Compile();
			}

			LPARAM R = Api.DefWindowProc(hWnd, msg, wParam, lParam);

			switch(msg) {
			case Api.WM_NCDESTROY:
				Api.PostQuitMessage(0);
				break;
			}
			return R;
		}

		static MethodInfo _compilerMethod;

		static int _Compile()
		{
			//Perf.First(100);
			Perf.Next(); //3 ms first time (ngen-compiled), then <50 mcs

			//System.Runtime.ProfileOptimization.SetProfileRoot(@"C:\Test");
			//System.Runtime.ProfileOptimization.StartProfile("Startup.Profile"); //does not make jitting the C# compiler assemblies faster
			//Perf.Next();

			if(_compilerMethod == null) {
				//Assembly asm = Assembly.Load("csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"); //works, same speed as LoadFile, but VS shows many warnings if this project uses different .NET framework version than csc (which is added to project references). Also, possibly could make the app start slower, especially if HDD. Better to load on demand through reflection.
				Assembly asm = Assembly.LoadFile(Folders.App + "csc.exe");
				if(!Util.Misc.IsAssemblyNgened(asm)) Output.Warning("csc.exe not ngened. The first compiling will be slow.");
				_compilerMethod = asm.EntryPoint;

				//get compiler output (errors etc)
				Console.SetOut(_consoleRedirWriter); //35 mcs

				Perf.Next(); //7 ms
			}

			var dom = AppDomain.CurrentDomain;
			string csFile = (string)dom.GetData("cs"), dllFile = (string)dom.GetData("dll");

			string[] g = new string[] {
				"/nologo", "/noconfig",
				"/out:" + dllFile, "/target:winexe",
				$"/r:{Folders.App}\\Catkeys.dll",
				//"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
				csFile
			};
			object[] p = new object[1] { g };

			//g[0]="/?";

			_compilerOutput.Clear();
			int r = (int)_compilerMethod.Invoke(0, p); //18-23 ms minimal (everything ngen-compiled, recently PC restarted), 33 ms with several /r; first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			if(r != 0) {
				Out(_compilerOutput);
			} else if(_compilerOutput.Length > 0) {
				Out(_compilerOutput);
			}

			if(r == 0) {
				for(int i = 0; i < 4; i++) {
					Perf.Next();
					_compilerMethod.Invoke(0, p);
				}
			}

			Perf.Next();
			Perf.Write();

			//Mes("now will GC.Collect");
			GC.Collect(); //releases a lot

			//Util.Debug_.OutLoadedAssemblies();
			//Show.TaskDialog("exit");

			return r;
		}

		static ConsoleRedirWriter _consoleRedirWriter = new ConsoleRedirWriter();
		static StringBuilder _compilerOutput = new StringBuilder();

		class ConsoleRedirWriter :TextWriter
		{
			public override void Write(string value) { _compilerOutput.Append(value); }
			public override void WriteLine(string value) { _compilerOutput.AppendLine(value); }
			public override Encoding Encoding { get { return Encoding.Unicode; } }
		}
	}
}
