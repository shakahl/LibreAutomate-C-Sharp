 /
function hlv item [flags] ;;flags: 1 don't deselect previous, 2 don't set focus, 4 ensure visible, 8 deselect

 Selects listview item.
 To select none, use item -1.
 To select all, use item -1 and flag 8.
 Note that listview controls with LVS_SINGLESEL style can have max 1 item selected.


LVITEMW li.stateMask=LVIS_FOCUSED|LVIS_SELECTED
if(item>=0)
	if(flags&9=0) SendMessage hlv LVM_SETITEMSTATE -1 &li
	if(flags&8=0) li.state=LVIS_SELECTED; if(flags&2=0) li.state|LVIS_FOCUSED
	SendMessage hlv LVM_SETITEMSTATE item &li
	if(flags&4) SendMessage hlv LVM_ENSUREVISIBLE item 0
else
	if(flags&8) li.state=LVIS_SELECTED
	SendMessage hlv LVM_SETITEMSTATE -1 &li
