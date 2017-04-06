\Dialog_Editor

 This example uses ActiveX control, if available.

function# hDlg message wParam lParam
if(hDlg) goto messages

typelib ShockwaveFlashObjects {D27CDB6B-AE6D-11CF-96B8-444553540000} 1.0

if(!ShowDialog("dlg_flash_ax" &dlg_flash_ax)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 4 6 112 24 "ShockwaveFlashObjects.ShockwaveFlash {D27CDB6E-AE6D-11CF-96B8-444553540000}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ShockwaveFlashObjects.ShockwaveFlash sh3
	sh3._getcontrol(id(3 hDlg))
	sh3.LoadMovie(0 _s.expandpath("$desktop$\digitalclock.swf"))
	sh3.Play
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
