str s=
 aa
 bb
 cc

if(findrx(s "^.+?b")>=0) out "found"; else out "not found"
if(findrx(s "^.+?b" 0 RX_DOTALL)>=0) out "found"; else out "not found"
if(findrx(s "(?s)^.+?b")>=0) out "found"; else out "not found"
