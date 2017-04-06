 /exe

out
#exe addfile "$qm$\qmzip.dll" 12
 #exe addfile "$qm$\qmzip.dll" 12 252
 #exe addfile "$qm$\qmzip.dll" 12 "MONO"

 out ExeExtractFile(12 "$desktop$\test.dll" 0x101)
 out ExeExtractFile(12 "$desktop$\test.dll" 1 252)
 out ExeExtractFile(12 "$desktop$\test.dll" 1 L"MONO")
out ExeExtractFile(12 "$desktop$\test.dll:fs" 1)
out dir("$desktop$\test.dll:fs")
out _s.searchpath("$desktop$\test.dll:fs")
 out GetModuleHandle("test")
out GetModuleHandle("test.dll:fs")


 BEGIN PROJECT
 main_function  Macro1919
 exe_file  $my qm$\Macro1919.exe
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
 guid  {78BF65CC-FFDC-4354-8072-552F8043D87E}
 END PROJECT
