 /
function str&cbVar selItem [$items]

 Formats cbVar to be passed to ShowDialog as a combo box variable.
 Inserts & depending on selItem.

 cbVar - combo box variable. Should not have lines that begin with &.
 selItem - 0-based item index.
 items - if used, adds to cbVar (then cbVar can be empty).


if(!empty(items)) cbVar.addline(items)
if(selItem<0) ret
selItem=findl(cbVar selItem); if(selItem<0) ret
cbVar.insert("&" selItem)
