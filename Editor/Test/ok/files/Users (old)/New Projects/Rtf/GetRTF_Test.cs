\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
rea3="&$desktop$\Document.rtf"
if(!ShowDialog("GetRTF_Test" &GetRTF_Test &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 6 6 96 48 ""
 4 Button 0x54032000 0x0 6 64 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	str s
	if(GetRTF(id(3 hDlg) s))
		mes s
		ShowText "" s
	case IDOK
	case IDCANCEL
ret 1
