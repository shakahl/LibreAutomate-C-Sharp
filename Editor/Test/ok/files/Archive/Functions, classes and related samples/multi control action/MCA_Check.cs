 /
function# [ARRAY(POINT)&a]

if(!&a) ret &MCA_Check
int i
for(i 0 a.len) SendMessage a[i].x BM_SETCHECK a[i].y 0
