 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x444 0x10000 0 0 253 116 ""
 3 RichEdit20A 0x54233044 0x200 8 6 120 61 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010806 "" ""

ret
 messages
if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
 int param=DT_GetParam(hDlg)

sel message
	case WM_INITDIALOG
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
