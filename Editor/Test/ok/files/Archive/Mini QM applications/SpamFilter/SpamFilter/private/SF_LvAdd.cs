 /
function# hlv index lparam ~s [~s1] [~s2] [~s3] [~s4] [~s5] [~s6] [~s7] [~s8] [~s9]

if(index<0) index=0x7FFFFFFF
LVITEM lvi.mask=LVIF_TEXT|LVIF_PARAM
lvi.iItem=index
lvi.pszText=s
lvi.lParam=lparam
index=SendMessage(hlv LVM_INSERTITEM 0 &lvi)

int i; str* p=&s
for i 1 getopt(nargs)-2
	lvi.iItem=index
	lvi.iSubItem=i
	lvi.pszText=p[i]
	SendMessage(hlv LVM_SETITEMTEXT index &lvi)

ret index
