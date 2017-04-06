 /exe

#exe addtextof "item1.bmp"
#exe addtextof "item2.bmp"
#exe addtextof "item3.bmp"

int i
for i 1 4
	out F"macro:item{i}.bmp"
	if(scan(F"macro:item{i}.bmp" 0 0 1))
		out "found"

 BEGIN PROJECT
 main_function  Macro1662
 exe_file  $my qm$\Macro1662.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D26686DA-65CE-47F8-8D7D-5627A199F9FF}
 END PROJECT
