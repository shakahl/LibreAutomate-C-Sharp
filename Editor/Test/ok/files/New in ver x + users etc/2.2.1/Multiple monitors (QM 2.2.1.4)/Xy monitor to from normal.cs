int x y
 x=10; y=100
 XyMonitorToNormal 2 x y
 XyNormalToMonitor 2 x y

 XyMonitorToNormal 2 x y 1
 XyNormalToMonitor 2 x y 1

int h=_hwndqm
XyMonitorToNormal h x y
 XyNormalToMonitor h x y

out "%i %i" x y

