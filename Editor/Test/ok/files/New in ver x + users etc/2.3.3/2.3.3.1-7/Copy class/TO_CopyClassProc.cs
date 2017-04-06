 /TO_CopyClass
function# iid QMITEM&q level CC_DATA&d

str s=q.name
s.findreplace(d.oldName d.newName 2|4 " _.=")

 out "%s %s" q.name s

 find new parent folder
int i fid
for(i 0 d.f.len) if(d.f[i].x=q.folderid) fid=d.f[i].y; break

i=newitem(s "" iid "" fid 16)
if(q.itype=5)
	POINT& f=d.f[]
	f.x=iid
	f.y=i

err+ out _error.description
