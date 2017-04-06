
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 322 "Dialog"
 3 SysTreeView32 0x54030000 0x0 4 4 162 314 ""
 1 Button 0x54030001 0x4 168 100 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG goto gInit
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 gInit
PF
ICsv x._create; x.Separator="|"
x.FromFile("q:\test\ok\LIST.csv")
PN
int h=id(3 hDlg)
int i n=x.RowCount
for i 0 n
	TvAdd(h 0 x.Cell(i 0))
	 TvAdd(h 0 +LPSTR_TEXTCALLBACK)
PN;PO
