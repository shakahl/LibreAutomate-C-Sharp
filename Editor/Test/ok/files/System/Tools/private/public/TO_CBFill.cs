 /
function h $items [lb] [seli] ;;lb: 0 combobox, 1 listbox, 2 listbox multisel.

int m1 m2 m3 ni i; str s
if(lb) m1=LB_RESETCONTENT; m2=LB_ADDSTRING; m3=LB_SETCURSEL
else m1=CB_RESETCONTENT; m2=CB_ADDSTRING; m3=CB_SETCURSEL
if(getopt(nargs)<4) seli=-1

SendMessage(h m1 0 0)
foreach s items
	if(i=seli or s[0]='&')
		ni=SendMessageW(h m2 0 @(s+(i!seli)))
		if(lb=2) SendMessage(h LB_SETSEL 1 ni); else SendMessage(h m3 ni 0)
	else SendMessageW(h m2 0 @s)
	i+1
if(lb=2) SendMessage(h LB_SETTOPINDEX 0 0)
