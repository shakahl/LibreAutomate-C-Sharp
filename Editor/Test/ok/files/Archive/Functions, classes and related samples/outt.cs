 /
function $s [caller]

 Shows text in QM output, like out, but with prefix "<time> : <link to outt call place> - ".

 s - text. Can contain <help #IDP_F1>tags</help>.
 caller - if 1, adds link to caller's caller, not to the direct caller. It is useful if the direct caller is another 'outX' function. Can be > 1 to use another caller in the call stack.

 EXAMPLE
 int i=17
 outt F"i is {i}"


DateTime t.FromComputerTime
str st=t.ToStr(2|4)

#if EXE&&(QMVER<=0x2040303)

GetCallStack _s 1
_s.getl(_s 2+caller) ;;get caller ("<>[]this[]caller[]...")
out "<>%s : %s - %s" st _s s

#else

lpstr si=getopt(itemname -(1+caller))
if(!si) ret
if si[0]='<' ;;sub-function
	int iid=getopt(itemid -(1+caller)) ;;gets sub-function id
	_s.getmacro(iid 1) ;;gets sub-function parent name
	si=_s.from(_s ":" si+findc(si '>')+1)

int pos=Statement(1+caller)

out "<>%s : <open ''%s /%i''>%s</open> - %s" st si pos si s

#endif
