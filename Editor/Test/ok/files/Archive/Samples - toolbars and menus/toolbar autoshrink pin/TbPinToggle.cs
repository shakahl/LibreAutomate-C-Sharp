 /
function [$tbname]

 Clicks 'auto shrink' in the right-click menu of a QM toolbar.

 tbname - toolbar name. If omitted or "", the mouse pointer must be on the client area of a toolbar.


 right click
if(len(tbname))
	_s=tbname; _s.ucase
	rig 0 0 win(_s "QM_toolbar") 1; err ret
	mou
else rig
 wait for menu
int hwndmenu=wait(10 WV win("" "#32768" "qm")); err ret
 click 'auto shrink' item (key s does not work because keyboard input goes to currently active window).
Acc a=acc("Shrink" "MENUITEM" hwndmenu)
a.Mouse(1)
mou
