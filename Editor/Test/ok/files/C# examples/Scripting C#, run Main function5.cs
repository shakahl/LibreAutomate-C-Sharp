/exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	x.Call("Test.Main")
	PN
PO

 BEGIN PROJECT
 main_function  Scripting C#, run Main function5
 exe_file  $my qm$\Scripting C#, run Main function5.qmm
 flags  6
 guid  {1C7DB3A0-05C5-4E98-BAE3-DA1FB8B0EA77}
 END PROJECT

#ret
//C# code
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

public class Test{
   public static void Main(){
        XElement xmlTree = new XElement("Root",
            new XElement("A1", 1),
            new XElement("A2", 2),
            new XElement("A3", 3)
        );
        XElement child1 = xmlTree.Element("A1");
        child1.AddAfterSelf(new XElement("NewA", 10));
        Console.WriteLine(xmlTree);
   }
}
