/exe
 out
#compile "__Dtor"
__Handle ht=CreateThread(0 0 &sub.ThreadProc 0 0 &_i)
0.5


#sub ThreadProc
function# param
 out GetCurrentThreadId
 opt nowarnings 1
 mes 1
Dtor m
 DtorThread- t_m
 _error.description.all(100000000)
mes 1; ret
spe
 atend sub.Atend
 min 0

sub.Inner


#sub Inner
DtorInner mi

 min 0
EnumWindows &sub.EnumProc 0


#sub EnumProc
function# hwnd param
DtorWP m
outw hwnd
min 0


#sub Atend
out "atend"

 BEGIN PROJECT
 main_function  test exception in unknown thread
 exe_file  $my qm$\test exception in unknown thread.qmm
 flags  6
 guid  {951F5511-B886-4FD3-A6E2-954E29A0C79C}
 END PROJECT
