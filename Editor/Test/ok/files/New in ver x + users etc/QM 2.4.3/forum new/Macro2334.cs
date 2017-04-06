POINT p pp
rep
	0.1 ;;calling GetCursorPos and memcmp every 100 ms will not consume too much CPU
	GetCursorPos &p
	if(!memcmp(&p &pp sizeof(p))) continue ;;if mouse not moved
	pp=p
	
	 out "%i %i" p.x p.y
	
	 Now get object from mouse and display tooltip if need.
	 To optimize, use something like above, eg compare current window/object with previous and continue if it is the same.
	
	 example without optimizations
	Acc a.FromMouse; err continue ;;slow! Use PerfFirs/PerfNext/PerfOut to measure speed. To optimize, before compare window/control from mouse (it is faster than Acc functions), unless you need all objects in all windows.
	str s=a.Name; err continue ;;quite slow too. For some objects may need a.Value instead.
	str ps; if(s=ps) continue
	ps=s
	s.trim
	if(!s.len or s="my-tooltip") OsdHide "my-tooltip"; continue
	 out s
	OnScreenDisplay s 0 p.x p.y+20 "" 12 0xff0000 1|4|16 "my-tooltip" 0xc0ffff
	