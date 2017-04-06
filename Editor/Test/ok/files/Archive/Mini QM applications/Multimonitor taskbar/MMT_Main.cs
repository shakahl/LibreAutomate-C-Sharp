 /exe

 Run this.
 Adds taskbar in a monitor.

 Limitation: does not support multiple instances.
 Limitation: should not be used as thread running in QM context, because then QM may crash when ending thread.

 Command line (optional):
 /m monitor - monitor where to show the taskbar. Here monitor is 1-based monitor index. Default: last used or 2. If unavailable, shows in monitor 1.


type MMTWINMON hwnd monitor
type MMTWINDOW hwnd str'text
type MMTVAR
	monitor hwnd htb bwidth
	__ImageList'il
	ITaskbarList'itblist
	ARRAY(MMTWINDOW)'a

MMTVAR- v

 get settings
if(findrx(_command "/m +(\d+)" 0 1 _s 1)>=0) v.monitor=val(_s); rset v.monitor "Monitor" "\MultiMonitorTaskbar"
else rget v.monitor "Monitor" "\MultiMonitorTaskbar" 0 2
if(MonitorFromIndex(v.monitor)=MonitorFromIndex(1)) v.monitor=1

 create taskbar window and wait until closed
MainWindow "" "@QM_MMT_Class" &MMT_WndProc 0 0 1000 30 WS_POPUP|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
