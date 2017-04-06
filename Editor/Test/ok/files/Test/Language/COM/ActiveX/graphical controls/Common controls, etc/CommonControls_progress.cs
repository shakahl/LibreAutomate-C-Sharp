\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib ComctlLib {6B7E6392-850A-101B-AFC0-4210102A8DA7} 1.3 

if(!ShowDialog("CommonControls_progress" &CommonControls_progress)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 139 38 "Form"
 1 Button 0x54030001 0x4 20 20 48 14 "OK"
 2 Button 0x54030000 0x4 72 20 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 2 4 132 13 "ComctlLib.ProgressBar"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	ComctlLib.ProgressBar pr3._getcontrol(id(3 hDlg))
	pr3.Value=50
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
