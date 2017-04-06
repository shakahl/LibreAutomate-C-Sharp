 /exe
 VbsExec "msgbox 1"
VbsExec ""
 VbsExec "macro:exe auto add - VbsExec"
 VbsAddCode ""; out VbsFunc("Fun" 100)
 out VbsEval("3*5")
 VbsExec "macro:Macro2048"
 _s="msgbox 4;"; VbsExec _s

 BEGIN PROJECT
 main_function  exe auto add - VbsExec
 exe_file  $my qm$\Macro2045.qmm
 flags  6
 guid  {79832A1E-DB42-4648-8F0E-CE36285F6496}
 END PROJECT
#ret
function Fun(byval a)
Fun=a*2
end function

msgbox 2
