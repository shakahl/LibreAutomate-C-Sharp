function [flags] [Acc&result] ;;flags: 1 must be child

 Gets focused object.
 It is object that has keyboard input focus.
 Stores result into this or other variable.

 flags:
   0 - get currently focused object, regardless of where it is. This variable does not have to be initialized.
   1 - get focused direct child object of this object (of this variable). For example, if this object is LIST, gets focused LISTITEM.
 result - Acc variable that receives the object. Default: this variable (for which is called this function).

 Added in: QM 2.3.3.


if(flags&1)
	if(!a) end ERR_INIT
	VARIANT v=a.Focus; err end _error
	if(_hresult) end ERR_FAILED
	Result(v result)
else
	if(&result) result=acc; if(!result.a) end ERR_FAILED
	else Acc a1=acc; if(a1.a) this=a1; else end ERR_FAILED
