int w=child(mouse); if(!w) w=win(mouse)
RECT rw rc

GetWindowRect w &rw
OnScreenRect 1 &rw
1
OnScreenRect 2 &rw
0.2
GetClientRect w &rc; MapWindowPoints w 0 +&rc 2
OnScreenRect 1 &rc
1
OnScreenRect 2 &rc
