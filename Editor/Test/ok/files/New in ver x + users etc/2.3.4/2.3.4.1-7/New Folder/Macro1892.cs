 ---- Recorded 2012.12.17 11:32:50 ----
opt slowkeys 1; opt slowmouse 1; spe 100
int w1=win("Start" "Button")
lef 30 28 w1 1 ;;push button 'Start'
int w2=wait(5 win("Start menu" "DV2ControlHost"))
lef 78 19 w2 1 ;;outline 'All Programs', menu item 'ABBYY eFormFiller 2.5 v6'
int w3=wait(6 win("ABBYY eFormFiller 2.5 v6" "FormFiller2MainWindowClass"))
lef 20 -14 w3 1 ;;menu item 'Byla'
lef 38 7 wait(5 WV win("" "#32768")) 1 ;;menu item 'Atidaryti formą ...'
 men 57601 w3 ;;Atidaryti formą ...
int w4=wait(6 win("Atidaryti..." "#32770"))
lef 385 219 w4 1 ;;push button 'Cancel'
act w3
lef 27 -8 w3 1 ;;menu item 'Byla'
lef 49 84 wait(5 WV win("" "#32768")) 1 ;;menu item 'Įkelti duomenis...'
 men 40202 w3 ;;Įkelti duomenis...
int w5=wait(5 win("Kraunami duomenys" "#32770"))
dou 114 67 w5 1
wait 30 WT w3 "Form_GPM308.ffdata - GPM308.mxfd - ABBYY eFormFiller 2.5 v6"
lef 323 -10 w3 1 ;;menu item 'Žinynas'
lef 38 7 wait(5 WV win("" "#32768")) 1 ;;menu item 'Žinynas'
 men 40004 w3 ;;Žinynas
int w6=wait(6 win("Help" "HH Parent"))
lef 109 203 w6 1 ;;outline, outline item 'Programos parinktys'
lef 520 11 w6 ;;push button 'Close'
act w3
lef 1668 16 w3 ;;push button 'Close'
int w7=wait(5 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 232 136 w7 1 ;;push button 'Yes'
int w8=wait(5 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 220 108 w8 1 ;;push button 'No'
opt slowkeys 0; opt slowmouse 0; spe -1
 --------------------------------------
