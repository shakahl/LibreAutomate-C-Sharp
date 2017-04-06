/exe
 out
str sm.getmacro("QmDll")
 lpstr s

Q &q
int j
rep 10
	 j+find(sm "[]dll ''qm.exe''" 0 1)>=0
	 j+findrx(sm "(?s)[]dll ''qm\.exe''[](.+?[])[]" 0 1|32)>=0
	j+findrx(sm "(?s)[]dll ''qm\.exe''[](.+?[])[]" 0 1)>=0
Q &qq
outq
out j

 BEGIN PROJECT
 main_function  Macro754
 exe_file  $my qm$\Macro754.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D5714259-2D38-4858-9609-3C55E7A055A5}
 END PROJECT
