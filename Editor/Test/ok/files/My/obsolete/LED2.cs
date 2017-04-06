spe
ifi- "+BorDlgLED"
	int h=win
	run "Q:\PF2\LED\LEDW.EXE" "" "" "Q:\PF2\LED" 0x200
	0.5
	ifa("LED Error") key Y
	7 WC "+BorDlgLED"
	0.2
	act h; err ret
	if(_winnt=6)
		mov 600 0 "+BorDlgLED"
		men 103 "+BorDlgLED" ;;2 English-Lithuanian
	0.3
dou
0.1
opt clip 1
str s.getsel
s.trim

s.stem

if(h) 0.02
h=win("" "BorDlgLED")
act h
s.setwintext(id(100 h))

File _led.Open("$desktop$\led.txt" "a")
_led.WriteLine(s)

err+
