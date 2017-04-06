function $wavFile nChannels sampleRate

 Creates and opens new empty file where you'll write audio data.
 Error if failed.

 wavFile - wav file.
 nChannels - 1 (mono) or 2 (stereo).
 sampleRate - sample rate, Hz. Eg 44100.


f.Create(wavFile CREATE_ALWAYS); err end _error
SetFilePointer f sizeof(WAVFILEHEADER) 0 0 ;;for header

m_nChannels=nChannels
m_sampleRate=sampleRate
m_nFrames=0
