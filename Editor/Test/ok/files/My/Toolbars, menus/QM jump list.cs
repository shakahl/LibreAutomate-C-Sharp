 Creates Windows 7 jump list for QM taskbar button and pinned icon.

#compile "__JumpList"
JumpList x
WINAPI7.SetCurrentProcessExplicitAppUserModelID(@"Quick Macros")
x.Begin("Quick Macros")
x.AddItem("Unlock qmhook32" "C:\Program Files\LockHunter\LockHunter.exe" "Q:\app\qmhook32.dll")
x.AddItem("Shutdown QM" "Q:\My QM\shutdown_qm.exe")
x.AddItem("Calc code size (tray)" "Q:\My QM\CalcCodeSizeTrayIcon.exe")
 x.AddItem("Run old QM" "Q:\My QM\Run old QM.exe" "" "q:\app\qm.exe")
 x.AddItem("-")
 x.AddItem("dlg_test" "Q:\My QM\dlg_test_tsm.exe")
x.AddAsTasks
x.End

 Windows bug:
 If we add a command line to the pinned icon, it adds a separate taskbar button when the program runs, instead of replacing the icon with button.
 Workaround: Pin the separate button too. Then change program path of the old pinned icon.
