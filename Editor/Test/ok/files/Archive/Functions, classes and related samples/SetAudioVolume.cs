 /
function ^v ;;v must be between 0 and 1 (eg 0.3)

 Sets audio output master volume.


#opt nowarnings 1 ;;PROPVARIANT is union containing composite types

 these declarations extracted from WINAPI2 reference file which is available in qm forum -> resources
interface# IMMDeviceCollection :IUnknown
	GetCount(*pcDevices)
	Item(nDevice IMMDevice*ppDevice)
	{0BD7A1BE-7A1A-44DB-8397-CC5392387B5E}
interface# IMMNotificationClient :IUnknown
	OnDeviceStateChanged(@*pwstrDeviceId dwNewState)
	OnDeviceAdded(@*pwstrDeviceId)
	OnDeviceRemoved(@*pwstrDeviceId)
	OnDefaultDeviceChanged(flow role @*pwstrDefaultDeviceId)
	OnPropertyValueChanged(@*pwstrDeviceId PROPERTYKEY'key)
	{7991EEC9-7E89-4D85-8390-6C703CEC60C0}
type AUDIO_VOLUME_NOTIFICATION_DATA GUID'guidEventContext bMuted FLOAT'fMasterVolume nChannels FLOAT'afChannelVolumes[1]
interface# IAudioEndpointVolumeCallback :IUnknown
	OnNotify(AUDIO_VOLUME_NOTIFICATION_DATA*pNotify)
	{657804FA-D6AD-4496-8A60-352752AF4F89}
interface# IMMDevice :IUnknown
	Activate(GUID*iid dwClsCtx PROPVARIANT*pActivationParams !**ppInterface)
	OpenPropertyStore(stgmAccess IPropertyStore*ppProperties)
	GetId(@**ppstrId)
	GetState(*pdwState)
	{D666063F-1587-4E43-81F1-B948E807363F}
interface# IAudioEndpointVolume :IUnknown
	RegisterControlChangeNotify(IAudioEndpointVolumeCallback'pNotify)
	UnregisterControlChangeNotify(IAudioEndpointVolumeCallback'pNotify)
	GetChannelCount(*pnChannelCount)
	SetMasterVolumeLevel(FLOAT'fLevelDB GUID*pguidEventContext)
	SetMasterVolumeLevelScalar(FLOAT'fLevel GUID*pguidEventContext)
	GetMasterVolumeLevel(FLOAT*pfLevelDB)
	GetMasterVolumeLevelScalar(FLOAT*pfLevel)
	SetChannelVolumeLevel(nChannel FLOAT'fLevelDB GUID*pguidEventContext)
	SetChannelVolumeLevelScalar(nChannel FLOAT'fLevel GUID*pguidEventContext)
	GetChannelVolumeLevel(nChannel FLOAT*pfLevelDB)
	GetChannelVolumeLevelScalar(nChannel FLOAT*pfLevel)
	SetMute(bMute GUID*pguidEventContext)
	GetMute(*pbMute)
	GetVolumeStepInfo(*pnStep *pnStepCount)
	VolumeStepUp(GUID*pguidEventContext)
	VolumeStepDown(GUID*pguidEventContext)
	QueryHardwareSupport(*pdwHardwareSupportMask)
	GetVolumeRange(FLOAT*pflVolumeMindB FLOAT*pflVolumeMaxdB FLOAT*pflVolumeIncrementdB)
	{5CDF2C82-841E-4546-9722-0CF74078229A}
interface# IMMDeviceEnumerator :IUnknown
	EnumAudioEndpoints(dataFlow dwStateMask IMMDeviceCollection*ppDevices)
	GetDefaultAudioEndpoint(dataFlow role IMMDevice*ppEndpoint)
	GetDevice(@*pwstrId IMMDevice*ppDevice)
	RegisterEndpointNotificationCallback(IMMNotificationClient'pClient)
	UnregisterEndpointNotificationCallback(IMMNotificationClient'pClient)
	{A95664D2-9614-4F35-A746-DE8DB63617E6}
def CLSID_MMDeviceEnumerator uuidof("{BCDE0395-E52F-467C-8E3D-C4579291692E}")
def eRender 0
def eMultimedia 0x00000001


if(v<0 or v>1) end ES_BADARG
if(_winnt>=6)
	IMMDeviceEnumerator en
	IMMDevice de
	IAudioEndpointVolume av
	en._create(CLSID_MMDeviceEnumerator)
	en.GetDefaultAudioEndpoint(eRender eMultimedia &de)
	de.Activate(uuidof(IAudioEndpointVolume) CLSCTX_ALL 0 &av)
	av.SetMasterVolumeLevelScalar(v 0)
	err+ ret
else
	int vi=(v*0x4000)+(pow(v 4)*0xBFFF)
	 out vi
	 waveOutSetVolume(0 vi<<16|vi)
	
	if(!mixerGetNumDevs) ret
	
	 get volume control id
	MIXERLINE mxl.cbStruct=sizeof(mxl)
	mxl.dwComponentType=MIXERLINE_COMPONENTTYPE_DST_SPEAKERS
	if(mixerGetLineInfo(0 &mxl MIXER_OBJECTF_MIXER|MIXER_GETLINEINFOF_COMPONENTTYPE)) ret
	MIXERCONTROL mxc
	MIXERLINECONTROLS mxlc.cbStruct=sizeof(mxlc)
	mxlc.dwLineID=mxl.dwLineID
	mxlc.dwControlType=MIXERCONTROL_CONTROLTYPE_VOLUME
	mxlc.cControls=1
	mxlc.cbmxctrl=sizeof(MIXERCONTROL)
	mxlc.pamxctrl=&mxc
	if(mixerGetLineControls(0 &mxlc MIXER_OBJECTF_MIXER|MIXER_GETLINECONTROLSF_ONEBYTYPE)) ret
	
	 set volume
	MIXERCONTROLDETAILS mxcd.cbStruct=sizeof(mxcd)
	mxcd.dwControlID=mxc.dwControlID
	mxcd.cChannels=1
	mxcd.cbDetails=sizeof(int)
	mxcd.paDetails=&vi
	if(mixerSetControlDetails(0 &mxcd MIXER_OBJECTF_MIXER|MIXER_SETCONTROLDETAILSF_VALUE)) ret
ret 1
