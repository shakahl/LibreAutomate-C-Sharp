 /
function# hlv index

LVITEM lvi.mask=LVIF_PARAM
lvi.iItem=index
if(SendMessage(hlv LVM_GETITEM 0 &lvi)) ret lvi.lParam
