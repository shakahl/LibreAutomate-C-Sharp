/exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	x.Main()
	PN
PO

 BEGIN PROJECT
 END PROJECT

#ret
//C# code
using System;
using System.Reflection;

class Test{
  public static void Main()
  {
    AppDomain currentDomain = AppDomain.CurrentDomain;
    Assembly[] assems = currentDomain.GetAssemblies();
    Console.WriteLine("List of assemblies loaded in current appdomain:");
    foreach (Assembly assem in assems){
      Console.WriteLine(assem.ToString());
    }
  }

}