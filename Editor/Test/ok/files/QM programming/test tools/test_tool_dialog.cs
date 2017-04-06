 /
function [$name]

1
int h=wait(5 WA win(name "#32770" "qm" 0x400))
for _i 1 1000
	int hc=child("" "*Edit" h 0 0 0 _i); if(!hc) ret
	if(GetWinStyle(hc)&ES_READONLY=0) break
act hc
"text"
1

_s.getwintext(h); if(_s~"Custom dialog") but 2 h; ret

but 1 h
