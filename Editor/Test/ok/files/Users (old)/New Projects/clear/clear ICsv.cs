out
 clear
ICsv x=CreateCsv(1)
x.FromString("a,b[]c,d")
x.ToString(_s); out _s
__test x
if(x) x.ToString(_s); out _s
outb &x sizeof(x)
