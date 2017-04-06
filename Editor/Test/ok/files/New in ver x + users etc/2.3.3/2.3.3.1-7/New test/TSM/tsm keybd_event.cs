out
act "Notepad"
key CaX; 0.5
 str s="MOO[13]"
str s="MOO TEXT[13]"
 str s="PAS[13]"
 str s="AKA[13]"
rep 5
	int i
	for i 0 s.len
		int k=s[i]
		keybd_event k 0 0 0
		keybd_event k 0 KEYEVENTF_KEYUP 0
		0.005

#ret
