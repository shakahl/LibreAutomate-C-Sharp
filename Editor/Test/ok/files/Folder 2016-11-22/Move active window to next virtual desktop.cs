 support multiple monitors, part 1
int wa=win; if(!wa) ret
int hmon=MonitorFromWindow(wa 0); if(!hmon) ret
int nTab=(MonitorIndex(hmon)-1)*2
 open desktop switcher window
key WT
int wtv=wait(5 WV win("Task View" "MultitaskingViewFrame"))
0.1
 support multiple monitors, part 2 (tested only with 2 monitors)
if(nTab) key T(#nTab)
 open context menu, and its submenu "Move to"
key Mm
 select first item and close the desktop switcher window
key Y Z

 tested: no accessible objects in the switcher window
