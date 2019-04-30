using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Au;
using Au.Types;
using static Au.NoClass;

using YamlDotNet.RepresentationModel;
using System.Net;

[module: DefaultCharSet(CharSet.Unicode)]

//note: DocFX does not replace/modify the yml files if the source code of the C# project not changed. Then ProcessYamlFile not called.
//	To apply changes of this script, change something in C# XML comments, save, then run this script.

unsafe class Program
{
	static void Main(string[] args)
	{
		try { _Main(); }
		catch(Exception e) { Print(e); }
	}

	static void _Main()
	{
		bool isConsole = Output.IsConsoleProcess;
		if(!isConsole) {
			Output.QM2.UseQM2 = true;
			Output.Clear();
		}

		var docfx = @"Q:\Programs\DocFx\docfx.exe";
		var objDir = @"Q:\Temp\Au\DocFX\obj";
		var docDir = @"Q:\app\Au\Au\_doc";
		var siteDir = docDir + @"\_site";
		var apiDir = docDir + @"\api";

		//ProcessYamlFile(apiDir + @"\Au.AaaDocFX.yml", true); return;
		//ProcessHtmlFiles(siteDir, true); return;

		//Compress(docDir); return;
		//Upload(docDir); return;
		//CompressAndUpload(docDir); return;

		foreach(var v in Process.GetProcessesByName("docfx")) v.Kill();
		if(isConsole) {
			int k = 0;
			foreach(var v in Wnd.FindAll(@"C:\WINDOWS\system32\cmd.exe", "ConsoleWindowClass")) { if(k++ > 0) v.Close(); }
		}

		File_.Delete(siteDir);
		Directory.SetCurrentDirectory(docDir);

		var t1 = Time.PerfMilliseconds;

		using(var fw = new FileSystemWatcher(apiDir, "*.yml")) {
			fw.Changed += (sen, e) => { if(e.Name.StartsWithI_("Au.")) ProcessYamlFile(e.FullPath, false); };
			fw.EnableRaisingEvents = true;
			fw.NotifyFilter = NotifyFilters.LastWrite;

			bool serving = false;
			try {
				Shell.RunConsole(o => {
					Print(o);
					if(o.StartsWith_("Serving")) throw new OperationCanceledException();
				}, docfx, $@"docfx.json --intermediateFolder ""{objDir}"" --serve");
			}
			catch(OperationCanceledException) {
				serving = true;
			}
			//if(!serving) { AuDialog.Show("error?"); return; } //need if this process is not hosted
			if(!serving) return;
		}

		var t2 = Time.PerfMilliseconds;

		ProcessHtmlFiles(siteDir, false);

		var t3 = Time.PerfMilliseconds; Print("speed (s):", (t2 - t1) / 1000, (t3 - t2) / 1000);

		Wnd.Find("Au - Microsoft Visual Studio ").Activate();
		Wnd.Find("* Chrome").Activate();
		Key("F5");

		1.s();
		if(AuDialog.ShowYesNo("Upload?")) CompressAndUpload(docDir);

		//Delete obj folder if > 1 GB. Each time it grows by 10 MB, and after a day or two can be > 1 GB. After deleting builds slower by ~50%.
		if(File_.Misc.CalculateDirectorySize(objDir) / 1024 / 1024 > 1000) { Print("Deleting obj folder."); File_.Delete(objDir); }
	}

	/// <summary>
	/// Workaround for DocFX bug: applies markdown to inline comments. It can damage whole text.
	/// Also something more.
	/// </summary>
	static void ProcessYamlFile(string path, bool test)
	{
		//Print(path);
		try {
			//var text = File.ReadAllText(path);
			var yaml = new YamlStream();
			using(var fs = File.OpenText(path)) yaml.Load(fs);

			var root = (YamlMappingNode)yaml.Documents[0].RootNode;

			var mark = new YamlScalarNode("au");
			if(root.Children.ContainsKey(mark)) return;

			bool save = false;
			foreach(YamlMappingNode item in (YamlSequenceNode)root.Children[new YamlScalarNode("items")]) {
				foreach(var name in s_names) {
					if(!item.Children.TryGetValue(name, out var node)) continue;
					//Print("----", name, node.NodeType);
					if(node is YamlScalarNode scalar) { //summary, remarks
						if(ProcessYamlValue(scalar)) save = true;
					} else if(node is YamlMappingNode map) { //syntax
						if(map.Children.TryGetValue("parameters", out node)) {
							foreach(YamlMappingNode par in node as YamlSequenceNode) {
								if(!par.Children.TryGetValue("description", out node)) continue;
								if(ProcessYamlValue(node as YamlScalarNode)) save = true;
							}
						}
						if(map.Children.TryGetValue("return", out node)) {
							if(!(node as YamlMappingNode).Children.TryGetValue("description", out node)) continue;
							if(ProcessYamlValue(node as YamlScalarNode)) save = true;
						}
					} else if(node is YamlSequenceNode seq) { //exceptions, example
						switch(name) {
						case "example":
							foreach(var v in seq) {
								if(ProcessYamlValue(v as YamlScalarNode)) save = true;
							}
							break;
						case "exceptions":
							foreach(YamlMappingNode v in seq) {
								if(!v.Children.TryGetValue("description", out node)) continue;
								if(ProcessYamlValue(node as YamlScalarNode)) save = true;
							}
							break;
						}
					}
				}
			}
			if(!save) return;
			root.Add(mark, "true");
			var tmp = path + ".tmp";
			using(var sw = File.CreateText(tmp)) {
				sw.WriteLine("### YamlMime:ManagedReference");
				yaml.Save(sw, false);
			}
			if(test) {
				//Print(File.ReadAllText(tmp));
			} else {
				File_.Move(tmp, path, IfExists.Delete);
			}
		}
		catch(Exception e) { Print(e); }
	}
	static string[] s_names = { "summary", "remarks", "example", "syntax", "exceptions" };

	static bool ProcessYamlValue(YamlScalarNode scalar)
	{
		//return false;
		var s = scalar.Value;
		int R = 0;

		//Print(s);
		//To avoid applying markdown in <c>...</c> (now <code>...</code>), enclose in <au><!--...--></au>.
		//	Markdown is not applied inside any <...>, including HTML comment. Also add <au>, else may not add <p> etc.
		//	Will remove the enclosing later, when processing HTML.
		//	Another way - escape markdown characters in <c>...</c> (prepend \). Problem: markdown is not applied in HTML blocks, eg HTML tables, except in certain parts.
		if(0 != s.RegexReplace_(@"(?s)(?<!<pre>)<code>.+?</code>", @"<au><!--$0--></au>", out s)) R |= 1;

		//Use C# colors in code blocks. Without it the javascript would guess, often incorrectly.
		if(0 != s.RegexReplace_(@"<pre><code>", @"<pre><code class=""cs"">", out s)) R |= 2;

		if(R == 0) return false;
		//if(0 != (R & 2)) Print(s);
		scalar.Value = s;
		return true;
	}

	static void ProcessHtmlFiles(string siteDir, bool test)
	{
		string apiDir = siteDir + @"\api";
		foreach(var f in File_.EnumDirectory(apiDir, 0, o => o.Name.Like_(test ? "Au.AaaDocFX*.html" : "Au.*.html"))) {
			var file = f.FullPath;
			//if(test) Print($"<><c 0xff>{file}</c>");
			var s = File.ReadAllText(file);
			bool modified = ProcessHtmlFile(ref s);
			if(modified) File.WriteAllText(!test ? file : file.Remove(file.Length - 1), s);
		}
	}

	static bool ProcessHtmlFile(ref string s)
	{
		//Print(s.Substring(s.IndexOf_("<h1 id=")));

		int nr = 0;

		//Remove the <au><!--<code>*xml*</code>--></au> enclosing.
		nr += s.RegexReplace_(@"<au><!--(<code>.+?</code>)--></au>", @"$1", out s);

		//<msdn>...</msdn> -> <a href="google search">
		nr += s.RegexReplace_(@"<msdn>(.+?)</msdn>", @"<a href=""https://www.google.com/search?q=site:docs.microsoft.com+$1"">$1</a>", out s);

		//Link Method(parameters) -> Type.Method. And remove #jump. Works for properties too.
		//Exclude those in auto-generated tables of class methods and properties.
		nr += s.RegexReplace_(@"(<a class=""xref"" href=""Au\.(?:Types\.|Triggers\.|Util\.)?+([\w\.\-]+\.\w+)\.html)#\w+"">.+?</a>(?!\s*</td>\s*<td class=""markdown level1 summary"">)", @"$1"">$2</a>", out s);
		//the same for enum
		nr += s.RegexReplace_(@"(<a class=""xref"" href=""Au\.(?:Types\.|Triggers\.|Util\.)?+(\w+)\.html)#\w+"">(\w+</a>)", @"$1"">$2.$3", out s); //note: enums must not be nested in types

		//remove double <p>. It inserts spaces above and below parameter descriptions etc.
		//	tested: trimming newlines in ProcessYamlValue does not help.
		nr += s.RegexReplace_(@"<p>\s*<p>", @"<p>", out s);
		nr += s.RegexReplace_(@"</p>\s*</p>", @"</p>", out s); //dangerous: can be eg <p>text<p>text</p></p>

		//remove <p> before </td>
		nr += s.RegexReplace_(@"<p>\s*</td>", @"</td>", out s);

		//In class member pages, in title insert a link to the type.
		nr += s.RegexReplace_(@"<h1\b[^>]* data-uid=""(Au\.(?:Types\.|Triggers\.|Util\.)?+([\w\.`]+))\.\w+\*?""[^>]*>\w+ (?=\w)",
			m => m.ExpandReplacement(@"$0<a href=""$1.html"">$2</a>.").Replace("`", "-"),
			out s);

		//nr += s.RegexReplace_(@"", @"", out s);

		//nr += s.RegexReplace_(@"", @"", out s);

		//Print(s.Substring(s.IndexOf_("<h1 id=")));

		return nr > 0;
	}

	static void CompressAndUpload(string docDir)
	{
		Compress(docDir);
		Upload(docDir);
	}

	static void Compress(string docDir)
	{
		var sevenZip = @"C:\Program Files\7-Zip\7z.exe";

		File_.Delete(docDir + @"\_site.tar");
		File_.Delete(docDir + @"\_site.tar.bz2");

		int r1 = Shell.RunConsole(out var s, sevenZip, $@"a _site.tar .\_site\*", docDir);
		if(r1 != 0) { Print(s); return; }
		int r2 = Shell.RunConsole(out s, sevenZip, $@"a _site.tar.bz2 _site.tar", docDir);
		if(r2 != 0) { Print(s); return; }

		File_.Delete(docDir + @"\_site.tar");

		Print("Compressed");
	}

	static void Upload(string docDir)
	{
		if(!Registry_.GetString(out var user, "kas", @"\Help")
			|| !Registry_.GetString(out var pass, "kaip", @"\Help")
			|| !Registry_.GetString(out var pass2, "kaip2", @"\Help")
			) throw new FileNotFoundException("user or password not found in registry");

		//upload
		pass = Encoding.UTF8.GetString(Convert_.Base64Decode(pass));
		var name = @"\_site.tar.bz2";
		using(var client = new WebClient()) {
			client.Credentials = new NetworkCredential(user, pass);
			client.UploadFile("ftp://ftp.quickmacros.com/public_html/qm3" + name, WebRequestMethods.Ftp.UploadFile, docDir + name);
		}
		Print("Uploaded");

		//extract
		using(var client = new WebClient()) {
			string r1 = client.DownloadString($"http://www.quickmacros.com/qm3/extract_help.php?kaip={pass2}");
			if(r1 != "done") { Print(r1); return; }
		}
		Print("<>Extracted to <link>http://www.quickmacros.com/qm3/help/</link>");
	}

	/* Q:\Programs\eclipse\workspace\test\extract_help.php:

<?php

if($_REQUEST['kaip'] != '{pass2}') die('invalid data'); //replace {pass2} with the registry value

$bz2=__DIR__ . '/_site.tar.bz2';
$help=__DIR__ . '/help';
$tar=__DIR__ . '/_site.tar';
rrmdir($help);
if(is_file($tar)) unlink($tar);
$p = new PharData($bz2);
$p->decompress(); // creates $tar
$p = new PharData($tar);
$p->extractTo($help); // extract all files
unlink($tar);
echo("done");

function rrmdir($src) {
    if(!is_dir($src)) return;
    $dir = opendir($src);
    while(false !== ( $file = readdir($dir)) ) {
        if (( $file != '.' ) && ( $file != '..' )) {
            $full = $src . '/' . $file;
            if ( is_dir($full) ) {
                rrmdir($full);
            }
            else {
                unlink($full);
            }
        }
    }
    closedir($dir);
    rmdir($src);
}

?>

	*/
}
