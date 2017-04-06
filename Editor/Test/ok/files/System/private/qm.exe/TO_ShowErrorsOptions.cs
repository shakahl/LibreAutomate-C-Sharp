\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Show RT Errors Options"
 4 Button 0x54012003 0x0 4 4 100 13 "Don't show these errors"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56035041 0x200 4 18 216 94 "0x0,0,0,2,0x10000000[],,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

str controls = "4 3"
str c4Don qmg3x

str rks="software\gindi\qm2\settings"
rget _i "DeclErrNo" rks; if(_i) c4Don=1
if !rget(qmg3x "DeclErrNoList" rks)
	qmg3x=
 ERR_INIT
 ERR_BADARG

if(!ShowDialog(dd 0 &controls _hwndqm)) ret

_i=val(c4Don); rset _i "DeclErrNo" rks
rset qmg3x "DeclErrNoList" rks
