str s.getclip; err ret
 out s

if(__cct.len=1) ;;!!rem
	_s.getmacro(__cct[0].iid 2); err
	if(!_s.len) __cct=0

int i
for(i 0 __cct.len)
	CCT_ITEM& r=__cct[i]
	if(dis(r.iid) or dis&16) continue
	if(findrx(s r.rx)>=0 or (!s.len and !r.rx.len))
		mac r.iid
	
err+
