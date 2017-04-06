/exe
out
 ExeQmGridDll

 __ImageList il.Load("resource:<dialog_resource_imagelist>il_de.bmp")
 out il
 #ret

 #exe addfile "resource:<dialog_resource_imagelist>il_de.bmp" 5
 #exe addfile "$qm$\il_qm.bmp" 5
 lpstr o="resource:<dialog_resource_imagelist>il_de.bmp"
 lpstr o=":4 $qm$\paste.ico[]resource:<dialog_resource_imagelist>il_de.bmp"
lpstr o=":5 $qm$\il_qm.bmp"
 lpstr o=":4 $qm$\paste.ico[]:5 $qm$\il_qm.bmp"

str s=
F
 ,"{o}",0x10,,,,,,-100
 one,1
 two,2
 three,0

 rep(200) s.addline(_s.RandomString(5 20 "a-z") 1);; qmcb3+",4"

 ICsv x._create
 x.FromString(s)

int R i
R=ShowDropdownListSimple(s i 0 1)
 R=ShowDropdownList(x i 0 1)
out "0x%X %i" R i

 IQmDropdown dd
 outx ShowDropdownList(x 0 0 0 0 0 0 0 dd)
 out dd

 BEGIN PROJECT
 main_function  test IQmDropdown
 exe_file  $my qm$\test IQmDropdown.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  22
 guid  {C57B7FD6-A86C-47BA-B6F8-556C8292A47C}
 END PROJECT
