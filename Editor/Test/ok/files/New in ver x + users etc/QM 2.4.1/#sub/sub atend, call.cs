/exe
atend sub.At
out call(&sub.Ca 5)
out 1

#sub At
out 2

#sub Ca
function a
out 3
ret a*2

 BEGIN PROJECT
 main_function  sub atend, call
 exe_file  $my qm$\sub atend, call.qmm
 flags  6
 guid  {E80C4E56-E4BC-4D80-ACA0-8D66F5CE3E1F}
 END PROJECT
