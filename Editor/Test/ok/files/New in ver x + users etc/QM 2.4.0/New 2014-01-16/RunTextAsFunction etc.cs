 out RunTextAsFunction("1; out 10; ret 100")
 out RunTextAsFunction("function# x y[]1; out x+y; ret x+y" 1 2)
out RunTextAsFunction2("function# x[]1; out x; ret x" 5)
 out RunTextAsFunction2("int i=5; out i; ret 7") ;;incorrect, must be 1 param, but works

 RunTextAsMacro("mes 1")
 _s="mes 2"; _s.setfile("$temp$\test.txt")
 RunFileAsMacro("$temp$\test.txt")

 int i
 for i 0 3
	 RunTextAsFunction F"mes {i}"
