function [timeout]

int+ bimhook=SetWindowsHookEx(WH_MOUSE_LL &BIMouseProc2 _hinst 0)
opt waitmsg 1
if(!timeout) timeout=-1
wait timeout
