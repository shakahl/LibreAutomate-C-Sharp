 /Menu icons test
function hwnd [dc]

int i hm=SendMessage(hwnd MN_GETHMENU 0 0)

int ivt=IsMenuVistaTheme(hm)

if(!dc) dc=GetDC(hwnd); int reldc=1

int op=SelectObject(dc m_pen)
int obr=SelectObject(dc GetStockObject(WHITE_BRUSH))

for(i 0 GetMenuItemCount(hm)) DrawItem(hwnd hm i dc ivt)

SelectObject dc op
SelectObject dc obr

if(reldc) ReleaseDC hwnd dc
