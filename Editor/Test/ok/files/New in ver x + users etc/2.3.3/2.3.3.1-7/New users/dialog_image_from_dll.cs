\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
if(!ShowDialog("dialog_image_from_dll" &dialog_image_from_dll &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x5400000E 0x0 4 4 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	str sDll="$qm$\qm.exe" ;;dll or exe
	int idResource=286 ;;bitmap resource id or name in the dll. To know it, use a resource editor program, for example Resource Hacker.
	
	int hDll=LoadLibraryEx(sDll.expandpath 0 LOAD_LIBRARY_AS_DATAFILE)
	if hDll
		__GdiHandle-- hBitmap=LoadImage(hDll +idResource IMAGE_BITMAP 0 0 0)
		SendMessage id(3 hDlg) STM_SETIMAGE IMAGE_BITMAP hBitmap
		FreeLibrary hDll
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
