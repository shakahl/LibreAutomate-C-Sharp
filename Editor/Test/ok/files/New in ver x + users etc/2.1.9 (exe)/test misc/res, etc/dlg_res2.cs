\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll user32 #DialogBoxParam hInstance $lpTemplateName hWndParent lpDialogFunc dwInitParam

int hm=GetExeResHandle
int- hcursor=LoadCursor(hm +1)

if(!ShowDialog("dlg_res2" &dlg_res2)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 12 Button 0x54032000 0x0 4 6 48 14 "res dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "*" ""

ret
 messages
if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
 int param=DT_GetParam(hDlg)

sel message
	case WM_INITDIALOG
	SetMenu hDlg LoadMenu(GetExeResHandle +1)
	ret 1
	case WM_DESTROY
	DT_DeleteData(hDlg)
	DestroyCursor hcursor
	case WM_COMMAND goto messages2
	case WM_SETCURSOR
	SetCursor hcursor
	ret 1
ret
 messages2
sel wParam
	case 12
	_i=DialogBoxParam(GetExeResHandle +1 hDlg &Dialog12_2 0)
	 out _i
	 out _s.dllerror
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
