 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 133 "Form"
 3 ListBox 0x54230101 0x200 6 6 96 48 ""
 4 Edit 0x54231044 0x200 106 6 96 48 ""
 5 Button 0x54032000 0x0 6 62 48 14 "Copy"
 6 Static 0x54000000 0x0 4 82 198 30 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;*
	PostMessage hDlg WM_COMMAND LBN_SELCHANGE<<16|3 id(3 hDlg)
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg) ;;*
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3
	_i=LB_SelectedItem(lParam)
	str sh
	sel _i
		case 0 sh="help text 0"
		case 1 sh="help text 1"
		case 2 sh="help text 2"
	sh.setwintext(id(6 hDlg))
	
	case 5 ;;Copy button clicked
	int i=LB_SelectedItem(id(3 hDlg))
	str s.getwintext(id(4 hDlg))
	s.setclip
	
	case IDOK
	DT_Ok hDlg ;;*
	case IDCANCEL DT_Cancel hDlg ;;*
ret 1

 * - not necessary in QM >= 2.1.9
