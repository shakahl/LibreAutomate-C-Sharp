out
EnumDisplayMonitors 0 0 &sub.Proc 0


#sub Proc
function# hMonitor hdcMonitor RECT*lprcMonitor dwData 
out hMonitor

MONITORINFOEX mi.cbSize=sizeof(MONITORINFOEX)
GetMonitorInfo(hMonitor +&mi)
lpstr s=&mi.szDevice
out s
out "%i %i" mi.rcMonitor.left mi.rcMonitor.right

ret 1
