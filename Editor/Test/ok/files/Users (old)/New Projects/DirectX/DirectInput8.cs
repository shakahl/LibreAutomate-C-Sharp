 Tried to use to recognize keyboard devices, but directinput does not allow this.

typelib DxVBLibA {E1211242-8E94-11D1-8808-00C04FC2C603} 1.0 ;;8

DxVBLibA.DirectX8 dx8._create
DxVBLibA.DirectInput8 din=dx8.DirectInputCreate
din.ConfigureDevices
DxVBLibA.DirectInputEnumDevices8 died=din.GetDIDevices(DxVBLibA.DI8DEVCLASS_KEYBOARD DxVBLibA.DIEDFL_INCLUDEALIASES)
 DxVBLibA.DirectInputDeviceInstance8 devi
 int i
 for i 1 died.GetCount+1
	 devi=died.GetItem(i)
	 out devi.GetGuidInstance

DxVBLibA.DirectInputDevice8 dev=din.CreateDevice(died.GetItem(1).GetGuidInstance)
 DxVBLibA.DirectInputEnumDeviceObjects deo=dev.GetDeviceObjectsEnum(0)
 DxVBLibA.DirectInputDeviceObjectInstance doi
 int i
 for i 1 deo.GetCount+1
	 doi=deo.GetItem(i)
	 out doi.GetName

 dev.SetCooperativeLevel
int ev=CreateEvent(0 0 0 0)
dev.SetEventNotification(ev)
dev.Acquire
 DxVBLibA.DirectXEvent8 e.DXCallback
 dx8.CreateEvent
wait 0 H ev

 ARRAY(DxVBLibA.DIDEVICEOBJECTDATA) a.create(256)
 dev.GetDeviceData(a 0)
 int i
 for i 0 a.len
	 out a[i].lData
 

dev.Unacquire
   