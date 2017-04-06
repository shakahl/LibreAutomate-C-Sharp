\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3 5"
str qmt3 e5
 qmt3=""
 qmt3="win(...)"
 qmt3="win(...)[]child(''nnn'' ''ccc'' {window})"
qmt3=
 win("name ''quot''" "class" "prog" 2|0x200 "cText=''one[]two''" 2)

qmt3=
 win("Calculator")
 child("" "Button" {window})
if(!ShowDialog("" &dialog_QM_Tools &controls _hwndqm)) ret
out qmt3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 265 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Tools 0x54030000 0x10000 0 0 242 110 "1 1"
 4 Button 0x54032000 0x0 0 121 48 14 "Button"
 5 Edit 0x54030080 0x200 54 120 38 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	PostMessage hDlg WM_APP 0 0
	case WM_APP
	but+ 522 hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case __TWN_WINDOWCHANGED
	 outw wParam
ret
 messages2
sel wParam
	case 4
	 SendMessage id(3 hDlg) WM_USER+2 GetDlgItemInt(hDlg 5 0 0) 0
	 but- id(GetDlgItemInt(hDlg 5 0 0) hDlg)
ret 1
