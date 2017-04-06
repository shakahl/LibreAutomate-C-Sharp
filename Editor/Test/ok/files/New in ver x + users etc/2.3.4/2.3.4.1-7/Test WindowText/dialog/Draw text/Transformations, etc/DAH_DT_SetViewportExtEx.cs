 /dlg_apihook
function hdc

 SetBkMode hdc TRANSPARENT
int LH=20
RECT r
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=20; dt.iRightMargin=10
int R showR

BSTR s
 SetViewportOrgEx(hdc 100 100 0)
SetMapMode hdc MM_ANISOTROPIC
 SIZE zvp; SetViewportExtEx(hdc 1 1 &zvp); SetRect &r 10 0 100 LH/2
SIZE zvp; SetViewportExtEx(hdc 2 2 &zvp); SetRect &r 10 0 100 LH/2
 SIZE zvp; SetViewportExtEx(hdc -1 1 &zvp); SetRect &r -10 0 -100 LH/2
 SIZE zvp; SetViewportExtEx(hdc -2 2 &zvp); SetRect &r -10 0 -100 LH/2
 SIZE zvp; SetViewportExtEx(hdc 1 -1 &zvp); LH=-LH; SetRect &r 10 0 100 LH/2
 SIZE zwp; SetWindowExtEx(hdc -2 2 &zwp)

 RECT rv; SetRect &rv 0 0 1000 1000; LPtoDP hdc +&rv 2; zRECT rv
 SIZE zw; if(GetViewportExtEx(hdc &zw)) out "%i %i" zw.cx zw.cy

s="ETO"; R=ExtTextOutW(hdc r.left r.top 0 0 s s.len 0); OffsetRect &r 0 LH
s="ETO clipped"; R=ExtTextOutW(hdc r.left r.top ETO_CLIPPED &r s s.len 0); OffsetRect &r 0 LH*2

R=DrawTextExW(hdc L"DTE" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE clipped" -1 &r 0 0); OffsetRect &r 0 LH
dt.iLeftMargin=15; dt.iRightMargin=5
R=DrawTextExW(hdc L"DTE margins cl" -1 &r 0 &dt); OffsetRect &r 0 LH

 zRECT r; RECT rr=r; LPtoDP hdc +&rr 2; zRECT rr
R=DrawTextExW(hdc L"DTE r" -1 &r DT_NOCLIP|DT_RIGHT 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE r cl" -1 &r DT_RIGHT 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE c" -1 &r DT_NOCLIP|DT_CENTER 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE c cl" -1 &r DT_CENTER 0); OffsetRect &r 0 LH
r.bottom+LH*2
R=DrawTextExW(hdc L"DTE vc" -1 &r DT_NOCLIP|DT_VCENTER|DT_SINGLELINE 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE b" -1 &r DT_NOCLIP|DT_BOTTOM|DT_SINGLELINE 0); OffsetRect &r 0 LH*6

DAH_GDIP_SetMapMode hdc r.top -1 1 ;;applies, draws flipped horizontally

 SetViewportExtEx(hdc zvp.cx zvp.cy 0)
 SetWindowExtEx(hdc zwp.cx zwp.cy 0)
 SetMapMode hdc MM_TEXT
