 /Compare2Macros
function action h1 h2 ;;action: 0 clear, 1 compare

if action
	if(!h1 or !h2) mes "Some of the selected controls is invisible or does not exist." "" "!"; ret
	if(h1=h2) action=0; h2=0

 ------ create markers and clear -------

int colorAdded(0xc0ffc0) colorDeleted(0xd0d0ff) colorModified(0xc0ffff)
int markAdd(29) markDel(30) markMod(31) i j h
int* pi ph(&h1)
for i 0 2
	h=ph[i]
	if(!h) continue
	hid h ;;faster
	pi=&markAdd
	for j 0 3
		SendMessage h SCI.SCI_MARKERDELETEALL pi[j] 0
		if(action=0) continue
		SendMessage h SCI.SCI_MARKERDEFINE pi[j] SCI.SC_MARK_BACKGROUND
		SendMessage h SCI.SCI_MARKERSETBACK pi[j] pi[j-3]
if(action=0) goto fin

 ------ get text -------

str s1 s2 sd
C2M_SciGetText h1 s1
C2M_SciGetText h2 s2

 ------ create diff -------

#compile "__Xdiff"
Xdiff x
x.DiffText(s1 s2 sd)
 out sd

 ------ apply -------

int i1 n1 i2 n2 m
lpstr z1 z2 z3 z4
foreach s1 sd
	sel s1[0]
		case '@'
		lpstr* sp=&z1
		pi=&i1
		tok s1 sp 4
		for(i 0 4) pi[i]=val(sp[i])
		i1-1; i2-1
		continue
		
		case '-'
		if(n1 and n2) h=h1; m=markMod; i=i1; i1+1
		else if(n1) h=h1; m=markDel; i=i1; i1+1
		else h=h2; m=markDel; i=i2; i2+1
		
		case '+'
		if(n1 and n2) h=h2; m=markMod; i=i2; i2+1
		else if(n2) h=h2; m=markAdd; i=i2; i2+1
		else h=h1; m=markAdd; i=i1; i1+1
	
	SendMessage h SCI.SCI_MARKERADD i m

 Q &qqqqq
 outq

 ------ fin -------

 fin
if(h1) hid- h1
if(h2) hid- h2
