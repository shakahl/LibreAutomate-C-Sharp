 fails, probably must be dll. Can instead use RegisterShellHookWindow, but it is unreliable.

int hh=SetWindowsHookEx(WH_SHELL &sub.Hook_WH_GETMESSAGE _hinst 0)
out hh
opt waitmsg 1
mes "shell hook working"
UnhookWindowsHookEx hh


#sub Hook_WH_GETMESSAGE
function# nCode wParam lParam
if(nCode<0) goto gNext

out wParam

 gNext
ret CallNextHookEx(0 nCode wParam +lParam)
