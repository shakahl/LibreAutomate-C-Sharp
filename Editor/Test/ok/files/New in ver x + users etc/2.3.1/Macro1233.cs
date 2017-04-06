/exe
str s

QMITEM q
 qmitem("Macro1232" 0 q 8)
 qmitem("encr" 0 q 8)
 qmitem("led - Copy.txt" 0 q 8)
out q.text

 out s.getmacro("Macro1232")
 out s.getmacro("encr")
 out s.getmacro("led - Copy.txt")

 BEGIN PROJECT
 main_function  Macro1233
 exe_file  $my qm$\Macro1233.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {9DAF66F1-D17F-41B2-A6DB-BDA181AC347D}
 END PROJECT
