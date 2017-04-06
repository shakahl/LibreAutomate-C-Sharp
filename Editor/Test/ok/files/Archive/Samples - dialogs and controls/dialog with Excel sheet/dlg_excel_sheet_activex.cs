 \Dialog_Editor

 Displays Excel worksheet as OWC11.Spreadsheet ActiveX control, if available.

typelib OWC11 {0002E558-0000-0000-C000-000000000046} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="&Page0[]Page1[]Page2"
if(!ShowDialog("" &dlg_excel_sheet_activex &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 265 163 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 96 80 ""
 1001 Static 0x54020000 0x4 104 6 24 10 "Page0"
 1003 Button 0x54032000 0x0 138 4 48 14 "set A3"
 1004 Button 0x54032000 0x0 190 4 48 14 "get A4"
 1002 ActiveX 0x54030000 0x0 104 22 156 114 "OWC11.Spreadsheet {0002E559-0000-0000-C000-000000000046} data:676C9BD4FF8B88A690B422072CD41ECE26C4D3BDDCD1149301"
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 4 138 255 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "0" ""

ret
 messages
sel message
	case WM_INITDIALOG
	OWC11.Spreadsheet sp1002
	
	goto selectpage
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 1003
	sp1002._getcontrol(id(1002 hDlg))
	sp1002.Range("A3").Value2="hi"
	
	case 1004
	sp1002._getcontrol(id(1002 hDlg))
	str s=sp1002.Range("A4").Value2
	mes s
	
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	 selectpage
	_i=LB_SelectedItem(id(3 hDlg))
	DT_Page hDlg _i
ret 1

