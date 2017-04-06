 /
function# iid FILTER&f

if wintest(f.hwnd "* Message" "OpusApp" "" 1)
	int c=child
	sel GetWinId(c)
		case [4097,4098] ret -2

err+
ret iid

 ret iid	;; run the autotext list item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other autotext lists can run.
