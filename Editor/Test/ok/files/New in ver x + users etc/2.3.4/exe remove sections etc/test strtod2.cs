 /exe
 out

out setlocale(LC_NUMERIC "Lithuanian")


str b.all(100)
out b.fix(_snprintf(b 100 "%g" 1.5))

lpstr se
out strtod("1.5" &se)

out val("1.5" 2)

 BEGIN PROJECT
 main_function  Macro1677
 exe_file  $my qm$\Macro1677.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {F50ED0C1-5463-43D9-9C63-33761B89A0D5}
 END PROJECT
