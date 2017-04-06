
 Releases DC and clears this variable.
 Calls ReleaseDC.
 Called implicitly by destructor.


if dc
	if(!ReleaseDC(WindowFromDC(dc) dc)) end "invalid DC handle" 8|2
	dc=0
