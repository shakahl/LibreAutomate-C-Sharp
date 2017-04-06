 /

 Capitalizes first character of each word.

 EXAMPLE
 str s="quick macros"
 s.CapWords
 out s ;;"Quick Macros"


ARRAY(lpstr) a
tok this a
int i
for i 0 a.len
	a[i][0]=toupper(a[i][0])
