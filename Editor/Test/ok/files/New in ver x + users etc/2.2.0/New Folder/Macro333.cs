dll "qm.exe" EscapeCString str*s flags ;;flags: 1 text with strings, 2 replace to QM esc

str s
 s="abcd"
 s="''abcd''"
 s=" ''abcd'' p"
 s=" ''ab\r\ncd'' p"
 s=" ''abcd'' \r\np"
 s=" ''ab\x40 cd'' p"
 s=" ''ab\x40 cd'' ''''"
 s=" ''ab\x40 cd'' ''str2''"
 s=" ''ab\x40 cd'' ''str2\ttab''"
 s=" ''abcd p"
 s=" ''ab\''cd'' p"
 s="ab\''cd"
 s="''\\''"
s=
  "a[]b" p
  "a[65]b" p
if(!EscapeCString(&s 3)) ret
out s
