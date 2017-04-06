 /dlg_apihook
function hdc

 SetBkMode hdc TRANSPARENT
int LH=30
RECT r; SetRect &r 0 0 100 LH/2
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=20; dt.iRightMargin=10
int R showR

BSTR s
SetGraphicsMode(hdc GM_ADVANCED)
 SetMapMode(hdc MM_LOENGLISH)
XFORM f
f.eDx=200 ;;move
 f.eDx=-200 ;;move
f.eDy=300 ;;move

f.eM11=1; f.eM22=1 ;;normal
 f.eM11=2; f.eM22=4 ;;scale
 f.eM11=0.8660; f.eM12=0.5000; f.eM21=-0.5000; f.eM22=0.8660 ;;rotate 30 degrees
 f.eM11=1; f.eM12=0; f.eM21=-f.eM12; f.eM22=-f.eM11 ;;flip vertically
 f.eM11=-1; f.eM12=0; f.eM21=-f.eM12; f.eM22=-f.eM11 ;;flip horz
 f.eM11=0; f.eM12=1; f.eM21=-f.eM12; f.eM22=-f.eM11 ;;rotate 90 closckwise
 f.eM11=0; f.eM12=-1; f.eM21=-f.eM12; f.eM22=-f.eM11 ;;rotate 90 -closckwise
 f.eM12=1 ;;shear
f.eM22=-1 ;;reflect (flip vertically)

if(!SetWorldTransform(hdc &f)) out "failed"; ret

 RECT rv; SetRect &rv 0 0 1000 1000; LPtoDP hdc +&rv 2; zRECT rv
 SIZE zw; if(GetViewportExtEx(hdc &zw)) out "%i %i" zw.cx zw.cy

 DPtoLP(hdc +&r 2)

 DAH_Uniscribe hdc
  DAH_UniscribeSSAnalyseSSOut hdc "CALL SSAnalyse/SSOut" r
 ret

s="SWT 0"
R=ExtTextOutW(hdc r.left r.top 0 0 s s.len 0); if(showR) out R
OffsetRect &r 0 LH
s="SWT 1"
R=ExtTextOutW(hdc r.left r.top ETO_CLIPPED &r s s.len 0); if(showR) out R
OffsetRect &r 0 LH*2

s="SWT 2"
R=DrawTextExW(hdc s -1 &r DT_NOCLIP 0); if(showR) out R
OffsetRect &r 0 LH
s="SWT 3"
R=DrawTextExW(hdc s -1 &r 0 0); if(showR) out R
OffsetRect &r 0 LH
s="SWT 4"
dt.iLeftMargin=15; dt.iRightMargin=5
R=DrawTextExW(hdc s -1 &r 0 &dt); if(showR) out R

DAH_GDIP_SetMapMode hdc r.top -1 1 ;;applies, draws flipped horizontally

 XFORM _f; _f.eM11=1; _f.eM22=1 ;;normal
 SetWorldTransform(hdc &_f)
