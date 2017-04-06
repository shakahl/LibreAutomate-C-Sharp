
if(_v) ret
if(_winnt<6) "not implemented for Windows XP"
#opt nowarnings 1
WINAPI2.IMMDeviceEnumerator en
WINAPI2.IMMDevice de
en._create(WINAPI2.CLSID_MMDeviceEnumerator)
en.GetDefaultAudioEndpoint(eRender eMultimedia &de)
de.Activate(uuidof(IAudioEndpointVolume) CLSCTX_ALL 0 &_v)
