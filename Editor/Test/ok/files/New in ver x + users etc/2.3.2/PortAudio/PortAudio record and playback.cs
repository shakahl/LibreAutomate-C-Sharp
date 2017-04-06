out

int numSeconds=5 ;;time, s
int sampleRate=44100 ;;quality, Hz
int numChannels=2 ;;1 or 2
str sFile="$desktop$\recorded.wav" ;;save here (optional)

/*******************************************************************/

ref portaudio "portaudio_api"

type paTestData frameIndex maxFrameIndex word*recordedSamples numChannels

PaStreamParameters inputParameters, outputParameters;
byte* stream;
int e i
int totalFrames numSamples numBytes;
PaDeviceInfo* pDI
str sMem
paTestData data;

e=Pa_Initialize; if(e) end Pa_GetErrorText(e)
atend PA_Atend &stream

totalFrames = numSeconds * sampleRate;
data.maxFrameIndex = totalFrames;
numSamples = totalFrames * numChannels;
numBytes = numSamples * sizeof(word);
data.recordedSamples = sMem.all(numBytes 2 0)
data.numChannels=numChannels

/* Record some audio. -------------------------------------------- */

inputParameters.device = Pa_GetDefaultInputDevice();
inputParameters.channelCount = numChannels;
inputParameters.sampleFormat = paInt16;
pDI=Pa_GetDeviceInfo(inputParameters.device)
inputParameters.suggestedLatency = pDI.defaultLowInputLatency;

e=Pa_OpenStream(&stream, &inputParameters, 0, sampleRate, 1024, paClipOff, &PA_RecordCallback, &data); if(e) goto done;

e=Pa_StartStream(stream); if(e) goto done;
out("recording...");

rep
	e=Pa_IsStreamActive(stream); if(e!=1) break
	0.1
	 out("index = %i", data.frameIndex);
if(e<0) goto done;

e=Pa_CloseStream(stream); if(e) goto done;
stream=0

/* Measure amplitude. */
#if 1
int _max v; long average
for(i 0 numSamples)
	v = abs(ConvertSignedUnsigned(data.recordedSamples[i] 2))
	if(v > _max) _max = v;
	average+v;

average / numSamples;
out "Max amplitude %i %%.  Average %i %%.", MulDiv(_max 100 0x8000) MulDiv(average 100 0x8000)
#endif

/* Write recorded data to a file. */
#if 1
type WAVFILEHEADER _RIFF size _WAVE _fmt_ size1 @audioFormat @nChannels sampleRate byteRate @blockAlign @bitsPerSample _data size2
WAVFILEHEADER h
memcpy &h._RIFF "RIFF" 4
memcpy &h._WAVE "WAVE" 4
memcpy &h._fmt_ "fmt " 4
memcpy &h._data "data" 4
h.size1=16
h.size2=totalFrames*numChannels*sizeof(word)
h.size=36+h.size2
h.audioFormat=1
h.nChannels=numChannels
h.sampleRate=sampleRate
h.byteRate=h.sampleRate*h.nChannels*sizeof(word)
h.blockAlign=h.nChannels*sizeof(word)
h.bitsPerSample=sizeof(word)*8

FILE* fid;
fid = fopen(_s.expandpath("$desktop$\recorded.wav"), "wb");
if(fid == 0)
	out("Could not open file.");
else
	fwrite(+&h sizeof(WAVFILEHEADER) 1 fid)
	fwrite(+data.recordedSamples, numChannels*sizeof(word), totalFrames, fid);
	fclose(fid);
	out("<>Saved to <link>$desktop$\recorded.wav</link>");

 if have WriteWavFile class, can use this instead
 #compile "__WriteWavFile"
 if !empty(sFile)
	 WriteWavFile wav.Begin(sFile numChannels sampleRate)
	 wav.Write(data.recordedSamples totalFrames)
	 wav.End
	 out "<>Saved to <link>%s</link>" sFile
#endif

/* Playback recorded data.  -------------------------------------------- */

data.frameIndex = 0;

outputParameters.device = Pa_GetDefaultOutputDevice();
outputParameters.channelCount = numChannels;
outputParameters.sampleFormat = paInt16;
pDI=Pa_GetDeviceInfo(outputParameters.device)
outputParameters.suggestedLatency = pDI.defaultLowOutputLatency;

e=Pa_OpenStream(&stream, 0, &outputParameters, sampleRate, 1024, paClipOff, &PA_PlayCallback, &data); if(e) goto done;

e=Pa_StartStream(stream); if(e) goto done;
out("playback...");

rep
	e=Pa_IsStreamActive(stream); if(e!=1) break
	0.1
if(e<0) goto done

e=Pa_CloseStream(stream); if(e) goto done;
stream=0
out("Done.");

/* Done.  -------------------------------------------- */

 done
if(e) end Pa_GetErrorText(e)
