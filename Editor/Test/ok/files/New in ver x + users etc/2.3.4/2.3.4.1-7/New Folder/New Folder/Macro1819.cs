int w=child(mouse); if(!w) w=win(mouse)
outw w
out GetWindowThreadProcessId(w 0)
RECT r; GetWindowRect w &r; zRECT r
