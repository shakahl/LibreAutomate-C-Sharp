 \
function# iid FILTER&f

if(iid) ifk((2)) ret iid; else ret -2

key (VK_BROWSER_BACK)

POINT p; GetCursorPos &p
int h=CreateWindowEx(WS_EX_TOOLWINDOW|WS_EX_TOPMOST "#32770" 0 WS_POPUP p.x-100 p.y-100 200 200 0 0 _hinst 0)
hid- h
outw h
opt waitmsg 1
rep() 0.1; ifk-((2)) 0.1; break
DestroyWindow h
