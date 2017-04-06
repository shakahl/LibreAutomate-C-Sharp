 /
function hwnd str&s

 Gets text from a scintilla-based control in QM.
 Such controls are: code editors (QM_Code class), output (QM_Output), tips (QM_Tips), qml viewer code view (QM_Code).
 Note that, if there is a link, it will get some garbage text around. In the control it is hidden.

 hwnd - control handle.
 s - receives text.

 EXAMPLE
 int h=id(2201 _hwndqm) ;;qm output
 ;int h=GetQmCodeEditor ;;currently active code editor
 str s
 SciGetText h s
 ShowText "" s


int lens=SendMessage(hwnd SCI.SCI_GETTEXTLENGTH 0 0)
s.fix(SendMessage(hwnd SCI.SCI_GETTEXT lens+1 s.all(lens)))
