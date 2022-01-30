/*/ role miniProgram; r System.Management.dll; r Libraries\YamlDotNet.dll; com WbemScripting 1.2 #55851133.dll; /*/
//using System.Management;
using YamlDotNet.RepresentationModel;

//TODO: <help ...><b> -> <see.

var yaml = new YamlStream();
print.it(yaml);

//Thread.CurrentThread.CurrentCulture = null;
//process.thisProcessCultureIsInvariant=false;
//print.it(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture, CultureInfo.InstalledUICulture);
//process.thisProcessCultureIsInvariant=false;
//print.it(Thread.CurrentThread.CurrentCulture);
//process.thisProcessCultureIsInvariant=true;
//print.it(Thread.CurrentThread.CurrentCulture);
//process.thisProcessCultureIsInvariant=false;
//print.it(Thread.CurrentThread.CurrentCulture);

//print.it(CultureInfo.InstalledUICulture);

//var d = DateTime.Now;

//CultureInfo.CurrentCulture = CultureInfo.InstalledUICulture;
//print.it(d.ToString());
//print.it(d.ToLongDateString() + "; " + d.ToShortTimeString());
//CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

//print.it(d.ToString());
//print.it(d.ToLongDateString() + "; " + d.ToShortTimeString());

//print.it(d.ToString(CultureInfo.InstalledUICulture));

//var c = CultureInfo.InstalledUICulture; //current user culture
//print.it(d.ToString(c));
//print.it(d.ToString("D", c) + "; " + d.ToString("t", c));

//print.it(CultureInfo.GetCultures(CultureTypes.AllCultures));

//var d1 = DateTime.Parse("2022-01-29");
//var d2 = DateTime.Parse("29.01.2022", new CultureInfo("de-DE"));
//print.it(d1, d2);

//var d = DateTime.Now; //get date and time of day
//var dd = d.Date; //get date without time of day
//var t = d.TimeOfDay; //get time of day without date
//print.it(d, dd, t);
//d = DateTime.Today; //get date without time of day

//var now = DateTime.Now;
//var past = new DateTime(2022, 1, 10);
//TimeSpan ts = now.Subtract(past);
//print.it(ts, ts.Days);


//dynamic wmi = Marshal.BindToMoniker("winmgmts:");
//var col = wmi.ExecQuery("SELECT Name FROM Win32_Process", 0, 16|32);
//foreach (var p2 in col) {
//	print.it(p2.Name);
//}

//using WbemScripting;

//var wmi = Marshal.BindToMoniker("winmgmts:") as SWbemServices;
//var col = wmi.ExecQuery("SELECT Name FROM Win32_Process", iFlags: (int)(WbemFlagEnum.wbemFlagReturnImmediately|WbemFlagEnum.wbemFlagForwardOnly));
//foreach (SWbemObject o in col) {
//	print.it(o.Properties_.Item("Name").get_Value());
//}
//foreach (dynamic p1 in col) {
//	try { print.it(p1.Name); }
//	catch {  }
//}

//var scope = new ManagementScope();
//scope.Connect();
//var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
//var searcher = new ManagementObjectSearcher(scope, query);
//foreach (var m in searcher.Get()) {
//	print.it(m["csname"]);
//}

//var pr = new ManagementClass("Win32_Process");
//pr.InvokeMethod("Create", new[] { "Notepad.exe" });

//print.clear();
//var scope = new ManagementScope(); scope.Connect();
//var query = new ObjectQuery("SELECT * FROM Win32_Process");
//var searcher = new ManagementObjectSearcher(scope, query);
//foreach (var m in searcher.Get()) {
//	print.it(m["Name"]);
//}

//using var watcher = new ManagementEventWatcher("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance isa \"Win32_Process\"");
//watcher.Options.Timeout = TimeSpan.FromSeconds(30);
//using var osd = osdText.showTransparentText("Open 2 applications to trigger events", -1);
//for (int i = 0; i < 2; i++) {
//	var e = watcher.WaitForNextEvent();
//	print.it(((ManagementBaseObject)e["TargetInstance"])["Name"]);
//}

