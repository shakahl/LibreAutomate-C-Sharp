using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Au;
using Au.Types;
using static Au.NoClass;

[module: DefaultCharSet(CharSet.Unicode)]

unsafe class Program
{
	static void Main(string[] args)
	{
		var docfx = @"Q:\Programs\DocFx\docfx.exe";
		var doc = @"Q:\app\Au\Au\_doc";
		Directory.SetCurrentDirectory(doc);

		var w = Wnd.Find(@"C:\Windows\System32\cmd.exe - * --serve", "ConsoleWindowClass");
		if(!w.Is0) w.Close();

		var site = @"Q:\app\Au\Au\_doc\_site";
		File_.Delete(site);

		var obj = @"Q:\Temp\Au\DocFX\obj";

		var t1 = Time.PerfMilliseconds;

		//Shell.Run("cmd.exe", $@"/K {docfx} docfx.json --intermediateFolder ""{obj}"" --serve", more: doc);
		//10.s();
		//w = Wnd.Find(@"C:\Windows\System32\cmd.exe - * --serve", "ConsoleWindowClass").OrThrow();

		//var p = new ProcessStartInfo(docfx, $@"docfx.json --intermediateFolder ""{obj}"" --serve");
		//p.UseShellExecute = false;
		//Process.Start(p);

		//10.s();

		//Shell.RunConsole(docfx, $@"docfx.json --intermediateFolder ""{obj}""");
		//RunConsole(null, null, docfx, $@"docfx.json --intermediateFolder ""{obj}""");

		Output.QM2.UseQM2 = true;
		Output.Clear();
		Print(Shell.RunConsole(o => Print(o), @"Q:\My QM\console4.exe"));

		//3.s();

		var t2 = Time.PerfMilliseconds;
		Print((t2 - t1) / 1000);
	}

}
