typelib DxVBLibA {E1211242-8E94-11D1-8808-00C04FC2C603} 1.0
DxVBLibA.DirectX8 dx._create
DxVBLibA.DirectInput8 di=dx.DirectInputCreate
DxVBLibA.DirectInputDevice8 dm=di.CreateDevice("{6F1D2B60-D5A0-11CF-BFC7-444553540000}")
dm.SetCooperativeLevel(_hwndqm DxVBLibA.DISCL_NONEXCLUSIVE|DxVBLibA.DISCL_BACKGROUND)
dm.SetCommonDataFormat(DxVBLibA.DIFORMAT_MOUSE2)
dm.Acquire

mou 0.5 0.5

DxVBLibA.DirectInputDeviceObjectInstance oi=dm.GetObjectInfo(0 DxVBLibA.DIPH_BYOFFSET)
int oid=oi.GetType>>8&255
out oid

ARRAY(DxVBLibA.DIDEVICEOBJECTDATA) a.create(1)
a[0].lData=128
a[0].lOfs=oid

dm.SendDeviceData(1 a DxVBLibA.DISDD_DEFAULT) ;;0x80004001, Not implemented
0.5
a[0].lData=0
dm.SendDeviceData(1 a DxVBLibA.DISDD_DEFAULT)
