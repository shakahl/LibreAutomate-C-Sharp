\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Edit 0x56030080 0x200 8 8 96 12 ""
 4 QM_Edit 0x56030080 0x200 8 28 208 12 ""
 1 Button 0x54030001 0x0 168 116 48 14 "OK"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4"
str qme3 qme4
qme3="zero"
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qme3
out qme4


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SendDlgItemMessage hDlg 4 QMEM_SETBUTTON 0 0 ;;add user button "open folder" and remove arrow
	SendDlgItemMessage hDlg 4 EM_SETCUEBANNER 0 @" File"
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_DROPDOWN<<16|3 ;;pressed the arrow button
	 show drop-down list, like ComboBox
	lpstr o=":5 $qm$\il_qm.bmp"
	str s=F"0,{o}[]zero,1[]one,2[]two,28"
	ICsv x._create; x.FromString(s)
	int i R=ShowDropdownList(x i 0 1 lParam)
	if(R&QMDDRET_SELOK=0) ret
	s=x.Cell(i+1 0); s.setwintext(lParam)
	
	case 4 ;;clicked the user button (BN_CLICKED)
	str sFile
	if(OpenSaveDialog(0 sFile)) sFile.setwintext(lParam)
ret 1
