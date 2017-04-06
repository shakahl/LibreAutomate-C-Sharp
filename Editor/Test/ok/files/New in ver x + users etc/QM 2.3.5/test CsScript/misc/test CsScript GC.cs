 /exe
 out

CsScript x.Init
int i
for i 0 100000
	x.AddCode("int Add(int a, int b) { return a+b; }" 1)
	rep(10) x.Call("Add" 10 5)
	if(!i) 3

 BEGIN PROJECT
 main_function  test CsScript GC
 exe_file  $my qm$\test CsScript GC.qmm
 flags  6
 guid  {D23BC410-163E-421D-A13B-477D1CC258AC}
 END PROJECT
