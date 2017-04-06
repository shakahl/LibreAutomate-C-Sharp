\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
if(!ShowDialog("Dialog46" &Dialog46 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x540330C4 0x200 10 10 96 48 ""
 4 ScrollBar 0x54030001 0x0 118 10 14 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x203000D "*" "" ""

ret
 messages
OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	SetScrollRange(id(4 hDlg) SB_CTL 0 100 0)
	int h=id(3 hDlg)
	SendMessage h EM_SETEVENTMASK 0 ENM_CHANGE
	 |ENM_REQUESTRESIZE
	 SendMessage h EM_SETEVENTMASK 0 ENM_REQUESTRESIZE
	
	case WM_VSCROLL
	sel GetWinId(lParam)
		case 4
		int sbcode(wParam&0xffff) sbpos(wParam>>16)
		 out "sbcode=%i sbpos=%i" sbcode sbpos
		sel sbcode
			case SB_THUMBTRACK
			 out sbpos
			SetScrollPos lParam SB_CTL sbpos 1
			
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case EN_CHANGE<<16|3
	out 1
	int n=SendMessage(id(3 hDlg) EM_GETFIRSTVISIBLELINE 0 0)
	out n
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code>0) out "0x%X" nh.code
sel nh.code
	case EN_REQUESTRESIZE
	 int n=SendMessage(id(3 hDlg) EM_GETFIRSTVISIBLELINE 0 0)
	 out n
	
