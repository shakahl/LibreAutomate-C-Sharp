function x y [flags] [Acc&result] ;;flags: 1 must be child, 2 screen coordinates.

 Gets object from point.
 Stores result into this or other variable.

 x, y - coordinates.
 flags:
   1 - get direct child object of this object (this variable). Error if there is no child there. If this flag not used, gets any object.
   2 - x and y are coordinates in screen. If this flag not used, x and y must be relative to this object (this variable).
 result - Acc variable that receives the object. Default: this variable (for which is called this function).

 REMARKS
 If flags is 2, this variable does not have to be initialized.


if(!a and flags&3!=2) end ERR_INIT
int xx yy cx cy
if(flags&2=0) a.Location(&xx &yy &cx &cy elem); x+xx; y+yy

if(flags&1)
	VARIANT v=a.HitTest(x y)
	if(_hresult) end ERR_FAILED
	Result(v result)
else
	if(&result) result=acc(x y 0); if(!result.a) end ERR_FAILED
	else Acc a1=acc(x y 0); if(a1.a) this=a1; else end ERR_FAILED
err+ end _error
