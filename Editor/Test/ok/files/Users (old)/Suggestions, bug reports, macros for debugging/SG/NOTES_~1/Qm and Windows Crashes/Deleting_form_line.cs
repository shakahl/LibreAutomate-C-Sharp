 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 400 400 189 26 "Form"
 5 Button 0x54032001 0x4 72 4 70 15 "Quit and &Cancel"
 4 Button 0x54032000 0x4 144 4 70 15 "Quit and &Save"
 3 Button 0x54032000 0x4 216 4 70 15 "Never &Mind"
 6 Button 0x54032000 0x4 0 4 70 15 "Quit and &Forfeit"
 END DIALOG
  y=350; xQuitandForfeit=190; xQuitandCancel=280; xQuitandSave=360; xNeverMind=450

 NOTE: deleting the "Form" line locks up the computer: mouse cursor moves but not processed,
 neither is keyboard

 Note: above note is from before October, i havent tested it with Qm 2.1.0