\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "4"
str e4
if(!ShowDialog("Dialog20" &Dialog20 &controls)) ret
 int h=ShowDialog("Dialog20" &Dialog20 &controls 0 1)
 rep
	 MSG m
	 if(!GetMessage(&m 0 0 0)) break
	 int i=GetMessageExtraInfo
	 if(i) out "%i %i %i" m.message m.wParam i
	 TranslateMessage &m
	 DispatchMessage &m
	

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 134 "Form"
 4 Edit 0x54030080 0x200 62 6 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 4 6 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020000 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY PostQuitMessage 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	 spe 100
	 opt waitmsg 1
	key a
	 keybd_event 65 0 0 4
	 0
	 out GetMessageExtraInfo
	 out GetMessageTime
	case IDOK
	case IDCANCEL
ret 1
