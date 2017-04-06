\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog36" &Dialog36)) ret

 BEGIN DIALOG
 0 "" 0x90CC0A44 0x100 0 0 217 129 "Dialog"
 3 ToolbarWindow32 0x54018800 0x0 0 0 217 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int htb=id(3 hDlg)
	int il1=ImageList_LoadImage(0 _s.expandpath("$qm$\de_ctrl.bmp") 16 0 0xFF00FF IMAGE_BITMAP LR_LOADFROMFILE)
	SendMessage htb TB_SETIMAGELIST 0 il1
	
	ARRAY(TBBUTTON) a.create(3)
	ARRAY(str) as="One[]Two[]Three"
	int i
	for i 0 a.len
		TBBUTTON& t=a[i]
		t.idCommand=1001+i
		t.iBitmap=i+1
		t.iString=SendMessage(htb TB_ADDSTRINGA 0 as[i]) ;;note: the string must be terminated with two 0. str variables normally have two 0, but if you will use unicode...
		t.fsState=TBSTATE_ENABLED
	
	SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
	SendMessage(htb TB_ADDBUTTONS a.len &a[0])
	
	SendMessage id(3 hDlg) TB_AUTOSIZE 0 0
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_ERASEBKGND
	RECT rc; GetClientRect hDlg &rc
	int hdc=wParam
	FillRect hdc &rc COLOR_BTNFACE+1
	 OSD_ProcExample hDlg hdc rc.right rc.bottom 0
	def WP_CAPTION 1
	int _hTheme=OpenThemeData(0 L"Taskbar")
	DrawThemeBackground(_hTheme hdc WP_CAPTION 0 &rc 0)
	CloseThemeData(_hTheme)
	ret DT_Ret(hDlg 1)
	
ret
 messages2
ret 1
