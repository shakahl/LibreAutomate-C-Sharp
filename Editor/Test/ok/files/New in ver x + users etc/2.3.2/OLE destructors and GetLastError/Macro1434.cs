out
 BSTR b="k"
 VARIANT vv="k"
 VARIANT v.vt=VT_VARIANT|VT_BYREF; v.pvarVal=&vv
 VARIANT v.vt=VT_BSTR|VT_BYREF; v.pbstrVal=&b

SetLastError 5
 b="oo"
 out a[1 1]
 a.redim(5)
 a=0
 b=0
 out VariantClear(&v)
 out "%i %i" v.vt v.lVal
Function178
out GetLastError