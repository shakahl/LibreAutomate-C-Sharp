def PORTAUDIO_H
type PaDeviceIndex = #
type PaDeviceInfo structVersion $name hostApi maxInputChannels maxOutputChannels ^defaultLowInputLatency ^defaultLowOutputLatency ^defaultHighInputLatency ^defaultHighOutputLatency ^defaultSampleRate
type PaError = #
type PaErrorCode = #
type PaHostApiIndex = #
type PaHostApiInfo structVersion type $name deviceCount defaultInputDevice defaultOutputDevice
type PaHostApiTypeId = #
type PaHostErrorInfo hostApiType errorCode $errorText
type PaSampleFormat = #
type PaStream :void'_
 ;;type PaStream = void
type PaStreamCallback = #
 ;;function# !*input !*output frameCount PaStreamCallbackTimeInfo*timeInfo statusFlags !*userData
type PaStreamCallbackFlags = #
type PaStreamCallbackResult = #
type PaStreamCallbackTimeInfo ^inputBufferAdcTime ^currentTime ^outputBufferDacTime
type PaStreamFinishedCallback = #
 ;;function !*userData
type PaStreamFlags = #
type PaStreamInfo structVersion ^inputLatency ^outputLatency ^sampleRate
type PaStreamParameters device channelCount sampleFormat ^suggestedLatency !*hostApiSpecificStreamInfo
type PaTime = ^
dll "$qm$\portaudio_x86" #Pa_AbortStream !*stream
dll "$qm$\portaudio_x86" #Pa_CloseStream !*stream
dll "$qm$\portaudio_x86" #Pa_GetDefaultHostApi
dll "$qm$\portaudio_x86" #Pa_GetDefaultInputDevice
dll "$qm$\portaudio_x86" #Pa_GetDefaultOutputDevice
dll "$qm$\portaudio_x86" #Pa_GetDeviceCount
dll "$qm$\portaudio_x86" PaDeviceInfo*Pa_GetDeviceInfo device
dll "$qm$\portaudio_x86" $Pa_GetErrorText errorCode
dll "$qm$\portaudio_x86" #Pa_GetHostApiCount
dll "$qm$\portaudio_x86" PaHostApiInfo*Pa_GetHostApiInfo hostApi
dll "$qm$\portaudio_x86" PaHostErrorInfo*Pa_GetLastHostErrorInfo
dll "$qm$\portaudio_x86" #Pa_GetSampleSize format
dll "$qm$\portaudio_x86" ^Pa_GetStreamCpuLoad !*stream
dll "$qm$\portaudio_x86" PaStreamInfo*Pa_GetStreamInfo !*stream
dll "$qm$\portaudio_x86" #Pa_GetStreamReadAvailable !*stream
dll "$qm$\portaudio_x86" ^Pa_GetStreamTime !*stream
dll "$qm$\portaudio_x86" #Pa_GetStreamWriteAvailable !*stream
dll "$qm$\portaudio_x86" #Pa_GetVersion
dll "$qm$\portaudio_x86" $Pa_GetVersionText
dll "$qm$\portaudio_x86" #Pa_HostApiDeviceIndexToDeviceIndex hostApi hostApiDeviceIndex
dll "$qm$\portaudio_x86" #Pa_HostApiTypeIdToHostApiIndex type
dll "$qm$\portaudio_x86" #Pa_Initialize
dll "$qm$\portaudio_x86" #Pa_IsFormatSupported PaStreamParameters*inputParameters PaStreamParameters*outputParameters ^sampleRate
dll "$qm$\portaudio_x86" #Pa_IsStreamActive !*stream
dll "$qm$\portaudio_x86" #Pa_IsStreamStopped !*stream
dll "$qm$\portaudio_x86" #Pa_OpenDefaultStream !**stream numInputChannels numOutputChannels sampleFormat ^sampleRate framesPerBuffer *streamCallback !*userData
 ;;streamCallback: function# !*input !*output frameCount PaStreamCallbackTimeInfo*timeInfo statusFlags !*userData
dll "$qm$\portaudio_x86" #Pa_OpenStream !**stream PaStreamParameters*inputParameters PaStreamParameters*outputParameters ^sampleRate framesPerBuffer streamFlags *streamCallback !*userData
 ;;streamCallback: function# !*input !*output frameCount PaStreamCallbackTimeInfo*timeInfo statusFlags !*userData
dll "$qm$\portaudio_x86" #Pa_ReadStream !*stream !*buffer frames
dll "$qm$\portaudio_x86" #Pa_SetStreamFinishedCallback !*stream *streamFinishedCallback
 ;;streamFinishedCallback: function !*userData
dll "$qm$\portaudio_x86" Pa_Sleep msec
dll "$qm$\portaudio_x86" #Pa_StartStream !*stream
dll "$qm$\portaudio_x86" #Pa_StopStream !*stream
dll "$qm$\portaudio_x86" #Pa_Terminate
dll "$qm$\portaudio_x86" #Pa_WriteStream !*stream !*buffer frames
def paAL 9
def paALSA 8
def paASIO 3
def paAbort 2
def paAudioScienceHPI 14
def paBadBufferPtr 0xFFFFD90C
def paBadIODeviceCombination 0xFFFFD8F7
def paBadStreamPtr 0xFFFFD8FC
def paBeOS 10
def paBufferTooBig 0xFFFFD8F9
def paBufferTooSmall 0xFFFFD8FA
def paCanNotReadFromACallbackStream 0xFFFFD907
def paCanNotReadFromAnOutputOnlyStream 0xFFFFD909
def paCanNotWriteToACallbackStream 0xFFFFD908
def paCanNotWriteToAnInputOnlyStream 0xFFFFD90A
def paClipOff 0x00000001
def paComplete 1
def paContinue 0
def paCoreAudio 5
def paCustomFormat 0x00010000
def paDeviceUnavailable 0xFFFFD8FF
def paDirectSound 1
def paDitherOff 0x00000002
def paFloat32 0x00000001
def paFormatIsSupported 0
def paFramesPerBufferUnspecified 0
def paHostApiNotFound 0xFFFFD905
def paInDevelopment 0
def paIncompatibleHostApiSpecificStreamInfo 0xFFFFD900
def paIncompatibleStreamHostApi 0xFFFFD90B
def paInputOverflow 0x00000002
def paInputOverflowed 0xFFFFD903
def paInputUnderflow 0x00000001
def paInsufficientMemory 0xFFFFD8F8
def paInt16 0x00000008
def paInt24 0x00000004
def paInt32 0x00000002
def paInt8 0x00000010
def paInternalError 0xFFFFD8FE
def paInvalidChannelCount 0xFFFFD8F2
def paInvalidDevice 0xFFFFD8F4
def paInvalidFlag 0xFFFFD8F5
def paInvalidHostApi 0xFFFFD906
def paInvalidSampleRate 0xFFFFD8F3
def paJACK 12
def paMME 2
def paNeverDropInput 0x00000004
def paNoDevice 0xFFFFFFFF
def paNoError 0
def paNoFlag 0
def paNonInterleaved 0x80000000
def paNotInitialized 0xFFFFD8F0
def paNullCallback 0xFFFFD8FB
def paOSS 7
def paOutputOverflow 0x00000008
def paOutputUnderflow 0x00000004
def paOutputUnderflowed 0xFFFFD904
def paPlatformSpecificFlags 0xFFFF0000
def paPrimeOutputBuffersUsingStreamCallback 0x00000008
def paPrimingOutput 0x00000010
def paSampleFormatNotSupported 0xFFFFD8F6
def paSoundManager 4
def paStreamIsNotStopped 0xFFFFD902
def paStreamIsStopped 0xFFFFD901
def paTimedOut 0xFFFFD8FD
def paUInt8 0x00000020
def paUnanticipatedHostError 0xFFFFD8F1
def paUseHostApiSpecificDeviceSpecification 0xFFFFFFFE
def paWASAPI 13
def paWDMKS 11
