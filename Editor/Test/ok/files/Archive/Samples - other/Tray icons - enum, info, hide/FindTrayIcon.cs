 /
function# hwnd $tooltip

 Returns tray toolbar control button id, or -1 if not found.
 hwnd - tray toolbar control handle.
 tooltip - tooltip. Can contain wildcard characters (* and ?).


int i n=SendMessage(hwnd TB_BUTTONCOUNT 0 0)
if(!n) ret -1

__ProcessMemory m.Alloc(hwnd 1000)
TBBUTTON b

for i 0 n
	if(!SendMessage(hwnd TB_GETBUTTON i m.address)) continue
	m.Read(&b sizeof(b))
	int t=SendMessage(hwnd TB_GETBUTTONTEXTW b.idCommand m.address+100)
	if(t<=0) continue
	str s
	m.ReadStr(s t*2 100 1)
	if(matchw(s tooltip 1)) ret b.idCommand
ret -1
