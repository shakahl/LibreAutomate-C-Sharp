out

ref WINAPI2

double waitPeriod; int channelCount
IAudioMeterInformation meter=GetAudioMeter(waitPeriod channelCount)

rep
	FLOAT peak; meter.GetPeakValue(peak)
	out peak
	
	 ARRAY(FLOAT) af.create(channelCount)
	 meter.GetChannelsPeakValues(af.len &af[0])
	 int i
	 for(i 0 af.len)  out af[i]
	
	wait waitPeriod
	1 ;;debug
