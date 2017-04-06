str ini="$desktop$\test.ini"

int i;;=5
byte b;;=1
double do;;=77.77
str s;;="sss"
POINT p;;.y=7

rget i "i" "key" ini; out i
rget b "b" "key" ini; out b
rget do "do" "key" ini; out do
rget int'dig "digit" "key" ini; out dig
rget s "s" "key" ini; out s
rget str'lps "strcon" "key" ini; out lps
rget p "p" "key" ini; out p.y
