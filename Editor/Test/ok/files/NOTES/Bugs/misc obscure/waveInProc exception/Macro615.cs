 Was exception when returning from waveInProc when it was called by Windows on open.
 Later started to work well.
 In C++ worked well always.

WAVEFORMATEX wf.cbSize=sizeof(wf)
wf.wFormatTag=WAVE_FORMAT_PCM
wf.nChannels=1 ;;mono
wf.nSamplesPerSec=44.1*1000 ;;44.1 kHz
wf.wBitsPerSample=16
wf.nBlockAlign=wf.nChannels*wf.wBitsPerSample/8
wf.nAvgBytesPerSec=wf.nSamplesPerSec*wf.nBlockAlign

 out GetCurrentThreadId
int hwi
int rc=waveInOpen(&hwi WAVE_MAPPER &wf &waveInProc 0 CALLBACK_FUNCTION)
if(rc) mes- rc






1




waveInClose hwi
