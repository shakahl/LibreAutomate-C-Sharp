\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
if(!ShowDialog("dialog_grid_ping_ip" &dialog_grid_ping_ip &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 114 "0x12,0,0,0,0x0[]Computer name,,,[]IP,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG goto gInit
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto gNotify
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 _______________________

 gInit
str csv=
 test1,192.168.0.1
 tes21,192.168.0.2
 test3,192.168.0.3

ICsv- t_csv=CreateCsv(1)
t_csv.FromString(csv)
t_csv.ToQmGrid(id(3 hDlg))

ret
 _______________________

 gNotify
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	NMITEMACTIVATE* na=+nh
	sel nh.code
		case NM_CLICK ;;when user clicks a row or empty space, and it does not begin cell edit mode
		if(na.iItem<0) ret
		 out "row click: %i %i" na.iItem na.iSubItem
		str IP=t_csv.Cell(na.iItem 1)
		mes IP
