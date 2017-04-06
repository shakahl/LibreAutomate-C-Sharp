function# hdlg sizeAll sizeSent fa fparam str*sp

if(hdlg)
	str s1 s2
	if(!IsWindow(hdlg)) lasterror="cancel"; ret
	SendDlgItemMessage hdlg 7 PBM_SETPOS iif(sizeAll>0 MulDiv(sizeSent 100 sizeAll) 0) 0
	if(sizeAll>=0) s1=sizeAll/1024; else s1="???"
	s2.format("%i KB of %s KB" sizeSent/1024 s1)
	SetDlgItemText(hdlg 4 s2)

if(fa and call(fa sizeAll sizeSent sp fparam)) lasterror="cancel"; ret

ret 1
