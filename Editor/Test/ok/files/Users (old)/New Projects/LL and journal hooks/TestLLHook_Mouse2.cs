function [timeout]

dll unicows [SetWindowsHookExW]int'SetWindowsHookEx2 idHook lpfn hmod dwThreadId

int+ bimhook=SetWindowsHookEx2(WH_MOUSE_LL &BIMouseProc2 _hinst 0)
out bimhook
opt waitmsg 1
if(!timeout) timeout=-1
wait timeout
