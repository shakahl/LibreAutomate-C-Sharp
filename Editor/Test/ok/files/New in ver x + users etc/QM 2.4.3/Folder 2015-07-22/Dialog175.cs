\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ComboBox 0x54230243 0x0 4 4 96 213 ""
 4 ComboBox 0x54230242 0x0 4 20 96 213 ""
 5 ComboBox 0x54230641 0x0 4 36 96 48 ""
 6 ListBox 0x54230101 0x200 108 4 96 48 ""
 7 ListBox 0x54230109 0x200 108 56 96 48 ""
 8 QM_ComboBox 0x54030243 0x0 4 88 96 13 ""
 9 QM_ComboBox 0x54030242 0x0 4 104 96 13 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4 5 6 7 8 9"
str cb3 cb4 cb5 lb6 lb7 qmcb8 qmcb9
qmcb8="0[]zero[]one[]two"
 qmcb9=qmcb8
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case CBN_DROPDOWN<<16|9
	 _i=CB_SelectedItem(lParam)
	 out _i
	lpstr o=":5 $qm$\il_qm.bmp"
	str s=
	F
	 ,"{o}"
	 one,1
	 two,2
	 three,0
	
	ICsv x._create
	x.FromString(s)
	
	int i R=ShowDropdownList(x i 0 0 lParam)
	if(R&0x10=0) ret
	 out i
	s=x.Cell(i+1 0)
	s.setwintext(lParam)
ret 1
