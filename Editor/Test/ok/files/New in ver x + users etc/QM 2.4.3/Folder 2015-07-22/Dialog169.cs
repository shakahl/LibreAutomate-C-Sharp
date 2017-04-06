\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_ComboBox 0x54230243 0x0 8 8 96 213 ""
 4 QM_ComboBox 0x54230242 0x0 8 28 96 213 ""
 5 QM_ComboBox 0x54230243 0x0 8 48 96 213 ""
 6 QM_ComboBox 0x54230243 0x0 8 68 96 213 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4 5 6"
str qmcb3 qmcb4 qmcb5 qmcb6
 Simplest. Adds three items and selects Mercury (index 0).
qmcb3="0[]Mercury[]Venus[]Earth"
 With icons. Adds three items, selects Mercury, and in the drop-down list displays the first three images from the imagelist file.
qmcb4="0,$qm$\il_qm.bmp[]Mercury,0[]Venus,1[]Earth,2"
 With check boxes. Adds three items, checks Mercury (its item flags contain 1), and sets control text to "Mercury".
qmcb5=",,1[]Mercury,,1[]Venus,,0[]Earth"
 With check boxes, using a 32-bit number to check items. Adds three items, checks Mercury and Earth (first and third bits in 0x5 are 1), and sets control text to "Mercury, Earth".
qmcb6=
 0x5,,3
 Mercury
 Venus
 Earth

if(!ShowDialog(dd 0 &controls)) ret

out qmcb3
out qmcb4
out qmcb5
out qmcb6
