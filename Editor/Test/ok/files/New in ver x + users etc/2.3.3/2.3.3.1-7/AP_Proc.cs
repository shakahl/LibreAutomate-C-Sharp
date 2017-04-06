 /Macro1477
function Acc&a level AP_PROC&d

int ip=d.ip;; out "%i %s" ip d.ap[ip]

if ip=d.ap.len-1
	d.aret=acc(d.name d.ap[ip] a "" "" d.flags&(0x4ff)|64)
else
	d.ip+1
	acc("" d.ap[ip] a "" "" d.flags&(16|32|128)|64|0x8000 &AP_Proc &d)
	d.ip=ip

ret !d.aret.a
