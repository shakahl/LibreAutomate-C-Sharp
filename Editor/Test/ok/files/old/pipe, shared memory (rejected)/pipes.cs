out
__Handle hr hw

if(!CreatePipe(&hr &hw 0 0)) end "failed" 16

mac "sub.Receiver" "" hr
0.1
WakeCPU
rep 3
	int nw
	PF
	if(!WriteFile(hw "test" 5 &nw 0)) end "failed" 16
	PN;PO
	out F"sender: nw={nw}"
	 0.5
	0.01

if(!WriteFile(hw "!" 2 &nw 0)) end "failed" 16
0.5


#sub Receiver
function hr

str s.all(1000)
int nr
rep
	 PF
	if(!ReadFile(hr s 1000 &nr 0)) end "failed" 16
	 PN;PO
	out F"receiver: nr={nr}"
	out s.lpstr
	if(s[0]='!') break
	 1
	
