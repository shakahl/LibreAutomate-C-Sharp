\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str lb3 lb4
lb3="one[]two[]three"
lb4=lb3
if(!ShowDialog("dlg_delete_lb_item" &dlg_delete_lb_item &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ListBox 0x54230101 0x200 10 16 96 48 ""
 4 ListBox 0x54230101 0x200 110 16 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 subclass 2 listbox controls
	int hlb=id(3 hDlg)
	SetProp hlb "WndProc_lb_delete" SubclassWindow(hlb &WndProc_lb_delete)
	hlb=id(4 hDlg)
	SetProp hlb "WndProc_lb_delete" SubclassWindow(hlb &WndProc_lb_delete)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
