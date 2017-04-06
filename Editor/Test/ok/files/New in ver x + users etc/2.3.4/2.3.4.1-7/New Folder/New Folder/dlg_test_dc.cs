\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("dlg_test_dc" &dlg_test_dc 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0
	case WM_TIMER
	sel wParam
		case 1
		RedrawWindow(hDlg, 0, 0, RDW_ERASE|RDW_FRAME|RDW_INVALIDATE|RDW_UPDATENOW|RDW_ALLCHILDREN)
	case WM_COMMAND goto messages2
	case WM_PAINT
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	TestDC ps.hDC 0
	EndPaint hDlg &ps
	case WM_NCPAINT
	 int hdc = GetDCEx(hDlg, wParam, DCX_WINDOW|DCX_INTERSECTRGN);
	int hdc = GetWindowDC(hDlg);
	TestDC hdc 1
	ReleaseDC(hDlg, hdc)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
