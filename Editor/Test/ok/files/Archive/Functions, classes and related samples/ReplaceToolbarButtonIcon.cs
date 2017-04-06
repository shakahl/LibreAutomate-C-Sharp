 /
function# lineindex $icon [$toolbarname]

 Replaces toolbar button text.
 The replacement is permanent, ie it changes toolbar text.
 Can be called while the toolbar is running or not.
 Returns 1 if successful, 0 if not.

 lineindex - 0-based line index in toolbar text.
 icon - new icon of the button.
 toolbarname - toolbar name. Can be omitted or "" if called from the toolbar itself.

 REMARKS
 The toolbar line must not have " *" before the " *" that is used for icon.


int iid
if(empty(toolbarname)) iid=getopt(itemid 3)
else iid=qmitem(toolbarname)
if(!iid) ret

str s.getmacro(iid)

int i=findl(s lineindex); if(i<0) ret
REPLACERX r.ifrom=i; r.repl=F"$1{icon}"
if(s.replacerx("(.+? \* *)[^[]]*" r 4)<0)
	r.repl=F" * {icon}[]"
	s.replacerx("([]|\z)" r 4)

s.setmacro(iid); err ret
ret 1