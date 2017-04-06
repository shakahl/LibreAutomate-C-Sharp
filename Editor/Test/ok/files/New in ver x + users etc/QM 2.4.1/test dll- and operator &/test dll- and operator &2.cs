 declare a user-defined function as a dll function
dll- "" DlTest x ;;declare
&DlTest=&Function274 ;;set address
DlTest 100 ;;call

 declare a dll function
dll- "" #MessageBox2 hWnd $lpText $lpCaption uType
&MessageBox2=GetProcAddress(GetModuleHandle("user32") "MessageBoxA")
MessageBox2 0 "text" "caption" 0

 declare C functions compiled at run time
dll- ""
	test_add a b
	test_sub a b
__Tcc+ g_test_tcc
if !g_test_tcc.f
	int* p=g_test_tcc.Compile("" "add[]sub")
	&test_add=p[0]
	&test_sub=p[1]
out test_add(4 5)
out test_sub(4 5)
#ret
int add(int a, int b){return a+b;}
int sub(int a, int b){return a-b;}
