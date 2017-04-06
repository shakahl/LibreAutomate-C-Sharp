 /exe
rep
	 __HFile f.Create("q:\my qm\Macro2052.exe" OPEN_EXISTING)
	__HFile f.Create("q:\my qm\Macro2052.exe" OPEN_EXISTING GENERIC_READ)
	0.001
	f.Close
	0.001

 BEGIN PROJECT
 main_function  Function247
 exe_file  $my qm$\Function247.qmm
 flags  6
 guid  {19E3B133-33C9-4920-8B1E-CE4C1457BF4C}
 END PROJECT
