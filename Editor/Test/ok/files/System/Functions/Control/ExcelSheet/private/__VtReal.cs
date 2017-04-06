 /
function# `&v

 Returns VARIANT type. If empty BSTR, returns 0.

if(v.vt=VT_BSTR and !v.bstrVal.len) ret
ret v.vt
