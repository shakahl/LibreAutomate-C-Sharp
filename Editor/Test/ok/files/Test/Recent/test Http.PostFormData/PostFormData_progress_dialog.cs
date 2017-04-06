function# action nbAll nbRead $_file nbAllF nbReadF fparam
 out "%i %i %s %i %i %i" nbAll nbRead _file nbAllF nbReadF fparam

if(!IsWindow(fparam)) ret 1

def PBM_SETPOS (WM_USER+2)
SendMessage id(3 fparam) PBM_SETPOS nbRead*100L/nbAll 0
lpstr st
sel action
	case 0 st="Sending data..."
	case 1
	st=_file
	SendMessage id(4 fparam) PBM_SETPOS nbReadF*100L/nbAllF 0
	case 2 st="All data sent"
	case 3 st="Receiving response..."
	case 4 st="Finished"
SetDlgItemText fparam 7 st
0
