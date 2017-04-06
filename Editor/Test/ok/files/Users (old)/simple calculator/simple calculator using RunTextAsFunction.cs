str s ss
inp- s "Ex: 50+30/4. Ex2: sqrt(pow(10 2)+pow(20 2))." "Calculator"
str f=
 mes (%s)
 #err 1
 mes "error"
ss.format(f s)
RunTextAsFunction ss
