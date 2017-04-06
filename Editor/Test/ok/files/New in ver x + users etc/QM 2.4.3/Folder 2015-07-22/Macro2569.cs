out
str s=
 0,,1
 zero,0,16
 one,1,
 two,0,16
 three,0,0

ARRAY(byte) a
int i
int R=ShowDropdownListSimple(s i a)
out "0x%X %i" R i
if(a.len) outb &a[0] a.len
