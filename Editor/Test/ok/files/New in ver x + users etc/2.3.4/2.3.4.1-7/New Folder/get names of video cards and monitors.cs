out
DISPLAY_DEVICE d.cb=sizeof(d)
DISPLAY_DEVICE dd.cb=sizeof(dd)
for _i 0 1000000
	if(!EnumDisplayDevices(0 _i &d 0)) break
	if(d.StateFlags&DISPLAY_DEVICE_ATTACHED_TO_DESKTOP=0) continue
	lpstr s=&d.DeviceString
	out "card: %s" s
	if EnumDisplayDevices(&d.DeviceName 0 &dd 0)
		s=&dd.DeviceString
		out "monitor: %s" s
