\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("SwapButtonImageOnHoverInDlg" &SwapButtonImageOnHoverInDlg)) ret

 BEGIN DIALOG
 0 "" 0x90080A44 0x0 0 0 135 17 "TrackMouseEvent"
 1 Button 0x54030001 0x4 38 0 48 14 "OK"
 2 Button 0x54030000 0x4 86 0 48 14 "Cancel"
 7 Button 0x54032000 0x0 1 1 33 15 " "
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ont hDlg
	
	lpstr sb=
 $my qm$\macro682.bmp
 $my qm$\macro610 (2).bmp
	ARRAY(__GdiHandle)-- ab
	if(!ab.len) foreach(_s sb) ab[]=LoadPictureFile(_s)
	
	int-- hbtn1; hbtn1=id(7 hDlg)
	SendMessage(hbtn1 BM_SETIMAGE IMAGE_BITMAP ab[0])
	
	case WM_SETCURSOR
	if wParam=hbtn1 and SendMessage(wParam BM_GETIMAGE IMAGE_BITMAP 0)!=ab[1]
		SendMessage(wParam BM_SETIMAGE IMAGE_BITMAP ab[1])
		SetTimer hDlg 1 50 0
		
	case WM_TIMER
	sel wParam
		case 1
		if(child(mouse)!=hbtn1)
			KillTimer hDlg wParam
			SendMessage(hbtn1 BM_SETIMAGE IMAGE_BITMAP ab[0])

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 7
		mes "hello"
ret 1
