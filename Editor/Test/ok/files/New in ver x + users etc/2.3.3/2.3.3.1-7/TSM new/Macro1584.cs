 act "Notepad"
 act "cmd.exe"
 act "Dreamweaver"
 act "Word"
int w=act("Test TSM");; _s.setwintext(id(3 w))

int* p=share; p[0]=0

ARRAY(KEYEVENT) a=key("456," V)
ARRAY(KEYEVENT) b.create(1)

int i
rep 100
	for i 0 a.len
		b[0]=a[i]
		_key b
		 0.001
		 0.005
		 0.01
		wait RandomNumber*0.01

#ret
