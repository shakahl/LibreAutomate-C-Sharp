 /
function` [$s]

 Returns VARIANT as an omitted argument. If s not empty, returns VARIANT(BSTR).

VARIANT v
if(empty(s)) v.vt=VT_ERROR; v.scode=DISP_E_PARAMNOTFOUND
else v=s
ret v
