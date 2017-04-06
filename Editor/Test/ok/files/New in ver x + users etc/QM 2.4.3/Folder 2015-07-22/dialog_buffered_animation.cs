\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 3 Button 0x54032000 0x0 168 8 48 14 "Client"
 4 Button 0x54032000 0x0 168 32 48 14 "Nonclient"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = ""
str sb3
if(!ShowDialog(dd &sub.DlgProc 0 _hwndqm)) ret


#sub DlgProc
function# hWnd message wParam lParam

OutWinMsg message wParam lParam
int- g_fCurrentState g_fNewState
sel message
	case WM_INITDIALOG
	g_fCurrentState=1; g_fNewState=1
	BufferedPaintInit
	
	SetWinStyle hWnd WS_THICKFRAME 1|8
	
	case WM_NCCALCSIZE
	RECT& r=+lParam
	r.bottom=r.top+r.bottom/2
	
	case WM_DESTROY
	BufferedPaintUnInit
	
	case WM_PAINT
	sub.OnPaint(hWnd)
	
	case WM_NCPAINT
	sub.OnNcPaint(hWnd)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	g_fNewState = !g_fCurrentState
	RedrawWindow hWnd 0 0 RDW_INVALIDATE
	case 4
	g_fNewState = !g_fCurrentState
	RedrawWindow hWnd 0 0 RDW_INVALIDATE|RDW_FRAME
	RedrawWindow hWnd 0 0 RDW_VALIDATE
ret 1


#sub Paint
function hWnd hdc RECT&rc state
FillRect(hdc, &rc, GetStockObject(WHITE_BRUSH));
int hIcon = LoadIcon(0, +iif(state IDI_APPLICATION IDI_ERROR));
if (hIcon)
	DrawIcon(hdc, rc.left, rc.top, hIcon);
	DestroyIcon(hIcon);


#sub OnPaint2
function hWnd hdc !nonClinet

def ANIMATION_DURATION 500

int- g_fCurrentState g_fNewState
// See if this paint was generated by a soft-fade animation
if(_winver>=0x600 and BufferedPaintRenderAnimation(hWnd, hdc)) out "rendered"; ret

RECT rc;
if nonClinet
	GetWindowRect hWnd &rc; OffsetRect &rc -rc.left -rc.top
	rc.left=20; rc.top=rc.bottom-50
else
	GetClientRect(hWnd, &rc);
	rc.right/2

if(_winver<0x600)
	out "API unavailable"
	 gNoAnim
	sub.Paint(hWnd, hdc, rc, g_fCurrentState)
	ret

out "we render"
BP_PAINTPARAMS paintParams.cbSize=sizeof(paintParams)
if(nonClinet) paintParams.dwFlags=BPPF_NONCLIENT
BP_ANIMATIONPARAMS animParams;
animParams.cbSize = sizeof(BP_ANIMATIONPARAMS);
animParams.style = BPAS_LINEAR;

// Check if animation is needed. If not set dwDuration to 0
animParams.dwDuration = iif((g_fCurrentState != g_fNewState) ANIMATION_DURATION  0);

int hdcFrom, hdcTo;
int hbpAnimation = BeginBufferedAnimation(hWnd, hdc, &rc, BPBF_COMPATIBLEBITMAP, &paintParams, &animParams, &hdcFrom, &hdcTo);
if(!hbpAnimation) out "failed"; goto gNoAnim
if(hdcFrom) sub.Paint(hWnd, hdcFrom, rc, g_fCurrentState);
if(hdcTo) sub.Paint(hWnd, hdcTo, rc, g_fNewState);

g_fCurrentState = g_fNewState;
EndBufferedAnimation(hbpAnimation, TRUE);


#sub OnPaint
function hWnd

PAINTSTRUCT ps;
int hdc = BeginPaint(hWnd, &ps);
sub.OnPaint2 hWnd hdc 0
EndPaint(hWnd, &ps);


#sub OnNcPaint
function hWnd
 out "ncp"
__Hdc dc=GetWindowDC(hWnd)
sub.OnPaint2 hWnd dc 1
