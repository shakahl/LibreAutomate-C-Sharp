 /exe
rep
	1
	lock macro2435 "macro2435_mutex" 0
	err out "Macro2435 is running"; continue
	lock- macro2435
	out "Macro2435 is NOT running"

 BEGIN PROJECT
 main_function  Function_exe
 exe_file  $my qm$\Function_exe.qmm
 flags  6
 guid  {F6C8F55D-872D-4CA4-9B8A-5326241B689D}
 END PROJECT
