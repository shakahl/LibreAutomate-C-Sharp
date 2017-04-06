str s ss
out rget(s "QM toolbar" "Software\GinDi\QM2\toolbars" 0 "" REG_BINARY)
 outb s 16 1

byte* ptr=s
int nBytes=s.len

ss.all(nBytes*3 2 32)
for(_i 0 nBytes) if(ptr[_i]>=32) ss[_i*3+1]=ptr[_i]

out ss