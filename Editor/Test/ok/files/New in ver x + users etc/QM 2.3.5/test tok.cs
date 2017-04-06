out
str s=
  a, b , "quo, te" "kkk, nnn", "nnnn ,mmmm","fff, kk", F"string, hhh", "bad, string 

ARRAY(str) a
ARRAY(lpstr) b
 tok(s a -1 ",[]''" 4 b)
 tok(s a -1 ",[]''" 4|0x2000 b)
tok(s a -1 ",[]" 4|0x2000 b)
 tok(s a -1 ",[]''" 4 b)
int i
for i 0 a.len
	out F"'{a[i]}'          '{b[i]}'"
	