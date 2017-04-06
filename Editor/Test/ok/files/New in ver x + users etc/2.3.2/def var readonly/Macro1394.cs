/exe
out
out _hwndqm
outx _hinst
outx _winver
outx _winnt
outx _iever
out _win64
out _unicode
out _qmdir
out _qmver_str
out _logfile
out _logfilesize
out _dialogicon
out "%i %i" _hfont GetStockObject(DEFAULT_GUI_FONT)

 BEGIN PROJECT
 main_function  Macro1394
 exe_file  $my qm$\Macro1394.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {52E1DD54-FC1D-4437-9E60-018291B23339}
 END PROJECT
