out

 PKEY_Device_FriendlyName does not exist in Windows SDK headers. Exists DEVPKEY_Device_FriendlyName, and it works.
 C++ definition from SDK devpkey.h:
 DEFINE_DEVPROPKEY(DEVPKEY_Device_FriendlyName, 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);
PROPERTYKEY pk.pid=14; memcpy &pk.fmtid uuidof("{a45c254e-df1c-4efd-8020-67d146a850e0}") sizeof(GUID)

IMMDeviceEnumerator pEnumerator
IMMDeviceCollection pCollection
IMMDevice pEndpoint
IPropertyStore pProps
word* pwszID

pEnumerator._create(CLSID_MMDeviceEnumerator)
pEnumerator.EnumAudioEndpoints(eRender DEVICE_STATE_ACTIVE &pCollection)
int count
pCollection.GetCount(&count)
int i
for i 0 count
	pCollection.Item(i &pEndpoint)
	pEndpoint.GetId(&pwszID)
	pEndpoint.OpenPropertyStore(STGM_READ, &pProps)
	PROPVARIANT varName
	pProps.GetValue(pk, &varName)
	str s1.ansi(varName.pwszVal) s2.ansi(pwszID)
	CoTaskMemFree(pwszID); pwszID=0; PropVariantClear &varName; pProps=0; pEndpoint=0
	out F"Endpoint {i}: ''{s1}'' ({s2})"
