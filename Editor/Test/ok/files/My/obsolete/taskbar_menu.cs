 /exe
str s=
 1 Unlock qmhook32
 2 Shutdown QM
 3 Calc code size (tray)
 4 Visual Studio (app)
 -
 100 dlg_test
sel ShowMenu(s)
	case 1 run "C:\Program Files\Unlocker\Unlocker.exe" "Q:\app\qmhook32.dll"
	case 2 run "Q:\My QM\shutdown_qm.exe"
	case 3 run "Q:\My QM\CalcCodeSizeTrayIcon.exe"
	case 4 run "$program files$\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe" "Q:\app\app.sln" "" "*"
	 case 5 run 
	 case 6 run 
	 case 7 run 
	 case 8 run 
	case 100 run "Q:\My QM\dlg_test_tsm.exe"

 BEGIN PROJECT
 main_function  taskbar_menu
 exe_file  $my qm$\taskbar_menu.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {AB67B7E9-2584-4B2D-AFD9-E89F42B56E9C}
 END PROJECT
