\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
 lb3="yellow background[]blue text"
if(!ShowDialog("Dialog95" &Dialog95 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ListBox 0x54230161 0x200 6 8 96 48 ""
 4 Button 0x54032000 0x0 8 74 70 12 "Change height"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages

CB_ItemColor2 hDlg message wParam lParam 3 &SampleCbItemColorProc

sel message
	case WM_INITDIALOG
	 create and set font
	__Font-- f
	f.Create("Arial" 12 1)
	f.SetDialogFont(hDlg "3")
	 add items
	int hlb=id(3 hDlg)
	LB_Add hlb "yellow background"
	LB_Add hlb "blue text"
	
	case WM_APP+5
	goto g1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	 g1
	f.Create("Arial" 18 1)
	f.SetDialogFont(hDlg "3")
	SendMessage hDlg WM_APP+140 0 0
	hlb=id(3 hDlg)
	SendMessage hlb LB_RESETCONTENT 0 0
	LB_Add hlb "yellow background"
	LB_Add hlb "blue text"
	
	case IDCANCEL
ret 1
