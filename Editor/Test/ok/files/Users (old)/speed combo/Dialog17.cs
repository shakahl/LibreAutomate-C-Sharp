\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
cb3="Fast (10 ms)[]Normal (100 ms)[]Slow (1000 ms)"
if(!ShowDialog("Dialog17" &Dialog17 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ComboBox 0x54230243 0x0 34 6 96 213 ""
 4 Button 0x54032000 0x0 34 24 96 14 "Start Macro327"
 5 Button 0x54032000 0x0 34 42 96 14 "Stop Macro327"
 6 Static 0x54000000 0x0 2 8 28 12 "Speed"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
double+ g_speed
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;not necessary in QM >= 2.1.9
	
	if(!g_speed) g_speed=0.1
	 select item that match current speed
	_i=g_speed*1000
	sel _i
		case 10 _i=0
		case 100 _i=1
		case 1000 _i=2
		case else ret 1
	CB_SelectItem(id(3 hDlg) _i)
	
	ret 1 ;;not necessary in QM >= 2.1.9
	case WM_DESTROY DT_DeleteData(hDlg) ;;not necessary in QM >= 2.1.9
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_SELENDOK<<16|3
	 set speed
	_i=CB_SelectedItem(lParam)
	sel _i
		case 0 g_speed=0.01
		case 1 g_speed=0.1
		case 2 g_speed=1
	
	case 4 mac "Macro327"
	case 5 shutdown -6 0 "Macro327"
	case IDOK
	DT_Ok hDlg ;;not necessary in QM >= 2.1.9
	case IDCANCEL DT_Cancel hDlg ;;not necessary in QM >= 2.1.9
ret 1
