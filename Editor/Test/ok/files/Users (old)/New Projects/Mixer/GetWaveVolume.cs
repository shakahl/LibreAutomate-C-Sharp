 /
function int&left int&right

 left and right channels, between 0 and 65535

dll winmm #waveOutSetVolume uDeviceID dwVolume
dll winmm #waveOutGetVolume uDeviceID *dwVolume

 waveOutSetVolume(0 right<<16|left)

int lr
waveOutGetVolume(0 &lr)
left=lr&0xffff
right=lr>>16
