 /exe 1

 Used by VS_goto. It uses mac to run this fun as .qmm. Calls CVisualStudio.GoTo(_command).

#compile "__CVisualStudio"
CVisualStudio x
x.GoTo(_command)

 BEGIN PROJECT
 main_function  CVisualStudio_GoTo
 exe_file  $my qm$\CVisualStudio_GoTo.qmm
 flags  6
 guid  {A8ADEB6B-B5F7-4398-8003-BBE2C498D8D5}
 END PROJECT
