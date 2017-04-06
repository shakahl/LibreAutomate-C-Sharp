 /
function# [ARRAY(POINT)&a]

if(!&a) ret &MCA_Show
int i
for(i 0 a.len) ShowWindow a[i].x iif(a[i].y SW_SHOW SW_HIDE)
