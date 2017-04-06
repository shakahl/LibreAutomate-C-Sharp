 /
function hWnd [flags] ;;flags: 1 horizontal tb, 2 vertical tb

 Adjusts QM toolbar width and height so that all (and only) buttons would be visible.

 EXAMPLE
 TB_AdjustSize win("TOOLBAR17" "QM_toolbar")


def TB_BUTTONCOUNT 0x00000418
def TB_GETITEMRECT 0x0000041D
def TB_GETBUTTONSIZE 0x0000043A
dll user32 #AdjustWindowRectEx RECT*lpRect dsStyle bMenu dwEsStyle

int htb=id(9999 hWnd)
RECT r; int cx cy i
for i 0 SendMessage(htb TB_BUTTONCOUNT 0 0)
	SendMessage(htb TB_GETITEMRECT i &r)
	if(r.bottom-r.top<15) continue ;;hor sep
	sel flags&3
		case 1
		if(r.right>cx) cx=r.right; else cx+r.right-r.left
		if(cy=0) cy=r.bottom
		case 2
		if(r.bottom>cy) cy=r.bottom; else cy+r.bottom-r.top
		if(cx=0) cx=r.right
		case else
		if(r.right>cx) cx=r.right
		if(r.bottom>cy) cy=r.bottom
if(cx) r.right=cx; r.bottom=cy
else i=SendMessage(htb TB_GETBUTTONSIZE 0 0); r.right=i&0xffff; r.bottom=i>>16
r.left=0; r.top=0
 out cx
AdjustWindowRectEx &r GetWinStyle(hWnd) 0 GetWinStyle(hWnd 1)
siz r.right-r.left r.bottom-r.top hWnd
