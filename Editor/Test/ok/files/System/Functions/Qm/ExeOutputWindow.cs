 /
function [flags] [$caption] [hwndOwner] ;;flags: 1 close, 2 don't show if QM is running, 4 topmost

 Shows a simple output window.

 flags:
   1 - close the output window. Other parameters are not used.
   2 - if QM is running, don't show the output window.
   4 - make the output window always-on-top.
 caption - title bar text.
 hwndOwner - owner window handle. The output window will be on top of this window. It also removes the taskbar button. You can use _hwndqm.

 REMARKS
 Normally this function is used in exe, where QM is unavailable, but works in QM too.
 Supports just basic output features. No tags/links, history, buffering.
 Uses <help>RedirectQmOutput</help> to capture all output text (<help>out</help>, errors, etc) of current process.
 Runs in separate thread. Allows single instance in current process.

 See also: <ExeConsoleRedirectQmOutput>


type ___EOW flags ~caption hwndOwner

if(flags&3=2 and win("" "QM_Editor")) ret
lock
int+ ___eow_hwnd
if flags&1
	if(___eow_hwnd) clo ___eow_hwnd
else
	if(IsWindow(___eow_hwnd)) ret
	___eow_hwnd=0
	___EOW* p._new; p.flags=flags; p.caption=caption; p.hwndOwner=hwndOwner
	mac "sub.Thread" "" p
	wait 5 V ___eow_hwnd

err+


#sub Thread
function ___EOW*p

int+ ___eow_hwnd
if(p.caption.len) p.caption.escape(1); else p.caption="QM exe output"
int exStyle=0; if(p.flags&4) exStyle|WS_EX_TOPMOST

str dd=
F
 BEGIN DIALOG
 1 "" 0x90CF08C8 {exStyle} 0 0 322 124 "{p.caption}"
 3 RichEdit20W 0x50203844 0x0 0 0 322 124 ""
 END DIALOG

if(!ShowDialog(dd &sub.DlgProc 0 p.hwndOwner 128)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	__SetAutoSizeControls hDlg "3s"
	RegWinPos hDlg "ExeOutputWindow pos" "" 0
	___eow_hwnd=hDlg
	RedirectQmOutput &sub.OutRedirProc
	hid- hDlg
	
	case WM_DESTROY
	RedirectQmOutput 0
	___eow_hwnd=0
	RegWinPos hDlg "ExeOutputWindow pos" "" 1
	
	case WM_APP goto gAddText
	
	case WM_APP+1 SetDlgItemText hDlg 3 ""
	
	case WM_CONTEXTMENU
	sel ShowMenu("1 Clear" hDlg)
		case 1 out
	
	case WM_SETFOCUS ret 1
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 gAddText
int h=id(3 hDlg)
lpstr s=+lParam
SendMessageW h EM_SETSEL -1 -1
SendMessageW h EM_REPLACESEL 0 @s
SendMessageW h WM_VSCROLL SB_BOTTOM 0

err+


#sub OutRedirProc
function# str&s reserved

int+ ___eow_hwnd

if &s
	 if(s.beg("<>")) s.replacerx("(?s)</?\w+.*?>"); s.get(s 2)
	s+"[10]"
	SendMessage ___eow_hwnd WM_APP 0 s
else
	SendMessage ___eow_hwnd WM_APP+1 0 0

ret 1
