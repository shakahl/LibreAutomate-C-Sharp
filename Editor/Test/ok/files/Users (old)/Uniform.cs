  Uniform:  L'Ecuyer Random number generator

 Global (or thread specific) integer variables s1 and s2
 hold the current state of the generator and must be
 initialised with values in the range [1-2147483562]
 and [1-2147483398] respectively.  The generator has a
 period of ~ 2.3 x 10^18.

function^ [mn] [mx]
int z k
dll msvcrt clock
int+ s1 s2
if(!s1 || !s2)
	s1 = clock() / (1 << 16)
	s2 = clock() % (1 << 16)
	if(s1 <= 0 || s1 > 2147483562) s1 = 1
	if(s2 <= 0 || s2 > 2147483398) s2 = 2147483398

k = s1/53668
s1 = 40014*(s1-(k*53668))-(k*12211)
if(s1 < 0) s1+2147483563
k = s2 / 52774
s2 = 40692 * (s2-(k*52774))-(k*3791)
if(s2<0) s2=s2+2147483399
z = s1 - s2
if(z<1) z=z+2147483562
double random = z * 4.65661305956E-10

if(mx)
	ret (mn+ ((mx-mn+1)*random))
else ret random


 For more information, visit:
 http://xarch.tu-graz.ac.at/autocad/lisp/xlisp21gbc/sources/xlrand.c.bak
 http://www.sct.gu.edu.au/~anthony/info/C/RandomNumbers
 http://www.orthogonal.com.au/hobby/random/
 http://cgm.cs.mcgill.ca/~luc/rng.html