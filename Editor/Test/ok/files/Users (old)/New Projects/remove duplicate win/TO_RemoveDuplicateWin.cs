\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
int i n from to
str txt
from=sub_to.SciGetSelText(0 txt)
if(!txt.len) txt.getmacro; from=0
to=from+txt.len
 out txt

ARRAY(str) a
str rx="(?m)^[\t,]*(int +)?([A-Za-z_]\w*) *= *(act\( *|wait\( *[\w\.]+ +(?:W\w +)?)?(win\(.+\))$"
 out rx
if(txt.len) findrx(txt rx 0 4 a)
ICsv c._create
for i 0 a.len
	c.AddRow2(-1 a[4 i] F"{1}")

str controls = "3"
str qmg3x
c.ToString(qmg3x)

if(!ShowDialog("TO_RemoveDuplicateWin" &TO_RemoveDuplicateWin &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Remove duplicate win(...)"
 3 QM_Grid 0x56031041 0x200 0 0 224 112 "0x33,0,0,4,0x0[]win(...),90%,,[]n,9%,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030600 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

#ret
int w=win("etc1" "etc")
	int w=win("et)c2" "etc")
w1=win("etc3" "etc")
int w2=act(win("etc4", "etc"))
int w2=wait(5 win("etc5" "etc"))
int w2=wait(5.2 WV win("etc6" "etc"))
int w=win("A" "B" "C")
int w=win("A" "B" "C" 8|1)
int w=win("comm") ;;comm
