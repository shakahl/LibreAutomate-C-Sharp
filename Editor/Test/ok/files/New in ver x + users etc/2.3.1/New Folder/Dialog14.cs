\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6 7 10 11"
str c3Che c4Che c5Che c6Che c7Che c10Che c11Che
if(!ShowDialog("Dialog14" &Dialog14 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x0 8 6 48 12 "Check"
 4 Button 0x54012003 0x0 8 24 48 12 "Check"
 5 Button 0x54012003 0x0 8 42 48 12 "Check"
 6 Button 0x54012003 0x0 8 58 48 12 "Check"
 7 Button 0x54012003 0x0 8 74 48 12 "Check"
 10 Button 0x54012003 0x0 8 88 48 12 "Check"
 11 Button 0x54012003 0x0 8 102 48 12 "Check"
 8 Button 0x54032000 0x0 86 16 48 14 "off"
 9 Button 0x54032000 0x0 86 32 48 14 "on"
 END DIALOG
 DIALOG EDITOR: "" 0x203000A "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [8,9]
	 ARRAY(int) a; int i
	 ControlsToArray hDlg "3 5-7 11" a
	 for(i 0 a.len) EnableWindow a[i] wParam=9
	
	 foreach(_i hDlg FE_Control "3 5-7 11") EnableWindow _i wParam=9
	
	 MultiControlAction hDlg "3 5-7 -11" MCA_Show wParam=9
	 MultiControlAction hDlg "3 5-7 -11" &MCA_Enable wParam=9
	MultiControlAction hDlg "3 5-7 -11" MCA_Check wParam=9
	 MultiControlAction hDlg "3 5-7 -11" MCA_NoTheme
	
	case IDOK
	case IDCANCEL
ret 1
