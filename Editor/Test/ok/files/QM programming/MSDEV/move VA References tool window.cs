 int w=win("VA References" "VBFloatingPalette")
 act w

int w=TriggerWindow
0.2

MoveWindowToMonitor w 2 0 4 4
siz 1100 740 w
Acc a.Find(w "OUTLINE" "" "class=SysTreeView32" 0x1004)
a.elem=1
a.Select(3)

 make floating. Must be after, because creates new window.
PostMessage w WM_NCRBUTTONDOWN HTCAPTION 0
PostMessage w WM_NCRBUTTONUP HTCAPTION 0
key fZ
1 ;;avoid second trigger
