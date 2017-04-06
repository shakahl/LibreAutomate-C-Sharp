function [button] [offsetx] [offsety] [noscroll] ;;button: 0 move, 1 left, 2 right, 3 middle, 4 left double.

 Moves mouse to the element and optionally clicks.

 button - mouse button. See above. If 0, does not click.
 offsetx, offsety - offset from top-left corner of the element. Default: somewhere near top-left.
 noscroll - if 0 or omitted, scrolls the element into view if need. Else does not scroll.


if(!el) end ERR_INIT
opt noerrorshere 1

int x y cx cy w
 g1
Location(x y cx cy)

if(getopt(nargs)>1) x+offsetx; y+offsety
else
	if(cx>60) cx=60
	if(cy>60) cy=60
	x+cx/2; y+cy/2

if(!noscroll)
	MSHTML.IHTMLDocument2 doc=el.document
	RECT r; sub_Htm.GetDocXY doc 0 r
	if(!PtInRect(&r x y)) noscroll=1; el.scrollIntoView; goto g1

if(button)
	w=Hwnd
	act GetAncestor(w 2); err
	DpiScreenToClient(w +&x)

spe -1
opt slowmouse -1
sel button
	case 0 mou x y
	case 1 lef x y w 1
	case 2 rig x y w 1
	case 3 mid x y w 1
	case 4 dou x y w 1
