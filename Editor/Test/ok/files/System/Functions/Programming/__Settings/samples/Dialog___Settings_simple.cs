\Dialog_Editor

 Shows how to use __Settings.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x200 0 0 224 112 "0x7,0,0,0,0x0[]Name,,,[]Value,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

str controls = "3"
str qmg3x

str settingsCSV=
 one,Onee,1
 two,Twoo,2
 three,Threee,3
 four,Fourr,4
__Settings x.Init(settingsCSV "__Settings_simple" "\test")
x.FromReg
x.ToGridVar(qmg3x)

out "two before: %s" x.GetStr("two")

if(!ShowDialog(dd 0 &controls)) ret

x.FromGridVar(qmg3x)
x.ToReg()

out "two after: %s" x.GetStr("two")
