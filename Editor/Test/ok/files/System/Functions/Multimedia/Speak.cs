 \
function# $text [flags] [$voice] [rate] [volume] ;;flags: 1 sync, 2 queue, 4 file, 8 XML, 16 not XML, 64 punctuation;  rate: -10 to 10; volume: 0 to 100

 Speaks text.

 text - text to speak.
 flags:
   1 - synchronous (wait until stops speaking).
   2 - if already speaking, wait until stops before speaking new text. Starting from QM 2.2.0, previous voice parameters are not inherited.
   4 - text is file containing text to speak.
   8 - text is XML. SAPI XML tags in text can be used to control text parameters while speaking. If flag 8 or 16 are not used, text is interpreted as XML if begins with <.
   16 - text is not XML, even if begins with <.
   64 - pronounce punctuation characters.
 voice - one of voices that you can see in Control Panel -> Speech. If voice is omitted, "" or does not exist, then speaks voice that is set in Control Panel.
 rate - speed adjustment. Can be value from -10 to 10. Default is 0.
 volume - volume, 1 to 100. Value 0 sets default volume.

 REMARKS
 Only one voice can speak at a time. If you call Speak while already speaking (using this function), stops speaking previous text, or, if flag 2 is used, waits until previous text ends. To stop speaking previous text without speaking new, call SpeakStop or Speak "".
 If flag 1 is used, returns 1 on success or -1 on error. If not, returns 1. Returns 0 if SAPI 5 is not installed.
 Note: By default does not wait until finished speaking. In exe the sound stops as soon as exe process ends.

 EXAMPLE
 Speak "text"


typelib __SpeechLib {C866CA3A-32F7-11D2-9602-00C04F8EE628} 5.0
#err 0

__Handle+ ___speak_stop_event
if(!___speak_stop_event) ___speak_stop_event=CreateEvent(0 0 0 0)
if(flags&2=0) PulseEvent ___speak_stop_event
if(empty(text)) ret 1

if(flags&1=0) mac "Speak" "" text flags|1 voice rate volume; ret 1

__SpeechLib.SpVoice spv._create

if(flags&4) text=_s.expandpath(text)
if(rate) spv.Rate=rate
if(volume) spv.Volume=volume
if(_winnt>=6) waveOutSetVolume 0 0xffffffff ;;on Vista+, this sets max wave out volume for qm only, because may be 0 initially

if !empty(voice)
	__SpeechLib.SpObjectToken vo
	foreach vo spv.GetVoices("" "")
		str sv=vo.GetDescription(0)
		if(sv~voice) spv.Voice=vo; break

spv.Speak(text flags|1)
opt waitmsg -1
wait 0 HM spv.SpeakCompleteEvent ___speak_stop_event

ret 1
err+ ret -1
