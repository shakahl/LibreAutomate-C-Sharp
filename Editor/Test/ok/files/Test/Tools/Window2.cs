act "Calculator"
act
act id(15 "Untitled - Notepad")
clo
clo+
max
min
res
hid
hid-
ont
mov ??? ???
siz ??? ???
mov 0 (l l) "" 4
MoveWindow win() x y cx cy 1
MoveWindow win() 5 5 5 cy 1
GetWinXY win() x y cx cy
Zorder(win())
Zorder(win() HWND_BOTTOM)
ifa("???")
ifi-(win("TO_FAVORITES" "QM_toolbar"))
if(IsIconic(win()))
if(!IsZoomed(win()))
if(!IsWindowVisible(win()))
if(!IsWindowEnabled(id(15 "Untitled - Notepad")))
_s="kkkkkjljk"; _s.setwintext(win())
str v0.getwintext(win("Toolbars"))
str a.getwinclass(win())
str a.getwinexe(win() 1)
int a=GetWinId(k)
int a=GetWinStyle(win())
int a=GetWinStyle(win("Untitled - Notepad") 1)
int a=GetParent(id(15 "Untitled - Notepad"))
int a=GetAncestor(win() 2)
int a=GetWindow(win() GW_CHILD)
int a=GetWindow(win() GW_HWNDFIRST)
int a=GetWindow(win() GW_HWNDNEXT)
int a=GetWindow(win() GW_OWNER)
int a=TriggerWindow
ArrangeWindows(0)
EnableWindow win() 1
EnableWindow win() 0
