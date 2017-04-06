\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="1[]2[]3[]4[]5[]6[]7[]8[]9[]"
if(!ShowDialog("Dialog123" &Dialog123 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 Button 0x54032000 0x0 2 6 48 14 "Button"
 3 ListBox 0x54230101 0x200 96 6 96 36 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""
 3 Edit 0x54231044 0x200 120 6 96 48 ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	goto g1
ret 1

 g1
int w=id(3 hDlg) ;;does not work
 int w=hDlg ;;works
outw w
RECT rc; GetClientRect w &rc
SetScrollPos(w SB_VERT 2 1) ;;does not scroll, just sets scrollbar position. Did not test SetScrollInfo, probably the same.
 out ScrollWindow(w 0 -rc.bottom 0 0)
out ScrollWindowEx(w 0 -rc.bottom 0 0 0 0 SW_SCROLLCHILDREN|SW_ERASE|SW_INVALIDATE)
