\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_display_image_parts" &dialog_display_image_parts)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040003 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 load a bitmap file containing many 16x16 images horizontally, create a memory device context (DC), and select the loaded bitmap into the DC
	__MemBmp- t_mb
	t_mb.Attach(LoadPictureFile("$qm$\il_qm.bmp"))
	
	 also create a pattern brush for tiling (example 2)
	__GdiHandle- t_brush
	__MemBmp mb.Create(16 16 t_mb.dc 16*3 0) ;;take 4-th image from t_mb
	t_brush=CreatePatternBrush(mb.bm)
	
	case WM_PAINT
	PAINTSTRUCT p; BeginPaint hDlg &p
	
	 example 1: display 10 images from the memory DC in the dialog diagonally
	int i
	for i 0 160 16
		BitBlt p.hDC i i 16 16 t_mb.dc i 0 SRCCOPY
	
	 example 2: display tiled image at the right side of the dialog
	RECT r; GetClientRect hDlg &r; r.left=r.right*0.7
	FillRect p.hDC &r t_brush
	
	EndPaint hDlg &p
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
