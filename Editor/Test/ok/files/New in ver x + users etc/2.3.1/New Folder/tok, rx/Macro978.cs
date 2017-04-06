out
str s="a {b <c (d) e> f} g"
 str s="a {b <c ''(d) e> f} g'' h"
 str s="''a {b <c ''(d) e> f} g'' h"
 str s="a {b <c '>'(d) e> f} g h"
 str s="a <b c d> e"
 str s="a <b c> d> e"
 str s="a <b c'>' d> e"
ARRAY(str) a
int i
 tok s a -1
 tok s a -1 "" 8|32|64
tok s a -1 "(<{}>) " 8|32|64
for i 0 a.len
	out a[i]
	