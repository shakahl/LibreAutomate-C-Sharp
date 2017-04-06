 /
function#

 Gets user input idle time, in seconds.
 It is time since the last mouse or keyboard event.

 Added in: QM 2.3.2.


LASTINPUTINFO in.cbSize=sizeof(in)
if(GetLastInputInfo(&in)) ret GetTickCount-in.dwTime/1000
