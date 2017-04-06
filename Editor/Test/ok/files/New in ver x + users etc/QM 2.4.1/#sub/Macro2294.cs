/exe
ExcelSheet es.Init
Excel.Worksheet x=es.ws
x._setevents("sub.x")
mes 1
 x._setevents
 mes 2

#sub x_SelectionChange
function Excel.Range'Target ;;Excel._Worksheet'x
out 1


#sub x_BeforeRightClick
function Excel.Range'Target @&Cancel ;;Excel._Worksheet'x
 function Excel.Range'Target ARRAY(int)Cancel ;;Excel._Worksheet'x
out 2

 BEGIN PROJECT
 main_function  Macro2294
 exe_file  $my qm$\Macro2294.qmm
 flags  6
 guid  {39E4F49E-77BA-47BF-ACED-DA2D2F176805}
 END PROJECT
