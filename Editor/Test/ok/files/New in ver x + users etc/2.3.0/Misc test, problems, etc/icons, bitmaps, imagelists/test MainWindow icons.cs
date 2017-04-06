 /exe
type HWVAR hwnd hwndedit ;;add more members if needed
HWVAR- v ;;also add this line in other functions of this thread where you want to use v
MainWindow "Hello World" "QM_HW_Class" &HW_WndProc 200 200 200 200

 BEGIN PROJECT
 main_function  Macro745
 exe_file  $my qm$\Macro745.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {279E769B-E83C-4115-8D31-02D18DCF42D9}
 END PROJECT
