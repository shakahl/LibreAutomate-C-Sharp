 /
function# htv hi str&s

TVITEMW t.mask=TVIF_TEXT; t.hItem=hi
BSTR b; t.pszText=b.alloc(300); t.cchTextMax=300
if(!SendMessage(htv TVM_GETITEMW 0 &t)) ret
s.ansi(b)
ret 1
