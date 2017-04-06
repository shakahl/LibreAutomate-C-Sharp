 /exe
Tray f.AddIcon(":1 $pf$\quick macros 2\copy.ico[]:2 $pf$\quick macros 2\paste.ico" "Initialisation")
int i
for i 1 6
	1
	f.Modify(i&1+1)
1

 BEGIN PROJECT
 main_function  Macro1090
 exe_file  $my qm$\Macro1090.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {DBD0B195-3772-43C9-9EC4-9B4C1818F311}
 END PROJECT
