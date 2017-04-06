\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("dlg_clock" &dlg_clock &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 6 4 208 13 ""
 4 Button 0x54032000 0x0 6 24 48 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0 ;;1 is timer id (you can set many timers), 1000 is timer period in ms
	goto g1
	
	case WM_TIMER
	if(wParam=1) ;;wParam is timer id
		 g1
		str s.time("%#c") ;;or DATE d.getclock; str s.time(d "%#c")
		s.setwintext(id(3 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	KillTimer hDlg 1
	case IDOK
	case IDCANCEL
ret 1
