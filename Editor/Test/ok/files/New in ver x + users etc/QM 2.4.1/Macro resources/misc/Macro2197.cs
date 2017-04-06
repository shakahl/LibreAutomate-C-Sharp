 /exe
 _s="resource:output.ico"
 #exe addfile "resource:<>test.ico" 1 RT_ICON
#exe addfile "$my qm$\copy.bmp" 1 RT_BITMAP
#exe addfile "resource:output.ico" 1 RT_GROUP_ICON
 mes "aaaaaaaaaaa" "" "q"


str controls = "3"
str sb3
 #exe addfile "resource:<>image:h75E892A1" "h75E892A1" "image"
#exe addfile "image:h75E892A1" "h75E892A1" "image"
sb3=F"resource:{`<>image:h75E892A1`}"
if(!ShowDialog("Macro2197" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x5400000E 0x0 0 0 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""


 BEGIN PROJECT
 main_function  Macro2197
 exe_file  $my qm$\Macro2197.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  7
 guid  {A8145068-0D41-4052-9D18-1095FAD18CA0}
 END PROJECT
