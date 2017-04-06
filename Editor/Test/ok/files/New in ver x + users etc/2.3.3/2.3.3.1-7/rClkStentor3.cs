 /
function $keycodes

 Right clicks, waits for popup menu, and presses keys.
 Error if fails.

 EXAMPLE
 mou 200 100
 rClkStentor "DDRDY"

spe -1
POINT p; xm(p)
 int m=getmonitor(p);
 if (m=3) or (m=4)
	rig p.x p.y
	int w1=wait(10 WV "+#32768") ;;wait for popup menu
	
	 press keys
	_s=
	F
	 function _
	 spe {getopt(speed)}
	 key {keycodes}
	 err+ end _error
	RunTextAsFunction2 _s

err end _error
