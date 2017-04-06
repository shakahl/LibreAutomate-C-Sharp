 /exe 1
out

ARRAY(str) a; int i
PowerPointSlideGetText a

out "--- text boxes ---"
for i 0 a.len
	out "%i. %s" i+1 a[i]

out "--- all ---"
str s=a
out s

 BEGIN PROJECT
 main_function  Macro1386
 exe_file  $my qm$\Macro1386.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A49F4578-00FD-40FF-8EFB-EDDE4DFF1652}
 END PROJECT
