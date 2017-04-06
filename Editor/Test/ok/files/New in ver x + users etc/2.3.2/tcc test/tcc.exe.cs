str c=
 #include <windows.h>
 void main()
 {
 //OutputDebugString("aaa");
 MessageBox(0, "aaaa", "bbbb", 0);
 int i;
 void* v=i;
 }

str tf.expandpath("$temp$\c.c")
c.setfile(tf)
str cl.format("q:\app\tcc\tccexe\lib\user32.def -run %s" tf)
 out cl

RunConsole "$qm$\tcc\tccexe\tcc.exe" cl
