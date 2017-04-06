 /exe
type ZUU str'a b
ZUU z
_s=_command
_s.setstruct(z); err mes- "incorrect command line"
mes _s.format("''%s'', %i" z.a z.b)

 BEGIN PROJECT
 main_function  Function132
 exe_file  $my qm$\Function132.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {AE16C9D2-D6EA-470E-8D11-15E2EC522FA9}
 END PROJECT
