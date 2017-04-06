 \
function hwnd idObject idChild

 edit this list
lpstr mapItemTextToTooltipText=
 one tooltip for one
 two multiline[]''tooltip''
 Cut tooltip for QM menu Edit -> Cut

 ___________________________________


int pid; GetWindowThreadProcessId(hwnd &pid); if(pid!=GetCurrentProcessId) ret ;;only QM process
Acc a.FromEvent(hwnd idObject idChild) ;;menu item accessible object
RECT r; a.Location(r.left r.top r.right r.bottom); r.right+r.left; r.bottom+r.top ;;rectangle in screen

str s=a.Name
if !s.len ;;QM menus are owner-drawn
	RECT u=r; MapWindowPoints 0 hwnd +&u 2
	WindowText wt.Init(hwnd u); wt.Capture; if(wt.n<1) ret
	s=wt.a[0].txt
s.gett(s 0 "[9]") ;;remove hotkey
 out s

err+ ret

IStringMap m._create; m.AddList(mapItemTextToTooltipText)
if(!m.Get2(s s)) ret
s.escape(0)

 out s
OnScreenDisplay s 0 r.right r.top "" 10 1 4|16 "qm_menu_item" 0xE0FFFF
 ShowTooltip s 5 r.right r.top 800 2 ;;shows multiple, not good
