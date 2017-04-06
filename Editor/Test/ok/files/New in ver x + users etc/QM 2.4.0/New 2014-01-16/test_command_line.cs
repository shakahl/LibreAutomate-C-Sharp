 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("test_command_line" &test_command_line &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x8 0 0 371 41 "Dialog"
 3 Edit 0x54030080 0x200 4 4 364 14 ""
 4 Button 0x54032000 0x0 4 22 48 14 "Run"
 5 Static 0x54000000 0x0 74 24 48 10 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	_s.setwintext(id(5 hDlg))
	_s.getwintext(id(3 hDlg))
	opt waitmsg 1
	run "q:\app\qmcl.exe" _s "" "" 0x400
	_s="Done"; _s.setwintext(id(5 hDlg))
ret 1

 BEGIN PROJECT
 main_function  test_command_line
 exe_file  $my qm$\test_command_line.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {3FB6D6F3-190B-4AA3-A6A0-79A70263ADED}
 END PROJECT
