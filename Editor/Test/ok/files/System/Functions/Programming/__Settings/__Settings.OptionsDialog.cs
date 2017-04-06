 \Dialog_Editor
function! [hDlg] [$title]

 Shows simple Options dialog.
 Returns: 1 - OK, 0 - Cancel.

 hDlg - owner window handle.
 title - dialog name. Default "Options".


str controls = "3"
str qmg3x
ToGridVar(qmg3x)

if(empty(title)) title="Options"
_s=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "{title}"
 3 QM_Grid 0x56035041 0x200 0 0 224 106 "0x7,0,0,0,0x0[],,,[],,,"
 1 Button 0x54030001 0x4 62 116 48 14 "OK"
 2 Button 0x54030000 0x4 112 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

#opt nowarnings 1
if(!ShowDialog(_s 0 &controls hDlg)) ret

FromGridVar(qmg3x)
ToReg; err

ret 1
