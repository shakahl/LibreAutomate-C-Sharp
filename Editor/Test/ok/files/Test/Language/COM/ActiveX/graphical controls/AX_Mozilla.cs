/exe
\Dialog_Editor
 WARNING:
 This control can be used only in single thread.
 If used more than once, the process crashes.
 Solution: run this function in separate process.
typelib MOZILLACONTROLLib {1339B53E-3453-11D2-93B9-000000000000} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_Mozilla" &AX_Mozilla)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 222 114 "MOZILLACONTROLLib.MozillaBrowser {1339B54C-3453-11D2-93B9-000000000000}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MOZILLACONTROLLib.MozillaBrowser mo3
	mo3._getcontrol(id(3 hDlg))
	mo3.Navigate("http://www.google.com")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  AX_Mozilla
 exe_file  $my qm$\AX_Mozilla.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {10293E33-685E-4C57-81B5-D7878A2E32B6}
 END PROJECT
