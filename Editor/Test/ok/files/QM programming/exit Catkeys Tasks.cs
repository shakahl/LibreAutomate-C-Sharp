 Add this to Build Events -> Pre-build:
 "q:\my qm\exit Catkeys Tasks.exe"

int w=win("" "CatkeysTasks")
if w
	clo w
	 0.1

 BEGIN PROJECT
 main_function  exit Catkeys Tasks
 exe_file  $my qm$\exit Catkeys Tasks.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {B95DB297-9054-4FF8-9C89-1A89DC7CCCC8}
 END PROJECT
