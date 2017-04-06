int Test(int a, int b)
{
int i;
for(i=0; i<1000; i++) { a=a+1; }
return a+b;
}

int Test2()
{
//TestAdd(0, 8);
int i, j=0;
//if((i&1)!=0)
//Print(""+i);
//Print("""
//stri\ng
//""");
for(i=0; i<1000; i++) j+=Test(i, i+8);
//for(i=0; i<100000; i++) j=TestAdd2(i, 8);

//string s; s.resize(100000);
//array<int8> s(100000);
//for(i=0; i<100000; i++) if(s[i]==65) j++;
//string s="abc";
//for(i=0; i<100000; i++) if(s.length==3) j++;

//array<int8> a(1), b;
//b=a;
//a[0]=6;
//Print(""+b[0]);

//string s="test";
//uint u=s;
//char_ptr p=s;
//Print(s+" "+u+" "+p);
//Print(" "+p);
//TestAsLpstr(s);
//TestStr();
//TestAsLpstr("connnn");
//TestStr("connn");
//j=VK_NEXT;

//for(int i=0, j=7; i<5; i++, j--){}
//int i, j; for(i=0, j=7; i<5; i++, j--){}

//switch(1) { case 1: Print("1"); case 2: Print("2"); }
//bool b=8;
//j=5.6;

//{ Moo k; Print(""+k.m); }
//TestAdd2(1, 2);
//mbox("test");

//TODO: test big code compilation speed. Compare with TCC. Test with JIT too.

return j;
}

int TestAdd2(int a, int b)
{
//Moo k;
	//zz(a);
	return a+b;
}

void TestStr(string&out s=void)
//void TestStr(const string&in s)
//void TestStr(const string& s)
{
//string k="lkjflkjskjfhjkahdfkhadkfhsdkhfjksdhfjkdshfjk";
//s.resize(10000000);
//char_ptr p=s;
//Print(" "+p);
//Print(s+" "+p);
Print(s);
//TestAsLpstr(s);
}

//test if(&param)

class Moo
{
Moo() { Print("ctor"); }
~Moo() { Print("dtor"); }
//private
int m;
}
