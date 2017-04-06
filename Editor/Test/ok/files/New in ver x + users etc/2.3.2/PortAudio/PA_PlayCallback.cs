 /PortAudio record and playback
 note: this is called in separate thread.

ref portaudio "portaudio_api"
function[c]# !*inputBuffer !*outputBuffer framesPerBuffer PaStreamCallbackTimeInfo*timeInfo statusFlags paTestData*data

word* rptr = &data.recordedSamples[data.frameIndex * data.numChannels];
word* wptr = +outputBuffer;
int i;
int finished;
long framesLeft = data.maxFrameIndex - data.frameIndex;
int stereo
int nbFrame(sizeof(word)*data.numChannels) nbBuffer(framesPerBuffer*nbFrame) nbLeft(framesLeft*nbFrame)

if framesLeft < framesPerBuffer
	/* final buffer... */
	memcpy wptr rptr nbLeft
	memset wptr+nbLeft 0 nbBuffer-nbLeft
	data.frameIndex += framesLeft;
	finished = paComplete;
else
	memcpy wptr rptr nbBuffer
	data.frameIndex += framesPerBuffer;
	finished = paContinue;

ret finished;
