 act "Notepad"
 act "cmd.exe"
int w=act("Test TSM");; _s.setwintext(id(3 w))

int* p=share; p[0]=0

int i
rep 2
	for i 1 4
		_key key(F"{i+3}")
		 0.001
		 0.005
		 0.01
		wait RandomNumber*0.1
	_key key(Y)
