\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("show_icons_in_folder" &show_icons_in_folder 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 639 541 "Dialog"
 3 Static 0x5400100D 0x20004 0 0 638 540 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	RECT rc; GetClientRect hDlg &rc
	__MemBmp- t_mb.Create(rc.right rc.bottom)
	FillRect t_mb.dc &rc COLOR_BTNFACE+1
	int hi r c
	Dir d
	foreach(d "$QM$\*.ico" FE_Dir 0x4)
		str sPath=d.FileName(1)
		 out sPath
		hi=GetFileIcon(sPath); if(!hi) continue
		DrawIconEx(t_mb.dc c*18 r*18 hi 16 16 0 0 DI_NORMAL)
		DestroyIcon hi
		c+1; if(c>50) c=0; r+1
		if(r>40) break
	
	case WM_DRAWITEM
	DRAWITEMSTRUCT& dr=+lParam
	FillRect(dr.hDC &dr.rcItem COLOR_APPWORKSPACE+1)
	if(t_mb.bm) BitBlt(dr.hDC 0 0 dr.rcItem.right dr.rcItem.bottom t_mb.dc 0 0 SRCCOPY)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

