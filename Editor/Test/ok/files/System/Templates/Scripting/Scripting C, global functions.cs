 Compiles C code below #ret and creates functions that can be called in any macro as easily/safely as dll functions.

 declare functions
dll- ""
	test_add a b
	test_sub a b

 compile C code and bind function addresses to the declarations
int compileOnce;;=32
__Tcc+ g_tcc_987 ;;this global variable contains the compiled code. Give it an unique name, eg g_tcc_MyStringFunctions1.
if !g_tcc_987.f or !compileOnce
	int* p=g_tcc_987.Compile("" "add[]sub" 0|compileOnce)
	&test_add=p[0]
	&test_sub=p[1]

 _____________________________________

 Then the C functions can be called in any macro like dll functions. Example:

out test_add(4 5)
out test_sub(4 5)

 more examples
out "<><open ''C example - C code in macro''>More C examples</open>"


#ret
//C code

int add(int a, int b)
{
	printf("%s: a=%i, b=%i", __func__, a, b);
	return a+b;
}

int sub(int a, int b)
{
	return a-b;
}
