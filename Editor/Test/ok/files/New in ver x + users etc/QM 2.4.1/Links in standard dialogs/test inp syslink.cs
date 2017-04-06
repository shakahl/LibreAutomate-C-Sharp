/exe
 out inp(_s "Test link")
str sl=F"<>Test <a id=''200''>id 200</a>[]Test <a href=''{&syslink_click} param''>callback</a>"
out inp(_s sl)
 out inp(_s sl "" "[]")
 out inp(_s sl "" "*")
 out _s

 out inpp("p" "Test link")
 out inpp("p" sl)
 inpp("p" sl)
 out inpp("[*D82180CEB2E1614306*]" sl) ;;hh

 out InputBox(_s 0 "simple")
 out InputBox(_s 0 sl)
out _s

 BEGIN PROJECT
 main_function  test mes syslink
 exe_file  $my qm$\test mes syslink.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {DC32AB08-7819-44CE-B50B-580C099C4361}
 END PROJECT
 manifest  
