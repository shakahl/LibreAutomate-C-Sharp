 \
function# iid FILTER&f

int+ g_keytime_F8
if(iid) g_keytime_F8=timeGetTime; ret iid
int timePressed=timeGetTime-g_keytime_F8
 ________________________

out timePressed
 more code...
 F8 0x4 //key_pressed_time
