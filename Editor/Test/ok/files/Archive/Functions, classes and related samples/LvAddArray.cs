 /
function# hlv index lparam image ARRAY(str)&a

 Adds item to ListView32 control that has LVS_REPORT style.

 a - array containing all items in row.


if(index<0) index=0x7FFFFFFF
LVITEMW lvi.mask=LVIF_TEXT|LVIF_PARAM|LVIF_IMAGE
lvi.iItem=index
lvi.pszText=@a[0]
lvi.lParam=lparam
lvi.iImage=image
index=SendMessage(hlv LVM_INSERTITEMW 0 &lvi)

int i
for i 1 a.len
	lvi.iItem=index
	lvi.iSubItem=i
	lvi.pszText=@a[i]
	SendMessage(hlv LVM_SETITEMTEXTW index &lvi)

ret index
