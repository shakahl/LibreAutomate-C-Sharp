rset 5 "QMA" "Software\GinDi"

RegKey k.Open("Software\GinDi")
DateTime t
if(RegQueryInfoKey(k 0 0 0 0 0 0 0 0 0 0 +&t)) end "failed"
out t.ToStr(4|8)

#ret
2013.02.04 12:40:15.735
