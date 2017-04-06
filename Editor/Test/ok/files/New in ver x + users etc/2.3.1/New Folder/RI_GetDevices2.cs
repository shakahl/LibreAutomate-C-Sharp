 /Macro865
function ARRAY(RIHID)&a devicetype

a.redim
int i nDev bSize; str devName
GetRawInputDeviceList(0 &nDev sizeof(RAWINPUTDEVICELIST))
RAWINPUTDEVICELIST* rl._new(nDev)
nDev=GetRawInputDeviceList(rl &nDev sizeof(RAWINPUTDEVICELIST))
for i 0 nDev
	RAWINPUTDEVICELIST& r=rl[i]
	if(r.dwType!=devicetype)
		 out r.dwType
		continue
	 out r.hDevice
	GetRawInputDeviceInfo(r.hDevice RIDI_DEVICENAME 0 &bSize)
	GetRawInputDeviceInfo(r.hDevice RIDI_DEVICENAME devName.all(bSize) &bSize)
	devName.fix
	out devName
	
	 GetRawInputDeviceInfo(r.hDevice RIDI_PREPARSEDDATA 0 &bSize)
	 GetRawInputDeviceInfo(r.hDevice RIDI_PREPARSEDDATA _s.all(bSize) &bSize)
	 out _s.fix
	
	RID_DEVICE_INFO di.cbSize=sizeof(RID_DEVICE_INFO)
	GetRawInputDeviceInfo(r.hDevice RIDI_DEVICEINFO &di &di.cbSize)
	sel di.dwType
		case 0 ;;mouse
		out di.mouse.dwId
		 out di.mouse.dwNumberOfButtons
		 out di.mouse.dwSampleRate
		 out di.mouse.fHasHorizontalWheel
		case 1 ;;keyboard
		out di.keyboard.dwType
	
	RIHID& rh=a[a.redim(-1)]
	rh.handle=r.hDevice
	devName.encrypt(2)
	memcpy &rh.kid devName 4
rl._delete
