 compile C code below #ret
__Tcc x.Compile("" "public_function")

 call C function (example)
out call(x.f 4)


#ret
//C code

int private_function(int x)
{
	return x*x;
}

int public_function(int x)
{
int k=5;
asm __volatile__(
"movl %1, %%eax;"
"movl %%eax, %0;"
:"=r"(x)        /* output */
:"r"(k)         /* input */
//:"%eax"         /* clobbered register */
);

	printf("%s: x=%i", __func__, x);
	return private_function(x)+1;
}
