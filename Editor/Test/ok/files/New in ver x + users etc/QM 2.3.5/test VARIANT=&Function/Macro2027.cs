out
int fptr=&CsScriptCallback; out fptr
out &CsScriptCallback
VARIANT v=&CsScriptCallback
outx v.vt
out v.lVal
out v

 FLOAT f=fptr; out f
 FLOAT ff=&CsScriptCallback; out ff ;;error
 CURRENCY f=fptr; out f
 CURRENCY ff=&CsScriptCallback; out ff ;;error
 DECIMAL f=fptr; out f
 DECIMAL ff=&CsScriptCallback; out ff ;;error
 BSTR f=fptr; out f
 BSTR ff=&CsScriptCallback; out ff ;;error

 byte* p
 Function238 p
 Function238 &Function238
