\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 ---- child dialogs ----

#compile __ChildDialog
ChildDialog- p0 p1

 page 0
type DCSCONTROLS844 ~controls ~e4
DCSCONTROLS844 d1.controls="4"
d1.e4="aaa"
#exe addtextof "dlg_cd_multipage_child0"
p0.Init(1001 "dlg_cd_multipage_child0" 0 &d1)

 page 1
type DCSCONTROLS845 ~controls ~c4
DCSCONTROLS845 d2.controls="4"
d2.c4=1
#exe addtextof "dlg_cd_multipage_child1"
p1.Init(1101 "dlg_cd_multipage_child1" 0 &d2)

 ---- main dialog ----

str controls = "3"
str lb3

lb3="&Page 0[]Page 1"

if(!ShowDialog("dlg_cd_multipage_main" &dlg_cd_multipage_main &controls)) ret

out "d1.e4=%s, d2.c4=%s" d1.e4 d2.c4

 BEGIN DIALOG
 0 "" 0x90C80A48 0x10100 0 0 265 163 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 82 80 ""
 1001 Static 0x44000000 0x0 92 4 168 126 ""
 1101 Static 0x44000000 0x0 92 4 168 126 ""
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 4 138 257 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
p0.Message(hDlg message wParam lParam)
p1.Message(hDlg message wParam lParam)
sel message
	case WM_INITDIALOG
	goto selectpage
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	 selectpage
	_i=LB_SelectedItem(id(3 hDlg))
	DT_Page hDlg _i
ret 1
