out
str s="aa bb  cc[9]dd"
ARRAY(str) as
ARRAY(POINT) ap
out RegexSplitString(s "\s+" as ap)
int i
for i 0 as.len
	out F"[{as[i]}]"
	out F"{ap[i].x} {ap[i].y}"
