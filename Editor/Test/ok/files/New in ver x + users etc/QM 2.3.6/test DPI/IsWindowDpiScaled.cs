 /Macro2115
function! hwnd

if(_winnt<6) ret

dll- user32
	#LogicalToPhysicalPoint hWnd POINT*lpPoint
	#LogicalToPhysicalPointForPerMonitorDPI hWnd POINT*lpPoint

 todo: DwmIs...

int+ ___dpi
if(!___dpi)
	int hdc=GetDC(0)
	___dpi=GetDeviceCaps(hdc LOGPIXELSX)
	ReleaseDC 0 hdc
if(___dpi=96) ret
 out ___dpi

hwnd=GetAncestor(hwnd 2); if(!hwnd) ret
 int is=GetProp(hwnd "qm_dpi_scaled") ;;same speed with or without
 if(is) ret is-1
int is

RECT r
if(GetWindowRect(hwnd, &r))
	r.right-1; r.bottom-1
	POINT p.x=r.right; p.y=r.bottom
	if(iif(_winver>=0x603 LogicalToPhysicalPointForPerMonitorDPI(hwnd, &p) LogicalToPhysicalPoint(hwnd, &p))) is=0!memcmp(&r.right &p 8)

 SetProp(hwnd "qm_dpi_scaled" is+1)
ret is
