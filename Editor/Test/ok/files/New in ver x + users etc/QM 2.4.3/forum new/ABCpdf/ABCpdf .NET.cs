 create variable that manages compiled C# code
CsScript x
x.SetOptions("references=ABCpdf")

 compile code after #ret
x.AddCode("")

 call static function
int R=x.Call("Class1.TestFunction" _s.expandpath("$desktop$\QM ABCpdf.pdf"))
out R


#ret
//C# code
using System;
using WebSupergoo.ABCpdf9;

public class Class1
{
static public int TestFunction(string saveToThisFile)
{
Doc theDoc = new Doc();
theDoc.FontSize = 96;
theDoc.AddText("Quick Macros");
theDoc.Save(saveToThisFile);
theDoc.Clear();

return 1;
}

//add more functions here if need
}
