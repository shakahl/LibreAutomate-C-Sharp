 Changes scrolling speed for the Scroll Macros (See Scroll_ macro for documentation)
 Note: hittin ESC in any dialog will reset its variable to the default

function str'action [factor]
 action - one of "deltaFactor", "scrollFactor", or "forceScroll"
 factor - new value for variable. if not supplied, then user is prompted for it

#compile Scroll_Def

if(action="deltaFactor") goto deltaFactor
if(action="scrollFactor") goto scrollFactor
if(action="forceScroll") goto forceScroll
bee; out "Invalid argument in function ScrollInput: %s" action; end

str tmp
int n of
 scrollFactor
of=_m.scrollFactor
if(getopt(nargs)=1)
	str controls = "40 41"
	str a b
	if(of=0) of=_m.lineDefault
	if(of<0) b=(of*-1); else a=of
	str oa(a) ob(b)
	ShowDialog("Scroll_Dialog" &Scroll_Dialog &controls)
	if(a.len && a<>oa) _m.scrollFactor=val(a)
	else if(b.len && b<>ob) _m.scrollFactor=val(b)*-1
else _m.scrollFactor=factor
_m.setScrollFactor=0

 Since user is changing WM_VSCROLL setting, turn off forced WM_MOUSESCROLL to allow WM_SCROLL

sel _m.forceScroll
	case 0: tmp=""
	case 1:  _m.forceScroll=0; tmp="(WM_MOUSEWHEEL was on. Now turned Off)"
	case 2: tmp="(WM_VSCROLL is on.)"
out "WM_VSCROLL Scroll factor: %i --> %i  Force scroll type: %i %s" of _m.scrollFactor _m.forceScroll tmp

ret _m.scrollFactor


 deltaFactor
int olddf=_m.deltaFactor
if(getopt(nargs)=1)
	int df=PopupMenu("Delta Factor[]-[]&1[]&2[]&3[]&4[]&5[]&6[]&7[]&8[]&9[]-[]Custom &Z[]" 0 0 0 1)
	if(df<13) df-2; if(df<0) df=0
	else inp- df "New delta factor?" "Scroll Macros" olddf
else df=factor; if(df<0) df=0
_m.setDeltaFactor=0
_m.deltaFactor=df

 Since user is changing WM_MOUSESCROLL setting, turn off forced WM_VSCROLL
sel _m.forceScroll
	case 0: tmp=""
	case 1: tmp="(WM_MOUSEWHEEL is on)"
	case 2: _m.forceScroll=0; tmp="(WM_VSCROLL was on, Now turned off.)"
out "WM_MOUSESCROLL Delta factor: %i --> %i  Force scroll type: %i %s" olddf df _m.forceScroll tmp

ret df


 forceScroll
if(getopt(nargs)=1)	_m.forceScroll=PopupMenu("&0 Default[]&1 Force WM_MOUSEWHEEL][]&2 Force WM_VSCROLL" 0 0 0 1)-1
else if(factor<0 || factor>2) factor=0; _m.forceScroll=factor
sel _m.forceScroll
	case 0: tmp="(Default)"
	case 1: tmp="(WM_MOUSEWHEEL is on)"
	case 2: tmp="(WM_VSCROLL is on)"
out "_m.forceScroll = %i  %s" _m.forceScroll tmp
ret _m.forceScroll
