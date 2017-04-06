 Use this on vmware to run QM from host PC.
 Don't use shortcut to QM from host PC, because then cannot run as admin, don't know why.
 Sometimes fails, shows msgbox "logon failure...". To fix, run as admin, then should run when started normally too.
 Create exe on host PC, and copy to each guest PC.

run "\\Q7C\Q\app\qm.exe" "v" ;;if using network (Q: must be shared)
 run "\\vmware-host\Shared Folders\Q\app\qm.exe" "v" ;;if using shared folders (problems with locked exe/dll files when need to replace to new version)
 run "Q:\app\qm.exe" "v" ;;when mapped. Cannot run QM as admin.

 BEGIN PROJECT
 main_function  QM in app
 exe_file  $qm$\app_plus\QM in app.exe
 icon  $qm$\qm.exe,0
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {326363E7-3B2A-432B-80CA-FBC65ECA9BC6}
 END PROJECT
