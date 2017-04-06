\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_ComboBox 0x54030242 0x0 8 8 96 13 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3"
str qmcb3
 qmcb3="0,,0x40[]one[]twwwwwwwwwwwwwwwwwwwwwwwo twwwwwwwwwwwwwwwwwwwwwwwotw wwwwwwwwwwwwwwwwwwwwwwotwwww wwwwwwwwwwwwwwwwwwwo[]three"
qmcb3="0,$qm$\il_qm.bmp,0x01,,,,,7[]one,1[]twwwwwwwwwwwwwwwwwwwwwwwo[]three[]four[]five[]six[]seven,3,16"
 rep(50) qmcb3.addline(_s.RandomString(7 17 "a-z") 1)
 ShowDropdownListSimple(qmcb3)
if(!ShowDialog(dd 0 &controls _hwndqm)) ret
