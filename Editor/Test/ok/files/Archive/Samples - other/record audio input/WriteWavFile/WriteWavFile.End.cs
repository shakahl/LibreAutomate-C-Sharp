 Writes file header and closes the file.
 Error if failed.


SetFilePointer f 0 0 0

WAVFILEHEADER h
memcpy &h._RIFF "RIFF" 4
memcpy &h._WAVE "WAVE" 4
memcpy &h._fmt_ "fmt " 4
memcpy &h._data "data" 4
h.size1=16
h.size2=m_nFrames*m_nChannels*sizeof(word)
h.size=36+h.size2
h.audioFormat=1
h.nChannels=m_nChannels
h.sampleRate=m_sampleRate
h.byteRate=m_sampleRate*m_nChannels*sizeof(word)
h.blockAlign=m_nChannels*sizeof(word)
h.bitsPerSample=sizeof(word)*8

if(!WriteFile(f &h sizeof(WAVFILEHEADER) &_i 0)) end _s.dllerror
f.Close
