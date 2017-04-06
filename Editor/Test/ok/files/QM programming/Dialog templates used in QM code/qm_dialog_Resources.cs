 \Dialog_Editor

str controls = "3"
str qmg3x
if(!ShowDialog("qm_dialog_Tags" 0 &controls)) ret

 BEGIN DIALOG
 1 "" 0x90CE0AC8 0x0 0 0 336 220 "Resources"
 3 SysListView32 0x5403104D 0x200 0 0 138 220 ""
 4 QM_DlgInfo 0x54010005 0x20000 142 0 194 200 ""
 5 Static 0x54000000 0x0 144 206 20 10 "Filter"
 6 ComboBox 0x54230242 0x0 166 204 134 213 ""
 7 Button 0x54032000 0x0 318 204 18 14 "?"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "" "" "" ""
