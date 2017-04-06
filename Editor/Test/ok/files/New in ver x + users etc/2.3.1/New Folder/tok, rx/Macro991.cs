out
 str s="a (b <c d) e> f)"
 int fl=8|64|0x200

 str s="a (b ''c d) e'' f) g"
 int fl=8|0x400

 str s="a (b ''c d) e'' f) g"
 int fl=4|8|16|32|64|128|0x200|0x400

 str s="a (b c d')' e f) g"
 int fl=8|0x400

str s="a (b c d'[34]' ')' ''mm)m''e f) g"
out s
int fl=8|4|128|0x600


ARRAY(str) a
tok s a -1 "" fl
int i
for i 0 a.len
	out "%s|" a[i]
	