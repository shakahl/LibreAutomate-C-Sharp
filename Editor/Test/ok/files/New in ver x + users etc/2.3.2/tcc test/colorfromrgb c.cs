str c=
 int main()
 {
 int i, j=0;
 for(i=0; i<1000; i++)
 {
 	j+=ColorFromRGB(10, 20, 30);
 }
 return j;
 }
 
 int ColorFromRGB(unsigned char r, unsigned char g, unsigned char b)
 {
 return r|(g<<8)|(b<<16);
 }

Q &q
__Tcc x.Compile(c "main")
Q &qq
int j=call(x.f)
Q &qqq
outq
out j


 1140 -2010566862 0 0 0 0 0
 1971210000
