 /dlg_apihook
function hdc CLogicalCoord&c

SetBkMode hdc TRANSPARENT
SetTextColor hdc 0x8000
int LH=30*c.ey
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=10; dt.iRightMargin=10
int R showR


BSTR s="CALL ExtTextOutW Ԏݭῼ"
 R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0); c.Offset(0 LH)
R=ExtTextOutW(hdc c.x c.y ETO_CLIPPED 0 s s.len 0); c.Offset(0 LH)
 ret
 R=ExtTextOutW(hdc c.x c.y ETO_CLIPPED +&c s s.len 0); c.Offset(0 LH)
 R=ExtTextOutW(hdc c.x c.y ETO_IGNORELANGUAGE|ETO_CLIPPED +&c L"CALL ExtTextOutW" 16 0); c.Offset(0 LH)
 R=ExtTextOutW(hdc c.x c.y ETO_IGNORELANGUAGE +&c L"CALL ExtTextOutW" 16 0); c.Offset(0 LH)
 s="aaaaaaaaaa bbbbbbbbbbbbbb cccccccccccccc ddddddddddddddd"
 R=ExtTextOutW(hdc c.x c.y ETO_CLIPPED +&c s s.len 0); c.Offset(0 LH)
 R=ExtTextOutW(hdc c.x c.y 0 0 L"Test" 4 0); c.Offset(0 LH)
 s="one[]two"; R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0); c.Offset(0 LH)
 s="&o[9][9]t[9]wo"; R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0); c.Offset(0 LH)
s="ETO pDx"
ARRAY(int) a.create(s.len); for(_i 0 a.len) a[_i]=20*c.ex
R=ExtTextOutW(hdc c.x c.y 0 0 s s.len &a[0]); c.Offset(0 LH)
s="ETO_PDY vert"
ARRAY(POINT) aa.create(s.len); for(_i 0 aa.len) aa[_i].y=-15*c.ex ;;vertical text
c.Offset(200 0); R=ExtTextOutW(hdc c.x c.y ETO_PDY 0 s s.len +&aa[0]); c.Offset(-200 0)
s="ETO_PDY horz"
aa.create(s.len); for(_i 0 aa.len) aa[_i].x=10*c.ex ;;horz text
R=ExtTextOutW(hdc c.x c.y ETO_PDY 0 s s.len +&aa[0]); c.Offset(0 LH)
 ret
 R=DrawTextExW(hdc L"CALL[]DrawTextExW" -1 +&c DT_NOCLIP 0); c.Offset(0 LH*1.5)
R=DrawTextExW(hdc L"CALL[]DrawTextExW" -1 +&c 0 0); c.Offset(0 LH*1.5)
 r.right=100
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_WORDBREAK 0); c.Offset(0 LH)
 ret

R=DrawTextExW(hdc L"CALL DrawTextExW Ԏݭῼ" -1 +&c 0 &dt); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_NOCLIP &dt); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CAL[]DrawTextExW" -1 +&c 0 &dt); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 &_r 0 &dt); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 &_r DT_NOCLIP 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_CENTER 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_RIGHT 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_BOTTOM|DT_SINGLELINE 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_CENTER|DT_BOTTOM|DT_SINGLELINE 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_RIGHT|DT_BOTTOM|DT_SINGLELINE 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_VCENTER|DT_SINGLELINE 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL A&BC" -1 +&c DT_PREFIXONLY 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"&CALL[9]DrawTextExW" -1 +&c DT_EXPANDTABS 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL WWWWWWWWWWWWWWWWWW DrawTextExW" -1 +&c 0 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL WWWWWWWWWWWWWWWWWW DrawTextExW" -1 +&c DT_NOCLIP 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL[]DrawTextExW" -1 +&c 0 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL[]DrawTextExW" -1 +&c DT_NOCLIP 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL WWWWWWWWWWWWWWWWWW DrawTextExW" -1 +&c DT_WORDBREAK 0); c.Offset(0 LH)
 R=DrawTextExW(hdc L"CALL DrawTextExW Ԏݭῼ" -1 +&c 0 0); c.Offset(0 LH)
 ret

R=TextOutW(hdc c.x c.y L"CALL TextOutW" 13); c.Offset(0 LH)
R=DrawTextW(hdc L"CALL DrawTextW" 14 +&c 0); c.Offset(0 LH)

_i=25
R=TabbedTextOutW(hdc c.x c.y L"CALL[9]TabbedTextOutW" 19 1 &_i 0); c.Offset(0 LH)
 R=TabbedTextOutW(hdc c.x c.y L"CALL[9]TabbedTextOutW[]TTO line2" 30 1 &_i 0); c.Offset(0 LH) ;;no
DrawStatusTextW(hdc +&c L"CALL DrawStatusTextW" 0); c.Offset(0 LH)

POLYTEXTW t.lpstr=L"CALL PolyTextOutW"; t.n=17; t.x=c.x; t.y=c.y
POLYTEXTW tt.lpstr=L"CALL PolyTextOutW2"; tt.n=18; tt.x=c.x+(170*c.ex); tt.y=c.y
tt.uiFlags=ETO_CLIPPED; memcpy &tt.rcl &c 16; InflateRect &tt.rcl 0 -8
 ARRAY(int) a.create(lstrlenW(t.lpstr)); for(_i 0 a.len) a[_i]=10
 tt.pdx=&a[0]
R=PolyTextOutW(hdc &t 2); c.Offset(0 LH)
 note: on XP SP0 PTOW fails. But PTOA works.

_i=&DrawShadowText; err _i=0 ;;XP
if _i
	R=DrawShadowText(hdc L"CALL DrawShadowText" 19 +&c 0 0xff0000 0xff00 7 7); c.Offset(0 LH)
	 R=DrawShadowText(hdc L"CALL DrawShadowText" 19 +&c DT_CENTER 0xff0000 0xff00 5 5); c.Offset(0 LH)
	 R=DrawShadowText(hdc L"CALL DrawShadowText" 19 +&c DT_CENTER|DT_BOTTOM|DT_SINGLELINE 0xff0000 0xff00 5 5); c.Offset(0 LH)
	
	int htheme=OpenThemeData(0 L"Button")
	R=DrawThemeText(htheme hdc 1 1 L"CALL DrawThemeText" 18 0 0 +&c); c.Offset(0 LH)
	_i=&DrawThemeTextEx; err _i=0 ;;Vista
	if _i
		DTTOPTS o.dwSize=sizeof(o); o.dwFlags=DTT_TEXTCOLOR; o.crText=0xff
		s="CALL DrawThemeTextEx"; R=DrawThemeTextEx(htheme hdc 1 1 s s.len 0 +&c &o); c.Offset(170 0)
		o.dwFlags|DTT_SHADOWTYPE|DTT_SHADOWOFFSET|DTT_SHADOWCOLOR; o.iTextShadowType=1; o.ptShadowOffset.y=7; o.crShadow=0x8080
		s="CALL[]DTTE shadow"; R=DrawThemeTextEx(htheme hdc 1 1 s s.len DT_NOCLIP +&c &o); c.Offset(-170 LH*2)
	CloseThemeData htheme

DAH_GDIP hdc c;; c.Offset(0 LH)

DrawStateW(hdc 0 0 L"CALL DrawStateW" 0 c.x c.y 0 0 DST_TEXT|DSS_DISABLED); c.Offset(0 LH) ;;uses TextOut

GrayStringW(hdc 0 0 L"CALL GrayStringW" 0 c.x c.y 200 LH); c.Offset(0 LH) ;;uses TextOut

 _______________________________________

 Test also:
   1. DirectWrite. New in Win7. Used in WPF. Test app: Witty.
   2. DirectX (D3DXCreateFont/DrawText).
   3. Opengl. Don't test. There is no easy text function.

