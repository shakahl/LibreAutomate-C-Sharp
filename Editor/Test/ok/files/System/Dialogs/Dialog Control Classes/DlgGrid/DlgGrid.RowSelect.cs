function row [flags] ;;flags: 1 add to selection, 2 ensure visible, 4 set cut-selected.  If row -1, removes selection.

 Selects a row, or removes selection from all.

 row - 0-based row index. If -1, removes selection from all.


LVITEM li.stateMask=iif(flags&4 LVIS_CUT LVIS_FOCUSED|LVIS_SELECTED)
if flags&1=0
	Send(LVM_SETITEMSTATE -1 &li)
	if(row<0) ret
	if(flags&4=0) li.state|LVIS_FOCUSED

li.state|iif(flags&4 LVIS_CUT LVIS_SELECTED)
Send(LVM_SETITEMSTATE row &li)

if(flags&2) Send(LVM_ENSUREVISIBLE row)
