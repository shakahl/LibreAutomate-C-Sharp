 /exe
mes 1 "" "q"
 int w=win("Document" "Afx:*" "" 0x8)
 scan ":10 Macro2212.bmp" id(27000 w) 0 1|2|16 ;;push button 'Show Code view (Press Ctrl ...'

 #exe addfile "" 
 __GdiHandle g
 CaptureImageOrColor b 0
 CaptureImageOrColor _i 1
 outx _i

 BEGIN PROJECT
 main_function  Macro2212
 exe_file  $my qm$\Macro2212.exe
 icon  $my qm$\shell32Vista.dll,24
 manifest  $qm$\default.exe.manifest
 version  3.7.8
 version_csv  ProductName,test[]ProductVersion,5.6
 flags  22
 guid  {9942B0A1-ECC7-4FDB-B154-572805B6596C}
 END PROJECT
