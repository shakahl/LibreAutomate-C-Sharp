function# VARIANT&v Acc&result

if(!&result) &result=&this
sel(v.vt)
	case VT_I4
	IDispatch b; if(v.lVal) b=a.Child(v.lVal); err
	if(b) result.a=b; result.elem=0; else result.a=a; result.elem=v.lVal
	case VT_DISPATCH result.a=v.pdispVal; result.elem=0
	case else end ERR_FAILED 2
ret 1
