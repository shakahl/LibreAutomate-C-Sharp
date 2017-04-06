\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages


if(!ShowDialog("test_rotate_text" &test_rotate_text)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x5400000D 0x0 26 14 98 84 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Font-- f.Create2("Courier New" 20 1 450)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_DRAWITEM
	if(wParam=3) ;;control id
		DRAWITEMSTRUCT& r=+lParam
		int of=SelectObject(r.hDC f.handle)
		BSTR b="This is a test"
		TextOutW r.hDC 0 r.rcItem.bottom-30 b b.len
		SelectObject r.hDC of

ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
