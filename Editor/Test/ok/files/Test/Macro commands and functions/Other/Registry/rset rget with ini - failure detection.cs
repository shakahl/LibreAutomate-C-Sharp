out
 out rset("g" "g")
 out rset("test" "n" "k" "$desktop$\rs?et.ini")

 int i
 out rset(5 "int" "k" "$desktop$\rset.ini")
 out rget(i "int" "k" "$desktop$\rset.ini" -1)
 out i

str s
out rset("abcdefghijklmn" "str" "m" "$desktop$\rset.ini")
out rget(s "str" "m" "$desktop$\rset.ini" "def    ")
out s

 double d
 out rset(5.4 "dou" "k" "$desktop$\rset.ini")
 out rget(d "dou" "k" "$desktop$\rset.ini" -1.1)
 out d

 long lo=7000000000000
 out rset(lo "long" "k" "$desktop$\rset.ini")
 lo=0
 out rget(lo "long" "k" "$desktop$\rset.ini" -3)
 out lo

 RECT r.bottom=7
 out rset(r "rect" "k" "$desktop$\rset.ini")
 r.bottom=0
 out rget(r "rect" "k" "$desktop$\rset.ini")
 out r.bottom
