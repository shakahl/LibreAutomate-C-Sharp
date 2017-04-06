 /
function'WINAPI2.IAudioMeterInformation [double&waitPeriod] [int&channelCount]

if(_winver<0x600) end "need Windows Vista+"
#opt nowarnings 1

ref WINAPI2

IMMDeviceEnumerator en._create(CLSID_MMDeviceEnumerator)
IMMDevice dev; en.GetDefaultAudioEndpoint(eRender eMultimedia &dev)
IAudioMeterInformation meter; dev.Activate(IID_IAudioMeterInformation CLSCTX_ALL 0 &meter)

if &waitPeriod
	IAudioClient ac; dev.Activate(IID_IAudioClient CLSCTX_ALL 0 &ac)
	long perDef perMin; ac.GetDevicePeriod(perDef perMin)
	waitPeriod=perDef/10000000.0 ;;10 ms

if(&channelCount) meter.GetMeteringChannelCount(channelCount)

ret meter
