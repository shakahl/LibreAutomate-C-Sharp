 /exe
 #exe addtextof "Macro1232"
#exe addtextof "led - Copy.txt"

 out _s.getmacro("Macro1232")
out _s.getmacro("led - Copy.txt")

 BEGIN PROJECT
 main_function  Macro1234
 exe_file  $my qm$\Macro1234.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {1E9B6762-2A04-4632-B608-564F00747395}
 END PROJECT
