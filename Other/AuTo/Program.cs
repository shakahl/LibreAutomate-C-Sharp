//This small program loads and executes other Au exe assemblies.
//Purpose: minimize code signing. Then can sign this single file once, and others can be unsigned.
//note: cannot load assembly of different runtime version, eg framework 3.5 -> 4.0. But can load eg framework 4.0 -> 4.6.2 (both use runtime 4.0).
//note: then setup programs must be .NET assemblies too.
//note: then app.config must be like of other projects.
//But maybe will not sign all exe. Only setup. Because, if they will execute user scripts, if some scripts are virus, then the host exe and certificate may be blacklisted.

using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		//MessageBox.Show(Environment.CommandLine);
		//return;

		//TEST: instead of command line args, use AuTo copies with different names.
		//TODO: remove icons from all other exe.

		var s = "Editor";
		if(args.Length >= 2 && args[0] == "/e") {
			int skipArgs = 2; bool install = false;
			switch(s = args[1]) {
			case "Setup":
				install = true;
				goto case "uninstall";
			case "uninstall":
				try {
					//restart as admin. Even if already admin, never mind.
					if(!(args.Length > 2 && args.Contains("/admin"))) {
						s = Environment.CommandLine;
						int i = s.IndexOf(" /e ");
						string path;
						if(install) {
							var f = Path.GetTempPath() + "Au.Setup";
							if(Directory.Exists(f)) Directory.Delete(f, true);
							Directory.CreateDirectory(f);
							var loc = Assembly.GetExecutingAssembly().Location;
							path = f + "\\" + Path.GetFileName(loc);
							File.Copy(loc, path);
						} else path = s.Remove(i).Trim('\"');
						var psi = new ProcessStartInfo(path, s.Substring(i + 1) + " /admin") { UseShellExecute = true, Verb = "runas" };
						var p = Process.Start(psi);
						p.WaitForExit();
						Environment.ExitCode = p.ExitCode;
						return;
					}
					//MessageBox.Show("running as admin");
					if(install) {
						//now we are in %TEMP%\Au.Setup directory, and running as admin
						var f = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
						File.Copy(@"Q:\app\Au\_\Au.Setup.exe", f + @"\Au.Setup.exe");
						File.Copy(@"Q:\app\Au\_\Au.dll", f + @"\Au.dll");
#else
						using(var web = new System.Net.WebClient()) {
							web.DownloadFile("http://www.quickmacros.com/com/Au.Setup.exe", f + @"\Au.Setup.exe");
						}
#endif
					} else {
						s = "Setup";
						skipArgs = 1;
					}
				}
				catch(Exception e) {
					MessageBox.Show(e.ToString(), "Setup failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				break;
			}
			args = args.Skip(skipArgs).ToArray();
		}

		try {
			var x = Assembly.Load("Au." + s + ", Culture=neutral, PublicKeyToken=112db45ebd62e36d");
			x.EntryPoint.Invoke(null, new object[] { args });
		}
		catch(Exception e) {
			MessageBox.Show(e.ToString(), "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
	}
}
