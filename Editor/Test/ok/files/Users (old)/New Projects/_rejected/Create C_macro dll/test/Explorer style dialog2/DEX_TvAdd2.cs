 /
function# htv hparent $s [lParam] [iimage]

 Adds new item to a tree view control.


TVINSERTSTRUCTW inss
inss.hParent = hparent
inss.hInsertAfter = TVI_LAST
TVITEMW& ti=inss.item
ti.mask = TVIF_TEXT
if(getopt(nargs)>=5 and iimage>=0)
	ti.mask |= TVIF_IMAGE|TVIF_SELECTEDIMAGE
	ti.iImage=iimage
	ti.iSelectedImage=iimage
if(!s) s=""
if(getopt(nargs)>3) ti.mask|TVIF_PARAM; ti.lParam=lParam

ti.pszText=@s
 ret WINAPI.TreeView_InsertItem
ret SendMessage(htv TVM_INSERTITEMW 0 &inss)
