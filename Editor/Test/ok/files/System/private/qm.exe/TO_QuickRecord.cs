function recFlags ;;recFlags currently not used

int i hwnd hc
POINT p; RECT r
str s sv sw sCom sCom2 svCol
__strt vd sc vdCol

 get window and control
xm p
hc=child(p.x p.y 0 1)
RecGetWindowName(hc &sc 0 &sw &sCom &sCom2)
if(sw.len) hwnd=GetAncestor(hc 2) ;;child
else hwnd=hc; hc=0; sc.swap(sw) ;;top-level
int color=pixel(p.x p.y)

 menu
_s=
 1000QM - insert statement
 -
 1Get window handle
 2Get control handle
 5Wait for window, get handle
 3Activate window, get handle
 -
 >Click
 	11Window
 	12Control
 	10Screen
 	-
 	13Mouse dialog...
 <
 >Get color
 	31Window
 	32Control
 	30Screen
 	-
 	33Dialog 'Wait for color'...
 <
 -
 22Find window or control...
 20Find accessible object...
 21Find html element...
 23Find image or color...
 24Run program...
 -
 Cancel

MenuPopup pm.AddItems(_s)
pm.SetBold(1000); pm.DisableItems("1000")
if(!hc) pm.DisableItems("2 4 12 32")
i=pm.Show()

sel(i) case [1,2,3,5,11,12,31,32] vd.VD("int w" sv)
sel(i) case [2,12,32] hwnd=hc; sc.WinReplace(sv) ;;control; replace {window} with window handle
sel(i) case [2,11,12,31,32] s=F"{vd}={sw}[]"
sel(i) case [30,31,32] vdCol.VD("int color" svCol)

sel i
	case 1 s=F"{vd}={sw}"
	case 2 s+F"{vd.VD(`int c`)}={sc}"
	case 3 s=F"{vd}=act({sw})"
	case 5 s=F"{vd}=wait(30 WA {sw})"
	case 10 s=F"lef {p.x} {p.y}"
	case 30 s=F"{vdCol}=pixel({p.x} {p.y})"
	case [11,12,31,32]
		int nc=__RecIsPointInNonclient(hwnd p.x p.y)
		if(nc) DpiGetWindowRect hwnd &r; p.x-r.left; p.y=p.y-r.top; else DpiScreenToClient hwnd &p
		sel i
			case [11,12] s+F"lef {p.x} {p.y} {iif(i=11 sv sc)} {!nc}"
			case [31,32] s+F"{vdCol}=pixel({p.x} {p.y} {iif(i=31 sv sc)} {!nc})"
		sub_to.Trim s " 0"
	case else
	sel i
		case 20 mac "EA_Main"
		case 21 mac "EH_Main"
		case 22 mac "TO_FindWindow" "" 3
		case 23 mac "TO_Scan" "" 0 1
		case 13 mac "TO_Mouse"
		case 33 mac "TO_Wait" "" 0 0 12
		case 24 TO_Fav "TO_FileRun" 0 iif(hc hc hwnd)
	ret

sel i
	case [2,11,12,31,32,10,30]
	if(i=2) sCom2.all
	if(sCom.len) s+F" ;;{sCom}"; if(sCom2.len) s+F", {sCom2}"
	else if(sCom2.len) s+F" ;;{sCom2}"

sel(i) case [30,31,32] s.formata("[]if(%s=0x%X)[][9]out F''0x{%s}''" svCol color svCol)

 out s
act _hwndqm; err
InsertStatement s
