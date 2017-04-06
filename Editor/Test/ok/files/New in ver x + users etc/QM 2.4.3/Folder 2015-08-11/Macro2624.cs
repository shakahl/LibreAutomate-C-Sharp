\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Edit 0x56030080 0x200 8 8 96 12 ""
 4 QM_Edit 0x56030080 0x204 8 24 96 12 ""
 5 QM_Edit 0x56030080 0x200 8 40 208 12 ""
 1 Button 0x54030001 0x0 168 116 48 14 "OK"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

 normal (client edge)
 3 QM_Edit 0x56030080 0x200 8 8 96 12 ""
 4 QM_Edit 0x56030080 0x204 8 24 96 12 ""
 5 QM_Edit 0x56030080 0x200 8 40 208 12 ""
 no border
 3 QM_Edit 0x56030080 0x000 8 8 96 10 ""
 4 QM_Edit 0x56030080 0x004 8 24 96 10 ""
 5 QM_Edit 0x56030080 0x000 8 40 208 10 ""
 static edge
 3 QM_Edit 0x56030080 0x20000 8 8 96 11 ""
 4 QM_Edit 0x56030080 0x20004 8 24 96 11 ""
 5 QM_Edit 0x56030080 0x20000 8 40 208 11 ""

str controls = "3 4 5"
str qme3 qme4 qme5
qme3="zero"
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qme3
 out qme4


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 int h=id(3 hDlg)
	 SetWindowTheme h L"" L""
	  out GetWindowTheme(h)
	 SetWindowPos h 0 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOZORDER|SWP_FRAMECHANGED
	
	SendDlgItemMessage hDlg 4 QMEM_SETBUTTON 0 0 ;;add "open folder" button and remove arrow
	
	 __Hicon hi=GetFileIcon("mouse.ico")
	 SendDlgItemMessage hDlg 4 QMEM_SETBUTTON 1 hi
	
	__ImageList il.Load("$qm$\il_qm.bmp")
	SendDlgItemMessage hDlg 4 QMEM_SETBUTTON MakeInt(2 3) il
	
	 SendDlgItemMessage hDlg 4 QMEM_SETBUTTON 3 107
	
	SendDlgItemMessage hDlg 5 QMEM_SETBUTTON 0x100 0 ;;add "open folder" button
	SendDlgItemMessage hDlg 5 EM_SETCUEBANNER 0 @" File"
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [CBN_DROPDOWN<<16|3,CBN_DROPDOWN<<16|5] ;;pressed the arrow button
	 show drop-down list, like ComboBox
	lpstr o=":5 $qm$\il_qm.bmp"
	str s=F"0,{o}[]zero,1[]one,2[]two,28"
	ICsv x._create; x.FromString(s)
	int i R=ShowDropdownList(x i 0 1 lParam)
	if(R&QMDDRET_SELOK=0) ret
	s=x.Cell(i+1 0); s.setwintext(lParam)
	
	case [4,5] ;;clicked the "open folder" button (BN_CLICKED)
	str sFile
	if(OpenSaveDialog(0 sFile)) sFile.setwintext(lParam)
ret 1
