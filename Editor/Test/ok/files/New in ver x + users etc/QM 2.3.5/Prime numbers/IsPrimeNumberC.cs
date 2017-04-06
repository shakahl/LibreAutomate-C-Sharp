 /Macro1967
function! x

if(x<2) ret

 __Tcc-- c.Compile("" "IPN")
__Tcc+ c; if(!c.f) c.Compile("" "IPN")

_i=sqrt(x)+1
ret call(c.f x _i)

#ret

int IPN(int x, int si)
{
int i;
for(i=2; i<si; i++) if(!(x%i)) break;
return i==si;
}

 This is only 3.5 times faster than IsPrimeNumber. >7 times faster if calling c.f directly.
