 create 2 files for testing
_s="int f1() { return 10; }"; _s.setfile("$temp qm$\f1.c")
_s="int f2() { return f1(); }"; _s.setfile("$temp qm$\f2.c")
 __________________________

__Tcc x.Compile(":$temp qm$\f1.c[]$temp qm$\f2.c" "f2")
out call(x.f)
