out
int w=win("" "QM_Editor")
RECT r; SetRect &r 100 0 300 300
 _____________________

RECT rw rc ri
GetClientRect w &rw; MapWindowPoints w 0 +&rw 2 ;;client rect in screen
OffsetRect &r rw.left rw.top ;;r in screen
OnScreenRect 1 &r; 1 ;;remove this
ARRAY(int) a; int i
child "" "" w 0x400 "" a
for i 0 a.len
	GetWindowRect a[i] &rc ;;control rect in screen
	if(!IntersectRect(&ri &r &rc)) continue
	outw a[i]
	OnScreenRect 0 &ri; 0.3 ;;remove this
	