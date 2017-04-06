out
QMITEM q
int i n
for i 1 13000
	int iid=qmitem(i 0 q 1)
	if(iid=i) continue
	out "%i %i %s" i iid q.name
	n+1
out n
