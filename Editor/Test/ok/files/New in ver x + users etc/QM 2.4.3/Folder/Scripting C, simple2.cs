 compile C code below #ret
__Tcc x.Compile("" "public_function")

 call C function (example)
out call(x.f 4 0)


#ret
//C code

int public_function(int x, int y)
{
	return x/y;
}

