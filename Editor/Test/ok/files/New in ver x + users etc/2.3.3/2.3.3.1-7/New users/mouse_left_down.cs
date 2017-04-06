if(getopt(nthreads)>1) shutdown -6 0 "mouse_left_down"; ret

POINT p0 p
xm p0
rep 20
	0.1
	ifk- (1)
		out "up" ;;debug
		ret
	xm p
	if(_hypot(p.x-p0.x p.y-p0.y)>2)
		out "drag" ;;debug
		ret

out "start something"
