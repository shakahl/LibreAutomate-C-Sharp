 mes 11
 MessageBox 0 "ff" "11" MB_TOPMOST
 111
 rep(1000000000) if(1) sel(_i) case 5: spe

 5
 out 1
 2

MessageLoop

 MSG m
 rep
	 if(GetMessage(&m 0 0 0)<1 or m.message=2000)
		 ret m.wParam
	 TranslateMessage &m
	 DispatchMessage &m

out 1
