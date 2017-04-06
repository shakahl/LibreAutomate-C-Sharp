\Dialog_Editor

str controls = "3"
str qmg3x
if(!ShowDialog("qm_dialog_Tags" 0 &controls)) ret

 BEGIN DIALOG
 1 "" 0x90CE0AC8 0x0 0 0 280 130 "Tags"
 3 QM_Grid 0x5603904D 0x200 0 0 138 130 "0x62,0,0,2,0x0[]Tags,,,"
 4 SysListView32 0x5403904D 0x200 142 0 138 130 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "" "" "" ""
