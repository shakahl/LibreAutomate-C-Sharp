 /dlg_apihook
function hdc

 SetBkMode hdc TRANSPARENT
int LH=-300
RECT r; SetRect &r 100 -100 1000 LH
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=20; dt.iRightMargin=10
int R showR

BSTR s="SetMapMode"

SetMapMode hdc MM_TWIPS
 SetMapMode hdc MM_HIENGLISH
__Font f.Create("Arial" 160)
int of=SelectObject(hdc f)

 RECT rv; SetRect &rv 0 0 1000 1000; LPtoDP hdc +&rv 2; zRECT rv
 SIZE zw; if(GetViewportExtEx(hdc &zw)) out "%i %i" zw.cx zw.cy

 R=ExtTextOutW(hdc r.left r.top 0 0 s s.len 0); if(showR) out R
R=ExtTextOutW(hdc r.left r.top ETO_CLIPPED &r s s.len 0); if(showR) out R
OffsetRect &r 0 LH*2

 R=DrawTextExW(hdc s -1 &r DT_NOCLIP 0); if(showR) out R
R=DrawTextExW(hdc s -1 &r 0 0); if(showR) out R
OffsetRect &r 0 LH*2

dt.iLeftMargin=1000; dt.iRightMargin=500; r.right=2000
R=DrawTextExW(hdc s -1 &r 0 &dt); if(showR) out R

DAH_GDIP_SetMapMode hdc 0 30 -30 ;;applies, draws flipped vertically

SelectObject(hdc of)
 SetMapMode hdc MM_TEXT
