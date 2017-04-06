 /exe 1
 Displays monikers of all active COM objects in ROT table.

out
IDispatch d._getactive(0 16 "."); err

 BEGIN PROJECT
 main_function  VS_EnumROT
 exe_file  $my qm$\VS_EnumROT.qmm
 flags  6
 guid  {E53610D5-6A9F-4889-923C-BD429497CD67}
 END PROJECT
