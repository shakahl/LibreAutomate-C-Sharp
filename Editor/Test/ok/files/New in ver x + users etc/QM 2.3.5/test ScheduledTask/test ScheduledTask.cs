/exe 1
#compile "__ScheduledTask"
ScheduledTask x
x.Create("Macro1998")
 x.Create(+qmitem("Macro1998") 2)
 x.Create2("run notepad" "$system$\notepad.exe")
 x.Create2("run notepad" "$system$\notepad.exe" "q:\app\winapi7.txt")
 x.Create2("run notepad" "$system$\notepad.exe" "" 0 "User")

x.ScheduleOnceAfter("0:1")
 x.ScheduleOnce("7:57")
 x.ScheduleOnce("8:15" "2013-04-22")

 x.ScheduleDaily("7:59")
 x.ScheduleDaily("7:59" 2)

 x.ScheduleWeekly("18:00")
 x.ScheduleWeekly("18:00" "6 7" 2)

 x.ScheduleMonthly("18:00" 15)
 x.ScheduleMonthly("18:00" 5 "1 12")

 x.ScheduleMonthlyDOW("18:00" 1)
 x.ScheduleMonthlyDOW("18:00" 5 0 "5 6")
 x.ScheduleMonthlyDOW("18:00" 5 7)
 x.ScheduleMonthlyDOW("18:00" 4 4)

 x.SetAdvancedScheduleOptions("2013-03-22" "2013-04-22" 10 120 2)
 x.Settings(0x18f0 20 100); x.Settings(0)

x.Save

 BEGIN PROJECT
 main_function  test ScheduledTask
 exe_file  $my qm$\Macro2000.qmm
 flags  6
 guid  {EA9C01E6-E0B8-4643-8209-82209E758D59}
 END PROJECT
