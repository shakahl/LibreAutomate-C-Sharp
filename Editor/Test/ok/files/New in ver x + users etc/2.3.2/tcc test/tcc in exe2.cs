 /exe
#exe addtextof "tcc in exe2 code"
__Tcc x.Compile("*tcc in exe2 code" "add");; err ret
out call(x.f 1 2)

 BEGIN PROJECT
 main_function  tcc in exe2
 exe_file  $my qm$\Macro1290.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CC58D348-B04D-49B0-A4A2-0ECBB4812BE6}
 END PROJECT
