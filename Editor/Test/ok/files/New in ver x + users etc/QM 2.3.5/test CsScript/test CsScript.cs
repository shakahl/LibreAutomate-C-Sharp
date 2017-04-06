/exe
 out
 RTOPTIONS r.net_clr_version="v2.0.50727"
 RtOptions 32 &r

str code=
 using System;
 using System.Windows.Forms;
 public class Test
 {
 public static int Add(int a, int b)
 {
 //Console.Write("test");   
 Console.Write(Environment.Version);
 if(a==1000)MessageBox.Show("test");
 return a+b;
 }
 }              

PF
CsScript x.Init
 x.SetOptions("debugConfig=true")
 x.SetOptions("noFileCache=true")
 x.SetOptions("noFileCache=true[]usingNamespaces=[]")
 x.SetOptions("usingNamespaces=[]references=System.Windows.Forms")
 x.SetOptions("references=System.Windows.Forms")
 x.SetOptions("compilerOptions=/warn:0")
PN
x.AddCode(code 0)
PN

rep 1
	_i=x.Call("Test.Add" 10 5)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript
 exe_file  $my qm$\test CsScript.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {9817E9D2-9EB4-483C-B0B6-A6C5A03C1D1B}
 END PROJECT

#ret
3.5
speed: 59468  357032  7753  
speed: 107  284671  544  
speed: 105  203426  559  

4.5
speed: 64990  355971  20197  
speed: 113  296120  517  
speed: 90  290585  521  
