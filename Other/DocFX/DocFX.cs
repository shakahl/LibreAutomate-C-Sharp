#define SERVE
#define SSH

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

using YamlDotNet.RepresentationModel;
using System.Net;
using Microsoft.Win32;
using Renci.SshNet;

[module: DefaultCharSet(CharSet.Unicode)]

//NOTES
//DocFX should use latest VS.

//When updating DocFX, also may need to update the "memberpage" NuGet package, and its name in docfx.json.

//DocFX does not replace/modify the yml files if the source code of the C# project not changed. Then ProcessYamlFile not called.
//	To apply changes of this script, change something in C# XML comments, save, then run this script.

//info: we don't use the full text search feature.
//	Uses CPU 10-20 s after each page [re]load.
//	Results are poorly sorted.
//	The Google site search does it much better, and often faster.
//	info: To enable it, add "_enableSearch": true in "globalMetadata".

unsafe class Program
{
	static void Main(string[] args) {
		try { _Main(); }
		catch (Exception e) { print.it(e); }
	}

	static void _Main() {
		bool isConsole = print.isConsoleProcess;
		if (!isConsole) {
			//print.qm2.use = true;
			print.clear();
		}

		var docfx = @"Q:\Programs\DocFx\docfx.exe";
		var objDir = @"Q:\Temp\Au\DocFX\obj";
		var docDir = @"Q:\app\Au\Other\DocFX\_doc";
		var siteDir = docDir + @"\_site";
		var apiDir = docDir + @"\api";

		//ProcessYamlFile(apiDir + @"\Au.AaaDocFX.yml", true); return;
		//ProcessHtmlFiles(siteDir, true); return;
		//ProcessToc(siteDir); return;
		//XrefMap(siteDir); return;

		//Compress(docDir); return;
		//Upload(docDir); return;
		//CompressAndUpload(docDir); return;

		PreprocessSource();
		//return;

		foreach (var v in Process.GetProcessesByName("docfx")) v.Kill();
		if (isConsole) {
			int k = 0;
			foreach (var v in wnd.findAll(@"C:\WINDOWS\system32\cmd.exe", "ConsoleWindowClass")) { if (k++ > 0) v.Close(); }
		}

		filesystem.delete(siteDir);
		Directory.SetCurrentDirectory(docDir);

		var t1 = perf.ms;

		using (var fw = new FileSystemWatcher(apiDir, "*.yml")) {
			fw.Changed += (sen, e) => {
				//print.it(e.Name);
				if (e.Name.Starts("Au.", true)) ProcessYamlFile(e.FullPath, false);
			};
			fw.EnableRaisingEvents = true;
			fw.NotifyFilter = NotifyFilters.LastWrite;

#if SERVE
			bool serving = false;
			try {
#endif
				int r = run.console(o => {
					print.it(o);
					if (o.Starts("Serving")) throw new OperationCanceledException();
				}, docfx, $@"docfx.json --intermediateFolder ""{objDir}"""
				//+ " --force"
#if SERVE
				+ " --serve"
#endif
				);
#if SERVE
			}
			catch (OperationCanceledException) {
				serving = true;
			}
			//if(!serving) { dialog.show("error?"); return; } //need if this process is not hosted
			if (!serving) return;
#else
			//print.it(r);
			if (r != 0) return;
#endif
		}

		var t2 = perf.ms;

		ProcessHtmlFiles(siteDir, false);

		var t3 = perf.ms; print.it("speed (s):", (t2 - t1) / 1000, (t3 - t2) / 1000);

		//wnd.find("* Chrome").Activate();
		//keys.send("F5");

		1.s();
		if (1 == dialog.show("Upload?", null, "1 Yes|2 No"/*, secondsTimeout: 5*/)) CompressAndUpload(docDir);

		//Delete obj folder if big. Each time it grows by 10 MB, and after a day or two can be > 1 GB. After deleting builds slower by ~50%.
		if (filesystem.more.calculateDirectorySize(objDir) / 1024 / 1024 > 500) { print.it("Deleting obj folder."); filesystem.delete(objDir); }
		//info: if DocFX starts throwing stack overflow exception, delete the obj folder manually. It is likely to happen after many refactorings in the project.
	}

	//preprocess and copy source files, because DocFX does not support latest C# features.
	//note: if using this, in docfx.json change src: "src": "Q:/Temp/Au/DocFX/source/"
	static void PreprocessSource() {
		var sourceDir1 = @"Q:\app\Au\Au";
		var sourceDir2 = @"Q:\Temp\Au\DocFX\source";
		filesystem.delete(sourceDir2);
		filesystem.createDirectory(sourceDir2);

		var usings = File.ReadAllText(sourceDir1 + @"\resources\global.cs");
		if (0 == usings.RxReplace(@"(?s)^.*#if !NO_GLOBAL\R(.+?)#else\R.+", "$1", out usings, 1)) throw new Exception("bad regex");
		usings = usings.RxReplace(@"(?m)^global ", "");

		foreach (var v in Directory.EnumerateFiles(sourceDir1, "*", SearchOption.AllDirectories)) {
			if (0 == v.Ends(true, ".cs", ".csproj")) continue;
			if (0 != v.Starts(true, @"Q:\app\Au\Au\bin\", @"Q:\app\Au\Au\obj\")) continue;
			if (v.Ends(@"\global.cs", true)) continue;

			var v2 = sourceDir2 + v[sourceDir1.Length..];
			//print.it(v, v2);
			if (v.Ends(".cs", true)) _PreprocessFile(v, v2);
			else filesystem.copy(v, v2);
		}

		void _PreprocessFile(string fileFrom, string fileTo) {
			var s = File.ReadAllText(fileFrom);

			//namespace X; -> namespace X { ... }
			//s = s.RxReplace(@"(?ms)^(namespace Au(?:\.\w+)?);(.+)", "$1{$2\r\n}", 1);

			s = s.RxReplace(@"\brecord (struct|class)\b", "$1");

			s = usings + s;

			filesystem.saveText(fileTo, s);
		}
	}

	/// <summary>
	/// Workaround for DocFX bug: applies markdown in inline code. It can damage whole text.
	/// Also something more.
	/// </summary>
	static void ProcessYamlFile(string path, bool test) {
		//print.it(path);
		try {
			//var text = File.ReadAllText(path);
			var yaml = new YamlStream();
			using (var fs = File.OpenText(path)) yaml.Load(fs);

			var root = (YamlMappingNode)yaml.Documents[0].RootNode;

			var mark = new YamlScalarNode("au");
			if (root.Children.ContainsKey(mark)) return;

			bool save = false;
			foreach (YamlMappingNode item in (YamlSequenceNode)root.Children[new YamlScalarNode("items")]) {
				foreach (var name in s_names) {
					if (!item.Children.TryGetValue(name, out var node)) continue;
					//print.it("----", name, node.NodeType);
					if (node is YamlScalarNode scalar) { //summary, remarks
						if (ProcessYamlValue(scalar)) save = true;
					} else if (node is YamlMappingNode map) { //syntax
						if (map.Children.TryGetValue("parameters", out node)) {
							foreach (YamlMappingNode par in node as YamlSequenceNode) {
								if (!par.Children.TryGetValue("description", out node)) continue;
								if (ProcessYamlValue(node as YamlScalarNode)) save = true;
							}
						}
						if (map.Children.TryGetValue("return", out node)) {
							if (!(node as YamlMappingNode).Children.TryGetValue("description", out node)) continue;
							if (ProcessYamlValue(node as YamlScalarNode)) save = true;
						}
					} else if (node is YamlSequenceNode seq) { //exceptions, example
						switch (name) {
						case "example":
							foreach (var v in seq) {
								if (ProcessYamlValue(v as YamlScalarNode)) save = true;
							}
							break;
						case "exceptions":
							foreach (YamlMappingNode v in seq) {
								if (!v.Children.TryGetValue("description", out node)) continue;
								if (ProcessYamlValue(node as YamlScalarNode)) save = true;
							}
							break;
						}
					}
				}
			}
			if (!save) return;
			root.Add(mark, "true");
			var tmp = path + ".tmp";
			using (var sw = File.CreateText(tmp)) {
				sw.WriteLine("### YamlMime:ManagedReference");
				yaml.Save(sw, false);
			}
			if (test) {
				//print.it(File.ReadAllText(tmp));
			} else {
				filesystem.move(tmp, path, FIfExists.Delete);
			}
		}
		catch (Exception e) { print.it(e); }
	}
	static string[] s_names = { "summary", "remarks", "example", "syntax", "exceptions" };

	static bool ProcessYamlValue(YamlScalarNode scalar) {
		//return false;
		var s = scalar.Value;
		int R = 0;

		//print.it(s);
		//To avoid applying markdown in <c>...</c> (now <code>...</code>), enclose in <au><!--...--></au>.
		//	Markdown is not applied inside any <...>, including HTML comment. Also add <au>, else may not add <p> etc.
		//	Will remove the enclosing later, when processing HTML.
		//	Another way - escape markdown characters in <c>...</c> (prepend \). Problem: markdown is not applied in HTML blocks, eg HTML tables, except in certain parts.
		if (0 != s.RxReplace(@"(?s)(?<!<pre>)<code>.+?</code>", @"<au><!--$0--></au>", out s)) R |= 1;

		//Use C# colors in code blocks. Without it the javascript would guess, often incorrectly.
		if (0 != s.RxReplace(@"<pre><code>", @"<pre><code class=""cs"">", out s)) R |= 2;

		if (R == 0) return false;
		//if(0 != (R & 2)) print.it(s);
		scalar.Value = s;
		return true;
	}

	static void ProcessHtmlFiles(string siteDir, bool test) {
		string files = "*.html";
		if (test) {
			//files = @"\api\Au.AaaDocFX*";
			//files = @"\api\Au.regexp.Replace";
			files = @"\api\Au.elmFinder.this";
			files = @"\api\Au.dialog.show";
			files = @"\articles\Wildcard expression";
			files += ".html";
		}
		foreach (var f in filesystem.enumerate(siteDir, FEFlags.AndSubdirectories | FEFlags.NeedRelativePaths)) {
			if (f.IsDirectory) continue;
			var name = f.Name; if (!name.Like(files, true) || name.Ends(@"\toc.html")) continue;
			var file = f.FullPath;
			//if(test) print.it($"<><c 0xff>{file}</c>");
			var s = File.ReadAllText(file);
			bool modified = ProcessHtmlFile(ref s, name.Starts(@"\api"), siteDir);
			if (modified) File.WriteAllText(!test ? file : file.Remove(file.Length - 1), s);
		}

		ProcessJs(siteDir);
	}

	static bool ProcessHtmlFile(ref string s, bool isApi, string siteDir) {
		int nr = 0;

		if (isApi) {
			//Remove the <au><!--<code>*xml*</code>--></au> enclosing.
			nr += s.RxReplace(@"<au><!--(<code>.+?</code>)--></au>", @"$1", out s);

			//Link Method(parameters) -> Type.Method. And remove #jump. Works for properties too.
			//Exclude those in auto-generated tables of class methods and properties.
			nr += s.RxReplace(@"(<a class=""xref"" href=""Au\.(?:Types\.|Triggers\.|More\.)?+([\w\.\-]+\.\w+)\.html)#\w+"">.+?</a>(?!\s*</td>\s*<td class=""markdown level1 summary"">)", @"$1"">$2</a>", out s);
			//the same for enum
			nr += s.RxReplace(@"(<a class=""xref"" href=""Au\.(?:Types\.|Triggers\.|More\.)?+(\w+)\.html)#\w+"">(\w+</a>)", @"$1"">$2.$3", out s); //note: enums must not be nested in types

			//In class member pages, in title insert a link to the type.
			nr += s.RxReplace(@"<h1\b[^>]* data-uid=""(Au\.(?:Types\.|Triggers\.|More\.)?+([\w\.`]+))\.\w+\*?""[^>]*>(?:Method|Property|Field|Event|Operator|Constructor) (?=\w)",
				m => m.ExpandReplacement(@"$0<a href=""$1.html"">$2</a>.").Replace("`", "-"),
				out s);

			//Remove anchor from the first hidden overload <hr>, to prevent scrolling.
			nr += s.RxReplace(@"(</h1>\s*<hr class=""overload"") id="".+?"" data-uid="".+?""", @"$1", out s);

			//Add "(+ n overloads)" link in h1 and "(next/top)" links in h2 if need.
			if (s.RxFindAll(@"<h2 class=""overload"" id=""(.+?)"".*?>Overload", out var a) && a.Length > 1) {
				var b = new StringBuilder();
				int jPrev = 0;
				for (int i = 0; i < a.Length; i++) {
					bool first = i == 0, last = i == a.Length - 1;
					int j = first ? s.Find("</h1>") : a[i].End;
					b.Append(s, jPrev, j - jPrev);
					jPrev = j;
					b.Append("<span style='font-size:14px; font-weight: 400; margin-left:20px;'>(");
					if (first) b.Append("+ ").Append(a.Length - 1).Append(" ");
					var href = last ? "top" : a[i + 1][1].Value;
					b.Append("<a href='#").Append(href).Append("'>");
					if (first) b.Append("overload").Append(a.Length == 2 ? "" : "s");
					else b.Append(last ? "back to top" : "next");
					b.Append("</a>)</span>");
				}
				b.Append(s, jPrev, s.Length - jPrev);
				s = b.ToString();
				nr++;
			}

		} else {
			//in .md we use this for links to api: [Class]() or [Class.Func]().
			//	DocFX converts it to <a href="">Class</a> etc without warning.
			//	Now convert it to a working link.
			nr += s.RxReplace(@"<a href="""">(.+?)</a>", m => {
				var k = m[1].Value;
				string href = null;
				foreach (var ns in s_ns) {
					if (filesystem.exists(siteDir + "/api/" + ns + k + ".html").isFile) {
						href = "../api/" + ns + k + ".html";
						break;
					}
				}
				if (href == null) { print.it($"cannot resolve link: [{k}]()"); return m.Value; }
				return m.ExpandReplacement($@"<a href=""{href}"">$1</a>");
			}, out s);
		}

		//<google>...</google> -> <a href="google search">
		nr += s.RxReplace(@"<google>(.+?)</google>", @"<a href=""https://www.google.com/search?q=$1"">$1</a>", out s);

		//<msdn>...</msdn> -> <a href="google search in docs.microsoft.com">
		nr += s.RxReplace(@"<msdn>(.+?)</msdn>", @"<a href=""https://www.google.com/search?q=site:docs.microsoft.com+$1"">$1</a>", out s);

		//javascript renderTables() replacement, to avoid it at run time. Also remove class table-striped.
		nr += s.RxReplace(@"(?s)<table(>.+?</table>)", @"<div class=""table-responsive""><table class=""table table-bordered table-condensed""$1</div>", out s);

		//the same for renderAlerts
		nr += s.RxReplace(@"<div class=""(NOTE|TIP|WARNING|IMPORTANT|CAUTION)\b",
			o => {
				string k = "info"; switch (o[1].Value[0]) { case 'W': k = "warning"; break; case 'I': case 'C': k = "danger"; break; }
				return o.Value + " alert alert-" + k;
			},
			out s);

		//replace something in syntax code blocks
		nr += s.RxReplace(@"(?<=<div class=""codewrapper"">)\s*<pre><code.+?(?=</code></pre>)",
			m => {
				var k = m.Value;
				k = k.RxReplace(@"\(\w+\)0", @"0");
				k = k.RxReplace(@"default\([^)?]+\? *\)", @"null");
				k = k.RxReplace(@"default\(.+?\)", @"default");
				return k;
			}
			, out s);

		//nr += s.RxReplace(@"", @"", out s);

		return nr > 0;
	}
	static string[] s_ns = { "Au.", "Au.Triggers.", "Au.Types.", "Au.More.", };

	//static void ProcessToc(string siteDir)
	//{
	//	var file = siteDir + @"\api\toc.html";
	//	var s = File.ReadAllText(file);

	//	//
	//	if(0==s.RxReplace(@"", @"", out s)) throw new Exception("regex failed");

	//	print.it(s);
	//	//File.WriteAllText(file, s);
	//}

	static void ProcessJs(string siteDir) {
		var file = siteDir + @"\styles\docfx.js";
		var s = File.ReadAllText(file);

		//prevent adding <wbr> in link text
		s = s.Replace("breakText();", "").Replace("$(e).breakWord();", "");

		//we process tables and alerts in HTML at build time. At run time the repainting is visible, and it slows down page loading, + possible scrolling problems.
		s = s.Replace("renderTables();", "").Replace("renderAlerts();", "");

		//prevent adding footer when scrolled to the bottom
		s = s.Replace("renderFooter();", "");
		//Somehow adds anyway. Need to add empty footer.tmpl.partial. But this code line does not harm.

		File.WriteAllText(file, s);
	}

	//From _site\xrefmap.yml extracts conceptual topics and writes to _\xrefmap.yml.
	//Could simply copy the file, but it is ~2 MB, and we don't need api topics.
	//Editor uses it to resolve links in code info.
	static void XrefMap(string siteDir) {
		var b = new StringBuilder();
		var s = File.ReadAllText(siteDir + @"\xrefmap.yml");
		foreach (var m in s.RxFindAll(@"(?m)^- uid:.+\R.+\R  href: (?!api/).+\R", (RXFlags)0)) {
			//print.it(m);
			b.Append(m);
		}

		string f2 = pathname.normalize(siteDir + @"..\..\..\..\..\_\xrefmap.yml");
		//print.it(f2);
		File.WriteAllText(f2, b.ToString());
	}

	static void CompressAndUpload(string docDir) {
		Compress(docDir);
		Upload(docDir);
	}

#if SSH //extracting with ssh is much faster than than with php script, although it seems the host makes it much slower that should be (tested ssh in powershell, the same)
	static void Compress(string docDir) {
		var sevenZip = @"C:\Program Files\7-Zip\7z.exe";

		filesystem.delete(docDir + @"\_site.tar");
		filesystem.delete(docDir + @"\_site.tar.gz");

		int r1 = run.console(out var s, sevenZip, $@"a _site.tar .\_site", docDir);
		if (r1 != 0) { print.it(s); return; }
		int r2 = run.console(out s, sevenZip, $@"a _site.tar.gz _site.tar", docDir);
		if (r2 != 0) { print.it(s); return; }

		filesystem.delete(docDir + @"\_site.tar");

		print.it("Compressed");
	}

	static void Upload(string docDir) {
		var rk = @"HKEY_CURRENT_USER\Software\Au\Help";
		var ip = Registry.GetValue(rk, "kur", null) as string;
		var port = (int)Registry.GetValue(rk, "kur2", null);
		var user = Registry.GetValue(rk, "kas", null) as string;
		var pass = Registry.GetValue(rk, "kaip", null) as string;
		if (ip == null || user == null || pass == null) throw new FileNotFoundException("connection info not found in registry");

		//upload. Use SFTP. Maybe possible with SSH, but I tried scp command and failed.
		pass = Encoding.UTF8.GetString(Convert.FromBase64String(pass)); //to encode: print.it(Convert.ToBase64String(Encoding.UTF8.GetBytes(pass)));
		var name = @"\_site.tar.gz";
		var path = docDir + name;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
		using (var client = new WebClient()) {
			client.Credentials = new NetworkCredential(user, pass);
			//client.UploadFile("ftp://quickmacros.com/public_html/au" + name, WebRequestMethods.Ftp.UploadFile, path);
			client.UploadFile("ftp://185.224.138.106/au" + name, WebRequestMethods.Ftp.UploadFile, path); //public_html is default at hostinger
		}
#pragma warning restore SYSLIB0014 // Type or member is obsolete
		filesystem.delete(path);
		print.it("Uploaded");

		//extract
		//perf.first();
		using (var client = new SshClient(ip, port, user, pass)) {
			client.Connect();
			//_Cmd("cd public_html/test"); _Cmd("pwd"); //cd does not work when separate command. Not tested: ShellStream.
			//_Cmd("cd public_html/test && pwd"); //ok
			//perf.next();
			_Cmd2("tar -zxf _site.tar.gz");
			_Cmd2("rm -r help", silent: true);
			_Cmd2("mv _site help");
			_Cmd2("rm _site.tar.gz", silent: true);
			//perf.nw(); //tar ~20 s, others fast
			client.Disconnect();

			void _Cmd(string s, bool silent = false) {
				var c = client.RunCommand(s);
				//print.it($"ec={c.ExitStatus}, result={c.Result}, error={c.Error}");
				if (!silent && c.ExitStatus != 0) throw new Exception(c.Error);
			}

			void _Cmd2(string s, bool silent = false) => _Cmd("cd public_html/au && " + s, silent);
		}
		print.it("<>Extracted to <link>https://www.quickmacros.com/au/help/</link>");
	}

}
#else
	static void Compress(string docDir)
	{
		var sevenZip = @"C:\Program Files\7-Zip\7z.exe";

		filesystem.delete(docDir + @"\_site.tar");
		filesystem.delete(docDir + @"\_site.tar.bz2");

		int r1 = run.console(out var s, sevenZip, $@"a _site.tar .\_site\*", docDir);
		if(r1 != 0) { print.it(s); return; }
		int r2 = run.console(out s, sevenZip, $@"a _site.tar.bz2 _site.tar", docDir);
		if(r2 != 0) { print.it(s); return; }

		filesystem.delete(docDir + @"\_site.tar");

		print.it("Compressed");
	}

	static void Upload(string docDir)
	{
		var user = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Au\Help", "kas", null) as string;
		var pass = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Au\Help", "kaip", null) as string;
		var pass2 = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Au\Help", "kaip2", null) as string;
		if(user == null || pass == null || pass2 == null) throw new FileNotFoundException("user or password not found in registry");

		//upload
		pass = Encoding.UTF8.GetString(Convert.FromBase64String(pass)); //to encode: print.it(Convert.ToBase64String(Encoding.UTF8.GetBytes(pass)));
		var name = @"\_site.tar.bz2";
		var path = docDir + name;
		using(var client = new WebClient()) {
			client.Credentials = new NetworkCredential(user, pass);
			//client.UploadFile("ftp://quickmacros.com/public_html/au" + name, WebRequestMethods.Ftp.UploadFile, path);
			client.UploadFile("ftp://185.224.138.106/au" + name, WebRequestMethods.Ftp.UploadFile, path);
		}
		filesystem.delete(path);
		print.it("Uploaded");

		//extract
		using(var client = new WebClient()) {
			string r1 = client.DownloadString($"https://www.quickmacros.com/au/extract_help.php?kaip={pass2}");
			if(r1 != "done") { print.it(r1); return; }
			//extracting used to be few seconds, but now with Hostinger maybe several minutes, and sometimes WebException: The operation has timed out.
			//	The slowness is not in rrmdir, because first time was slow too. Or maybe partially in rrmdir.
		}
		print.it("<>Extracted to <link>https://www.quickmacros.com/au/help/</link>");
	}

}

#if Extract // Q:\Programs\eclipse\workspace\test\extract_help.php:

<?php

if($_REQUEST['kaip'] != 'pass2') die('invalid data'); //replace pass2 with the registry value

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

#endif
#endif

#if Disqus //add this to the bottom of help pages

<hr style="margin-top: 50px"/>
<div id="disqus_thread"></div>
<script>
/**
*  RECOMMENDED CONFIGURATION VARIABLES: EDIT AND UNCOMMENT THE SECTION BELOW TO INSERT DYNAMIC VALUES FROM YOUR PLATFORM OR CMS.
*  LEARN WHY DEFINING THESE VARIABLES IS IMPORTANT: https://disqus.com/admin/universalcode/#configuration-variables*/
/*
var disqus_config = function () {
this.page.url = PAGE_URL;  // Replace PAGE_URL with your page's canonical URL variable
this.page.identifier = PAGE_IDENTIFIER; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
};
*/
(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://autepad.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
</script>

#endif
