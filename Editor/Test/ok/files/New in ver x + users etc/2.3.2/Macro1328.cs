str s=
 line 1
 "line 2"

s.escape(1) ;;escape
out s
s.escape(0) ;;unescape
out s

str s2="one, two"
s2.escape(9) ;;urlencode
out s2
s2.escape(8) ;;urldecode
out s2
