 /
function hcb

 Automatically completes when you type in combo box.
 Call from dialog procedure, on CBN_EDITCHANGE.
 hcb - combo box handle (use lParam).

 EXAMPLE
	 case CBN_EDITCHANGE<<16|3
	 CB_AutoComplete lParam


ifk(B) ret
ifk(X) ret

str s ss; int i

s.getwintext(hcb)
if(s.len)
	i=CB_FindItem(hcb s)
	if(i>=0)
		CB_GetItemText(hcb i ss)
		if(ss.len>s.len)
			ss.setwintext(hcb)
			SendMessage(hcb CB_SETEDITSEL 0 ss.len<<16|s.len)
