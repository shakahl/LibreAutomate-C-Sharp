 /
function# lParam [idAction] [$icon] [$func]

 Called on WM_USER+7 to support adding to favorites.


__FAVRET& f=+lParam
if(func and func&0xffff0000=0) f.dlg=func
else f.dlg=iif(empty(func) getopt(itemid 1) qmitem(func 1))
f.ctrl=idAction
f.icon=icon
ret 1
