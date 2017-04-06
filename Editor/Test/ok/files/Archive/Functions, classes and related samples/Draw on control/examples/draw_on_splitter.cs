\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 InitSplitter

str controls = "3 4"
str e3 e4
if(!ShowDialog("draw_on_splitter" &draw_on_splitter &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54231044 0x200 0 0 112 112 ""
 4 Edit 0x54231044 0x200 118 0 106 112 ""
 5 QM_Splitter 0x54030000 0x0 112 0 6 112 ""
 6 QM_Splitter 0x54030000 0x0 0 112 110 6 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_DrawOnControl id(5 hDlg) &draw_on_splitter_proc 0 ;;vertical splitter
	DT_DrawOnControl id(6 hDlg) &draw_on_splitter_proc 1 ;;horizontal splitter
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
