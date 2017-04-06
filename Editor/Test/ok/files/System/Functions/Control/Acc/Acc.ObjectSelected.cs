function [ARRAY(Acc)&result]

 Gets selected child object(s).
 Stores result into this or other variable.

 result - variable that receives selected objects. Default: this variable (for which is called this function).

 REMARKS
 Use this function to get selected items of various lists.
 If result not used, error if selected are multiple objects.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT
if(&result) result=0
VARIANT v(a.Selection) vv; err end _error
if(_hresult) ret
if(&result)
	sel(v.vt)
		case VT_I4 result.create(1); result[0]=this; result[0].elem=v.lVal
		case VT_DISPATCH result.create(1); result[0].a=v.pdispVal
		case VT_UNKNOWN
		foreach(vv v.punkVal) Result(vv result[])
		if(!result.len) end ERR_FAILED
		case else end ERR_FAILED
else if(v.vt=VT_UNKNOWN) end "more than 1 item is selected. Use array."
else Result(v this)
