int hwnd=val(_command) ;;Open dialog handle
 mov 10000 0 hwnd ;;hide the Open dialog. enable this line if works well

 g1
if(!inp(_s)) ;;i use inp for simplicity. replace it with your dialog code (ShowDialog etc)
	 Cancel
	clo hwnd
	ret

_s.setwintext(id(1148 hwnd)) ;;set text of 'File name:' edit box
but 1 hwnd; err ret ;;press OK

 wait until closed, because sometimes OK does not close it
wait 2 -WC hwnd
err ;;stil exists after 2 s
	int w2=win("Open" "#32770" "" 64 hwnd 0) ;;error message box?
	if(w2)
		act w2
		wait 0 -WC w2 ;;wait until closed and retry
		goto g1 ;;retry
	mes "something wrong"
	clo hwnd
