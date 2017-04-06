 /exe 4

__SharedMemory m.Create("test" 0x10000)
 __SharedMemory m.Open("test")
int* p=m.mem
out p[0]

 BEGIN PROJECT
 main_function  Macro2758
 exe_file  $my qm$\Macro2758.qmm
 flags  6
 guid  {962479E7-9F82-479C-9AC7-7F8BB9FB0925}
 END PROJECT
