function# action nbAll nbRead $_file nbAllF nbReadF PFD_CALLBACK&d
 out "%i %i %s %i %i" nbAll nbRead _file nbAllF nbReadF

if(!IsWindow(d.hwnd)) ret 1

def PBM_SETPOS (WM_USER+2)
SendMessage id(3 d.hwnd) PBM_SETPOS nbRead*100L/nbAll 0
lpstr st
sel action
	case 0 st="Sending data..."
	case 1
	st=_file
	SendMessage id(4 d.hwnd) PBM_SETPOS nbReadF*100L/nbAllF 0
	if(nbReadF=nbAllF) d.nbFilesTotal+nbAllF
	case 2 st="All data sent"
	case 3 st="Receiving response..."
	case 4 st="Finished"
SetDlgItemText d.hwnd 7 st
0
