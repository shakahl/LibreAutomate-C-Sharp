str s
sub.SciGetText(id(2201 _hwndqm) s)
if(!s.len) ret

int iid=qmitem("QM output text")
if(iid) s.setmacro(iid); else iid=newitem("QM output text" s "" "" "" 128)
mac+ iid
men 33546 id(2213 _hwndqm) ;;Right Editor
key Cf Ca


#sub SciGetText
function hwnd str&s
int lens=SendMessage(hwnd SCI.SCI_GETTEXTLENGTH 0 0)
s.fix(SendMessage(hwnd SCI.SCI_GETTEXT lens+1 s.all(lens)))
