//gets all child/descendant processes of explorer

#region begin_script
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using System.Linq;
//using System.Windows.Forms;
//using System.IO;
//using System.Runtime.InteropServices;

using qm;
using static qm.Output;
using Winapi;

public static class ThisScript {
[STAThread]public static void Main(string[] args) {
#endregion

int pidParent=Process.NameToId("explorer");
if(0==pidParent) return;
var a=new List<int>{pidParent}; int i;

IntPtr hs=api.CreateToolhelp32Snapshot(api.TH32CS_SNAPPROCESS, 0);
api.PROCESSENTRY32 p; p.dwSize=sizeof(api.PROCESSENTRY32);
Process32First(hs, out p);
for(;;) {
	for(i=0; i<a.len; i++) {
		if(p.th32ParentProcessID==a[i]) {
			Out($"pid={p.th32ProcessID}  name={p.szExeFile}");
			a.Add(p.th32ProcessID);
		}
	}
	if(!Process32Next(hs, out p)) break;
}
api.CloseHandle(hs);

foreach(int i in a)
	ShutDownProcess(a[i]);

#region end_script
} //Main
} //ThisScript
#endregion
