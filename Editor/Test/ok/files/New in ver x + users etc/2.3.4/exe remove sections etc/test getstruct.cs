 /exe

 IStringMap m=CreateStringMap
 m.AddList("one vienas[]two du")
 out m.Get("two")

POINT p pp
p.y=8
str s.getstruct(p 1)
out s
s.setstruct(pp 1)
out pp.y

 BEGIN PROJECT
 main_function  Macro1692
 exe_file  $my qm$\Macro1692.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {DCE7FF2B-3B92-48CC-BDCC-FE84FAAF3B46}
 END PROJECT
