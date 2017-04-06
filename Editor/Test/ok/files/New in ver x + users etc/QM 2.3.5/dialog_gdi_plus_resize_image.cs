\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_gdi_plus_resize_image" &dialog_gdi_plus_resize_image 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 116 48 14 "Change"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "*" "" "" ""

ret
 messages
#compile "__Gdip"
sel message
	case WM_INITDIALOG
	GdipImage-- t_im t_im2
	if(!t_im.FromFile("q:\test\app_55.png")) ret
	if(!t_im2.FromFile("$documents$\foto\__kate.jpg")) ret
	
	case WM_PAINT
	if(!t_im) ret
	
	PAINTSTRUCT ps
	BeginPaint hDlg &ps
	t_im.DrawResize(ps.hDC 0 0 100 0 7)
	t_im2.DrawResize(ps.hDC 150 0 100 0 7)
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 ;;Change
	if(!t_im2.FromFile("q:\test\app_55.png")) ret
	if(!t_im.FromFile("$documents$\foto\__kate.jpg")) ret
	InvalidateRect hDlg 0 1
ret 1
