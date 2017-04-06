 /
function# x y

 Returns child window from point.
 If there is tooltip, returns child window behind.

int h=child(x y 0)
if(!h)
	h=win(x y)
	if(SendMessage(h WM_NCHITTEST 0 x|(y<<16)) == -1) ;;transparent to mouse messages (eg tooltip)
		rep
			h=GetWindow(h GW_HWNDNEXT)
			if(IsWindowVisible(h)) break
		h=child(x y h 32)
	else ret
ret h
