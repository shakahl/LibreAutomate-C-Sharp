 create array
str* p._new(10)
int i
for i 0 p._len
	p[i].from("line" i)

 remove element 3
int rem=3
int nelementstomove=p._len-rem-1
if(nelementstomove)
	p[rem].all
	memmove &p[rem] &p[rem+1] nelementstomove*sizeof(str)
	p[p._len-1].lpstr=0
p._resize(p._len-1)

 results
out
for i 0 p._len
	out p[i]

p._delete
