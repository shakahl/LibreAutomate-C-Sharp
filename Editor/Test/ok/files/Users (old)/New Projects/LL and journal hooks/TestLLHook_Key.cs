function [timeout]

int+ bikhook=SetWindowsHookEx(WH_KEYBOARD_LL &BIKeyboardProc2 _hinst 0)
opt waitmsg 1
if(!timeout) timeout=-1
wait timeout
