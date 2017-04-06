 /dlg_apihook
function hdc

#ifdef BeginBufferedAnimation

 SetViewportOrgEx(hdc 20 20 0)

SetBkMode hdc TRANSPARENT
int LH=30
RECT r; SetRect &r 5 0 300 LH
int R showR

RECT rc
 GetClipBox hdc &rc
 SetRect &rc 0 0 100 100
SetRect &rc 10 5 100 100
int hwnd=WindowFromDC(hdc)
 out BufferedPaintRenderAnimation(hwnd hdc)
BP_ANIMATIONPARAMS ap.cbSize=sizeof(ap); ap.style=BPAS_LINEAR
int hdc2 hbp=BeginBufferedAnimation(hwnd hdc &rc 0 0 &ap &_i &hdc2)
 out hbp

FillRect hdc2 &rc GetStockObject(WHITE_BRUSH)

 SetViewportOrgEx(hdc 20 20 0)
 SetViewportOrgEx(hdc2 30 30 0)

 POINT p
  GetViewportOrgEx(hdc2 &p)
 LPtoDP hdc2 &p 1
 out "%i %i" p.x p.y

 XFORM f; if(GetWorldTransform(hdc2 &f)) outb &f sizeof(f)

DAH_DrawMain hdc2

EndBufferedAnimation(hbp 1)
