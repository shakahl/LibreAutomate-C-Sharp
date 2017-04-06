 /dialog_QM_Tools
function# message wParam word*w

str s sw sc; int i
if message=WM_SETTEXT
	sw.ansi(w)
	sel numlines(sw)
		case 1
			i=1
		case 2
			 text format:
			 "child(... {window})|id(... {window})\r\nwin(...)"
			sc.getl(sw 1)
			sw.getl(sw 0)
			i=2
		case 0
			i=1
			sw.all
		case else ret
	sub_to.SetTextNoNotify mw_heW sw
	sub_to.SetTextNoNotify mw_heC sc
	_WinSelect(i)
	ret 1

if mw_what=0 and m_flags&0xC00=0xC00 and mw_comments.len ;;get comments if screen too. Used in Mouse dialog.
else if(mw_what<1) ret

if message=WM_GETTEXTLENGTH ;;else WM_GETTEXT
	int R=mw_comments.len+200
	if(mw_what) R+GetWindowTextLengthW(mw_heW)+GetWindowTextLengthW(mw_heC)*2
	ret R

if(!mw_what) s="0"
else if(!_WinGetText(s sw)) ret

lpstr co; if(mw_what=2 or m_flags&0x400) co=mw_comments
s=F"{mw_what}[]{s}[]{sw}[]{co}[]"

s.unicode
int n=s.len/2; if(n>=wParam) n=wParam-1; if(n<0) ret
memcpy w s n*2; w[n]=0
ret n
