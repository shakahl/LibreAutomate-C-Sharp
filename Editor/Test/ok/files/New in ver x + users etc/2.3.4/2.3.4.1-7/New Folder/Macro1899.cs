 /exe
mes "Some text"

 #exe addtextof "Macro1899"

ret
 _i=&CreateXml
 zip "" ""

 BEGIN PROJECT
 main_function  Macro1899
 exe_file  $qm$\Macro18.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {1C81F5CF-430B-4610-A7C9-FD1F685D7665}
 END PROJECT
 manifest  $qm$\default.exe.manifest
 exe_file  \\gintaras\myprojects\test\Macro18.exe
