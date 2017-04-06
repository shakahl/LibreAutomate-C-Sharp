str s.getsel ss
if(!s.len) ret
AC_Prepare &s
s.findreplace("#define" "def" 2)
CH_Compact s &ss
ss.replacerx("(\d)L(?=\r)" "$1" 8)
ss.setsel

