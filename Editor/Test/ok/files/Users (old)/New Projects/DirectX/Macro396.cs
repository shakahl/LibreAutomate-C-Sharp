typelib DxVBLibA {E1211242-8E94-11D1-8808-00C04FC2C603} 1.0
DxVBLibA.DirectX8 dx._create
DxVBLibA.DirectInput8 di=dx.DirectInputCreate
DxVBLibA.DirectInputDevice8 dm=di.CreateDevice("{6F1D2B60-D5A0-11CF-BFC7-444553540000}")
 DxVBLibA.DirectXEvent8 de._create
 int he=dx.CreateEvent(de)
 dm.SetEventNotification(he)
 wait 0 H he
dm.SetCooperativeLevel(_hwndqm DxVBLibA.DISCL_NONEXCLUSIVE|DxVBLibA.DISCL_BACKGROUND)
dm.SetCommonDataFormat(DxVBLibA.DIFORMAT_MOUSE2)
 dm.SetProperty ;;set absolute coordinates
dm.Acquire
rep
	DxVBLibA.DIMOUSESTATE2 ms
	dm.GetDeviceStateMouse2(ms)
	out "%i %i" ms.lX ms.lY
	if(ms.Buttons[0]) out "left"
	if(ms.Buttons[1]) out "right"
	if(ms.Buttons[2]) out "middle"
	if(ms.Buttons[3]) out "x1"
	if(ms.Buttons[4]) out "x2"
	0.05
