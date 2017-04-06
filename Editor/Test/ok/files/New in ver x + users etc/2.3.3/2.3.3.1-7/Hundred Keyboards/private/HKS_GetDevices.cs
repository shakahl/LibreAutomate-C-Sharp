 /
function ARRAY(HKS_HID)&a devicetype

a.redim
int i nDev bSize; str devName
GetRawInputDeviceList(0 &nDev sizeof(RAWINPUTDEVICELIST))
RAWINPUTDEVICELIST* rl._new(nDev)
nDev=GetRawInputDeviceList(rl &nDev sizeof(RAWINPUTDEVICELIST))
for i 0 nDev
	RAWINPUTDEVICELIST& x=rl[i]
	if(x.dwType!=devicetype) continue
	 out x.hDevice
	GetRawInputDeviceInfo(x.hDevice RIDI_DEVICENAME 0 &bSize)
	GetRawInputDeviceInfo(x.hDevice RIDI_DEVICENAME devName.all(bSize) &bSize)
	devName.fix
	 out devName
	HKS_HID& r=a[]
	r.handle=x.hDevice
	r.kid=Crc32(devName devName.len)
rl._delete
