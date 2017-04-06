\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_DlgInfo 0x54000004 0x20000 0 0 224 114 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" "3"

str controls = "3"
str qmdi3
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
		QmDlgInfoControlSetFont id(3 hDlg) "Courier New" 12
		
		 set text after changing font
		str s="<>Word 1<c 255>Word2</c>Word3[]Word 4<c 255>Word5</c>Word6[]<help>act</help>[]<code>SendMessage(id(3 hDlg), SCI.SCI_STYLESETFONT</code>"
		s.setwintext(id(3 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1