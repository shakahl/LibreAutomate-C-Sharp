 /exe
PF
rep(1000) Speed2 1 2
PN
#compile "__CSpeed"
CSpeed x
rep(1000) x.Speed2(1)
PN
rep(1000) call &Speed2 1 2
PN
PO

 165, 163, 156, 147, 139, 137
 134/128

 BEGIN PROJECT
 main_function  test UDF speed
 exe_file  $my qm$\Macro2043.qmm
 flags  6
 guid  {94087166-0028-4C4A-A5B2-56C607E0AF79}
 END PROJECT
