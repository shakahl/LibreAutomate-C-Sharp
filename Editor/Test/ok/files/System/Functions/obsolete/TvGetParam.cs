function# tv hItem

if(!hItem) ret
TVITEMW ti
ti.mask = TVIF_PARAM; ti.hItem = hItem
SendMessage(tv TVM_GETITEMW 0 &ti)
ret ti.lParam
