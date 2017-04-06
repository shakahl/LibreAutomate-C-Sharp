MsgBoxAsync "[9][9][9][9][9][9]" "Progress meter"
int w=wait(30 WA win("Progress meter" "#32770"))
int c=child("" "Static" w)
int i
for(i 0 5)
	str s=i
	s.setwintext(c)
	0.5
clo w
