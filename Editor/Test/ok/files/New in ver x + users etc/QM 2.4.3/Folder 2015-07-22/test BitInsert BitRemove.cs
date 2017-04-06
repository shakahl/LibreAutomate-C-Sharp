out
 int i=8
 outx -i
 ret

int x r
x=0xaaaaaaaa
 x=~x

r=sub.BitInsert(x 1 0)

 r=sub.BitRemove(x 0)

lpstr s
s=_itoa(x _s.all(100) 2); out "%.*m%s" 32-len(s) '0' s
s=_itoa(r _s.all(100) 2); out "%.*m%s" 32-len(s) '0' s


#sub BitInsert
function# x pos bit
int mask=-(1<<pos)
ret (x&~mask) | ((x<<1)&mask) ~ (1<<pos) | (bit<<pos)


#sub BitRemove
function# x pos
int mask=-(1<<pos)
ret (x&~mask) | ((x>>1)&mask) 
