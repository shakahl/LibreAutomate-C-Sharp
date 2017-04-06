 /exe

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Hung"
 3 Button 0x54032000 0x0 8 8 48 14 "Hung"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 -1)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_ACTIVATE
	 if(wParam) outw win; LockSetForegroundWindow LSFW_LOCK
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	wait 30
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_hung
 exe_file  $my qm$\dlg_hung.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {62933437-1116-436B-BAB5-81B781F1A0FC}
 END PROJECT
