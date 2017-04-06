/exe
#compile __Dtor
 atend sub.Atend
 min 0
int k=4096*10
 if(SetThreadStackGuarantee(&k)) out k; else out _s.dllerror
 k=0
 if(SetThreadStackGuarantee(&k)) out k; else out _s.dllerror
 ret
sub.Test


#sub Test
 opt err
 opt noerrorshere
Dtor d

 out 1
 _i/0
dll "qm.exe" StackOverflow i
 rep 2
	 StackOverflow 1
	 err out _error.description
sub.SO(1)
out 2


#sub SO
function k
RECT q w e r t y u i o p a s d;; f g h j l z x c v b n m
 out k
 type TBIG !b[50000]
 TBIG v
sub.SO(k+1)

 BEGIN PROJECT
 main_function  stack overflow
 exe_file  $my qm$\stack overflow.qmm
 flags  6
 guid  {F468D896-9DA7-45CD-B645-93CEFC07E105}
 END PROJECT


#sub Atend
out _s.getstruct(_error)
