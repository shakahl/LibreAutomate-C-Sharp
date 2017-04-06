 close Selenium IDE
int w
w=win("Selenium IDE" "Mozilla*WindowClass" "" 0x4)
if(w)
	act w
	 select wrong formatter
	key A{ocR} Y
	
	clo w
	int w1=wait(1 win("Save?" "MozillaDialogClass")); err
	if(w1) key An
 run Selenium IDE
w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x4)
act w
Acc a.Find(w "PUSHBUTTON" "Selenium IDE" "a:id=selenium-ide-button" 0x1005)
a.DoDefaultAction
 record something
lef 544 144 w 1 ;;link 'Download'
wait 30 WT w "Quick Macros - Download - Mozilla Firefox"
lef 366 138 w 1 ;;link 'Home'
mou
 w=win("Selenium IDE" "Mozilla*WindowClass" "" 0x4)
 act w
