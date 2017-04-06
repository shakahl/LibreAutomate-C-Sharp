Label :sub.Sub1
Label :sub.Sub2
Label :sub.Sub3
Label :sub.Sub4
Label :sub.Sub5
Label :sub.Sub6


#sub Sub1 m ;;Recorded 2014.05.06 07:29:10
opt save; opt slowkeys; opt slowmouse; spe 100
int w1=act(win("app - Microsoft Visual Studio" "wndclass_desked_gsk"))
lef 181 16 child("Menu Bar" "MsoCommandBar" w1) 1 ;;menu bar 'Menu Bar', menu item 'app Properties...', "~:9122FEC3"
int w2=wait(25 win("app Property Pages" "#32770"))
lef 716 8 w2 ;;push button 'Close', "~:506452C1"
opt restore


#sub Sub2 m ;;Recorded 2014.05.06 07:29:51
opt save; opt slowkeys; opt slowmouse; spe 100
int w3=act(win("app - Microsoft Visual Studio" "wndclass_desked_gsk"))
lef 800 9 child("Menu Bar" "MsoCommandBar" w3) 1 ;;menu bar 'Menu Bar', menu item 'Open File in Solution...', "~:8D9719EB"
int w4=wait(5 win("Open File in Solution [91]6 of 465]" "#32770"))
'"hhhhh" B(#7)  ;; "hhhhh" Backspace(*7)
wait 30 WT w4 "Open File in Solution [91]465 of 465]"
lef 41 9 id(2 w4) 1 ;;push button 'Cancel', "~:019EFABB"
opt restore


#sub Sub3 m
if(FileExists("dsds"))
	


#sub Sub4 m
Dir d
foreach(d "*" FE_Dir)
	str path=d.FullPath
	out path
	


#sub Sub5 m ;;Recorded 2014.05.08 08:47:02
opt save; opt slowkeys; opt slowmouse; spe 100
int w5=act(win("app - Microsoft Visual Studio" "wndclass_desked_gsk"))
lef 194 333 child("" "VsTextEditPane" w5 0x0 "" 4) 1 ;;editable text 'DocWin_Text.cpp', "~:3521A616"
lef 325 334 child("" "VsTextEditPane" w5 0x0 "" 4) 1 ;;editable text 'DocWin_Text.cpp', "~:8E35D972"
opt restore


#sub Sub6 m ;;Recorded 2014.05.08 08:48:16
opt save; opt slowkeys; opt slowmouse; spe 100
int w6=act(win("app - Microsoft Visual Studio" "wndclass_desked_gsk"))
lef 267 288 child("" "VsTextEditPane" w6 0x0 "" 4) 1 ;;editable text 'DocWin_Text.cpp', "~:64E0478A"
lef 265 425 child("" "VsTextEditPane" w6 0x0 "" 4) 1 ;;editable text 'DocWin_Text.cpp', "~:4448F3FD"
opt restore
