 rep() ifk((1)) 0.01; else break

 mou+ 20 20
 0.1

 #ifndef PostMessage
 dll user32 #PostMessage hWnd wMsg wParam lParam
 def WM_USER          0x400
 #endif

PostMessage _hwndqm WM_USER+33 0 0
 info: use wParam 0 to call testR, 1 to call testU
