 /
function# htv htviParent $text [lParam] [iimage]

 Adds item to a tree view control.
 Returns item handle.

 htv - tree view control handle.
 htviParent - parent item handle or 0.
 text - text.
   QM 2.3.2. Can be LPSTR_TEXTCALLBACK.
 lParam - lParam of the item.
 iimage - icon index in imagelist. If omitted or -1, the item will not have icon.


TVINSERTSTRUCTW in
in.hParent = htviParent
in.hInsertAfter = TVI_LAST
TVITEMW& ti=in.item
ti.mask = TVIF_TEXT
if(getopt(nargs)>=5 and iimage>=0)
	ti.mask |= TVIF_IMAGE|TVIF_SELECTEDIMAGE
	ti.iImage=iimage
	ti.iSelectedImage=iimage
if(getopt(nargs)>3) ti.mask|TVIF_PARAM; ti.lParam=lParam

if text!LPSTR_TEXTCALLBACK
	if(!text) text=""
	ti.pszText=@text

ret SendMessage(htv TVM_INSERTITEMW 0 &in)
