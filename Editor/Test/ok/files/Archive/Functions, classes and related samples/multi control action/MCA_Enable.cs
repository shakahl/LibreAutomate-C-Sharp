 /
function# [ARRAY(POINT)&a]

if(!&a) ret &MCA_Enable
int i
for(i 0 a.len) EnableWindow a[i].x a[i].y
