 /
function# [ARRAY(POINT)&a]

if(!&a) ret &MCA_NoTheme
dll- uxtheme #SetWindowTheme hwnd @*pszSubAppName @*pszSubIdList
if(_winver<0x501) ret

int i
for(i 0 a.len) SetWindowTheme a[i].x L"" L""
