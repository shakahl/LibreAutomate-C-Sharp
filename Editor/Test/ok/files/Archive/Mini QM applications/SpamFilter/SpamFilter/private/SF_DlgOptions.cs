 \Dialog_Editor
function# [hDlg] [message] [wParam] [lParam]
if(hDlg) goto messages

str controls = "6 8 9 10 20 13 24 23 16 17 19 30"
str e6acc e8per c9Ret e10lin e20fir c13Det c24Che e23ff e16pla c17Lau e19app c30Sho

e6acc=o.accountsr
e8per=o.period
e20fir=o.period0
e10lin=o.nlines
e16pla=o.sound
e19app=o.mailapp
e23ff=o.ff
if(o.flags&1) c17Lau=1
if(o.flags&2) c9Ret=1
if(o.flags&4) c13Det=1
if(o.flags&8) c24Che=1
if(o.flags&16) c30Sho=1

if(!ShowDialog("SF_DlgOptions" &SF_DlgOptions &controls wParam)) ret

lpstr rk="\SpamFilter"
int f=o.flags~255
if(c17Lau=1) f|1
if(c9Ret=1) f|2
if(c13Det=1) f|4
if(c24Che=1) f|8
if(c30Sho=1) f|16
rset f "flags" rk
_i=val(e8per); rset _i "period" rk; if(_i<3) out "Warning: some email providers may ban you if 'Check every' is too small"
rset val(e20fir) "period0" rk
rset val(e10lin) "nlines" rk
rset e6acc "accounts" rk
rset e16pla "sound" rk
rset e23ff "ff" rk
rset e19app "mailclient" rk

SF_GetOptions
if(o.flags&2 and o.nlines<100) mes "Number of body lines is very small. If message is downloaded partially, SpamFilter may not properly filter spam, and will not be able to restore whole message if it is deleted due to improper filters." "" "!"
SF_SetTimer o.period*30

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 265 198 "QM SpamFilter Options"
 4 Static 0x54020000 0x4 4 4 44 21 "Check accounts"
 6 Edit 0x54231044 0x204 54 2 138 32 "acc"
 3 Button 0x54032000 0x4 198 4 32 14 "Default"
 29 Button 0x54032000 0x4 230 4 32 14 "All"
 5 Button 0x54032000 0x4 198 20 64 14 "Accounts ..."
 7 Static 0x54020000 0x4 4 42 46 13 "Check every"
 8 Edit 0x54032000 0x204 54 40 32 14 "per"
 11 Static 0x54020000 0x4 88 42 16 12 "min"
 9 Button 0x54012003 0x4 118 40 92 14 "Retrieve only headers +"
 10 Edit 0x54032000 0x204 212 40 32 14 "lin"
 12 Static 0x54020000 0x4 246 42 18 13 "lines"
 14 Static 0x54020000 0x4 4 60 56 13 "First check after"
 20 Edit 0x54032000 0x204 64 58 22 14 "fir"
 21 Static 0x54020000 0x4 88 60 12 10 "s"
 13 Button 0x54012003 0x4 170 76 90 13 "Detect connection"
 24 Button 0x54012003 0x4 170 90 90 12 "Check after connected"
 22 Static 0x54020000 0x4 4 78 48 12 "Filter functions"
 23 Edit 0x54231044 0x204 54 76 102 30 "ff"
 18 Static 0x54020000 0x4 8 126 34 12 "Play"
 16 Edit 0x54030080 0x204 54 124 186 14 "pla"
 26 Button 0x54032000 0x4 242 124 16 14 "..."
 17 Button 0x54012003 0x4 8 158 76 12 "Launch email app"
 19 Edit 0x54030080 0x204 54 140 186 14 "app"
 28 Button 0x54032000 0x4 242 140 16 14 "..."
 1 Button 0x54030001 0x4 4 182 48 14 "OK"
 2 Button 0x54030000 0x4 56 182 48 14 "Cancel"
 15 Button 0x54032000 0x4 4 92 48 14 "New"
 30 Button 0x54012003 0x0 94 158 48 12 "Show SF"
 32 Static 0x54000000 0x0 8 142 42 12 "Email app"
 27 Button 0x54020007 0x4 164 64 98 42 "Internet connection"
 31 Button 0x54020007 0x4 4 114 258 58 "When new messages arrive"
 25 Static 0x54000010 0x20004 4 176 257 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "*" ""

ret 1
 messages
sel message
	case WM_INITDIALOG
#if EXE
	TO_Show hDlg "15 22 23" 0
#endif
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 29 TO_SetText "<all>" hDlg 6
	case 3 TO_SetText "<default>" hDlg 6
	case 5 MailSetup hDlg
	
#if !EXE
	case 15
	str s ss
	if(!inp(s "Name" "New filter function" "SF_Filter")) ret
	int i=newitem(s "" "SF_SampleFF" "" "" 4); err ret
	s.getmacro(i 1)
	ss.getwintext(id(23 hDlg)); if(ss.len and !ss.end("[]")) ss+"[]"
	ss+s; TO_SetText ss hDlg 23
#endif
	
	case 26 if(TO_Browse3(hDlg 16 "sounddir" "$Windows$\Media" "wav[]*.wav[]" "wav" _s)) bee _s; err
	case 28 TO_Browse3 hDlg 19 "rundir" "C:\" ".exe[]*.exe[]" "exe"
	case IDOK
	case IDCANCEL
ret 1
