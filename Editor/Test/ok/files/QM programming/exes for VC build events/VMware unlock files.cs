int wAct=win
#if EXE=1
run "exit_qm.exe" _command "" "" 0x400
#endif

#ret ;;currently not using vmware shared folders

 disable/enable shared folders
spe 1
int w1=win("VMware Player" "VMPlayerFrame")
if(!w1) ret
act w1
key CA
'Cd             ;; Ctrl+D
int w2=wait(20 win("Virtual Machine Settings" "#32770"))
'CT             ;; Ctrl+Tab
'DD Ad         ;; Down Down Alt+D
'Y              ;; Enter
wait 0 w1
'Cd             ;; Ctrl+D
int w3=wait(16 win("Virtual Machine Settings" "#32770"))
'CT             ;; Ctrl+Tab
'DD Ae          ;; Down Down Alt+E
'Y              ;; Enter

act wAct; err

 BEGIN PROJECT
 main_function  VMware unlock files
 exe_file  $qm$\app_plus\VMware_unlock_files.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {CD96E995-2833-4333-829A-8313BB753695}
 END PROJECT
