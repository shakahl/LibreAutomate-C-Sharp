 Possibly unstable.
 When dragging QM window, cursor blinks.
 Once keyboard and mouse stopped working completely.

 change these values
int brightness=90 ;;brightness threshold, %. Can be 1-100.
int x(100) y(100) ;;pixel coordinates
str macro="pb_macro" ;;macro to run when brightness of the pixel becomes >= the threshold

 to start the trigger, run this function
 it adds tray icon 'lightning'
 to stop previously started trigger, run this function again or Ctrl+click the tray icon

 -------------------------------------

if(getopt(nthreads)>1)
	shutdown -6 0 "PixelBrightnessTrigger"
	ret

AddTrayIcon "lightning.ico"
int plum
rep
	0.01
	 x=xm; y=ym ;;gets mouse coordinates. Can use this for testing
	int c=pixel(x y)
	word hue lum sat
	ColorRGBToHLS c &hue &lum &sat
	lum=lum*100/240
	 out lum ;;shows pixel brightness, %. Can use this for testing
	if(lum>=brightness and plum<brightness) mac macro
	plum=lum
