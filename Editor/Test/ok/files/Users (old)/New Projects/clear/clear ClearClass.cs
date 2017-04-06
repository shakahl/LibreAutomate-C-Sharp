 out
 clear
#compile "__ClearClass"
ClearClass x.i=5; x.s="sssssss"; x.s.flags=1
out x.i
out x.s
 __test x
x.Clear
out x.i
out x.s
outb &x sizeof(x)
