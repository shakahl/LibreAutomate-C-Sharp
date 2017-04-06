 /
function# hwnd [htb] [RECT&rr]

int w=GetWindowTextWidth(hwnd)+30
RECT r; GetWindowRect(hwnd &r)
if(&rr) rr=r
int wmax=r.right-r.left-90
if(htb)
	GetWindowRect(htb &r); wmax-(r.right-r.left)
if(w>wmax) w=wmax
ret w
