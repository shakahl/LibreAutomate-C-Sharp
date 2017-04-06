 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 1001"
str lb3Act c1001For

TO_FavSel wParam lb3Act "Log off[]Shutdown computer[]Restart computer[]Shutdown and power off[]Hibernate[]Suspend[]Lock, or switch user[][]Exit QM[]Restart QM[]Hide QM[]Show QM[]Reload QM file[]End macro"

if(!ShowDialog("TO_Shutdown" &TO_Shutdown &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 265 163 "Shutdown"
 3 ListBox 0x54230101 0x204 4 4 96 80 "Act"
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 1001 Button 0x54012003 0x0 110 4 48 13 "Force"
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
