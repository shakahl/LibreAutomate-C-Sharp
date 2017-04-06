/exe
out
 int hh=SetWindowsHookEx(WH_CALLWNDPROC &sub.Hook_WH_GETMESSAGE 0 GetCurrentThreadId)

ShowMenu("one[]two")

 UnhookWindowsHookEx hh


#sub Hook_WH_GETMESSAGE
function# nCode remove CWPSTRUCT&m
if(nCode<0) goto gNext

OutWinMsg m.message m.wParam m.lParam

 gNext
ret CallNextHookEx(0 nCode remove &m)

 note: cannot hook windows of other processes.

 BEGIN PROJECT
 main_function  Macro506
 exe_file  $my qm$\Macro506.qmm
 flags  6
 guid  {3A02C40D-25B9-404A-8FAA-E3FD4E1D0CF9}
 END PROJECT
