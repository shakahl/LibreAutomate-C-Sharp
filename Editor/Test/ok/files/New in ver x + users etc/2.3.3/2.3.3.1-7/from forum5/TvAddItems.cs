 /
function htv

SetWinStyle htv TVS_CHECKBOXES 1
SendMessage htv TVM_DELETEITEM 0 0 

int i

Acc a=acc("" "LISTITEM" win("Yahoo! Messenger" "YahooBuddyMain") "SysListView32" "" 0x1)
if(a.a)
	i=a.a.ChildCount
	for a.elem 1 i
		_s=a.Name
		TvAdd htv 0 _s

err+
	foreach _s "one[]two[]three"
		TvAdd htv 0 _s
