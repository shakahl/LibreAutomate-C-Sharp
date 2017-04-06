out
 interface# IntRef :IUnknown Test(int&x)
 interface# IntRef :IUnknown Test(int*x)
 IntRef k.Test(_i)

dll "qm.exe" TestRef int*x

int x=5
int* p=&x
out TestRef(&x)
out TestRef(x)
 out TestRef(+x) ;;exception (old and new)
 x=&x; out TestRef(+x)
out TestRef(p)
out TestRef(*p)
 out TestRef(0) ;;exception
 out TestRef(10)
long k=7
out TestRef(+&k)
 out TestRef(+k) ;;exception
 Function276 x
 Function276 &x

RECT r; GetWindowRect _hwndqm r
zRECT r
