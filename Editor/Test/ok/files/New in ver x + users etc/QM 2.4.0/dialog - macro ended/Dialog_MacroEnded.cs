 \Dialog_Editor
function# [__mustBe0] [timeoutS] [$cbMacros] [__reserved]

 Shows dialog with macro name, error status, and you can run a macro again.
 Call this function when macro ends, for example from a function registered by <help>atend</help>.

 __mustBe0 - must be 0.
 timeoutS - if not 0, closes the dialog after timeoutS seconds. You can click in the dialog to stop the timer.
 cbMacros - optional list of macros to add to the combo box.
   Don't need to add current macro, it will be added as first item and selected, unless cbMacros begins with & character.
 __reserved - don't use.

 EXAMPLE
  MacroX:
 atend atend_Dialog_MacroEnded
 act "no such window" ;;test error

  Function atend_Dialog_MacroEnded:
 Dialog_MacroEnded 0 10


if(__mustBe0) goto messages

type ___DME timeout !isError
___DME d.timeout=timeoutS; d.isError=_error.source!0

str status=iif(d.isError _error.description "success")
str name.getmacro(getopt(itemid 3) 1)

str controls = "6"
str cb6

cb6=cbMacros; if(!cb6.beg("&")) cb6=F"&{name}[]{cb6}"

str dd=
F
 BEGIN DIALOG
 0 "" 0x90C808C8 0x80 0 0 223 72 "QM - Macro Ended"
 3 Static 0x54000000 0x0 6 6 214 10 "Macro: {name}"
 4 Static 0x54000000 0x0 6 18 214 16 "Status: {status}"
 5 Static 0x54000000 0x0 6 36 36 10 "Next run"
 6 ComboBox 0x54230242 0x0 44 36 176 213 ""
 1 Button 0x54030001 0x4 44 54 48 14 "Run"
 2 Button 0x54030000 0x4 172 54 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "*" "" "" ""

if(!ShowDialog(dd &Dialog_MacroEnded &controls 0 128 0 0 &d -1 -1)) ret

ret
 messages
int hDlg(__mustBe0) message(timeoutS) wParam(cbMacros) lParam(__reserved)
___DME& r=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	DT_SetBackgroundColor hDlg 2 0xf0f0f0 iif(r.isError 0x4040ff 0x80E0A0)
	hid- hDlg
	if(r.timeout) SetTimer hDlg 1 1000 0
	
	case WM_TIMER
	sel wParam
		case 1
		r.timeout-1; if(r.timeout=0) clo hDlg; ret
		_s.from("Cancel " r.timeout)
		_s.setwintext(id(2 hDlg))
	
	case WM_SETCURSOR
	if(lParam>>16=WM_LBUTTONDOWN) KillTimer hDlg 1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK ;;Run
	_s.getwintext(id(6 hDlg))
	if(!_s.len) ret
	opt nowarnings 1
	mac _s; err out _error.description; ret
	case IDCANCEL
ret 1
