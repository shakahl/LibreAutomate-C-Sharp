 /
function# hwnd [ARRAY(int)&ai] [ARRAY(str)&as]

 Gets selected items in a multisel listbox control.
 Returns number of selected items.

 hwnd - control handle.
 ai - array variable that receives indices of selected items. Optional, can be 0.
 as - array variable that receives text of selected items. Optional, can be 0.

 REMARKS
 The control must belong to current process. Use eg in dialog procedures.


if(&ai) ai=0
if(&as) as=0

int i n
n=SendMessage(hwnd LB_GETSELCOUNT 0 0); if(n<1) ret
if &ai or &as
	ARRAY(int) _ai; if(!&ai) &ai=_ai
	ai.create(n)
	n=SendMessage(hwnd LB_GETSELITEMS n &ai[0]); if(n<0) n=0
	if(n!ai.len) ai.redim(n)
	if &as
		as.create(n)
		for(i 0 n) LB_GetItemText(hwnd ai[i] as[i])
ret n
