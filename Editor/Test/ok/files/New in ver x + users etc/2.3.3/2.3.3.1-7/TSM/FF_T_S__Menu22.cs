 /
function# iid FILTER&f

 out "FF: %i" iid
_s.getstruct(f 1); _s.escape(1); out "FF: %i %s" iid _s
 ret -2
 ret -3
ret iid


 ret iid		;; run the TS menu item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other TS menus can run.
