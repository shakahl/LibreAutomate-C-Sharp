RECT r
r.left=10; r.top=10; r.right=100; r.bottom=100
ARRAY(int) a
if(!GetRectPixels(r a)) end "failed"

str md5
_s.fromn(a.psa.pvData a.len(1)*a.len(2)*4)
md5.encrypt(10 _s)

out md5
