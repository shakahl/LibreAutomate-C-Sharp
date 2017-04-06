dll "qm.exe"
	#_Test0
	#_Test1 a
	#_Test2 a b
	#_Test3 a b c
	#_Test4 a b c d

int pf(&PF) pn(&PN) p0(&_Test0) p1(&_Test1) p2(&_Test2) p3(&_Test3) p4(&_Test4)
__Tcc x.Compile("" "TestAll" 0 0 "PF[]PN[]_Test0[]_Test1[]_Test2[]_Test3[]_Test4" &pf)
str s.all(10000 2)
ARRAY(int) a.create(10000)

WakeCPU
call(x.f s.lpstr &a[0])
PO


#ret
//C code

int Test() { return 1; }
int Test1(int a) { return 1; }
int Test2(int a, int b) { return 1; }
int Test3(int a, int b, int c) { return 1; }
int Test4(int a, int b, int c, int d) { return 1; }

void TestAll(char* s, int* a)
{
int i, j=1;
for(i=0; i<10000; i++) j*=2;
PF();
//loop
for(i=0; i<10000; i++) j*=2;
PN();
//if, expression
for(i=0; i<10000; i++) { if(j) j=j && (j*2+1 > 1); }
PN();
//string iteration
for(i=0; i<10000; i++) j+=s[i];
PN();
//array iteration
for(i=0; i<10000; i++) j+=a[i];
PN();
//user function call
for(i=0; i<10000; i++)
{
	j=Test();
	j=Test1(i);
	j=Test2(i, 0);
	j=Test3(i, j, 0);
	j=Test4(i, j, 1000000, 0);
}
PN();
//system function call
for(i=0; i<10000; i++)
{
	j=_Test0();
	j=_Test1(i);
	j=_Test2(i, 0);
	j=_Test3(i, j, 0);
	j=_Test4(i, j, 1000000, 0);
}
PN();
}
