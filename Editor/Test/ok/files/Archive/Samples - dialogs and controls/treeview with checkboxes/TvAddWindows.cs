 /
function htv

SetWinStyle htv TVS_CHECKBOXES 1
SendMessage htv TVM_DELETEITEM 0 0 ;;delete all

ARRAY(int) a; int i
win "" "" "" 0 0 0 a
for i 0 a.len
	int h=a[i]
	if(GetWinStyle(h 1)&WS_EX_TOOLWINDOW) continue
	str s.getwintext(h); if(!s.len) continue
	 out s
	TvAdd htv 0 s h
	