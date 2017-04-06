 /
function ARRAY(RIHID)&a devicetype

a.redim
int i nDev bSize; str devName
GetRawInputDeviceList(0 &nDev sizeof(RAWINPUTDEVICELIST))
RAWINPUTDEVICELIST* rl._new(nDev)
nDev=GetRawInputDeviceList(rl &nDev sizeof(RAWINPUTDEVICELIST))
for i 0 nDev
	if(rl[i].dwType!=devicetype) continue
	 out rl[i].hDevice
	GetRawInputDeviceInfo(rl[i].hDevice RIDI_DEVICENAME 0 &bSize)
	GetRawInputDeviceInfo(rl[i].hDevice RIDI_DEVICENAME devName.all(bSize) &bSize)
	devName.fix
	 out devName
	RIHID& r=a[a.redim(-1)]
	r.handle=rl[i].hDevice
	devName.encrypt(2)
	memcpy &r.kid devName 4
rl._delete
