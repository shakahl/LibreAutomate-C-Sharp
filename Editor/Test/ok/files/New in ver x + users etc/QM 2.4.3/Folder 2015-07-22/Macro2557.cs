out
lpstr o=":5 $qm$\il_qm.bmp"

str s=
F
 ,"{o}"
 one,1
 two,2
 three,0

rep(200) s.addline(_s.RandomString(5 20 "a-z") 1);; qmcb3+",4"

ICsv x._create
x.FromString(s)

ARRAY(byte) a
 a.create(3); a[2]=1

 RECT r; SetRect &r 500 300 700 500

int R i
 R=ShowDropdownListSimple(s i a 1)
 R=ShowDropdownListSimple(s i 0 0 id(2216 _hwndqm))
 R=ShowDropdownListSimple(s i 0 0 0 r)
R=ShowDropdownList(x i 0 0 0 0)
out "0x%X %i" R i

if(a.len) outb &a[0] a.len
