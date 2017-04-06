 /
function hwnd ARRAY(TRAYICONINFO)&a mask ;;mask: 1 get tooltip, 2 get hwnd, hicon, callbackMsg, callbackId

 Gets tray icon data for all icons.

 hwnd - tray toolbar control handle. If 0, the function tries to find the control.
 a - receives each icon info:
   .tooltip - tooltip text.
   .hidden - 0 if the icon is visible, 1 if hidden.
   .idCommand - toolbar control button id.
   .hwnd - handle of window that added the icon.
   .hicon - icon handle.
   .callbackMsg, .callbackId - on mouse and other events the window receives this message with this id.
 mask - must be 3 to get all info. If some info not needed, use 0, 1 or 2. It will make faster.

 EXAMPLE
 out
 #compile "__TRAYICONINFO"
 ARRAY(TRAYICONINFO) a
 EnumTrayIcons 0 a 3
 int i
 for i 0 a.len
	 TRAYICONINFO& r=a[i]
	 if(r.hidden) continue
	 RecGetWindowName r.hwnd &_s
	 out "tt='%s'  idcmd=%i  window=%s  hicon=%i  callbackId=%i  callbackMsg=%i" r.tooltip r.idCommand _s r.hicon r.callbackId r.callbackMsg


type __SHTRAYICON hwnd callbackId callbackMsg unknown[2] hIcon
type __SHTRAYICON64 %hwnd callbackId callbackMsg unknown[2] %hIcon
type TBBUTTON64 iBitmap idCommand !fsState !fsStyle !bReserved[6] %dwData %iString

a=0
if(!hwnd) hwnd=child("" "ToolbarWindow32" "+Shell_TrayWnd" 0x1); if(!hwnd) end ES_WINDOW
int i n=SendMessage(hwnd TB_BUTTONCOUNT 0 0)
if(!n) ret

__ProcessMemory m.Alloc(hwnd 4096)
int ab(m.address) as(ab+50)
TBBUTTON b

for i 0 n
	if(!SendMessage(hwnd TB_GETBUTTON i ab)) continue
	m.Read(&b sizeof(b))
	TRAYICONINFO& ti=a[]
	ti.hidden=b.fsState&TBSTATE_HIDDEN!0
	ti.idCommand=b.idCommand
	if(mask&1)
		int r=SendMessage(hwnd TB_GETBUTTONTEXTW b.idCommand 0)
		if(r<0 or r>2000 or SendMessage(hwnd TB_GETBUTTONTEXTW b.idCommand as)<0) ti.tooltip=""
		else m.ReadStr(ti.tooltip r*2 50 1)
	if(mask&2)
		if(!_win64)
			__SHTRAYICON sti
			m.ReadOther(&sti +b.dwData sizeof(sti))
			ti.callbackId=sti.callbackId
			ti.callbackMsg=sti.callbackMsg
			ti.hicon=sti.hIcon
			ti.hwnd=sti.hwnd
		else
			__SHTRAYICON64 sti64
			m.ReadOther(&sti64 +b.iString sizeof(sti64)) ;;64-bit dwData offset == 32-bit iString offset
			ti.callbackId=sti64.callbackId
			ti.callbackMsg=sti64.callbackMsg
			ti.hicon=sti64.hIcon
			ti.hwnd=sti64.hwnd
