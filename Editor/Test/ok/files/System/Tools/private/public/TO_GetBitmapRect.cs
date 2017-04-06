 /
function! hbm RECT&r
BITMAP bi
if(GetObjectW(hbm sizeof(bi) &bi))
	r.left=0; r.top=0
	r.right=bi.bmWidth
	r.bottom=bi.bmHeight
	ret 1
