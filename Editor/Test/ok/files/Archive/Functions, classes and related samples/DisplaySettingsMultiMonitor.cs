function! monitor [colorbits] [xpixels] [ypixels] [xmonitor] [ymonitor]

 Changes display resolution and/or other settings of specified monitor.
 You can set only required arguments, others set to 0.
 If all arguments except monitor are 0, then only shows available display settings.
 Returns 1 if successful, 0 if fails.
 If needs to restart computer, shows message box and returns 0.

 monitor - 1-based monitor index. 0 and 1 is primary monitor.
 colorbits - color resolution (4, 8, 16, 24 or 32 bits).
 xpixels, ypixels - width and height.
 xmonitor, ymonitor - monitor position. For primary monitor must be 0.

 EXAMPLES
 DisplaySettingsMultiMonitor(2) ;;show available settings of monitor 2

 DisplaySettingsMultiMonitor(2 0 1024 768) ;;change display resolution of monitor 2

 DisplaySettingsMultiMonitor(2 0 0 0 1024 200) ;;change position of monitor 2


def CDS_UPDATEREGISTRY  0x00000001
def DM_BITSPERPEL       0x00040000
def DM_PELSWIDTH        0x00080000
def DM_PELSHEIGHT       0x00100000
def DISP_CHANGE_SUCCESSFUL       0
def DISP_CHANGE_RESTART          1
type DEVMODE !dmDeviceName[32] @dmSpecVersion @dmDriverVersion @dmSize @dmDriverExtra dmFields {{@dmOrientation @dmPaperSize @dmPaperLength @dmPaperWidth @dmScale @dmCopies @dmDefaultSource @dmPrintQuality} []{POINTL'dmPosition dmDisplayOrientation dmDisplayFixedOutput}} @dmColor @dmDuplex @dmYResolution @dmTTOption @dmCollate !dmFormName[32] @dmLogPixels dmBitsPerPel dmPelsWidth dmPelsHeight {dmDisplayFlags []dmNup} dmDisplayFrequency dmICMMethod dmICMIntent dmMediaType dmDitherType dmReserved1 dmReserved2 dmPanningWidth dmPanningHeight
type MONITORINFO cbSize RECT'rcMonitor RECT'rcWork dwFlags
type MONITORINFOEX :MONITORINFO'_base !szDevice[32]
dll user32
	#EnumDisplaySettings $lpszDeviceName iModeNum DEVMODE*lpDevMode
	#ChangeDisplaySettingsEx $lpszDeviceName DEVMODE*lpDevMode hwnd dwflags !*lParam
	#GetMonitorInfo hMonitor MONITORINFO*lpmi

 monitor index -> device name
str devname
if(monitor and monitor!=1)
	int hm=MonitorFromIndex(monitor)
	if(monitor>0 and hm=MonitorFromIndex(1)) ret
	MONITORINFOEX mi.cbSize=sizeof(mi)
	if(!GetMonitorInfo(hm +&mi)) ret
	lpstr t=&mi.szDevice; devname=t

DEVMODE d.dmSize=sizeof(d)
int i
for i 0 1000
	if(EnumDisplaySettings(devname i &d)=0) break
	if(colorbits=0 and xpixels=0 and ypixels=0 and xmonitor=0 and ymonitor=0)
		out "colorbits=%i  xpixels=%i  ypixels=%i  frequency=%i" d.dmBitsPerPel d.dmPelsWidth d.dmPelsHeight d.dmDisplayFrequency
		continue
	if((colorbits=0 or colorbits=d.dmBitsPerPel) and (xpixels=0 or xpixels=d.dmPelsWidth) and (ypixels=0 or ypixels=d.dmPelsHeight))
		d.dmFields=0
		if(colorbits) d.dmFields|DM_BITSPERPEL
		if(xpixels) d.dmFields|DM_PELSWIDTH
		if(ypixels) d.dmFields|DM_PELSHEIGHT
		if(xmonitor or ymonitor) d.dmFields|DM_POSITION; d.dmPosition.x=xmonitor; d.dmPosition.y=ymonitor
		i=ChangeDisplaySettingsEx(devname &d 0 CDS_UPDATEREGISTRY 0)
		if(i=DISP_CHANGE_SUCCESSFUL) ret 1
		if(i=DISP_CHANGE_RESTART) mes "Display settings will be changed after you restart computer." "Display Settings" "i"
		ret
