 out
str s="<>"
int i
for i 0 250
	int c=RandomInt(1 0xffffff)
	s.formata("<c 0x%X>0x%X</c>[]" c c)
out s
out "<><i>italic</i>"
 OutStatusBar s
 OutStatusBar "<><i>italic</i>"
