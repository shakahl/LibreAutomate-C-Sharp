 /
function# POINT&p BSTR&a BSTR&b

 ret VarBstrCmp(a b 0x409 fl)-1
if(p.y&2) ret _wcsicmp(a b)
ret wcscmp(a b)
