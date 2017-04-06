out
 act "Notepad"
 act "t Word"
 act "WordPad"
 act "Form"
act
0.2
key Ca
key X
 mac "noidle"

 run "winword.exe" "" "" "" 0x200
 1

 opt clip 1
spe

int i; str s
for i 0 10
	s.from("a " i "[9]aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa[]")
	 s.setsel(0 id(15 "Notepad"))
	s.setsel
	 key Y
	 0.25
	