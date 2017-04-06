 /
function [UDTRIGGER&p]

type CCT_ITEM @iid ~rx
ARRAY(CCT_ITEM)+ __cct

if(!&p) __cct=0; ret
__cct.create(p.niids)
int i
for(i 0 __cct.len)
	CCT_ITEM& r=__cct[i]
	r.iid=p.iids[i]
	r.rx.getmacro(r.iid 2)
	r.rx.gett(r.rx 1 " " 2)
	r.rx.trim(34); r.rx.escape(0)
	