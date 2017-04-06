function [$icon] [$tooltip] [flags] [func] [param] [$onclick] [$onrclick] ;;flags: 1 func is window handle, 2 set icon for window, 4 no opt waitmsg, 8 restore. Press F1 and read more.

 Adds tray icon.

 icon - icon file.
   Can be ico or other. Can be with icon index, like "shell32.dll,8".
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.
   Can be list of icon files. Then you can later change icon with Modify function.
   If not specified, uses icon of thread main macro/function (in exe - exe icon).
 tooltip - tooltip text. Displays max 127 characters.
 func - receives notifications from tray icon about mouse events (when clicked, or mouse moved). Can be:
   Omitted or 0. No notifications.
   Address of <help "Callback_Tray_AddIcon">callback function</help>.
     A template is available in menu -> File -> New -> Templates.
   A window handle. Use flag 1.
     The window will receive WM_USER+101 messages, where wParam is address of Tray object (Tray& x=+wParam), lParam is mouse message.
     These parameters are ignored: param, onclick, onrclick.
 param - some value to pass to the callback function as x.param.
 onclick - name of macro, function or menu. It will be launched on left click.
   If this function is called from macro, onclick should not be macro. If in exe, must be function.
 onrclick - the same with right click.

 REMARKS
 After tray icon is added, current thread must stay running and able to process messages.
   For example, you can add icon in dialog procedure, under WM_INITDIALOG. Then Tray variable should have thread scope (Tray- ...).
   Or after adding call MessageLoop function, or wait function.
 The tray icon will be automatically deleted when thread ends.
 You can Ctrl+click tray icon to delete it and end thread.

 Try <open>Tray_sample_simplest</open> and other samples in the folder.

 See also: <AddTrayIcon>


#opt nowarnings 1
def NIF_SHOWTIP (0x00000080)

int+ ___newtaskbar
if(!___newtaskbar) ___newtaskbar=RegisterWindowMessage("TaskbarCreated")
if(flags&8)
	__Shell_NotifyIconW(NIM_ADD &nd 3); ret

int i ii hwnd; str s

nd.uFlags=NIF_ICON|NIF_MESSAGE

if(m_flags&1=0) ;;first time
	if(flags&4=0) opt waitmsg 2
	 function, window, macros
	if(func) if(flags&1) hwnd=func; else __m_func=func
	this.__m_onclick=onclick; this.__m_onrclick=onrclick
	 icons
	if(icon<=0xffff or !icon[0]) ;;get default icon
		icon=0
		if(hwnd) m_a[].hicon=GetWindowIcon(hwnd)
		else m_a[].hicon=__GetQmItemIcon(+getopt(itemid 3)) ;;use icon of thread main function (in exe - exe icon)
	else ;;add one or more icons
		m_a.redim(numlines(icon))
		foreach(s icon) m_a[i].icfile=s; m_a[i].hicon=GetFileIcon(s); i+1
else if(icon>0xffff) ;;add or select icon by file name
	if(icon[0])
		for(ii 0 m_a.len) if(m_a[ii].icfile=icon) break ;;already exists?
		if(ii=m_a.len) __TRAYIC& si=m_a[]; si.icfile=icon; si.hicon=GetFileIcon(icon)
	else nd.uFlags~NIF_ICON
else if(icon) ii=icon-1 ;;index in array
else nd.uFlags~NIF_ICON

if(ii<m_a.len) nd.hIcon=m_a[ii].hicon; else nd.hIcon=0

 tooltip
if(tooltip)
	str Ttitle="MY Title"
	lstrcpynW(&nd.szTip @tooltip 128)
	lstrcpynW(&nd.szInfo @tooltip 256)
	lstrcpynW(&nd.szInfoTitle @Ttitle 256)
	nd.uVersion=4
	__Shell_NotifyIconW(NIM_SETVERSION &nd)
if(nd.szTip[0]) 
	nd.uFlags|NIF_TIP|NIF_SHOWTIP|NIF_INFO

this.param=param

if(m_flags&1) __Shell_NotifyIconW(NIM_MODIFY &nd 1) ;;change icon or/and tooltip
else ;;add new
	if(hwnd)
		if(flags&2 and nd.hIcon) SendMessage(hwnd WM_SETICON 0 nd.hIcon)
	else
		if(!m_hwnd)
			__RegisterWindowClass+ ___tray_class; if(!___tray_class.atom) ___tray_class.Register("QM_Tray_Class" &sub.WndProc 4)
			m_hwnd=CreateWindowExW(0 +___tray_class.atom 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
			SetWindowLong m_hwnd 0 &this
		hwnd=m_hwnd; m_flags|2
	
	nd.hWnd=hwnd
	nd.uCallbackMessage=WM_USER+101
	nd.uID=&this
	nd.cbSize=sizeof(NOTIFYICONDATAW)
	if(__Shell_NotifyIconW(NIM_ADD &nd 10)) m_flags|1
	else end "Failed to add tray icon" 8

err+

 m_flags: 1 icon is added, 2 own hwnd


#sub WndProc
function# hWnd message wParam lParam

Tray& t

sel message
	case WM_USER+101
	&t=+wParam
	int mod=GetMod
	if(t.__m_func) call t.__m_func wParam lParam
	else if(lParam=WM_LBUTTONUP and mod=2) shutdown -7
#opt nowarnings 1
	sel lParam
		case WM_LBUTTONUP if(t.__m_onclick.len and !mod) mac t.__m_onclick; err
		case WM_RBUTTONUP if(t.__m_onrclick.len and !mod) mac t.__m_onrclick; err
#opt nowarnings 0
	ret
	
	case else
	if(message=___newtaskbar)
		&t=+GetWindowLong(hWnd 0)
		if(&t) t.AddIcon("" "" 8)

ret DefWindowProcW(hWnd message wParam lParam)