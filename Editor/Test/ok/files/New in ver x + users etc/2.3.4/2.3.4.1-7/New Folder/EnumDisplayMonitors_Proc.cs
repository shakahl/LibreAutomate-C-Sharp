 /
function# hMonitor hdcMonitor RECT*lprcMonitor ARRAY(str)&a

out hMonitor
MONITORINFOEX m.cbSize=sizeof(m)
GetMonitorInfo(hMonitor +&m)
lpstr s=&m.szDevice
out s

ret 1
