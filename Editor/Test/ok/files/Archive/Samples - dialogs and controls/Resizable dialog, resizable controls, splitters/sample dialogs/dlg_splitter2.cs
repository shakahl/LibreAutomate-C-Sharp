\Dialog_Editor

 Dialog with many splitters.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 9 12"
str lb3 e4 e5 rea9 c12Che
if(!ShowDialog("dlg_splitter2" &dlg_splitter2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 317 191 "Dialog"
 3 ListBox 0x54230101 0x200 4 20 94 94 ""
 4 Edit 0x54231044 0x200 102 4 116 70 ""
 5 Edit 0x54230844 0x20000 102 78 102 36 ""
 10 Static 0x54000000 0x0 26 8 72 12 "Text"
 9 RichEdit20A 0x54233044 0x200 38 118 276 48 ""
 12 Button 0x54012003 0x0 206 102 48 12 "Check"
 11 Button 0x54032000 0x0 224 4 92 14 "Button"
 14 Static 0x54000000 0x0 224 20 48 5 ""
 1 Button 0x54030001 0x4 120 174 48 14 "OK"
 2 Button 0x54030000 0x4 172 174 48 14 "Cancel"
 8 QM_Splitter 0x54000000 0x0 4 114 214 4 ""
 6 QM_Splitter 0x54000000 0x0 98 4 4 110 ""
 7 QM_Splitter 0x54000000 0x0 102 74 116 4 ""
 13 QM_Splitter 0x54000000 0x0 218 4 6 72 ""
 15 QM_Splitter 0x54000000 0x0 32 118 6 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
