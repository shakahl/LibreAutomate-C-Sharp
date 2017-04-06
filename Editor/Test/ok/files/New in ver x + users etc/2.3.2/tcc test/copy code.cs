str c=
 #define __stdcall __attribute__((stdcall))
 int __stdcall MessageBoxA(int hWnd, char* lpText, char* lpCaption, int uType);
 
 int main()
 {
 int i=4;
 goto g1;
 i=8;
 g1:
 //MessageBoxA(0, "aaaa", "bbbb", 0); //code cannot be moved if dll functions used. Probably hardcodes function address relative to code.
 return i;
 }
 void b(){}

__Tcc x
int* a=x.Compile(c "main[]b" 0 "user32")

cmove a[0] a[1]-a[0]

out call(a[0])

