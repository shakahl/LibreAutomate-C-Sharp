 /exe
 \Dialog_Editor
sub.Test


#sub Test
typelib MSForms {0D452EE1-E08F-101A-852E-02608C4D0BB4} 2.0

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 96 48 "SHDocVw.WebBrowser"
 4 QM_Grid 0x56031041 0x200 0 52 96 48 "0[]A[]B"
 5 ActiveX 0x54030000 0x0 108 0 96 48 "MSForms.OptionButton"
 6 ActiveX 0x54030000 0x0 108 52 96 13 "MSDataListLib.DataCombo {F0D2F21C-CCB0-11D0-A316-00AA00688B10} data:F822915ABC6829D3BED0DC96B231561A79D32EF7C2AE5D6EADA54B8A236FBEAD53106463C252301304"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040302 "*" "" "" ""
 4 QM_Grid 0x56031041 0x200 0 52 96 48 "0[]A[]B"
 5 ActiveX 0x54030000 0x0 108 0 96 48 "MSForms.OptionButton {8BD21D50-EC42-11CE-9E0D-00AA006002F3}"

str controls = "3 4"
str ax3SHD qmg4
if(!ShowDialog(dd 0 &controls)) ret

 BEGIN PROJECT
 main_function  Dialog191
 exe_file  $my qm$\Dialog191.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  7
 guid  {348445FB-D847-4B57-B2E0-7752EB6237F3}
 END PROJECT

 ENC88;;;;
