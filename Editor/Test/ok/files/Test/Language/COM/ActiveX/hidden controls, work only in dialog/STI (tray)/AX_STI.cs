\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib STI "%com%\UI\STI (tray)\STI.ocx"

if(!ShowDialog("AX_STI" &AX_STI)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 6 44 38 "STI.STI"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	STI.STI t._getcontrol(id(3 hDlg))
	t._setevents("t__DSTIEvents")
	 t.IconFile=_s.expandpath("$qm$\deb next.ico")
	t.IconType=1
	t.Appear
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
