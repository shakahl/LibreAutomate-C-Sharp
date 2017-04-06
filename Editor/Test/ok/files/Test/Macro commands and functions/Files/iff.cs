iff "notepad.exe"
	out "iff notepad.exe"
iff "$media$\ding.wav"
	out "iff $media$\ding.wav"
iff- "$desktop$\test.txt"
else
	out "not iff- $desktop$\test.txt"
