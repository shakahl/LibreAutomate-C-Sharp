/exe
out
wait 0 H mac("sub.Thread")
#sub Thread
1

 SetWindowsHookEx(WH_CALLWNDPROC &sub.HookProc 0 GetCurrentThreadId)
 SetWindowsHookEx(WH_MOUSE_LL &sub.Hook_WH_MOUSE_LL _hinst 0)

 out SetTimer(0 0 500 0)
 out 1
 MSG m; GetMessage(&m 0 0 0)
 out 2

 SetCursor LoadCursor(0 +IDC_ARROW)

str dd=
 BEGIN DIALOG
 0 "" 0x10C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

int-- t_waitCursor=LoadCursor(0 +IDC_WAIT)

OutWinMsg message wParam lParam _s
 if(IsMouseCursor(IDC_WAIT)) out F"<><c 0xff>{_s}</c>"
 if(GetCursor=t_waitCursor) out F"<><c 0xff>{_s}</c>"
 else out _s

 int-- t_ft
  if(t_ft=0 and message=WM_NCHITTEST)
 if(t_ft=0 and IsMouseCursor(IDC_WAIT))
	 t_ft=1
	 SetCursor LoadCursor(0 +IDC_ARROW)

 out GetCursor

 0.01
sel message
	case WM_CREATE
	SetCursor LoadCursor(0 +IDC_ARROW)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog193
 exe_file  $my qm$\Dialog193.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {EF462CF0-EE4D-4830-9477-B7C8F75D03B0}
 END PROJECT


#sub HookProc
function# nCode remove CWPSTRUCT&m
if(nCode<0) goto gNext

 ret 1

int-- t_waitCursor=LoadCursor(0 +IDC_WAIT)
 OutWinMsg m.message m.wParam m.lParam
if(GetCursor=t_waitCursor)
 if !IsMouseCursor(IDC_ARROW)
	OutWinMsg m.message m.wParam m.lParam

 gNext
ret CallNextHookEx(0 nCode remove &m)

 note: cannot hook windows of other processes.


#sub Hook_WH_MOUSE_LL
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext

 if(message!=WM_MOUSEMOVE) OutWinMsg message 0 0 _s; out "%s at %i %i" _s m.pt.x m.pt.y

if(IsMouseCursor(IDC_WAIT)) out 1

 gNext
ret CallNextHookEx(0 nCode message &m)
