\Dialog_Editor

 Resizable dialog with 2 splitters.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str lb3 e4 e5
if(!ShowDialog("dlg_autosize_controls_and_splitters" &dlg_autosize_controls_and_splitters &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 225 133 "Dialog"
 3 ListBox 0x54230101 0x200 0 0 96 110 ""
 4 Edit 0x54231044 0x200 100 0 126 70 ""
 5 Edit 0x54230844 0x20000 100 74 126 36 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 6 QM_Splitter 0x54000000 0x0 96 0 4 110 ""
 7 QM_Splitter 0x54000000 0x0 100 70 126 4 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
DT_AutoSizeControls hDlg message "3sv 4sh 5s 6sv 7sh 1m 2m"
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
