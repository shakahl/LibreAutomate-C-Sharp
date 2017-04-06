 /
function# [UDTRIGGER&p]

int i vk mod fl iid
str s

for(i 0 __kt2a.len) if(__kt2a[i].regged) UnregisterHotKey(__kt2hwnd i); __kt2a[i].regged=0

if(&p and p.niids and dis&16=0)
	__kt2a.redim(p.niids)
	for(i 0 p.niids)
		KT2_ITEM& k=__kt2a[i]
		iid=p.iids[i]
		 out _s.getmacro(iid 1)
		s.getmacro(iid 2)
		s.gett(s 1 " " 2)
		if(KT2_Parse(s mod vk fl 0))
			k.iid=iid; k.flags=fl; k.mod=mod; k.vk=vk
			if(!dis(iid)) k.regged=RegisterHotKey(__kt2hwnd i mod vk)
	ret 1
else __kt2a.redim
