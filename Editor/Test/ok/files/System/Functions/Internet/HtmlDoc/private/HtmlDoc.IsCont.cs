function# $*t

if(!empty(*t)) ret 1
VARIANT* ni=+(t+4)
if(ni.vt and ni.lVal)
	if(ni.vt=VT_BSTR and !ni.bstrVal.len) ret
	ret 1
