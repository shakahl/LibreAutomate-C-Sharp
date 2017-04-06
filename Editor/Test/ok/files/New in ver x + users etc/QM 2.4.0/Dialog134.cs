\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str lb3 lb4 lb5
lb3="a[]b[]c"
lb5="one[]two[]three"
if(!ShowDialog("Dialog134" &Dialog134 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 3 ListBox 0x54230101 0x200 0 0 96 48 ""
 4 ListBox 0x54230101 0x200 106 0 96 48 ""
 5 ListBox 0x54230101 0x200 56 56 96 48 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3 ;;listbox 1
	int hlb2=id(4 hDlg)
	SendMessage hlb2 LB_RESETCONTENT 0 0
	_i=LB_SelectedItem(lParam)
	str s ss
	sel _i
		case 0 s="1[]2[]3"
		case 1 s="5[]6[]7"
	foreach ss s
		LB_Add hlb2 ss
	
	case LBN_SELCHANGE<<16|4 ;;listbox 2
	_i=LB_SelectedItem(lParam)
	out _i
	
	case LBN_SELCHANGE<<16|5 ;;listbox 3
	int selectedInLB2 selectedInLB3
	selectedInLB2=LB_SelectedItem(id(4 hDlg))
	selectedInLB3=LB_SelectedItem(id(5 hDlg))
	mes F"selectedInLB2={selectedInLB2}[]selectedInLB3={selectedInLB3}"
	
	case IDOK
	case IDCANCEL
ret 1
