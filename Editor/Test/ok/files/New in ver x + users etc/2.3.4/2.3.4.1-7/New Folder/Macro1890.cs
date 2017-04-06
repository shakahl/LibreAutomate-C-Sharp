 ---- Recorded 2012.12.17 11:21:03 ----
opt slowmouse 1
int w1=win("Start" "Button")
lef 25 29 w1 1 ;;push button 'Start'
int w2=wait(5 win("Start menu" "DV2ControlHost"))
3
lef 79 13 id(100 w2) 1 ;;outline 'All Programs', menu item 'ABBYY eFormFiller 2.5 v6'
int w3=wait(10 win("ABBYY eFormFiller 2.5 v6" "FormFiller2MainWindowClass"))
lef 34 11 id(106 w3) 1 ;;tool bar, push button 'Įkelti duomenis'
int w4=wait(6 win("Kraunami duomenys" "#32770"))
lef+ 402 33 child("FolderView" "SysListView32" w4 0x0 "id=1") ;;list 'Folder View', indicator 'Position'
lef- 405 66 child("FolderView" "SysListView32" w4 0x0 "id=1")
dou 65 54 child("FolderView" "SysListView32" w4 0x0 "id=1") 1 ;;list 'Folder View', list item 'Mokesčių deklaracijos'
dou 38 52 child("FolderView" "SysListView32" w4 0x0 "id=1") 1 ;;list 'Folder View', list item 'už 2010'
dou 47 35 child("FolderView" "SysListView32" w4 0x0 "id=1") 1
wait 30 WT w3 "Form_GPM308.ffdata - GPM308.mxfd - ABBYY eFormFiller 2.5 v6"
lef+ 295 232 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1
lef- 145 50 child("AUKŠTAIČIŲ G. 8-14, ROKIŠKIO M., ROKIŠKIO R. SAV[]. " "RichEdit20W" w3) 1
'"klklkllk"     ;; "klklkllk"
lef+ 572 326 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1
lef- 32 10 child("2012-12-17" "SysDateTimePick32" w3) 1
lef+ 815 333 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1
lef- 17 12 child("" "Button" w3) 1
lef+ 426 367 child("" "$FormFillerViewClass$" w3 0x0 "id=2") 1
lef- 11 12 child("" "Button" w3) 1
lef 1666 18 w3 ;;push button 'Close'
int w5=wait(80 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 58 11 id(6 w5) 1 ;;push button 'Yes'
int w6=wait(5 win("ABBYY eFormFiller 2.5 v6" "#32770"))
lef 47 10 id(7 w6) 1 ;;push button 'No'
 --------------------------------------
