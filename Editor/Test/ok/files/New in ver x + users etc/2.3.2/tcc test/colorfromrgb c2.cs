str c=
 int ColorFromRGB(unsigned char r, unsigned char g, unsigned char b)
 {
 return r|(g<<8)|(b<<16);
 }

Q &q
__Tcc x.Compile(c "ColorFromRGB")
Q &qq
int j
rep 1000
	j+call(x.f 10 20 30)
Q &qqq
outq
out j


 1140 -2010566862 0 0 0 0 0
 1971210000
