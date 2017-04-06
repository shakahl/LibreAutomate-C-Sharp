\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6 7 8"
str e3 c4Che e5 cb6 lb7 lb8
cb6="zero[]one[]two"
lb7="zero[]one[]two"
lb8="zero[]one[]two"
if(!ShowDialog("dlg_save_control_values" &dlg_save_control_values &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 294 "Dialog"
 1 Button 0x54030001 0x4 64 276 48 14 "OK"
 2 Button 0x54030000 0x4 116 276 48 14 "Cancel"
 3 Edit 0x54030080 0x200 50 6 52 14 ""
 4 Button 0x54012003 0x0 6 26 52 12 "Check"
 5 Edit 0x54030020 0x200 168 6 52 14 ""
 6 ComboBox 0x54230243 0x0 168 26 52 213 ""
 7 ListBox 0x54230101 0x200 50 44 52 30 ""
 8 ListBox 0x54230109 0x200 168 44 52 30 ""
 10 msctls_hotkey32 0x54030000 0x200 50 84 106 14 ""
 11 msctls_trackbar32 0x54030000 0x0 50 102 106 18 ""
 12 SysDateTimePick32 0x56030000 0x0 50 144 106 14 "2010.01.16"
 13 SysIPAddress32 0x54030000 0x200 50 124 106 14 "0.0.0.0"
 14 SysMonthCal32 0x54030000 0x0 50 164 102 96 ""
 9 Static 0x54000000 0x0 6 44 28 13 "Listbox"
 15 Static 0x54000000 0x0 126 44 30 26 "Listbox multisel"
 16 Static 0x54000000 0x0 6 6 40 12 "Edit"
 17 Static 0x54000000 0x0 124 6 40 12 "Password"
 18 Static 0x54000000 0x0 124 26 40 12 "Combo"
 19 Static 0x54000000 0x0 6 84 40 13 "Hotkey"
 20 Static 0x54000000 0x0 4 102 42 12 "Trackbar"
 21 Static 0x54000000 0x0 4 124 42 13 "IP address"
 22 Static 0x54000000 0x0 4 144 42 12 "DateTime"
 23 Static 0x54000000 0x0 4 166 42 12 "MonthCal"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	LoadDialogControlValues hDlg "\Test\SaveControls" "xml" "test password"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	SaveDialogControlValues hDlg "\Test\SaveControls" "xml" "3 4 5 6 7 8 10 11 12 13 14" "test password"
	case IDCANCEL
ret 1
