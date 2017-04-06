 This function opens MIDI input device
function midiinid

if(tls0) midiInClose tls0
int e = midiInOpen(&tls0 midiinid &MT_MidiInProc 0 0x00030000)
if(e) goto error
e = midiInStart(tls0)
if(e) midiInClose tls0; goto error
ret
	
 error
midiInGetErrorText e _s.all(300 2) 300
mes _s.fix() "MidiIn Error" "x"
tls0 = 0
