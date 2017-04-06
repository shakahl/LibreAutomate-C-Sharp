 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	int hre=CreateControl(0 "RichEdit20W" "" WS_VSCROLL|ES_MULTILINE|ES_WANTRETURN|ES_AUTOVSCROLL|ES_SELECTIONBAR 0 0 100 100 hWnd 1)
	SendMessage hre EM_LIMITTEXT 4*1024*1024 0 ;;4 MB
	SetWinStyle hWnd WS_EX_NOACTIVATE 6
	goto wm_size
	
	case WM_COMMAND
	
	case WM_NOTIFY
	
	case WM_MOUSEACTIVATE ret 1 ;;allow activating

	case WM_SIZE
	 wm_size
	RECT r; GetClientRect(hWnd &r)
	MoveWindow id(1 hWnd) 0 22 r.right r.bottom-22 1
	
	case WM_DESTROY
	if(SendMessage(id(1 hWnd) EM_GETMODIFY 0 0)) Note_OpenSave 1 hWnd
	lpstr template=+RemoveProp(hWnd "note_template")
	if(template) free(template)
	RemoveProp(hWnd "note_name")
