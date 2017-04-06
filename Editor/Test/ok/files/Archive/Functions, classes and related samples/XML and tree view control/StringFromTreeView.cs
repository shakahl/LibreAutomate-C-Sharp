 /
function str&s htv [htvi] [__indent]

 Gets tree view control items to tab-indented string.

 s - variable that receives string.
 htv - control handle.
 htvi - parent item handle. Can be 0.
 __indent - don't use.


if(!__indent) s.all
htvi=SendMessage(htv TVM_GETNEXTITEM iif(htvi TVGN_CHILD TVGN_ROOT) htvi)
rep
	if(!htvi) break
	TVITEMW t.mask=TVIF_TEXT|TVIF_CHILDREN
	t.hItem=htvi
	BSTR b; if(!b) b.alloc(1000)
	t.pszText=b; t.cchTextMax=1001
	SendMessage(htv TVM_GETITEMW 0 &t)
	_s.ansi(b)
	str si.all(__indent 2 9) ;;creates string of __indent tabs. Change to other characters if need.
	s.formata("%s%s[]" si _s)
	if t.cChildren
		StringFromTreeView s htv htvi __indent+1
	htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT htvi)

err+ end _error
