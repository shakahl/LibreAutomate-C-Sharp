 /exe
 JsExec "print(''k'');"
 JsExec ""
 JsExec "macro:exe auto add - JsExec"
JsAddCode ""; out JsFunc("Fun" 100)
 out JsEval("3*5")

 BEGIN PROJECT
 main_function  exe auto add - JsExec
 exe_file  $my qm$\exe auto add - JsExec.qmm
 flags  6
 guid  {4368207A-5314-4261-89C5-FA5F497FF248}
 END PROJECT
#ret

function Fun(a)
{
return a*2;
}
