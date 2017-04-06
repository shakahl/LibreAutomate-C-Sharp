 /
function hwndSci str&s

 Gets text of a Scintilla control or a Scintilla-based control.
 Supports controls in other processes too.
 QM uses Scintilla for its code editor, output, statusbar and some other controls.


opt noerrorshere 1

int pid
if(!GetWindowThreadProcessId(hwndSci &pid)) end ERR_HWND

int n=SendMessage(hwndSci SCI.SCI_GETTEXTLENGTH 0 0)
if(!n) s=""; ret
n+1

if pid=GetCurrentProcessId
	s.fix(SendMessage(hwndSci SCI.SCI_GETTEXT n s.all(n)))
else
	__ProcessMemory m
	n=SendMessage(hwndSci SCI.SCI_GETTEXT n m.Alloc(hwndSci n))
	m.ReadStr(s n)
