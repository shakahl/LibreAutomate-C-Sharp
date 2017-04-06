 /Macro399
function

ref HIDSDI "$my qm$\hidsdi.txt"

GUID hidClass
HIDSDI.HidD_GetHidGuid(&hidClass)
int hDevInfoSet=WINAPI2.SetupDiGetClassDevs(&hidClass 0 0 WINAPI2.DIGCF_PRESENT|WINAPI2.DIGCF_DEVICEINTERFACE)
 int hDevInfoSet=WINAPI2.SetupDiGetClassDevs(0 0 0 WINAPI2.DIGCF_PRESENT|WINAPI2.DIGCF_DEVICEINTERFACE|WINAPI2.DIGCF_ALLCLASSES)
WINAPI2.SP_DEVICE_INTERFACE_DATA ida.cbSize=sizeof(WINAPI2.SP_DEVICE_INTERFACE_DATA)
int i n
for i 0 100
	if(!WINAPI2.SetupDiEnumDeviceInterfaces(hDevInfoSet 0 &hidClass i &ida)) break
	WINAPI2.SetupDiGetDeviceInterfaceDetail(hDevInfoSet &ida 0 0 &n 0)
	WINAPI2.SP_DEVICE_INTERFACE_DETAIL_DATA* idet._new(n)
	idet.cbSize=5
	if(!WINAPI2.SetupDiGetDeviceInterfaceDetail(hDevInfoSet &ida idet n 0 0)) break
	lpstr path=&idet.DevicePath
	out path
	__HFile hDevice=CreateFile(path GENERIC_READ|GENERIC_WRITE FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING FILE_ATTRIBUTE_NORMAL|FILE_FLAG_OVERLAPPED 0)
	 __HFile hDevice.Create(path OPEN_EXISTING 0 FILE_SHARE_READ|FILE_SHARE_WRITE FILE_ATTRIBUTE_NORMAL|FILE_FLAG_OVERLAPPED)
	out hDevice ;;-1, because exlusively opened by Windows
	CloseHandle(hDevice)
WINAPI2.SetupDiDestroyDeviceInfoList(hDevInfoSet)
