 /exe

 mac "Function176"
 mac F"Fun{'c'%%c}tion176"

 _s.getfile(":1 $desktop$\test.txt")
 _s.getfile(F":{1} $desktop$\test.txt")
_s.getfile(F":1 $desktop$\test{'.'%%c}txt")
out _s

 BEGIN PROJECT
 main_function  Macro1324
 exe_file  $my qm$\Macro1324.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {4DC0ED9F-595D-41A7-B506-D4FB959FBE04}
 END PROJECT
