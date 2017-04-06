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
	wait 0 H RunTextAsFunction(F"spe {getopt(speed)}[]key {keycodes}") ;;press keys

err end _error
