dll "qm.exe" #DecimalToInt64 DECIMAL'x %*la
out

DECIMAL x
long k

 x=-5
 x="-1111111111222222222"
 x="-11111111112222222222"
 x="-111111111122222222223333"
 x="-1111111111.000000001"
 x="-1111111111.9999999999"
 x="-1111111111.22222222223333"
 x="10E-20"
 x="111111111122222222223333333333E-30"
 x="1E20"

 x.scale=20
 x.Lo64=11111111112222222

int hr=DecimalToInt64(x &k)
if(hr) out _s.dllerror("" "" hr)
out k

BSTR b=x
out b

int i=x
out i
