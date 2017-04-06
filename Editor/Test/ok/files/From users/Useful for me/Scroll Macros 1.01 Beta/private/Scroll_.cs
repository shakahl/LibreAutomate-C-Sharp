 /
function up [complexForced] [simpleForced]

 Scrolling function for the set of Scroll Macros
 Options:
 simpleForced -  1 - force use of WM_MOUSEWHEEL, single scroll (no factor)
                 2 - force use of WM_VSCROLL, default scroll factor
              - overrides complexForced setting
 complexForced - overrides value of _m.forceScroll, behaves the same way:
                 0 - selectively use WM_MOUSEWHEEL or WM_VSCROLL
                 1 - force use of WM_MOUSEWHEEL, with factors
                 2 - force use of WM_VSCROLL, with factors


#compile Scroll_Def

int cw sf df mWheel scroll line page forcedId ld

if(!_m.lineDefault) Scroll_Init

 cw=child(xm ym 0)
 if(!cw) cw=child(xm ym+15); if(!cw) cw=child(xm ym-15)	;; special case - scrolling stops in certain cases of text highlighting - this should fix it
 if(!cw) end

POINT p; GetCursorPos(&p)
cw=RealChild(p.x p.y); if(!cw) cw=win(mouse)

sel simpleForced
	case 1:
	scroll=WHEEL_DELTA*(iif(!up -1 1)); SendNotifyMessage(cw, WM_MOUSEWHEEL, (scroll<<16), (p.y<<16)|p.x); ret
	case 2:
	line=iif(up=0 SB_LINEDOWN SB_LINEUP); rep _m.lineDefault; SendNotifyMessage(cw, WM_VSCROLL, line, 0L)
	ret
	
sf=_m.scrollFactor; df=_m.deltaFactor
int cf
if(getopt(nargs)=2)	forcedId=complexForced; else forcedId=_m.forceScroll
 if(forcedId<>2) mWheel=Needs_WM_MOUSEWHEEL(win(p.x p.y))

if(mWheel || forcedId=1)
	if(_m.setDeltaFactor) df=Scroll_Input("deltaFactor")
	if(df>1) scroll=WHEEL_DELTA*df
	else scroll=WHEEL_DELTA
	if(!up) scroll*-1
	SendNotifyMessage(cw, WM_MOUSEWHEEL, (scroll<<16), (p.y<<16)|p.x)
else
	if(_m.setScrollFactor) sf=Scroll_Input("scrollFactor")
	if(up) line=SB_LINEUP; page=SB_PAGEUP
	else line=SB_LINEDOWN; page=SB_PAGEDOWN
	if(!sf) rep _m.lineDefault; SendNotifyMessage(cw, WM_VSCROLL, line, 0L)
	else if(sf<0)
		sf=sf*-1
		rep sf;	 SendNotifyMessage(cw WM_VSCROLL, page, 0L)
	else rep sf; SendNotifyMessage(cw, WM_VSCROLL, line, 0L)


 
 HISTORY - SCROLL MACROS
 ========================
 
 DATE         AUTHOR, COMMENTS & ADDED MACROS

 2004-12-08   By Ravinder Singh. callrs@yahoo.com
              Version 1: Basic functionality
                 Scroll_Up,                 Scroll_Up_MW
                 Scroll_Down,               Scroll_Down_MW
                 Needs_WM_MOUSEWHEEL,       Scroll_Init

 2004-12-10   R. Singh.
              Version 1.01 Beta: Re-ordered. Added scroll-by-multiple lines/pages
                 Tilda_,                    Scroll_ 
                 Scroll_Input,              Scroll_Up_VS
                 Scroll_Down_VS,            Scroll_Dialog


 IDEAS FOR THE FUTURE
 =====================
    - Auto scroll while a mouse button is held down
    - Allow user to configure scroll settings (# of lines, or page scroll)
      for any chosen application windows


