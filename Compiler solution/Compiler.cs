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

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	public class Compiler
	{
		const string _msgClass = "Au.Compiler";

		public static void Main()
		{
			Wnd.Misc.MyWindow.RegisterClass(_msgClass);
			var mw = new MsgWindow();
			mw.CreateMessageWindow(_msgClass);

			var dom = AppDomain.CurrentDomain;
			dom.SetData("hwndCompiler", mw.Handle.Handle);
			Api.SetEvent((IntPtr)dom.GetData("eventInited"));

			//message loop (not Application.Run() because then loads slower etc)
			Native.MSG m;
			while(Api.GetMessage(out m, default, 0, 0) > 0) { Api.DispatchMessage(m); }
		}

		class MsgWindow :Wnd.Misc.MyWindow
		{
			protected override LPARAM WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
			{
				switch(message) {
				case Api.WM_USER:
					var r = _Compile();
					//if(r == 0) { _Compile(); _Compile(); _Compile(); }
					Perf.NW();
					return r;

					//_TestScript();
					//_TestScript();
					//_TestScript();
					//_TestScript();
					//Perf.NW();
					//return 0;
				}

				var R= base.WndProc(w, message, wParam, lParam);

				switch(message) {
				case Api.WM_NCDESTROY:
					Api.PostQuitMessage(0);
					break;
				}
				return R;
			}
		}

#if true
		static int _Compile()
		{
			//Perf.First(100);
			Perf.Next(); //3 ms first time (ngen-compiled), then <50 mcs

			var dom = AppDomain.CurrentDomain;
			string csFile = (string)dom.GetData("cs"), outFile = (string)dom.GetData("out");

			bool inMemoryAsm = true;

			//Print(csFile);
			//Print(outFile);

			//var source = File.ReadAllText(csFile);
			var source = @"
using System;
using Au;
using static Au.NoClass;

public static class Test
{
		public static void Main()
		{
			Print(Folders.ThisApp);
//Thread.Sleep(5000);
			//Print(""end"");
		}
	}
";

			var sRef = new string[] { typeof(object).Assembly.Location, Folders.ThisApp + "Au.dll" };
			//var sRef = new string[] { typeof(object).Assembly.Location };

			var references = new List<PortableExecutableReference>();
			foreach(var s in sRef) {
				references.Add(MetadataReference.CreateFromFile(s));
			}

			var tree = CSharpSyntaxTree.ParseText(source);

			var options = new CSharpCompilationOptions(OutputKind.WindowsApplication, allowUnsafe: true);

			var compilation = CSharpCompilation.Create("A", new[] { tree }, references, options);

			MemoryStream ms = inMemoryAsm ? new MemoryStream() : null;

			//Emitting to file is available through an extension method in the Microsoft.CodeAnalysis namespace
			var emitResult = inMemoryAsm ? compilation.Emit(ms) : compilation.Emit(outFile);

			//If our compilation failed, we can discover exactly why.
			if(!emitResult.Success) {
				foreach(var diagnostic in emitResult.Diagnostics) {
					Print(diagnostic.ToString());
				}
				return 1;
			}

			//copy non-.NET references to the output directory
			var outDir = Path_.GetDirectoryPath(outFile);
			var netDir = Folders.Windows + "Microsoft.NET\\";
			for(int i = 1; i < references.Count; i++) {
				var r = references[i];
				var s1 = r.FilePath;
				if(s1.StartsWith_(netDir, true)) continue;
				var s2 = outDir + "\\" + Path_.GetFileName(s1);
				//PrintList(s1, s2);

				if(File_.ExistsAsFile(s2)) {
					FileInfo f1 = new FileInfo(s1), f2 = new FileInfo(s2);
					if(f1.LastWriteTimeUtc == f2.LastWriteTimeUtc && f1.Length == f2.Length) continue;
				}
				File_.Copy(s1, s2, IfExists.Delete);
				//TODO: exception handling
			}

			GC.Collect();

#if true
			var ad = AppDomain.CreateDomain("ad1");
			if(inMemoryAsm) {
				ad.SetData("ms", ms);
				ad.DoCallBack(() =>
				{
					var ad2 =AppDomain.CurrentDomain;
					var ms2 = ad2.GetData("ms") as MemoryStream;
					Assembly asm = ad2.Load(ms2.ToArray());
					asm.EntryPoint.Invoke(null, null);
				});
			} else {
				ad.ExecuteAssembly(outFile);
			}
			AppDomain.Unload(ad);
#endif

			return 0;
		}

		//		static int _Compile()
		//		{
		//			//Perf.First(100);
		//			Perf.Next(); //3 ms first time (ngen-compiled), then <50 mcs

		//			var dom = AppDomain.CurrentDomain;
		//			string csFile = (string)dom.GetData("cs"), outFile = (string)dom.GetData("out");

		//			//Print(csFile);
		//			//Print(outFile);

		//			var source = File.ReadAllText(csFile);
		////			var source = @"
		////using System;
		////public static class Test{
		////public static int Main(){
		////return 1+2;
		////}
		////}
		////";

		//			var sRef = new string[] { typeof(object).Assembly.Location, Folders.ThisApp + "Au.dll" };
		//			//var sRef = new string[] { typeof(object).Assembly.Location };

		//			var perf = Perf.StartNew();
		//			var references = new List<PortableExecutableReference>();
		//			foreach(var s in sRef) {
		//				references.Add(MetadataReference.CreateFromFile(s));
		//			}
		//			perf.Next();

		//			var tree = CSharpSyntaxTree.ParseText(source);
		//			perf.Next();

		//			CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.WindowsApplication, allowUnsafe: true);

		//			var compilation = CSharpCompilation.Create("A", new[] { tree }, references, options);
		//			perf.Next();

		//			//Emit to stream
		//			var ms = new MemoryStream();
		//			var emitResult = compilation.Emit(ms);
		//			perf.Next();

		//			//If our compilation failed, we can discover exactly why.
		//			if(!emitResult.Success) {
		//				foreach(var diagnostic in emitResult.Diagnostics) {
		//					Print(diagnostic.ToString());
		//				}
		//			} else {
		//				//Load into currently running assembly. Normally we'd probably
		//				//want to do this in an AppDomain
		//				var ourAssembly = Assembly.Load(ms.ToArray());
		//				var type = ourAssembly.GetType("Test");
		//			perf.Next();

		//				//Invokes our main method and writes "Hello World" :)
		//				//object r=type.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);
		//				//Print(r);
		//				type.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);
		//			perf.NW();
		//			}

		//			return 0;
		//		}


		//static async void _TestScript()
		//{
		//	Perf.Next();
		//	object result = await CSharpScript.EvaluateAsync("1 + 2");
		//	Print(result);
		//}
#else
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
				Assembly asm = Assembly.LoadFile(Folders.ThisApp + "csc.exe");
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
				$"/r:{Folders.ThisApp}\\Au.dll",
				//"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
				csFile
			};
			object[] p = new object[1] { g };

			//g[0]="/?";

			_compilerOutput.Clear();
			int r = (int)_compilerMethod.Invoke(0, p); //18-23 ms minimal (everything ngen-compiled, recently PC restarted), 33 ms with several /r; first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			if(r != 0) {
				Print(_compilerOutput);
			} else if(_compilerOutput.Length > 0) {
				Print(_compilerOutput);
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
			//ShowRen.Show("exit");

			return r;
		}
#endif
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
