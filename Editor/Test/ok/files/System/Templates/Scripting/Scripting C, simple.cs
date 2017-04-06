 compile C code below #ret
__Tcc x.Compile("" "public_function")

 call C function (example)
out call(x.f 4)

 more examples
out "<><open ''C example - C code in macro''>More C examples</open>"


 Note:
 Compiles/deletes the C code each time you run this macro.
 If need global C functions (compiled once, used in any macro), use another C template. See also macro "C example - compile once".


#ret
//C code

int private_function(int x)
{
	return x*x;
}

int public_function(int x)
{
	printf("%s: x=%i", __func__, x);
	return private_function(x)+1;
}
