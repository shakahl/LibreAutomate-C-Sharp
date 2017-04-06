/exe
 out mes("Test link" "" "i")
 out mes("Test link" "" "YNi")
 out mes("Test id 200[]Test callback" "" "i")
str sl=F"<>Test <a id=''200''>id 200</a>[]Test <a href=''{&syslink_click} param''>callback</a>"
out mes(sl "" "iYN")

 act "ddddddddd"; err out ErrMsg(2)
 out ErrMsg(2 sl)

 MsgBoxAsync "text"
 MsgBoxAsync sl


 MES m.timeout=2; m.default=555;; m.style="OC"
 out mes("" "" m)


 BEGIN PROJECT
 main_function  test mes syslink
 exe_file  $my qm$\test mes syslink.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {DC32AB08-7819-44CE-B50B-580C099C4361}
 END PROJECT
 manifest  

#ret
