int hwnd=TriggerWindow
if(hwnd) clo hwnd

int w=win("Dependency Walker" "Afx:*" "" 0x4)
max w
int c=child("" "SysTreeView32" w 0x0 "id=59648") ;;outline
TreeViewCollapseAll c 1

spe 10
lef+ 10 1.007 c
lef- 10 0.95 w
mou
