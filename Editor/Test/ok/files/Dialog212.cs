
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

int d=ShowDialog(dd 0 0 0 1)
MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	
	sel m.message
		case WM_KEYDOWN
		str s.all(100)
		GetKeyNameText m.lParam s 100
		out F"{m.wParam} 0x{m.lParam} {s.fix}"
	
	DispatchMessage &m
