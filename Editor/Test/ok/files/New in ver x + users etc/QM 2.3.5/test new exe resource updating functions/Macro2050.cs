 /exe

#exe addfile "$my qm$\test\test.txt" 102 100
#exe addfile "$my qm$\test\test.txt" 100 "WAVE"
#exe addfile "$my qm$\test\test.txt" 101 "WAVE"

 int+ g_i
  def G_I g_i
 #set g_i set_g_i
 #exe addfile "$my qm$\test\test.txt" g_i "WAVE"
out 1

 BEGIN PROJECT
 main_function  Macro2050
 exe_file  $my qm$\Macro2050.qmm
 flags  6
 guid  {A37EFDBA-BDA8-489A-BD2C-80204F7BB431}
 END PROJECT
