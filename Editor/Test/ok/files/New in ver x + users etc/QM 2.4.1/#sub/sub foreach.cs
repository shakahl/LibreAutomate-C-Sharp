 /exe
int i
foreach i 0 sub.FE_Sub
	out i

#sub FE_Sub
function# int&r k
if(r>4) ret
r+1
ret 1

 BEGIN PROJECT
 main_function  sub foreach
 exe_file  $my qm$\sub foreach.qmm
 flags  6
 guid  {611130AF-FF79-47AC-A7F4-A34A4F9DE989}
 END PROJECT
