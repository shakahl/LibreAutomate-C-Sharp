int Test() { return 1; }
int Test1(int a) { return 1; }
int Test2(int a, int b) { return 1; }
int Test3(int a, int b, int c) { return 1; }
int Test4(int a, int b, int c, int d) { return 1; }

void TestAll()
{
string s; s.resize(10000);
array<int> a(10000);

int i, j=1; bool b=false;
for(i=0; i<10000; i++) j*=2;
PF();
//loop
for(i=0; i<10000; i++) j*=2;
PN();
//if, expression
for(i=0; i<10000; i++) { if(j!=0) b=(j!=0) && (j*2+1 > 1); }
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
	j=_Test();
	j=_Test1(i);
	j=_Test2(i, 0);
	j=_Test3(i, j, 0);
	j=_Test4(i, j, 1000000, 0);
}
PN();
PO();
}
