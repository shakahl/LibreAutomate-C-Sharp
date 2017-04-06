dll user32 #SetParent hWndChild hWndNewParent
int+ flo
int h
if(!flo)
	h=child("" "QM_SC" _hwndqm)
	 h=id(2201 _hwndqm)
	RECT r; GetWindowRect h &r
	SetParent h 0
	SetWinStyle h WS_CHILD 2
	SetWinStyle h WS_CAPTION|WS_THICKFRAME 1|8
	mov r.left r.top h
	flo=1
else
	h=win("" "QM_SC")
	 h=win("" "QM_Output")
	SetWinStyle h WS_CAPTION|WS_THICKFRAME 2
	SetWinStyle h WS_CHILD 1|8
	SetParent h _hwndqm
	SendMessage _hwndqm WM_SIZE 0 0
	flo=0
