 /exe

str allFiles=
F
 :1 item1.bmp
 :2 item2.bmp
 :3 item3.bmp

int i
for i 1 4
	out F":{i} item{i}.bmp"
	if(scan(F":{i} item{i}.bmp" 0 0 1))
		out "found"

 BEGIN PROJECT
 main_function  Macro1661
 exe_file  $my qm$\Macro1661.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  23
 end_hotkey  0
 guid  {FB34BB3F-29A2-4FAB-A1E6-28F1EB181021}
 END PROJECT
