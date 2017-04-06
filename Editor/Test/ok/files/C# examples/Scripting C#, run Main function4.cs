 /exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	out x.Call("Program.IsServiceInstalled" "quickmacros2")
	PN
PO

 BEGIN PROJECT
 main_function  Scripting C#, run Main function4
 exe_file  $my qm$\Scripting C#, run Main function4.qmm
 flags  6
 guid  {D45C532F-EE9A-467E-A80B-F220DDB56874}
 END PROJECT

#ret
//C# code
using System;
using System.ServiceProcess;

public static class Program{
public static bool IsServiceInstalled(string serviceName)
{
  // get list of Windows services
  ServiceController[] services = ServiceController.GetServices();

  // try to find service name
  foreach (ServiceController service in services)
  {
    if (service.ServiceName == serviceName)
      return true;
  }
  return false;
}}