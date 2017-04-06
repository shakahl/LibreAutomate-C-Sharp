out
 act "Notepad"
 act "t Word"
 act "WordPad"
act
key CH
 mac "noidle"
0.1

 run "winword.exe" "" "" "" 0x200
 1

 opt clip 1
int sp
 sp=100
spe sp

int i; str s
for i 0 10
	spe
	key HDSE
	spe sp
	 key D
	 0.1
	s.getsel
	out s
	