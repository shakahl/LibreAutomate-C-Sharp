 rset "onevalue" "one" "key" "$desktop$\test.ini"
 rset "twovalue" "two" "key" "$desktop$\test.ini"
 rset "threevalue" "three" "key" "$desktop$\test.ini"

str sn s1 s2 s3
 rget s1 "one" "key" "$desktop$\test.ini"
 rget s2 "two" "key" "$desktop$\test.ini"
 rget s3 "three" "key" "$desktop$\test.ini"

sn="one=two=three"
GetStringsFromIni "$desktop$\test.ini" &sn

out s1
out s2
out s3
