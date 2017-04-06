 ---- Recorded 2012.12.17 11:29:41 ----
opt slowkeys 1; opt slowmouse 1; spe 100
double F=1.0 ;;speed
int w1=win("Start" "Button")
lef 35 25 w1 1; 2.50*F ;;push button 'Start'
int w2=wait(5 win("Start menu" "DV2ControlHost"))
lef 66 13 id(100 w2) 1; 1.94*F ;;outline 'All Programs', menu item 'ABBYY eFormFiller 2.5 v6'
int w3=wait(5 win("ABBYY eFormFiller 2.5 v6" "FormFiller2MainWindowClass"))
lef 35 11 id(106 w3) 1; 5.59*F ;;tool bar, push button 'Įkelti duomenis'
int w4=wait(5 win("Kraunami duomenys" "#32770"))
dou 62 36 child("FolderView" "SysListView32" w4 0x0 "id=1") 1; 10.53*F
wait 30 WT w3 "Form_GPM308.ffdata - GPM308.mxfd - ABBYY eFormFiller 2.5 v6"
lef+ 630 266 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1; 0.14*F
lef- 71 15 child("" "RichEdit20W" w3) 1; 1.67*F
'"sdfsdfs"; 3.24*F  ;; "sdfsdfs"
lef+ 932 368 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1; 0.12*F
lef- 13 13 child("" "Button" w3) 1; 1.71*F
lef 1671 18 w3; 5.32*F ;;push button 'Close'
int w5=wait(20 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 48 14 id(6 w5) 1; 2.50*F ;;push button 'Yes'
int w6=wait(7 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 45 12 id(7 w6) 1 ;;push button 'No'
opt slowkeys 0; opt slowmouse 0; spe -1
 --------------------------------------
