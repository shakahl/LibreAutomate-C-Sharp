 /
function hEdit $items

 Automatically completes when you type in edit control.
 Call from dialog procedure, on EN_CHANGE. For grid control (edit or combo cell), call on GRID.LVN_QG_CHANGE.

 hEdit - edit control handle.
 items - list of items.

 EXAMPLE
	 case EN_CHANGE<<16|3
	 Edit_AutoComplete lParam "one[]two[]three"


ifk(B) ret
ifk(X) ret

str s1 s2

s1.getwintext(hEdit)
if s1.len
	foreach s2 items
		if(s2.begi(s1) and s1.len<s2.len)
			s2.setwintext(hEdit)
			SendMessage hEdit EM_SETSEL s1.len s2.len
			break
