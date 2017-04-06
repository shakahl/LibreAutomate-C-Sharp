 /
function ARRAY(int)&a

int n
sel a.ndim
	case 1 n=a.len
	case 2 n=a.len(1)*a.len(2)
	case else end ERR_BADARG

lpstr c=
 void main(unsigned int* a, int n)
 {
 unsigned int* a2=a+n;
 while(a<a2) *a++=((*a&0xff)<<16)|(*a&0xff00)|((*a&0xff0000)>>16);
 }

__Tcc+ __caab_c
if(!__caab_c.f) __caab_c.Compile(c "main")
call __caab_c.f a.psa.pvData n
