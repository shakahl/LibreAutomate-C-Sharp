function `item [flags] ;;flags: 1 add to selection, 2 case insens

 Selects combo box item.

 item - item text (exact or with *?), or 0-based index.
 flags (QM 2.3.1):
   1 - add to selection.
   2 (QM 2.3.3) - case insensitive.


if(!el) end ERR_INIT

MSHTML.IHTMLSelectElement elsel=+el
int index

sel item.vt
	case VT_I4
	index=item
	if(index>=elsel.length) end ERR_ITEM
	
	case VT_BSTR
	str s(item) ss
	MSHTML.IHTMLOptionElement elop
	index=-1
	foreach elop elsel
		ss=elop.text
		if(!matchw(ss s flags&2!0)) continue
		index=elop.index; goto g1
	end ERR_ITEM
	
	case else end ERR_ITEM

 g1
if flags&1
	MSHTML.IHTMLOptionElement oe=elsel.item(index)
	if(oe.selected) ret
	oe.selected=-1
else
	if(index=elsel.selectedIndex) ret
	elsel.selectedIndex=index

err+ end _error

MSHTML.IHTMLElement3 e=+elsel
e.FireEvent("onchange")
err+
