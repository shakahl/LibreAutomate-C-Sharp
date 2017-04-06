/exe
 Q &q
 rep 10000
	  spe
	  _i=5+2
	 f1r
 Q &qq
 outq

Q &q
int i
for i 0 10000
	 spe
	_i=5+2
	 f1r
Q &qq
outq

 BEGIN PROJECT
 main_function  Macro403
 exe_file  $my qm$\Macro403.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0B26F0A0-21B7-4C19-B8E6-581F7FB9E1AE}
 END PROJECT
