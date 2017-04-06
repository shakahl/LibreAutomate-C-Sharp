out "dtor"
 EnumThreadWindows(GetCurrentThreadId &sub.ETWProc 0)
EnumDisplayMonitors 0 0 &sub.EnumMon 0


#sub ETWProc
function# hwnd param
outw hwnd
ret 1

#sub EnumMon
function# hMonitor hdcMonitor RECT*lprcMonitor dwData 
out hMonitor

 MONITORINFOEX mi.cbSize=sizeof(MONITORINFOEX)
 GetMonitorInfo(hMonitor +&mi)
 lpstr s=&mi.szDevice
 out s
 out "%i %i" mi.rcMonitor.left mi.rcMonitor.right

ret 1
