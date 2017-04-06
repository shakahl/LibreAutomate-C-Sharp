function# `x `y ;;Returns: 0 int int, 1 double int, 2 int double, 3 double double

 Gets values of x and y and stores in this members, depending on whether x and/or y are double.


memset &this 0 sizeof(__POINTID)
int r
if(x.vt=VT_R8) r|1; dx=x.dblVal; else ix=x.lVal
if(y.vt=VT_R8) r|2; dy=y.dblVal; else iy=y.lVal
ret r

 definition of this class:
 type __POINTID ix iy ^dx ^dy
