 /
function# hwndctrl [ctrltype]

 Obsolete.


ret SendMessage(hwndctrl iif(ctrltype LB_GETCOUNT CB_GETCOUNT) 0 0)
