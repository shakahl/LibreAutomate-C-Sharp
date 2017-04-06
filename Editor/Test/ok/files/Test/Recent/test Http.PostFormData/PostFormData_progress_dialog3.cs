function# action nbAll nbSent $_file nbAllF nbSentF PFD_CALLBACK2&c

if(!IsWindow(c.hwnd)) ret 1

def PBM_SETPOS (WM_USER+2)
lpstr st
sel action
	case 0 st="Sending data..."
	case 1
	st=_file
	SendMessage id(4 c.hwnd) PBM_SETPOS nbSentF*100L/nbAllF 0 ;;file
	SendMessage id(3 c.hwnd) PBM_SETPOS c.nbSent+nbSentF*100L/c.nbTotal 0 ;;total
	if(nbSentF=nbAllF) c.nbSent+nbAllF
	case 2 st="All data sent"
	case 3 st="Receiving response..."
	case 4 st="Finished"
SetDlgItemText c.hwnd 7 st
0
