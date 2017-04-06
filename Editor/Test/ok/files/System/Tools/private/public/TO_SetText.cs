 /
function $s hDlg idEdit [flags] ;;1 append line (if multiline Edit), 2 only if empty, 4 don't set focus, 8 move caret to the end

 Replaces text of an Edit or editable ComboBox control.
 Sets focus.
 If hDlg 0, uses idEdit as HWND.

if(!idEdit) ret
int h=iif(hDlg id(idEdit hDlg) idEdit)

if(flags&2 and GetWindowTextLengthW(h)) ret

sel WinTest(h "Edit[]ComboBox")
	case 1 if(flags&1 and GetWinStyle(h)&ES_MULTILINE and !empty(s)) flags|8; _s.getwintext(h); if(_s.len) _s.addline(s 1); s=_s
	case 2 h=child("" "Edit" h)

int f=3
if(flags&4) f=1
if(flags&8) f|4
EditReplaceSel h 0 s f
