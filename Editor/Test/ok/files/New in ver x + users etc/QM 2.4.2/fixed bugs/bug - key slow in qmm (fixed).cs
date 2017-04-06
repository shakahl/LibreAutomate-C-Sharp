 /exe 2

 key sysnc did not work in qmm if qmm is admin and QM is user and target window is admin, because default sync method does not work.

 run "notepad.exe"
 run "notepad.exe" "" "" "" 0x10300 ;;run an admin program
act "Notepad"
0.5
 spe
 opt keysync 1
PF
rep 1
	key aY
PN;PO

 BEGIN PROJECT
 main_function  bug - key slow in admin qmm if QM as user
 exe_file  $my qm$\bug - key slow in admin qmm if QM as user.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {A10ABFDB-DA8A-4B5F-B2F8-7B60DD143E39}
 END PROJECT

#ret

