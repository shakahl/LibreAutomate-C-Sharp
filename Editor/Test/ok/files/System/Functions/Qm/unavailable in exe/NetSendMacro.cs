 /
function# $computer $password $macro [$folder] [$trigger]

 Sends QM item (macro, function, etc) to other computer on local network or Internet.
 Returns 0 on success, or error code, same as <help>net</help>.

 computer, password - same as with <help>net</help>.
 macro - macro name or +id.
 folder, trigger - same as with <help>newitem</help>.

 REMARKS
 QM on the remote computer must be running and allow to run macros from other computers.
 Supports encrypted password. In Options -> Security, use "net" as function name.


QMITEM q
int iid
if(macro&0xffff0000) iid=qmitem(macro 1 q 8)
else iid=macro; qmitem(iid 1 q 9); macro=q.name
str sr s.getmacro(iid) st

sel q.itype
	case 1 st="Function"
	case 2 st="Menu"
	case 3 st="Toolbar"
	case 4 st="T.S. Menu"
	case 6 st="Member"

ret net(computer password "NewItem" sr macro s st trigger folder 17)

err+ err end _error
