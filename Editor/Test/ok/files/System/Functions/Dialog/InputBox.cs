 /
function# str&inputText [flags] [$staticText] [$captionText] [hwndOwner] [x] [y] [int&checkVar] [$checkText] [timeoutS] [dlgProc] ;;flags: 1 on Cancel end macro, 2 use inputText as default, 4 multiline, 8 password, 16 digits, 32 button, 64 raw x y, 128 OK on timeout

 Shows input box dialog.
 Returns: 1 OK, 0 Cancel, 2 timeout with flag 128.

 inputText - variable that receives text.
 flags - see above.
 staticText - text above the edit field. Supports <help #IDP_SYSLINK>links</help>.
 captionText - title bar text.
 hwndOwner - owner window handle. If not used, the dialog is always-on-top.
 x, y - dialog position in screen. By default, 0 is screen center, negative is from screen right or top.
 checkVar - variable that sets and receives check box value (0 or 1). If omitted or 0, does not show check box.
 checkText - check box text.
 timeoutS - max number of seconds to show the dialog if user does not type or click. On timeout the function returns 0; if flag 128, returns 2.
 dlgProc - address of optional <help #IDH_DIALOG_EDITOR#A9>dialog procedure</help> function, like with smart dialogs.
   Flag 32 adds button [...], id 7. Then dlgProc can do something when user clicks it. By default the button does nothing.
   The function also can use the edit box (id 4) and other controls.

 REMARKS
 Uses <help>ShowDialog</help>.
 Supports <help>_monitor</help>.

 See also: <inp>.
 Added in: QM 2.3.5.

 EXAMPLES
 str s
 if(!InputBox(s 0 "Text:")) ret
 out s

 InputBox(_s 1 "The macro will end on Cancel.")

 _s="default"
 if(!InputBox(_s 2)) ret

 if(!InputBox(_s 0 "" "Input box at screen right-bottm" 0 -1 -1)) ret

 if(!InputBox(_s 0 "" "Input box with timeout" 0 0 0 0 "" 10)) ret


if(empty(captionText)) captionText="QM - Input"
str sStatic(staticText) sCaption(captionText) sCheck(checkText)
sStatic.escape(1); sCaption.escape(1); sCheck.escape(1)

int estDl(0x10189) stEd(iif(flags&4 0x54231044 0x54030080)) stCh(0x44012003) stBt(0x44030000)
if(hwndOwner) estDl~WS_EX_TOPMOST
if(flags&8) stEd|ES_PASSWORD
if(flags&16) stEd|ES_NUMBER
if(&checkVar) stCh|WS_VISIBLE

int wDl(223) hDl(68) wSt(212) wEd(208) hEd(14)
if(flags&4) wDl+100; hDl+150; wSt+100; wEd+100; hEd+150
if(flags&32) stBt|WS_VISIBLE; wEd-16
if(timeoutS) wSt-24
str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80ACC 0x{estDl} 0 0 {wDl} {hDl} "{sCaption}"
 4 Edit 0x{stEd} 0x200 8 26 {wEd} {hEd}
 7 Button 0x{stBt} 0 {wDl-23} 26 16 14 "..."
 5 Button 0x{stCh} 0 8 {hDl-20} 98 16 "{sCheck}"
 6 Static 0x44000002 0x0 {wDl-23} 0 20 9
 1 Button 0x54030001 0 116 {hDl-18} 48 14 "OK"
 2 Button 0x54030000 0 168 {hDl-18} 48 14 "Cancel"
 3 _SysLink 0x54030000 0x0 8 4 {wSt} 21 "{sStatic}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "" "" "" ""

str controls = "4 5"
str e4 c5
if(flags&2) e4=inputText
if(&checkVar) c5=checkVar!0

type ___INPBOX flags timeoutS dlgProc
___INPBOX z.flags=flags; z.timeoutS=timeoutS; z.dlgProc=dlgProc

#if !EXE
__PlayQmSound 3
#endif
int R=ShowDialog(dd &sub.DlgProc &controls hwndOwner flags&64 0 0 &z x y)
if !R
	if(flags&1) end
	ret

inputText=e4
if(&checkVar) checkVar=val(c5)
ret R


#sub DlgProc
function# hDlg message wParam lParam

___INPBOX& z=+DT_GetParam(hDlg); if(!&z) ret
int R i
sel message
	case WM_INITDIALOG
	if z.timeoutS>0
		hid- id(6 hDlg)
		SetTimer hDlg 1 1000 0
	
	case WM_TIMER
	sel wParam
		case 1
		z.timeoutS-1
		if(z.timeoutS>0) _s=z.timeoutS; _s.setwintext(id(6 hDlg))
		else KillTimer hDlg 1; if(z.flags&128) DT_Ok hDlg 2; else DT_Cancel hDlg
		ret 1
	
	case WM_SETCURSOR
	if lParam>>16!WM_MOUSEMOVE
		 g1
		if(z.timeoutS>0) KillTimer hDlg 1; z.timeoutS=0
	
	case WM_COMMAND
	R=1
	sel(wParam) case EN_CHANGE<<16|4 goto g1; case EN_KILLFOCUS<<16|4 if(GetFocus) goto g1
	
	case WM_NOTIFY
	R=1
	NMHDR* nh=+lParam
	if nh.idFrom=3
		i=__SysLinkOnClick(hDlg nh)
		if(i) DT_EndDialog(hDlg i)

if(z.dlgProc) ret call(z.dlgProc hDlg message wParam lParam)
ret message=WM_COMMAND
