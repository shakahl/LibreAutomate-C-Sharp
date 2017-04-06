lpstr c=
 int add(int a, int b)
 {
 return a+b;
 }

Q &q
__Tcc x.Compile(c "add")
Q &qq
outq
out call(x.f 1 2)


 __Tcc+ g_tcctest
 if(!g_tcctest.IsCompiled)