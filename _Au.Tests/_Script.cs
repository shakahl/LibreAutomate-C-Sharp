using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Au;
using Au.Types;
using static Au.AStatic;
using Au.Triggers;
using Au.Controls;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime;
using Microsoft.Win32;

class Script :AScript
{
	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();

		//Print(Api.GetOEMCP());
		//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//var e = Encoding.GetEncoding(Api.GetOEMCP());
		//Print(e.EncodingName);

		//AExec.RunConsole(@"C:\Program Files\dotnet\dotnet.exe", "--info");

		//Print("test");

		//test: native exceptions such as access violation.

		//200.ms();
		//int i = 0;
		//foreach(var f in AFile.EnumDirectory(@"Q:\Test\ok\.compiled")) {
		//	if(f.IsDirectory || f.Size>6_000) continue;
		//	Print(f.Name);
		//	APerf.First();
		//	//var a = File.ReadAllBytes(f.FullPath);
		//	//using var fs = File.OpenRead(f.FullPath);
		//	using var fs = OpenRead(f.FullPath);
		//	APerf.Next();
		//	var a = new byte[fs.Length];
		//	fs.Read(a, 0, a.Length);
		//	APerf.NW();
		//	if(++i > 7) break;
		//}

		//Print(args);

		//string s = "one two three";
		////var r = s[4..7];
		////Print(r);
		//int i = 4, j = 7;
		//var ii = ^2;
		//var r = s[i..ii];
		//Print(r);
		//int i = 5;
		//Print(TestRange(s));

		//Span<char> k= new Span<char>(

	}

	//FileStream OpenRead(string file)
	//{
	//	var h = Api.CreateFile(file, Api.GENERIC_READ, Api.FILE_SHARE_ALL, default, Api.OPEN_EXISTING);
	//	return new FileStream(h, FileAccess.Read, ownsHandle: true);
	//}

	string TestRange(string s, Range? range=null)
	{
		var r = range ?? Range.All;
		int i = r.Start.Value;
		Print(r, r.Start, r.End, i);
		return s[r];
	}
}
