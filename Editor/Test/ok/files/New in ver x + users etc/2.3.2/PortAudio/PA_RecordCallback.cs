 /PortAudio record and playback
 note: this is called in separate thread.

ref portaudio "portaudio_api"
function[c]# !*inputBuffer !*outputBuffer framesPerBuffer PaStreamCallbackTimeInfo*timeInfo statusFlags paTestData*data

word* rptr = +inputBuffer;
word* wptr = &data.recordedSamples[data.frameIndex * data.numChannels];
int framesToCalc nBytes;
int i;
int finished;
long framesLeft = data.maxFrameIndex - data.frameIndex;

if framesLeft<framesPerBuffer
	framesToCalc = framesLeft;
	finished = paComplete;
else
	framesToCalc = framesPerBuffer;
	finished = paContinue;
nBytes=framesToCalc*sizeof(word)*data.numChannels

if(inputBuffer=0) memset wptr 0 nBytes
else memcpy wptr rptr nBytes

data.frameIndex += framesToCalc;
ret finished;
