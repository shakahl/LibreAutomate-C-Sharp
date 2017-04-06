\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_ComboBox 0x54230242 0x0 8 8 96 213 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3"
str cb3
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_DROPDOWN<<16|3
	sub.GetProcesses _s
	DT_SetControl hDlg 3 _s
	
	case IDOK
	case IDCANCEL
ret 1


#sub GetProcesses
function str&csv

ARRAY(str) a; int i
EnumProcessesEx 0 a 3
ICsv x._create
for i 0 a.len
	str s0.getfilename(a[i]) s1(i) s2 s3(a[i])
	x.AddRowSA(-1 4 &s0)
 x.Sort(4)

s0.all; s1=a; s2=0x401|8; s3.all
x.AddRowSA(0 4 &s0)

x.ToString(csv)
 out csv
