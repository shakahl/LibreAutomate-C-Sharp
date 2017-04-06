 /
function# wt [hwnd]

 Waits while window is busy and not responding.
 Returns 1 if suscessful, 0 on timeout.

 wt - max wait time in milliseconds.
 hwnd - window handle. If 0 - active window.


int r i t0 t1 t2 nidle

if(!hwnd) hwnd=win
for i 0 1000000000
	t1=GetTickCount(); if(!i) t0=t1;
	SendMessageTimeout(hwnd, 0, 0, 0, 0, 200, &r); ;;use small time to be able to smoothly end thread
	t2=GetTickCount();
	if(t2-t1<100) nidle+1; if(nidle>=2) break;
	else
		nidle=0;
		if(t2-t0>=wt) ret
ret 1
