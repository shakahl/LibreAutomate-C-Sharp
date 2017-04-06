out
DISPLAY_DEVICE d.cb=sizeof(d)
int i
for i 0 1000
	if(!EnumDisplayDevices(0 i &d 0)) break
	out "0x%X %s" d.StateFlags &d.DeviceName
