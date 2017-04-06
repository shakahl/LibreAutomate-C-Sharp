 \
function# iid FILTER&f

type MOUSETB iid FILTER'f RECT'rh
MOUSETB+ __mtb
 ________________________

 this code runs as FF, on button down

if(iid>0)
	if(getopt(nthreads)) ret ;;probably one of two: 1. OnScreenDisplay (5 s). 2. Autorepeated key while waiting for key up.
	 _s.getmacro(iid 1); if(wintest(f.hwnd _s.ucase "QM_toolbar")) ret ;;on toolbar itself
	_s.getmacro(iid 1); if(win(_s.ucase "QM_toolbar")) ret ;;toolbar already running
	__mtb.iid=iid; __mtb.f=f ;;save data
	if(f.ttype!=2) __mtb.f.x=xm; __mtb.f.y=ym ;;if not mouse trigger, get mouse pos
	iid=getopt(itemid)
	 out "1 %I64i" perf
	mac iid "" -1 ;;run this function now
	 out "2 %I64i" perf
	 ret iid
	ret -1
 ________________________

 this code runs not as FF

int h
if(iid=-1) ;;on button down
	 out "3 %I64i" perf
	 run toolbar
	0.1
	h=mac(__mtb.iid); err ret
	int W H; GetWinXY h 0 0 W H
	mov __mtb.f.x-(W/2) __mtb.f.y-(H/2) h
	 create hole
	int rw=CreateRectRgn(0 0 W H)
	W/3; H/3
	__mtb.rh.left=W; __mtb.rh.top=H; __mtb.rh.right=W*2; __mtb.rh.bottom=H*2; MapWindowPoints h 0 +&__mtb.rh 2
	
	 zw win(mouse)
	 mouse_event MOUSEEVENTF_RIGHTUP 0 0 0 0
	  0.02
	 mouse_event MOUSEEVENTF_RIGHTDOWN 0 0 0 0
	
	int r2=CreateRectRgn(W H W*2 H*2)
	CombineRgn(rw rw r2 RGN_DIFF)
	SetWindowRgn h rw 1
	DeleteObject r2
	 if toolbar is transparent, run thread that makes it opaque
	 mac getopt(itemid) h -2
	 wait for button up
	0.001 ;;solves this (in most cases): In QM, if using LL mouse hook, if click time is short, often waits until timeout and fails
	RECT r
	rep
		int e=0
		 out 1
		if(__mtb.f.ttype=2) wait(5 M); err e=1 ;;mouse trigger
		else wait(5 K); err e=1 ;;key trigger
		 out 2
		if(!IsWindow(h)) ret
		if(!e) break
		GetWindowRect h &r
		if(!PtInRect(&r xm ym)) clo h; ret
	 mouse on tb?
	int tb=id(9999 h)
	if(tb=child(mouse)) ;;yes. Click tb and close
		lef
		clo h
	else ;;mouse not on tb
		 mouse in hole?
		GetWindowRect tb &r
		POINT p pp; xm p; pp=p
		if(PtInRect(&r p.x p.y)) ;;yes. Close tb and resend the button
			clo h
			 if mouse trigger, resend trigger button
			if(__mtb.f.ttype!=2) ret
			spe 1
			sel __mtb.f.tkey
				case 6 lef
				case 7 rig
				case 8 mid
				case 4 MouseXButton 1
				case 5 MouseXButton 2
		else ;;not in hole (outside tb). Move mouse into tb, click and close
			r.right-5; r.bottom-5
			if(p.x<r.left) p.x=r.left; else if(p.x>r.right) p.x=r.right
			if(p.y<r.top) p.y=r.top; else if(p.y>r.bottom) p.y=r.bottom
			lef p.x p.y
			clo h
			mou pp.x pp.y
else if(iid=-2) ;;on button down, thread 2
	h=val(_command)
	if(!IsWindow(h)) ret
	int i transp
	if(GetLayeredWindowAttributes(h 0 &transp &i) and i&LWA_ALPHA and i<255)
		 increase opacity
		for i 0 13
			if(!IsWindow(h)) ret
			Transparent h transp
			if(i>3 or !PtInRect(&__mtb.rh xm ym)) transp+32
			0.03
	else wait 0.4 -WC h; err
	 if mouse not moved for 1 s, disable toolbar for 5 s (eg to enable drag and drop)
	if(__mtb.f.ttype!=2) ret ;;don't disable if key trigger
	wait 1 -WC h; err
	if(!IsWindow(h)) ret
	if(xm=__mtb.f.x and ym=__mtb.f.y)
		clo h
		OnScreenDisplay "The toolbar now is temporarily inactive (5 s)" 5 0 0 "" 0 0 2

 err+

 TODO:
 test with mm triggers etc
