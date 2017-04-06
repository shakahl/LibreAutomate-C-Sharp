function @*data nFrames

 Writes audio data to the file.
 Can be called multiple times.
 Error if failed.

 data - sound data as array of frames. A frame is 1 (if mono) or 2 (if stereo) 16-bit sample(s).
 nFrames - number of frames.


if(!WriteFile(f data nFrames*m_nChannels*sizeof(word) &_i 0)) end _s.dllerror
m_nFrames+nFrames
