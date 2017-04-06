function` `v x

out "---- in func"

 out x
 
 ARRAY(BSTR) b=v
 for(_i 0 b.len) out b[_i]
 
 ARRAY(str) a=v
 out a

ARRAY(str) r="three[]four"
out "---- array created"
VARIANT vr=r
out "---- returning"
ret vr
