out
int i

BSTR t.alloc(0x10000)
for(i 0 0x10000) t[i]=i
CharLowerBuffW(t.pstr 0x10000)
for i 0 0x10000
	int x=t[i]
	if(x!=i) out "0x%X  %C %C  0x%X" i i x x

str u.all(0x10000)
 PF
 for i 'A' 0x10000
	  CharUpperW(+i)
	 u[i]=IsCharUpperW(+i)
 PN
 PO

 PF
 BSTR b.alloc(0x10000) t.alloc(0x10000)
 PN
 for(i 0 0x10000) b[i]=i
 if(!GetStringTypeW(CT_CTYPE1 b.pstr+'A' 0x10000-'A' t)) ret
 PN
 for(i 0 0x10000) u[i]=t[i]&1
 PN
 PO
