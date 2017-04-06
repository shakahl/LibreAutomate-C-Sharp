/exe
out
 int w1=win("TOOLBAR NAME")
 int w1=win("QM_")

 zw GetWindow(w1 GW_OWNER)

 int h=win("Find" "" "")
 int h=win("Find" "" "Notepad" 32)
 int h=win("Find" "" win("Notepad") 32)
 int h=win("" "QM_toolbar" "Notepad" 32)
 int h=win("" "QM_toolbar" "+Notepad" 32)
int h=win("" "QM_toolbar" win("Notepad") 32)

 int h=win("Find" "" "" 64 win("Notepad") 0)
 int h=win("" "QM_toolbar" "" 64 win("Notepad") 0)

zw h

 BEGIN PROJECT
 main_function  Macro488
 exe_file  $my qm$\Macro488.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {827274C3-A003-4AF8-8861-0B477D35D86C}
 END PROJECT
