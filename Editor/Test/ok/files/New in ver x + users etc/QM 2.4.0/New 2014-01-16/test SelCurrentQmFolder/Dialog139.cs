\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="Page0[]Page1[]Page2"
if(!ShowDialog("Dialog139" &Dialog139 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 265 163 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 96 80 ""
 1001 Static 0x54020000 0x4 106 4 48 13 "Page0"
 1101 Static 0x54020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x54020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 0 138 800 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "" "0" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int page
#if !EXE
	page=SelCurrentQmFolder("S1[]S2")-1
#endif
	LB_SelectItem id(3 hDlg) page
	DT_Page hDlg page
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	_i=LB_SelectedItem(id(3 hDlg))
	DT_Page hDlg _i
ret 1
