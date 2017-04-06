 /exe
str s ss
inp- s "Ex: 2*2. Ex2: (50+30)/4. Ex3: Sqr(20^2 + 10^2)" "Calculator"
mes VbsEval(s)
err mes "error"

 BEGIN PROJECT
 main_function  simple calculator using VBScript
 exe_file  $my qm$\Macro974.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0F3C08DA-FC08-4FD5-9EE6-D62BD6B77B97}
 END PROJECT
