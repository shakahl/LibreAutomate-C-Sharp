 find Back button
int w=win("- Google Chrome" "Chrome_WidgetWin_1")
if(!w) ret
Acc a.Find(w "TOOLBAR" "Google Chrome Toolbar|main" "" 0x1082)
a.Find(a.a "BUTTONDROPDOWN" "Back" "" 0x1005)
 is it disabled?
if a.State&STATE_SYSTEM_UNAVAILABLE ;;disabled
	OnScreenDisplay "close" 1
else
	OnScreenDisplay "back" 1
