int i
int h=win("window name" "window class")
i=0
rep
	wait 0 C 0xFFFFFF 317 468 h ;;wait for white
	i+1
	 out i
	if(i=50) break
	wait 0 -C 0xFFFFFF 317 468 h ;;wait for other color

 here add command to click button
