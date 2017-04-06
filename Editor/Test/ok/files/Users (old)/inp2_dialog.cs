 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A44 0x180 0 0 224 69 ""
 3 Static 0x54000000 0x0 6 6 216 12 ""
 4 Edit 0x54030080 0x200 6 24 214 14 ""
 1 Button 0x54030001 0x4 118 50 48 14 "OK"
 2 Button 0x54030000 0x4 170 50 48 14 "Cancel"
 5 Edit 0x4C030080 0x200 118 78 10 8 ""
 6 Edit 0x4C030080 0x200 130 78 10 8 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "inp2" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;not necessary in QM >= 2.1.9
	int x=GetDlgItemInt(hDlg 5 &_i 1)
	int y=GetDlgItemInt(hDlg 5 &_i 1)
	ret 1 ;;not necessary in QM >= 2.1.9
	case WM_DESTROY DT_DeleteData(hDlg) ;;not necessary in QM >= 2.1.9
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	DT_Ok hDlg ;;not necessary in QM >= 2.1.9
	case IDCANCEL DT_Cancel hDlg ;;not necessary in QM >= 2.1.9
ret 1
