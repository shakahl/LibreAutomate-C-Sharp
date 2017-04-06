__Tcc x.Compile("" "foo")
Q &q
int r=call(x.f 0)
Q &qq
outq
out r

#ret
int foo(int n)
{
int i, j;
char s[1000]={};
for(i=0; i<1000; i++)
	{
	for(j=0; j<1000; j++) s[j]+=10;
	}
return s[40];
}
