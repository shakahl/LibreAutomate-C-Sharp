 /exe
 PsCmd "get-date"
 PsCmd "macro:exe auto add - PsCmd"
PsCmd ""

 BEGIN PROJECT
 main_function  exe auto add - PsCmd
 exe_file  $my qm$\exe auto add - PsCmd.qmm
 flags  6
 guid  {8994B9EE-16AD-44B2-A739-D14AB0906346}
 END PROJECT
#ret
get-date
