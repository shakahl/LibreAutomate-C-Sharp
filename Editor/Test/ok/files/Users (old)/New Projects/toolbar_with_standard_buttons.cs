\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 note: the buttons are old style, not like on XP/Vista

if(!ShowDialog("" &toolbar_with_standard_buttons)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 356 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ToolbarWindow32 0x54030001 0x0 0 0 223 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	TBADDBITMAP b.hInst=HINST_COMMCTRL; b.nID=IDB_STD_SMALL_COLOR
	SendMessage h TB_ADDBITMAP 0 &b
	
	ARRAY(TBBUTTON) a.create(15)
	ARRAY(str) as="1[]2[]3[]4[]5[]6[]7[]8[]9[]10[]11[]12[]13[]14[]15"
	int i
	for i 0 a.len
		TBBUTTON& t=a[i]
		t.idCommand=1001+i
		t.iBitmap=i
		t.iString=SendMessage(h TB_ADDSTRINGA 0 as[i]) ;;note: the string must be terminated with two 0. str variables normally have two 0, but if you will use unicode...
		t.fsState=TBSTATE_ENABLED
	
	SendMessage(h TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
	SendMessage(h TB_ADDBUTTONS a.len &a[0])
	SendMessage(h TB_AUTOSIZE 0 0)

	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

