
 Clears this variable (deletes DC and bitmap). Optional.


if(dc)
	DeleteObject(SelectObject(dc oldbm)); bm=0
	DeleteDC(dc); dc=0
