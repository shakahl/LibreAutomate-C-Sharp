\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str e3 lb4

lb4="&abde[]abcd[]abcdef"

if(!ShowDialog("dlg_incremental_search_in_combo" &dlg_incremental_search_in_combo &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 2 2 96 14 ""
 4 ListBox 0x54230101 0x200 2 18 96 90 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	SendMessage(id(4 hDlg) LB_SELECTSTRING -1 _s.getwintext(lParam))
	case IDOK
	case IDCANCEL
ret 1
