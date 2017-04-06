 /exe

dll "q:/app/qmgrid.dll"
	@QmGridRegisterClass
	QmGridSetCellStyleMulti hlv style row ifrom ito

#if EXE
QmGridRegisterClass
#endif

dlg_QM_Grid 0 0 0 0

 BEGIN PROJECT
 main_function  Macro1276
 exe_file  $my qm$\Macro1276.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {3C5BD1B2-6C58-4208-83CF-4D779ECFFF33}
 END PROJECT
