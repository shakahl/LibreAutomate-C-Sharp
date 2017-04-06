out
str s="tyu"
rset s "name" "key" "$desktop$\test.ini"
rset s "name2" "key" "$desktop$\test.ini"
rset s "name3" "key2" "$desktop$\test.ini"
rset s "name4" "key2" "$desktop$\test.ini"

str ss
out rget(ss "name" "key" "$desktop$\test.ini")
out ss

out rget(ss "nameno" "key" "$desktop$\test.ini" "no")
out ss

 out rset("" "name" "key" "$desktop$\test.ini" -1)
 out rset("" "" "key2" "$desktop$\test.ini" -2)
