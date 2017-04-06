function VARIANT&v

 Initializes this variable from a variable of type VARIANT.
 Supported VARIANT types: VT_I8 (DateTime or FILETIME), VT_DATE (DATE), VT_BSTR (string containing date time).
 Also can be VT_EMPTY, or VT_I4 (int) with value 0. Then makes this variable empty.
 Error if v is not of one of these types.


opt noerrorshere 1

if(empty(v)) t=0; ret
sel v.vt
	case VT_I8 t=v.llVal
	case VT_DATE FromDATE(v.date)
	case VT_BSTR FromStr(v.bstrVal)
	case VT_I4 if(!v.lVal) t=0; else end ERR_BADARG
	case else end ERR_BADARG
