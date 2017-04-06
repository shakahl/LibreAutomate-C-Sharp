 /exe
 \Dialog_Editor

 WINAPIV.SetProcessDPIAware
 out WINAPIV.IsProcessDPIAware
str controls = "3 5"
str e3 c5Che
e3="a[]b[]c[]d[]e[]f[]g[]h[]i[]j[]k[]l[]m[]n[]"
if(!ShowDialog("Dialog131" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 122 77 "Dialog"
 4 Button 0x54032000 0x0 126 284 48 14 "Button"
 3 Edit 0x54231044 0x0 0 0 93 115 ""
 5 Button 0x54012003 0x0 95 3 23 14 "Check"
 END DIALOG
 DIALOG EDITOR: "" 0x203050D "*" "" "" ""

 BEGIN PROJECT
 main_function  Dialog131
 exe_file  $my qm$\Dialog131.exe
 icon  <default>
 manifest  
 flags  6
 guid  {9882DC62-F693-4721-BB5B-96B8FD5E12D7}
 END PROJECT
 manifest  $qm$\default.exe.manifest
