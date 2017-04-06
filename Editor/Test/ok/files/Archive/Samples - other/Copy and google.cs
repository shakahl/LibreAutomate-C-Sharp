str s.getsel
if(!s.len) ret
s.trim
if(findc(s 32)>=0) s-"''"; s+"''"
s.escape(9)
s-"http://www.google.com/search?q="
run s
