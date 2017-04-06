run "notepad"
1
key "aaa"
clo "Notepad"

 beginning
 ...

int errorMessage=wait(2 WA win("Error Message" "#32770"))
err errorMessage=0

if errorMessage
	out 1
	key An ;;press Alt+N to close
	key "*"
	goto beginning
else
	out 0
