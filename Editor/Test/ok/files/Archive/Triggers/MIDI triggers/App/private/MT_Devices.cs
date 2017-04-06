 This function fills listbox with names of MIDI devices
function# hlist deviceid

int i n=midiInGetNumDevs
MIDIINCAPS mc

for i 0 n
	midiInGetDevCaps(i &mc sizeof(MIDIINCAPS))
	CB_Add hlist &mc.szPname
if(n) SendMessage hlist CB_SETCURSEL deviceid 0
ret n