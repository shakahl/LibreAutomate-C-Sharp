 /
function# hlv index lparam image ~s [~s1] [~s2] [~s3] [~s4] [~s5] [~s6] [~s7] [~s8] [~s9]

 Adds item to ListView32 control that has LVS_REPORT style and 1 to 10 columns.


if(index<0) index=0x7FFFFFFF
LVITEM lvi.mask=LVIF_TEXT|LVIF_PARAM|LVIF_IMAGE
lvi.iItem=index
lvi.pszText=s
lvi.lParam=lparam
lvi.iImage=image
index=SendMessage(hlv LVM_INSERTITEM 0 &lvi)

int i; str* p=&s
for i 1 getopt(nargs)-2
	lvi.iItem=index
	lvi.iSubItem=i
	lvi.pszText=p[i]
	SendMessage(hlv LVM_SETITEMTEXT index &lvi)

ret index
