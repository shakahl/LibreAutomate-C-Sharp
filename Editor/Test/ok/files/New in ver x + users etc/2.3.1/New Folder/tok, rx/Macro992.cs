out
str s="a ė c"
ARRAY(str) a
ARRAY(lpstr) aa
int nt=tok(s a -1 "" 0 aa)
 int nt=tok(s a -1 "" 0 0)
 int nt=tok(s 0 -1 "" 0 aa)
 int nt=tok(s 0 -1 "" 0 0)
 str s1 s2 s3; int nt=tok(s a -1 "" 0 &s3) ;;error (OK)
 str s1 s2 s3; int nt=tok(s &s3 -1 "" 0 aa) ;;error (OK)
int i
for i 0 nt
	if(a.len and aa.len) out "%s|%s|" a[i] aa[i]
	else if(a.len) out "%s|" a[i]
	else if(aa.len) out "%s|" aa[i]
	