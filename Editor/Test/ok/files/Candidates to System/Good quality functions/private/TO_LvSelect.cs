 /
function hlv index [flags] ;;flags: 1 add to selection, 2 ensure visible, 4 set cut-selected.  If index -1, removes selection.

 Selects a row, or removes selection from all.

 index - 0-based row index. If -1, removes selection from all.


LVITEM li.stateMask=iif(flags&4 LVIS_CUT LVIS_FOCUSED|LVIS_SELECTED)
if flags&1=0
	SendMessage(hlv LVM_SETITEMSTATE -1 &li)
	if(index<0) ret
	if(flags&4=0) li.state|LVIS_FOCUSED

li.state|iif(flags&4 LVIS_CUT LVIS_SELECTED)
SendMessage(hlv LVM_SETITEMSTATE index &li)

if(flags&2) SendMessage(hlv LVM_ENSUREVISIBLE index 0)
