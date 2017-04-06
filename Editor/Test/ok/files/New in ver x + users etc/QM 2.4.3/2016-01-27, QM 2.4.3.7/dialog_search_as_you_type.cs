 Run this macro. Type something from the list, eg o or r.
 Need QM 2.4.3.


str sList
sList=
 one
 two
 three
 four

ARRAY(str) aList=sList
IQmDropdown ddl
int inERS

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 8 96 12 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040303 "*" "" "" ""

str controls = "3"
str e3
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc v
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
	case EN_CHANGE<<16|3 sub.OnTextChanged lParam
ret 1


#sub OnTextChanged v
function hEdit

if(inERS) ret
if(ddl) ddl.Close; ddl=0

str s.getwintext(hEdit)
if(!s.len) ret

ICsv x._create
x.AddRow1(0 "")
int i
for i 0 aList.len
	if(find(aList[i] s 0 1)<0) continue
	x.AddRow1(-1 aList[i])
if(!x.RowCount) ret

if(ShowDropdownList(x i 0 1 hEdit 0 0 0 ddl)&QMDDRET_SELOK=0) ret
s=x.Cell(i+1 0)

inERS=1
EditReplaceSel hEdit 0 s 1
inERS=0
