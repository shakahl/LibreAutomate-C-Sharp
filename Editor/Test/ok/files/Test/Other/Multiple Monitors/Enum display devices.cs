#compile "multimonapi"

DISPLAY_DEVICE dd.cb=sizeof(DISPLAY_DEVICE)
int i j
lpstr s
str ss
for i 0 100000
	if(!EnumDisplayDevices(0 i &dd 0)) break
	s=&dd.DeviceName; ss=s
	 out s
	 s=&dd.DeviceString; out s
	 s=&dd.DeviceID; out s
	if(dd.StateFlags&1=0) continue
	 continue
	for j 0 100000
		if(!EnumDisplayDevices(ss j &dd 0)) break
		if(dd.StateFlags&1=0) continue
		s=&dd.DeviceName; out s
		 s=&dd.DeviceString; out s
		 out dd.StateFlags
		s=&dd.DeviceID; out s

 1 attached
 2 
 4 primary