/exe
 act "Word"
0.1

 outp+ "ABC"
 #ret

key Ca X
0.2
_s="123456789 "
str ss; rep(10) ss+_s
str s; rep(1) s.addline(ss)

 spe 10
outp+ s
 key Y
outp+ ss
 key Y

 opt keychar 1
 _key2 key((s))

 BEGIN PROJECT
 main_function  Macro1643
 exe_file  $my qm$\Macro1643.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {EFAF66B6-B89C-49F3-A6FE-619E8689A0E7}
 END PROJECT
