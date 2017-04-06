out
str s="one, two three"
ARRAY(str) a aa
 ARRAY(lpstr) a
 ARRAY(str) aa
 out tok(s a)
 out tok(s a 2)
 out tok(s a -1 ",")
 out tok(s a -1 2)
 out tok(s a -1 "," 0x1000)
 out tok(s a -1 "" 0 aa)
 out tok(s 0 -1 "" 0)
 out tok(s a -1 "" 0)
 out tok(s 0 -1 "" aa)
out a
out "----"
out aa
out "----"
