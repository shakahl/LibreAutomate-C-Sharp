\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile "____UseComUnregistered"
__UseComUnregistered ucu.Activate("Project2.X.manifest")

typelib Project2 "$qm$\_com\ocx\Project2.ocx"

if(!ShowDialog("dlg_unregistered_activex" &dlg_unregistered_activex 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 87 96 48 "Project2.UserControl1 {E46F9BB9-8BD9-4AAA-BD7D-1DFD779EC13F}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030201 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
