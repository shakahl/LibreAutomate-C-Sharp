 /exe
out run("notepad" "" "" "" 0x400)

 out wait(0 WP "Notepad")

 __Handle h1=run("notepad")
 out wait(0 H h1)

 __Handle h1=run("notepad")
 __Handle h2=run("notepad")
 out wait(0 HM h1 h2)

 opt waitmsg 1
 PostQuitMessage 5

 out wait(0 HM g_h1 g_h2)

 ARRAY(int) a.create(2); a[0]=g_h1; a[1]=g_h2
 out wait(0 HM a)

 BEGIN PROJECT
 main_function  Macro950
 exe_file  $my qm$\Macro950.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A841368D-7005-4B01-9D7A-58348D9744DF}
 END PROJECT
