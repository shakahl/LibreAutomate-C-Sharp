typelib DxVBLibA {E1211242-8E94-11D1-8808-00C04FC2C603} 1.0
DxVBLibA.DirectX8 dx._create
DxVBLibA.DirectInput8 di=dx.DirectInputCreate
DxVBLibA.DirectInputDevice8 dm=di.CreateDevice("{6F1D2B61-D5A0-11CF-BFC7-444553540000}")
dm.SetCooperativeLevel(_hwndqm DxVBLibA.DISCL_NONEXCLUSIVE|DxVBLibA.DISCL_BACKGROUND)
dm.SetCommonDataFormat(DxVBLibA.DIFORMAT_KEYBOARD)
 dm.SetProperty ;;set absolute coordinates
dm.Acquire
10
 rep
	 DxVBLibA.DIKEYBOARDSTATE ks
	 dm.GetDeviceStateKeyboard(ks)
	 out ks.key[1] ;;esc
	 0.5
