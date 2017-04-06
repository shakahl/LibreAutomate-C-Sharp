\Dialog_Editor

 Rich edit control does not show images by default. It seems need to implement an intercface.

if(!GetModuleHandle("Msftedit.dll")) LoadLibrary("Msftedit.dll")

str dd=
 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 RichEdit50W 0x54233044 0x200 0 0 224 114 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

str controls = "3"
str re3
 re3="&$desktop$\cat.rtf" ;;does not work with RichEdit50W
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	SendMessage h EM_LIMITTEXT 0x7FFFFFFE 0
	RichEditLoad(h "$desktop$\cat.rtf")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
