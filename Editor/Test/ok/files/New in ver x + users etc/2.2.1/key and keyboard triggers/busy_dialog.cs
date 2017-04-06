\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &busy_dialog)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 417 135 "Dialog A"
 4 Edit 0x54231044 0x200 0 0 418 114 ""
 3 Button 0x54032000 0x0 0 116 48 14 "Button"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	srand GetTickCount
	 SetTimer hDlg 1 50 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	 case WM_TIMER
	 if(wParam=1)
		 0.06
ret
 messages2
sel wParam
	case 3
	out
	act id(4 hDlg)
	 opt keynosync 1
	 'kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkmY
	key (_s.getmacro("Macro512"))
	 'Ykkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkm
	 act "Notepad"
	 0.1
	 out
	 'F6F6F6

	case EN_CHANGE<<16|4
	ret
	double w=rand/35000.0
	w=pow(w 6)
	 if(w>=0.1) out w
	wait w
	 0.1
	case IDOK
	case IDCANCEL
ret 1
