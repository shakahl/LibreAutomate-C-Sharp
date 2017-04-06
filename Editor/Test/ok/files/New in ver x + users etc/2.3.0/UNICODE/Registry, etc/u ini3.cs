out
str ini="$desktop$\test.ini"

 ---

rset "some string" "str" "x" ini

rset 5 "int" "x" ini

POINT p
p.x=1
p.y=2
rset p "struct" "x" ini

 ---

str s
int i
POINT pp

out rget(s "str" "x" ini)
out s

out rget(i "int" "x" ini)
out i

out rget(pp "struct" "x" ini)
out pp.x
out pp.y

