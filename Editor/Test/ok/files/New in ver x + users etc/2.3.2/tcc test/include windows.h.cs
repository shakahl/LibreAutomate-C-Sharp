str c=
 #include <windows.h>
 void main()
 {
 MessageBox(0, "aaaa", "bbbb", 0);
 MessageBox(0, "aaaa", "bbbb", 0);
 }

Q &q
__Tcc x.Compile(c "main" 0 "user32")
Q &qq
outq
call x.f
