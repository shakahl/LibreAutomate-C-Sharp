2
out
int hh=SetWindowsHookEx(WH_FOREGROUNDIDLE &ForegroundIdleProc _hinst GetWindowThreadProcessId(win 0))
out hh
opt waitmsg 1
2
UnhookWindowsHookEx(hh)
