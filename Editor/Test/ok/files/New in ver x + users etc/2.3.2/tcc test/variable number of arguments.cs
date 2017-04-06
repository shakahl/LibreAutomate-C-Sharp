str c=
 int main(int nArg, ...)
 {
 int r=0;
 int* p=&nArg+1;
 while(nArg--) r+=*p++;
 return r;
 }

__Tcc x.Compile(c "main")
out call(x.f 3 1 2 3)
 out call(x.f 30 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1)
