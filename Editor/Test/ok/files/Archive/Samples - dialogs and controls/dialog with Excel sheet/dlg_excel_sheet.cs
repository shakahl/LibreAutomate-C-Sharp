\Dialog_Editor

 Displays Excel worksheet in web browser control.
 MS Office with Excel must be installed.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 1002"
str lb3 ax1002SHD
lb3="&Page0[]Page1[]Page2"
 ax1002SHD="$personal$\book1.xls" ;;painting problems
if(!ShowDialog("" &dlg_excel_sheet &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 265 163 "Dialog"
 3 ListBox 0x54230101 0x204 4 4 96 80 ""
 1001 Static 0x54020000 0x4 104 6 24 10 "Page0"
 1003 Button 0x54032000 0x0 138 4 48 14 "set A3"
 1004 Button 0x54032000 0x0 190 4 48 14 "get A4"
 1002 ActiveX 0x54030000 0x0 108 22 154 110 "SHDocVw.WebBrowser"
 1101 Static 0x44020000 0x4 106 4 48 13 "Page1"
 1201 Static 0x44020000 0x4 106 4 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 5 Static 0x54000010 0x20004 4 138 254 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "0" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we1002
	we1002._getcontrol(id(1002 hDlg))
	we1002.Navigate(_s.expandpath("$personal$\book1.xls"))
	
	goto selectpage
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [1003,1004]
	we1002._getcontrol(id(1002 hDlg))
	Excel.Workbook eb
	ExcelSheet es
	eb=we1002.Document
	es.ws=eb.ActiveSheet
	if(wParam=1003) es.SetCell("hi" 1 3)
	else
		es.GetCell(_s 1 4); err mes "error"; ret ;;error if editing the cell
		mes _s
	
	case IDOK
	case IDCANCEL
	case LBN_SELCHANGE<<16|3
	 selectpage
	_i=LB_SelectedItem(id(3 hDlg))
	DT_Page hDlg _i
ret 1

