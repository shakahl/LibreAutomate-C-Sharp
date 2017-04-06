 /exe
 out
 lpstr macro="Macro2048"
 macro=+1
lpstr macro="Function244"
macro=+2

 #exe addtextof "Macro2048"
 out _s.getmacro(macro)

 #if EXE
 out qmitem(macro)
 #else
 QMITEM q
 int iid=qmitem(macro 1 q 8)
 out iid
 out q.text

#exe addfunction "Function244"
mac macro
1

 BEGIN PROJECT
 main_function  Macro2048
 exe_file  $my qm$\Macro2048.qmm
 flags  6
 guid  {58AF20D5-DBFD-4789-AF69-F7DEFE0D5059}
 END PROJECT
