 /exe

#exe addfunction "func_one"
#exe addfunction "func_two"

#opt nowarnings 1

str functions=
 one
 two

str s
foreach s functions
	mac F"func_{s}"

MES m.x=500
mes "mac1[][]Click OK to end this macro and all function threads and this process." "" m

 BEGIN PROJECT
 main_function  mac1
 exe_file  $my qm$\mac1.exe
 END PROJECT
