 \
function !*&stream

if(stream) portaudio.Pa_CloseStream(stream)
portaudio.Pa_Terminate
