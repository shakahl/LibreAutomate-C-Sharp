\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
if(!ShowDialog("" &dlg_png_gdiplus &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 328 168 "Dialog"
 3 Static 0x5400000E 0x0 242 102 66 58 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030201 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	case WM_PAINT
	__Stream-- t_ispng
	Q &q
	if(!t_ispng.is) t_ispng.CreateOnFile("q:\test\app_55.png" STGM_READ)
	Q &qq; outq
	
	PAINTSTRUCT ps
	BeginPaint hDlg &ps
	Q &q
	 __DTSI_Draw("q:\test\app_55.png" ps.hDC 100 50)
	__DTSI_Draw(t_ispng ps.hDC 100 50)
	Q &qq; outq
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
