 /exe

 scan "macro:Macro1439.bmp" 0 0 1|2
 wait 0 S "macro:Macro1439.bmp" 0 0 1|2

ARRAY(RECT) a
wait 0 S "macro:Macro1439.bmp" 0 0 0x400 0 a
1
int i
for i 0 a.len
	zRECT a[i]
	mou a[i].left a[i].top; 1

 BEGIN PROJECT
 main_function  Macro1444
 exe_file  $my qm$\Macro1444.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C5D70529-85FF-41BE-A02E-3C1FD8F628A0}
 END PROJECT
