 /exe
#compile "__CGet"

 CGet g1 g2
 g1=g2

 type TWA ARRAY(CGet)'a
 TWA x y.create(3)
 x=y

 #exe addfunction "CGet="
 int a b
 CGet* p1 p2
 p1=+&a
 p2=+&b
 *p1=*p2


 BEGIN PROJECT
 main_function  Macro668
 exe_file  $my qm$\Macro668.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  7
 end_hotkey  0
 guid  {AFCB546D-490D-4E2D-8EA9-003D604547B1}
 END PROJECT
