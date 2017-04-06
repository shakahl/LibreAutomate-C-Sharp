/exe 4
out _s.expandpath("$temp qm$")
_s.setfile("$temp qm$\test.txt")
 _s.setfile("$temp$\Low\test.txt")
WshExec "Wsh.Echo 3"

 BEGIN PROJECT
 main_function  Macro2087
 exe_file  $my qm$\Macro2087.qmm
 flags  6
 guid  {70133FCB-B216-4973-9615-202F1848C7B9}
 END PROJECT
