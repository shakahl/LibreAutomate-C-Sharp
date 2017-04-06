 /exe
 \Dialog_Editor

str controls = "4 5 6 8 9 10 11 12 3 7 13 14 15 16"
str si4 si5 si6 si8 si9 si10 si11 si12 sb3 sb7 sb13 sb14 sb15 sb16
si4="&:3 $my qm$\ico\32 2.ico"
si5="&:4 $system$\shell32.dll,5"
si6="&resource:<display images from macro resources - icon, cursor, jpg, png>output.ico"
si8=":6 C:\Windows\Cursors\aero_arrow.cur"
si9="C:\Windows\Cursors\aero_arrow_xl.cur"
si10=":5 C:\Windows\Cursors\aero_busy.ani"
si11="resource:aero_unavail.cur"
si12="resource:aero_busy.ani"
sb3=":1 Q:\ico and bmp\bmp\AavmGuih_132.bmp"
sb7="resource:<>test.bmp"
sb13="resource:<test images in annotations>image:h1CCB37B1"
sb14=":5 $my qm$\bitmap30x30.png"
sb15="resource:<display images from macro resources - icon, cursor, jpg, png>test.png"
 sb16="resource:~image:hE0801C30" ;;not supported
 sb16="~image:hE0801C30" ;;not supported
if(!ShowDialog("" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 263 180 "Dialog"
 4 Static 0x54000003 0x0 66 4 16 16 ""
 5 Static 0x54000003 0x0 104 4 16 16 ""
 6 Static 0x54000003 0x0 140 4 16 16 ""
 8 Static 0x54000003 0x0 66 34 16 16 ""
 9 Static 0x54000003 0x0 104 34 16 16 ""
 10 Static 0x54000003 0x0 140 34 16 16 ""
 11 Static 0x54000003 0x0 174 34 16 16 ""
 12 Static 0x54000003 0x0 204 34 16 16 ""
 3 Static 0x5400000E 0x0 0 0 16 16 "" "$my qm$\Copy.bmp"
 7 Static 0x5400000E 0x0 54 70 16 16 ""
 13 Static 0x5400000E 0x0 144 70 16 16 ""
 14 Static 0x5400000E 0x0 238 70 16 16 ""
 15 Static 0x5400000E 0x0 54 108 16 16 ""
 16 Static 0x5400000E 0x0 54 156 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""
 4 QM_DlgInfo 0x54000000 0x20000 116 0 96 48 "<><image ''resource:<test images in annotations>image:hE0801C30''>test</image>"

 BEGIN PROJECT
 main_function  dialog_image_resources_of_all_types
 exe_file  $my qm$\dialog_image_resources_of_all_types.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  23
 guid  {B25F9A9D-257E-4913-9AE8-9C75561120BB}
 END PROJECT
