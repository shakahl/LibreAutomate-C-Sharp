\Dialog_Editor
/exe
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
e4="some text"
if(!ShowDialog("dlg_test_copy_paste" &dlg_test_copy_paste &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 58 118 48 14 "Paste"
 4 Edit 0x54231044 0x200 0 0 222 114 ""
 5 Button 0x54032000 0x0 6 118 48 14 "Copy"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 ;;paste
	_s.setwintext(id(4 hDlg))
	act id(4 hDlg)
	"test"
	 TEST_CLIPBOARD
	 TEST_KEY
	
	case 5 ;;copy
	act id(4 hDlg)
	_s.getsel
	out _s
	
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_test_copy_paste
 exe_file  $my qm$\dlg_test_copy_paste.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {EA174A10-BA43-4456-883B-0A61E4E4B36E}
 END PROJECT
