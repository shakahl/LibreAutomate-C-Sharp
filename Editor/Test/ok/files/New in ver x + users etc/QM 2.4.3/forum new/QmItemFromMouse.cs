 /
function#

 Returns id of QM item from mouse in the list of macros.
 Returns 0 if mouse is not on an item.
 Does not work in exe.

 EXAMPLE
 int iid=QmItemFromMouse; if(!iid) ret
 str name.getmacro(iid 1)
 out name


#if !EXE
int htv=id(2202 _hwndqm)
TVHITTESTINFO ht
GetCursorPos(&ht.pt); ScreenToClient(htv &ht.pt)
SendMessage(htv TVM_HITTEST 0 &ht)
if(ht.flags&(TVHT_ONITEM|TVHT_ONITEMRIGHT|TVHT_ONITEMINDENT)=0) ret
ret TvGetParam(htv ht.hItem)
