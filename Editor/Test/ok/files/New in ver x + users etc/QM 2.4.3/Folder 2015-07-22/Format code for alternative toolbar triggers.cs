out
ARRAY(QMITEMIDLEVEL) a
GetQmItemsInFolder "" a
int i iid
for i 0 a.len
	iid=a[i].id
	QMITEM q
	if(!qmitem(iid 1|2|16|32|64 q 7|32|64)) continue
	if(q.itype!3 or q.ttype!3) continue
	out F"<>{q.name} <c 0x8000>{q.triggerdescr}</c>"
	