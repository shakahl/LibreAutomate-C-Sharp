 /exe
SetEnvVar "myqm" "q:\my qm"
 //css_dir q:\my qm\test;
 //css_reference cs;
 //css_reference q:\app\cs.dll;
 //css_reference q:\app\cs\cs.dll;
 //css_reference q:\my qm\test\cs.dll;
 using System; using System.Reflection; [assembly: AssemblyConfiguration("is good\r\nline 2")]
 //css_searchdir %myqm%\test;
 //css_reference cs;
 //css_reference System.Windows.Forms;
 //css_co /warn:3
 //css_reference %myqm%\test\cs.dll;
 //css_reference %myqm%\test\cs2.dll;
 //css_reference cs.dll;
 //css_reference cs2.dll;
str code=
 using System;
 using System.Windows.Forms;
 
 class Script
 {
     static public void Main(int useKee)
     {
        //AppDomain.CurrentDomain.AppendPrivatePath(@"cs");
        //Console.WriteLine(Test.MsgBox(10, 5)); 
        Console.WriteLine(Test.Add(10, 5));                                       
        //if(useKee==1) Cee.Kee();
        System.Windows.Forms.Clipboard.Clear();
     }
     static public void Func2()
     {
           Cee.Kee(); 
     }
     static public void AddPath() 
     {
 //#pragma warning disable 618
            AppDomain.CurrentDomain.AppendPrivatePath(@"cs");
 //#pragma warning restore 618
     }
 }

 _s.expandpath("$temp$\fff.cs"); code.setfile(_s); code=_s
 CsFunc code 1; ret
 CsExec code "5"; ret

PF
CsScript x
 x.Exec(code); ret
 x.SetOptions("searchDirs=%myqm%\test")
x.SetOptions("searchDirs= %myqm%\test ; %myqm%\test2[]references=cs.dll; cs2.dll")
 x.SetOptions("searchDirs= %myqm%\test ; %myqm%\test2[]references= q:\my qm\test\cs.dll ; q:\my qm\test\cs2.dll ; ")
 //css_reference 
 //css_reference %myqm%\test\cs2.dll;
 x.SetOptions("searchDirs=%myqm%\test[]noFileCache=true[]compilerOptions=/warn:3")
 x.SetOptions("searchDirs=%myqm%\test[]noFileCache=true")
 x.SetOptions("searchDirs=%myqm%\test[]noFileCache=true")
 x.SetOptions("searchDirs=%myqm%\test[]inMemoryAsm=")
 x.SetOptions(1 "appDirs=csscript;debug")
 SetCurDir "c:\windows"
 x.SetOptions(1 "searchDirs=csscript;debug")
 x.SetOptions("inMemoryAsm=")
 x.Load("%myqm%\test\cs.dll")
 opt nowarningshere 1
x.AddCode(code)
PN
 PO; ret
 CsScript xx.Load("%myqm%\test\cs.dll")
 _s.getfile("%myqm%\test\cs.dll"); CsScript xx.LoadFromMemory(_s _s.len)
 x.Call("Main")
 x.Call("AddPath")
x.Call("Main" 1)
x.Call("Func2")
PN
PO

 BEGIN PROJECT
 main_function  test CsScript directive reference
 exe_file  $my qm$\test CsScript directive reference.qmm
 flags  6
 guid  {8A5458BB-D704-4065-9FEB-DF1AA50A8723}
 END PROJECT

 if(1==0) Private();
 static void Private()
 {
        Console.WriteLine(Test.MsgBox(10,   5));
 }
