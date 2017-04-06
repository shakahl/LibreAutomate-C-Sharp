 Create integer variable h and get QM code edit control handle
int h = id(2210 "Quick Macros")
 Create integer variable y and assign 10
int y = 10
 Click 5 times, each time adding 16 to y
rep 5
	lef 50 y h
	0.5 ;;wait 0.5 s
	y + 16
 Select text
lef+ 20 15 h
lef- 20 80 h
 Restore mouse cursor position
mou
