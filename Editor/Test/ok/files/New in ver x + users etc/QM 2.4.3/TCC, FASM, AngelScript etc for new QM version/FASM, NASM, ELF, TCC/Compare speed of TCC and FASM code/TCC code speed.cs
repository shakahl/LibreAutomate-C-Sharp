out
__Tcc2 x.Compile("" "public_function")

PF
_i=call(x.f 0)
PN;PO
out _i


#ret
//C code


int testR3(int x)
{
	return x+1;
}

int public_function(int x)
{
	for(int i=100000;
		i>0;
		i--)
		x+=testR3(1);
	return x;
}
