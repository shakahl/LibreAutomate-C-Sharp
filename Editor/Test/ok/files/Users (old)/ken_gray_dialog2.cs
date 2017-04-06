\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

type KEN_GRAY_DIALOG2 ~controls ~e3 ~c5Che
KEN_GRAY_DIALOG2 d.controls="3 5"
if(!ShowDialog("ken_gray_dialog2" &ken_gray_dialog2 +&d)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 2 Button 0x54030001 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x204 4 4 96 14 ""
 4 Button 0x54032000 0x4 118 116 48 14 "Save"
 5 Button 0x54012003 0x4 6 32 48 13 "Check"
 END DIALOG
 DIALOG EDITOR: "KEN_GRAY_DIALOG2" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 ;;Save
	KEN_GRAY_DIALOG2& dd=DT_GetControls(hDlg)
	out dd.e3
	out dd.c5Che
	 dd.e3.setfile("C:\qm\projects.txt")
	
	case IDCANCEL DT_Cancel hDlg
ret 1
