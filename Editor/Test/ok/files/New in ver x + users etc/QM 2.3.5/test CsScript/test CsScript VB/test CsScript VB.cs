 /exe
 out

_s=
 Imports System
 Imports System.Windows.Forms
 Module Module1
     Sub Main()
         System.IO.File.Delete("nn?>nnnnnnnnnnnnn")
         'throw System.Exception("e")
         Console.WriteLine("Hello World! (VB)")
         MessageBox.Show("Hello World! (VB)")
     End Sub
 End Module
_s.setfile("$temp qm$\vb.vb")

PF
CsScript x.Init
 x.SetOptions("altCompiler=CSSCodeProvider.dll[]references=System.Windows.Forms[]noFileCache=true")
 x.SetOptions("language=VB[]references=System.Windows.Forms")
 x.SetOptions("language=VB[]references=System.Windows.Forms[]debugConfig=true")
x.SetOptions("language=VB")
 x.SetOptions("noFileCache=true")
PN
 x.AddCode("" 0)
 x.Exec("$temp qm$\vb.vb")
x.Exec("")
 x.Exec(_s)
PN

rep 1
	 _i=x.Call("Test.Add" 10 5)
	 IDispatch d=x.CreateObject("Test"); _i=d.Add2(100 200)
	 _i=x.Call("Module1.Add3" 10 5)
	 _i=x.Call("Add3" 10 5)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript VB
 exe_file  $my qm$\test CsScript VB.qmm
 flags  6
 guid  {EE026D5D-AB7F-48C1-8211-02B460613806}
 END PROJECT
#ret
 Imports System
 imports System.Windows.Forms
 Public Class Test
     Shared Sub Main()
         'System.IO.File.Delete("nn?>nnnnnnnnnnnnn")
         Console.WriteLine("Hello World! (VB)")
         MessageBox.Show("Hello World! (VB)")
     End Sub

 Public Shared Function Add(a as Integer, b as Integer) As Integer
 return a+b
 End Function
 
 Public Function Add2(a as Integer, b as Integer) As Integer
 return a+b
 End Function

 End Class

 Public Module Module1
 Function Add3(a as Integer, b as Integer) As Integer
 return a+b
 End Function
 End Module
