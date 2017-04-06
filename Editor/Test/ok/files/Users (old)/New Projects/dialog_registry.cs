\Dialog_Editor

type DLGVAR ~controls ~e3 ~c4Che
DLGVAR d.controls="3 4"

if(rget(_s "DLGVAR" "\test")) _s.setstruct(d)

if(!ShowDialog("" 0 &d)) ret

_s.getstruct(d); rset(_s "DLGVAR" "\test")

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 6 6 96 14 ""
 4 Button 0x54012003 0x0 6 26 48 12 "Check"
 END DIALOG
 DIALOG EDITOR: "TYO" 0x2020103 "" "" ""
