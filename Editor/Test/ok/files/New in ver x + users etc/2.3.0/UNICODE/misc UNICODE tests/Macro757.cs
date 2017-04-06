out
str s.all(256 2)
int i
for i 0 256
	s[i]=i

BSTR b.alloc(256)
GetStringTypeEx LOCALE_USER_DEFAULT CT_CTYPE1 s 256 b

for i 0 256
	int x=b[i]
	out "%s %s %s %s %s %s %s %s" _outc(i) iif(x&C1_UPPER "U" iif(x&C1_LOWER "L" " ")) iif(x&C1_DIGIT "D" " ") iif(x&C1_SPACE "S" " ") iif(x&C1_PUNCT "P" " ") iif(x&C1_CNTRL "C" " ") iif(x&C1_BLANK "B" " ") iif(x&C1_ALPHA "A" " ")

