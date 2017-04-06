 int hlv=child(1 "" "SysListView32" win("" "ExploreWClass")) ;;Windows Explorer, details view. Does not work on Windows 7.
 Acc a=acc("qm.exe" "LISTITEM" hlv "" "" 0x1001)

int hlv=id(2 win("Registry Editor"))
Acc a=acc("file" "LISTITEM" hlv "SysListView32" "" 0x1001)

int isubitem=2 ;;for example

RECT r.top=isubitem
__ProcessMemory m.Alloc(hlv 100)
m.Write(&r sizeof(r))
if(!SendMessage(hlv LVM_GETSUBITEMRECT a.elem-1 m.address)) ret
m.Read(&r sizeof(r))

 coordinates are in hlv client
mou r.left+5 r.top+5 hlv 1
