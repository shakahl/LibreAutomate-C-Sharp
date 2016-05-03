using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

#if MONO
using Mono.CSharp;
#endif

namespace Catkeys.Tasks
{
	class Compile
	{
		public static void Init()
		{
			var thr = new Thread(_AppDomainThread);
			thr.Start();
		}

		static void _AppDomainThread()
		{
			//Out(AppDomain.CurrentDomain.FriendlyName);
			Perf.First();

			//AppDomainSetup domainSetup = new AppDomainSetup()
			//{
			//	ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
			//	ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
			//	ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
			//	LoaderOptimization = LoaderOptimization.MultiDomainHost
			//};

			//var domain=AppDomain.CreateDomain("Compiler", null, domainSetup); //same speed, but need to test later
			var domain = AppDomain.CreateDomain("Compiler");
			Perf.Next();
			//domain.ExecuteAssembly(Paths.CombineApp("Compiler.exe")); //25 ms (5 ms after updating Win 10 to 10576 and performing WinDefender quick scan)
			domain.DoCallBack(_DomainCallback); //22 ms (1 ms after updating||scanning), but need more testing later
												//domain.CreateInstance(typeof(Compile).Assembly.FullName, "Catkeys.Compiler"); //too
												//Perf.Next();
												//domain.DoCallBack(_DomainCallback);
												//Perf.Next();
												//domain.DoCallBack(_DomainCallback);
			Perf.Next();
			AppDomain.Unload(domain);
		}

		static void _DomainCallback()
		{
			//Out(AppDomain.CurrentDomain.FriendlyName);
			//while(true) {
			//	break;
			//}
#if ROSLYN
		TestRoslyn();
#endif
		}

		//class Compiler :MarshalByRefObject
		//{ //note CreateInstance(... "Catkeys.Compiler") fails if this is a child class of Compile, even if both classes are public
		//	public Compiler()
		//	{
		//		Out(AppDomain.CurrentDomain.FriendlyName);
		//	}
		//}

#if MONO
	static void MonoCompile(Assembly a, string inFile, string outFile)
	{
		//var g=new string[] { $"-out:{outFile}", inFile};
		var g=new string[] { "-noconfig", $"-out:{outFile}", inFile};

		CommandLineParser commandLineParser = new CommandLineParser(Console.Out);
		CompilerSettings compilerSettings = commandLineParser.ParseArguments(g);
		if (commandLineParser.HasBeenStopped)
		{
			return;
		}

		CompilerContext c=new CompilerContext(compilerSettings, new ConsoleReportPrinter());
		object o=a.CreateInstance("Mono.CSharp.Driver", false,
				BindingFlags.CreateInstance,
				null, new object[1] {c}, null, null);
				
		MethodInfo mi=o.GetType().GetMethod("Compile");

		//Speed.Next();
		if((bool)mi.Invoke(o, null) && c.Report.Errors==0) //40 ms; with -noconfig 20 ms
		{
			if (c.Report.Warnings > 0)
			{
				Out($"Compilation succeeded - {c.Report.Warnings} warning(s)");
			}
		}
		else
		{
			Out($"Compilation failed: {c.Report.Errors} error(s), {c.Report.Warnings} warnings");
		}

		//Can be used either mcs.ehe or Mono.CSharp.dll. Both have Driver and other classes with same code.
		//However Mono.CSharp.dll is much slower. Maybe mcs is compiled with VS and the dll with Mono?
		//mcs.exe speed: default 40, -noconfig 20
		//Mono.CSharp.dll speed: default 100, -noconfig 50

		//could use this, but Driver is internal, need to use reflection to access it
		//Driver driver = new Driver();
		//if (driver.Compile() && driver.Report.Errors == 0)
		//{
		//	if (driver.Report.Warnings > 0)
		//	{
		//		Out($"Compilation succeeded - {c.Report.Warnings} warning(s)");
		//	}
		//	return;
		//}
		//Out($"Compilation failed: {c.Report.Errors} error(s), {c.Report.Warnings} warnings");
	}

	static void TestMono()
	{
		Speed.First();
		Assembly a = Assembly.Load(@"mcs, Version=4.0.3.0, Culture=neutral, PublicKeyToken=null"); //60 ms
		//Assembly a = Assembly.LoadFile(Path.Combine(Application.StartupPath, "mcs.exe")); //a little slower
		//Assembly a = Assembly.Load(@"Mono.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756"); //60 ms
		Speed.Next();

		for(int i=1; i<=5; i++)
		{
			MonoCompile(a, @"Q:\Test\test.cs", $@"Q:\Test\test{i}.exe");
			Speed.Next();
		}

	}
#endif //#if MONO

#if ROSLYN
	static void TestRoslyn()
	{
		//Out("test");
		//TODO: auto-ngen compiler. Need admin.

		Speed.First();
		//System.Runtime.ProfileOptimization.SetProfileRoot(@"C:\Test");
		//System.Runtime.ProfileOptimization.StartProfile("Startup.Profile"); //does not make jitting the C# compiler assemblies faster
		//Speed.Next();

		//Assembly a = Assembly.LoadFile(@"Q:\Test\Csc\csc.exe"); //error
		//Assembly a = Assembly.LoadFile(@"C:\Program Files (x86)\MSBuild\14.0\Bin\csc.exe"); //error
		Assembly a = Assembly.LoadFile(Paths.CombineApp("csc.exe")); //ok
		//Assembly a = Assembly.Load("csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"); //works, same speed as LoadFile, but VS shows many warnings if this project uses different .NET framework version than csc (which is added to project references). Also, possibly could make the app start slower, especially if HDD. Better to load on demand through reflection.
		MethodInfo m = a.EntryPoint;
		string[] g = new string[] {null, "/nologo", "/noconfig", @"C:\Test\test.cs"};
		object[] p = new object[1] { g };

		//g[0]="/?";
		Speed.Next(); //16 ms
		for(int i=1; i<=4; i++)
		{
			g[0] = $@"/out:C:\Test\test{i}.exe";
			m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			Speed.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Speed.Next();
		}

		//MessageBox.Show("now will GC.Collect");
		GC.Collect();
	}
#endif //#if ROSLYN
	}
}
