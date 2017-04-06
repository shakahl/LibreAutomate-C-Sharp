 /
function# FFNode&x level str&s FFNODEINFO&ni

 Callback function for Acc.FindFF. Created from a template in File -> New -> Templates menu.

_s=x.Attribute("src")
if(_s.len) s.addline(_s)
ret 1
