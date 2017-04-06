 /
function htv 

Acc a=acc("" "CHECKBUTTON" htv)
if(!a.a) ret

act htv
rep
	if(a.State&STATE_SYSTEM_CHECKED=0)
		a.Select(2)
		key (VK_SPACE)
	a.Navigate("next"); err break