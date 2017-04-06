 This macro shows how to call C functions like dll functions, not with call().

 declare functions with dll-
dll- ""
	test_add a b
	test_sub a b

 compile C code and bind function addresses to the declarations
int compileOnce=32 ;;remove =32 if want to recompile always, eg when editing C code
__Tcc+ g_test_472
if !g_test_472.f or !compileOnce
	int* p=g_test_472.Compile("" "add[]sub" 0|compileOnce)
	&test_add=p[0]
	&test_sub=p[1]

 call functions
out test_add(4 5)
out test_sub(4 5)

#ret

int add(int a, int b){return a+b;}
int sub(int a, int b){return a-b;}
