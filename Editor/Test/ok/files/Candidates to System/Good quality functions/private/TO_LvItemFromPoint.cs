 /
function# hlv [int&part] [POINT&p]

 Sends LVM_HITTEST and returns its return value.

 part - receives LVHT_.
 p - point in hlv client area. If not used, gets point from mouse.

LVHITTESTINFO ht
if(&p) ht.pt=p; else xm &ht.pt hlv 1
int i=SendMessage(hlv LVM_HITTEST 0 &ht)
if &part
	if i<0 and ht.flags==LVHT_NOWHERE
		RECT rc; GetClientRect(hlv &rc)
		if(!PtInRect(&rc ht.pt.x ht.pt.y)) ht.flags=0
	part=ht.flags
ret i
