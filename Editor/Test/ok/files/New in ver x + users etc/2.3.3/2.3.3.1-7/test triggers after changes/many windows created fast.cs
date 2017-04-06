ARRAY(int) a.create(180)
int i
Q &q
for i 0 a.len
	a[i]=CreateWindowEx(0 "#32770" 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
Q &qq
outq
outw a[0]
1
for i 0 a.len
	DestroyWindow a[i]
