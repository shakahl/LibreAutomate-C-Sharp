/exe
#compile "__ScheduledTask"
str macro="Macro1998"
ScheduledTask x
PF
x.Create2("run notepad" "$system$\notepad.exe" "" 0 "User")
PN
x.ScheduleOnceAfter("0:1")
PN
x.Save
PN
PO

 BEGIN PROJECT
 main_function  Macro2000
 exe_file  $my qm$\Macro2000.qmm
 flags  6
 guid  {EA9C01E6-E0B8-4643-8209-82209E758D59}
 END PROJECT
