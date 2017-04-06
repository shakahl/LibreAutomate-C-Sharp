 This macro clicks menu item "Sort by -> Item type" in popup menu of desktop.

rig 200 100 ;;right click desktop
int w1=wait(10 WV "+#32768") ;;wait for popup menu
Acc a=acc("Sort by" "MENUITEM" w1 "" "" 0x1001) ;;get menu item object
a.Mouse
w2=win
outw v1
outw v2
 Acc a=acc("Item type" "MENUITEM" w1 "" "" 0x1001) ;;get menu item object
 a.DoDefaultAction ;;click
 mou ;;return mouse

 To capture popup menu items:
   In 'Find Accessible Object' dialog, right click "Drag" icon, and click "Capture when Shift pressed".
   Invoke the popup menu, move mouse to the item, and press Shift.
