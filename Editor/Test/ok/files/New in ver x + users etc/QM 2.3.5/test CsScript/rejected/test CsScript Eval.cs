/exe
 out

str code.getmacro("cs1")

PF
CsScript x.Init
PN
rep 1
	_s=x.Eval("Mark" "func(string user) { return ''Hello '' + user; }") ;;compiles every time, ie speed is ~0.5 s
	PN
PO
out _s
