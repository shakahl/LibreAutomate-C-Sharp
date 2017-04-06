 /test dialog image preview
function

DIPDATA- d
SendMessage d.hlv LVM_DELETEALLITEMS 0 0
d.a=0

int i fl
if(d.flags&1) fl|4
GetFilesInFolder d.a d.folder "(\.png|\.jpg|\.jpeg)$" fl|0x10000
for i 0 d.a.len
	str s.getfilename(d.a[i] 1)
	 Dir k.dir(d.a[i])
	 out k.FileSize
	TO_LvAdd d.hlv -1 0 0 s
