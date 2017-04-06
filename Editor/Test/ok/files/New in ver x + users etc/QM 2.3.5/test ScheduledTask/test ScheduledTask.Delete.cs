 /exe 1
ScheduledTask x
out x.Delete("Macro1998")
 out x.Delete(+qmitem("Macro1998"))
 out x.Delete2("run notepad")

 BEGIN PROJECT
 main_function  test ScheduledTask.Delete
 exe_file  $my qm$\test ScheduledTask.Delete.qmm
 flags  6
 guid  {27406B7A-8729-42A4-A36E-C25F9888F867}
 END PROJECT
