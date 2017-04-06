str s="x y"
s[1]=0
rset s "aą" "software\gindi\qm2\settings\č"

 s.all(2000 2 'k')
 out rset(s "big" "software\gindi\qm2\settings\č")
 
 _s.all(1000 2 'b')
 out rset("tt" _s "software\gindi\qm2\settings\č")

s.all(200 2 'K')
out rset("tt" "i" _s.from("software\gindi\qm2\settings\" s))
