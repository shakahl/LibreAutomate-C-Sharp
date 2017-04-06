 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "6 7 8 9 10 11 24 27"
str o6Bef o7Aft o8Fir o9Las o10Fir o11Las e24 e27
if(!ShowDialog("" &dialog_test_sqlite_tree &controls)) ret

 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 223 561 "Sqlite Test"
 3 Button 0x54032000 0x0 176 0 48 14 "Insert"
 4 Button 0x54032000 0x0 176 126 48 14 "Delete"
 5 Button 0x54032000 0x0 176 16 48 14 "Move"
 6 Button 0x54032009 0x0 176 36 46 10 "Before"
 7 Button 0x54002009 0x0 176 46 46 10 "After"
 8 Button 0x54002009 0x0 176 56 46 10 "First child"
 9 Button 0x54002009 0x0 176 66 46 10 "Last child"
 10 Button 0x54002009 0x0 176 76 46 10 "First sib"
 11 Button 0x54002009 0x0 176 86 46 10 "Last sib"
 12 Button 0x54032000 0x0 176 142 48 14 "Rename"
 13 Button 0x54032000 0x0 176 158 48 14 "Text"
 14 Button 0x54032000 0x0 176 174 48 14 "Trigger"
 15 Button 0x54032000 0x0 176 190 48 14 "Icon"
 16 Button 0x54032000 0x0 176 206 48 14 "Flags"
 17 Button 0x54032000 0x0 176 230 48 14 "Viewer"
 18 Button 0x54032000 0x0 176 246 48 14 "Import"
 19 Button 0x54032000 0x0 176 262 48 14 "Export"
 20 Button 0x54032000 0x0 176 278 48 14 "Add SHF"
 21 Button 0x54032000 0x0 176 300 48 14 "Resource..."
 33 Button 0x54032000 0x0 176 424 48 14 "Get text"
 34 Button 0x54032000 0x0 176 440 48 14 "Get trigger"
 35 Button 0x54032000 0x0 176 456 48 14 "Get icon"
 36 Button 0x54032000 0x0 176 472 48 14 "Get SHF"
 100 Button 0x54032000 0x0 176 500 48 14 "Test"
 24 Edit 0x54030080 0x200 176 350 46 14 ""
 23 Button 0x54032000 0x0 176 366 48 14 "Find"
 25 Static 0x54000000 0x0 176 342 46 8 "Name"
 26 Static 0x54000000 0x0 176 388 48 10 "Trigger"
 27 Edit 0x54030080 0x200 176 398 46 14 ""
 28 Button 0x54032000 0x0 176 98 48 14 "Clone"
 22 Button 0x54032000 0x0 176 316 48 14 "Enum"
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "*" "" "" ""

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
