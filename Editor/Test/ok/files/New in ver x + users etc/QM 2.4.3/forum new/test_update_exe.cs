 Add this code at the beginning of your exe code.
 To understan this code, begin reading from the 'if need to update' line.

str exe1(ExeFullPath) exe2
if matchw(exe1 "*_new.exe") ;;updating
	1
	cop- exe1 exe2.fromn(exe1 exe1.len-8 ".exe" -1) ;;copy *_new.exe to *.exe
	run exe2 ;;run *.exe
	ret

exe2.fromn(exe1 exe1.len-4 "_new.exe" -1)
if FileExists(exe2)
	del- exe2; err
else if 'Y'=mes("Update exe?" "" "YN") ;;if need to update  (THE UPDATING PROCESS STARTS HERE)
	cop- exe1 exe2 ;;get new version. Cannot replace self. Save as *_new.exe.
	run exe2 ;;run *_new.exe, let it replace and run *.exe
	ret

err+ mes- exe1 "Failed to update" "!"
 ____________________________________________

mes "Running." "" "i" ;;remove this
 ...

 BEGIN PROJECT
 main_function  test_update_exe
 exe_file  $my qm$\test_update_exe.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {5FF76B52-3E95-48C2-90AB-3803F3E8A40E}
 END PROJECT
