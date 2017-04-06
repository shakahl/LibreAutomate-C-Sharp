 /Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3Act

TO_FavSel wParam lb3Act "Run macro[]"

if(!ShowDialog("TO_Mac" &TO_Mac &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 265 163 "Form"
 3 ListBox 0x54230101 0x204 4 4 96 80 "Act"
 1001 Static 0x54020000 0x4 106 4 48 13 "This will start macro or executable function to run asynchronously (as separate macro). Note: this is not the same as to call function that runs in the same macro."
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 4 138 258 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010200 "*" "0"

ret
 messages
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK ret TO_Ok(hDlg)
	case LBN_SELCHANGE<<16|3
	_i=LB_SelectedItem(lParam)
	DT_Page hDlg _i
ret 1
