/exe

int w1=win("Form1")

 out
 DotNetShowControlNames w1


int w2=DotNetFindControl(w1 "TextBox1")
zw w2

 #compile __DotNetControls
 DotNetControls c.Init(w1)
 c.GetControlName(w1 _s)
 out _s

 int m=RegisterWindowMessage("WM_GETCONTROLNAME")
 out SendMessage(w1 m 0 0)

 out DotNetCanGetName(w1)


 BEGIN PROJECT
 main_function  Macro627
 exe_file  $my qm$\Macro627.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {F6C06128-BE5B-4380-8FBD-8623FE07A40D}
 END PROJECT
