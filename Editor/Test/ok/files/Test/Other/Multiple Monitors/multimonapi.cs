type MONITORINFO cbSize RECT'rcMonitor RECT'rcWork dwFlags
type MONITORINFOEX :MONITORINFO'mi !szDevice[CCHDEVICENAME]
type DISPLAY_DEVICE cb !DeviceName[32] !DeviceString[128] StateFlags !DeviceID[128] !DeviceKey[128]
dll user32
	#MonitorFromPoint POINT'pt dwFlags 
	#MonitorFromRect RECT*lprc dwFlags 
	#MonitorFromWindow hwnd dwFlags 
	#GetMonitorInfo hMonitor MONITORINFO*lpmi 
	#EnumDisplayMonitors hdc RECT*lprcClip lpfnEnum dwData 
	#EnumDisplayDevices $lpDevice iDevNum DISPLAY_DEVICE*lpDisplayDevice dwFlags 

