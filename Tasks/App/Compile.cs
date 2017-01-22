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
			//Print(AppDomain.CurrentDomain.FriendlyName);
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
			//Print(AppDomain.CurrentDomain.FriendlyName);
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
		//		Print(AppDomain.CurrentDomain.FriendlyName);
		//	}
		//}

#if ROSLYN
	static void TestRoslyn()
	{
		//Print("test");
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
