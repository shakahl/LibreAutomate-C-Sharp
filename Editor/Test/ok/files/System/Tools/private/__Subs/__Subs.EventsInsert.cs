function hwndEditor idMacro $message $sAppend flags ;;flags: 1 WM_ message

 Inserts "[9]case message{sAppend}[]" in idMacro, or just finds/shows existing "[9]case message".

if flags&1=0
	if(!e.cmdBegin) mes "The event procedure must have sel wParam."; ret
	sel message
		case "1" message="IDOK"
		case "2" message="IDCANCEL"

int hCE=GetQmCodeEditor
str sNewCode sFindExistingCode

sNewCode.from("[9]case " message)
sFindExistingCode=F"^\Q{sNewCode}\E\b"
sNewCode+sAppend; sNewCode+"[][9][]"

 minimize DE/ME and open macro
RECT r1 r2; GetWindowRect hCE &r1; GetWindowRect hwndEditor &r2; if(IntersectRect(&r1 &r1 &r2)) min hwndEditor
mac+ idMacro; act hCE

 find existing code
FINDRX f; if(flags&1) f.ifrom=e.wmBegin; f.ito=e.wmEnd; else f.ifrom=e.cmdBegin; f.ito=e.cmdEnd
int j i=findrx(sText sFindExistingCode f 8 j)
if i<0 ;;insert
	i=f.ito
	SendMessage hCE SCI.SCI_GOTOPOS i 0
	InsertStatement sNewCode 0 0 1
	i=SendMessage(hCE SCI.SCI_GETCURRENTPOS 0 0)-2
else i+j ;;already exists

SendMessage hCE SCI.SCI_GOTOPOS i 0
if(GetCaretXY(i j 0 _i)) sub_sys.TooltipOsd "[]Add your code here[]" 16 "Events" 0 i j+_i

err+
