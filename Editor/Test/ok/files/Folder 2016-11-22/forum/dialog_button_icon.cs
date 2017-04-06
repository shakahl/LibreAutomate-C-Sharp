
str icons=
 $qm$\copy.ico
 $qm$\paste.ico
ARRAY(__Hicon) ai
foreach(_s icons) ai[]=GetFileIcon(_s)
int imageType=IMAGE_ICON

 for other image file types use this:
 str images=
  $my qm$\file.bmp
  $my qm$\file.png
  $my qm$\file.gif
  $my qm$\file.jpg
 ARRAY(__GdiHandle) ai
 foreach(_s images) ai[]=LoadPictureFile(_s)
 int imageType=IMAGE_BITMAP

int hButton

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x0 0 0 135 17 "Dialog"
 1 Button 0x54030001 0x4 38 0 48 14 "OK"
 2 Button 0x54030000 0x4 86 0 48 14 "Cancel"
 3 Button 0x54032000 0x0 1 1 34 15 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "" "" "" ""

if(!ShowDialog(dd &sub.DlgProc)) ret

 note: the button must have non-empty text. Or it must have style BS_ICON or BS_BITMAP, that is why I added the SetWinStyle line below.


#sub DlgProc v
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	hButton=id(3 hDlg)
	if(GetWindowTextLengthW(hButton)=0) SetWinStyle hButton iif(imageType=IMAGE_ICON BS_ICON BS_BITMAP) 1
	SendMessage(hButton BM_SETIMAGE imageType ai[0])
	
	case WM_SETCURSOR
	if wParam=hButton and SendMessage(hButton BM_GETIMAGE imageType 0)!=ai[1]
		SendMessage(hButton BM_SETIMAGE imageType ai[1])
		SetTimer hDlg 1 50 0
		
	case WM_TIMER
	sel wParam
		case 1
		if(child(mouse)!=hButton)
			KillTimer hDlg wParam
			SendMessage(hButton BM_SETIMAGE imageType ai[0])

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
