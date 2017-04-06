 /
function add hwnd [hicon] [$tooltip]

 Obsolete. Use <help>AddTrayIcon</help>.


type ___NOTIFYICONDATA_IE4 cbSize hWnd uID uFlags uCallbackMessage hIcon !szTip[64]
dll shell32 [Shell_NotifyIconA]#___Shell_NotifyIcon_IE4 dwMessage ___NOTIFYICONDATA_IE4*lpData

___NOTIFYICONDATA_IE4 nd
nd.cbSize=sizeof(___NOTIFYICONDATA_IE4)
nd.hWnd=hwnd
nd.uID=1

if add
	nd.uFlags=NIF_ICON|NIF_MESSAGE
	nd.uCallbackMessage=WM_USER+100
	if(hicon) nd.hIcon=hicon; else nd.hIcon=GetFileIcon("$qm$\macro.ico")
	if(tooltip) nd.uFlags|NIF_TIP; lstrcpyn(&nd.szTip tooltip 64)
	___Shell_NotifyIcon_IE4(NIM_ADD &nd)
else
	___Shell_NotifyIcon_IE4(NIM_DELETE &nd)
