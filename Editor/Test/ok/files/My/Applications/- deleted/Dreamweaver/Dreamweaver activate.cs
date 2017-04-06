int x y cx cy; GetWinXY TriggerWindow x y cx cy
int h=win(x-1 y-1); if(!h) ret
if(wintest(h "Dreamweaver")) ret
act "Dreamweaver"
