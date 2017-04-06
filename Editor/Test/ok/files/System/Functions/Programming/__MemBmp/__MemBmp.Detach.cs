function#

 Returns handle of bitmap of this variable and clears this variable (deletes DC).


if(!bm) ret
SelectObject(dc oldbm)
DeleteDC(dc); dc=0
_i=bm; bm=0
ret _i
