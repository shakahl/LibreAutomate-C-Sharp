 /dlg_apihook
function hdc

SetBkMode hdc TRANSPARENT
int x(10) y(10)
RECT r; SetRect &r x y x+200 y+20
int R
SetTextAlign hdc TA_UPDATECP
 SetTextAlign hdc TA_UPDATECP|TA_RIGHT; x+200
 SetTextAlign hdc TA_UPDATECP|TA_BOTTOM
MoveToEx hdc x y 0

R=ExtTextOutW(hdc 100 100 0 0 L"A " 2 0)
R=ExtTextOutW(hdc 100 100 0 0 L"B " 2 0)
R=ExtTextOut(hdc 100 100 0 0 "a " 2 0)
R=ExtTextOut(hdc 100 100 0 0 "b " 2 0)
R=TextOutW(hdc 100 100 L"C " 2)
R=TextOut(hdc 100 100 "c " 2)

R=DrawTextExW(hdc L"D " -1 &r DT_NOCLIP 0)
R=DrawTextEx(hdc "d " -1 &r DT_NOCLIP 0)
R=DrawTextW(hdc L"E " -1 &r DT_NOCLIP)
R=DrawText(hdc "e " -1 &r DT_NOCLIP)

y+30
MoveToEx hdc x y 0
_i=25; R=TabbedTextOutW(hdc 100 100 L"F[9]F " 4 1 &_i 0) ;;doesn't add tabs
_i=25; R=TabbedTextOut(hdc 100 100 "f[9]f " 4 1 &_i 0)

y+30
RECT rs; SetRect &rs 5 y 200 y+20
MoveToEx hdc rs.left+20 rs.top 0
DrawStatusTextW(hdc &rs L"G " 0) ;;draws rect where rc, but draws text from CP, possibly somewhere outside

y+30
MoveToEx hdc x y 0
POLYTEXTW t.lpstr=L"H1 "; t.n=3; t.x=100; t.y=300
POLYTEXTW tt.lpstr=L"H2 "; tt.n=3; tt.x=100; tt.y=300
R=PolyTextOutW(hdc &t 2)

#if _winver>=0x501
y+30
MoveToEx hdc x y 0
R=DrawShadowText(hdc L"I " 2 &r DT_NOCLIP 0xff0000 0xff00 5 5)

int htheme=OpenThemeData(0 L"Button")
R=DrawThemeText(htheme hdc 1 1 L"J " 2 DT_NOCLIP 0 &r)
#endif
#if _winver>=0x600
DTTOPTS o.dwSize=sizeof(o); o.dwFlags=DTT_TEXTCOLOR; o.crText=0xff
R=DrawThemeTextEx(htheme hdc 1 1 L"K " 2 DT_NOCLIP &r &o)
#endif
#if _winver>=0x501
CloseThemeData htheme
#endif

y+60
MoveToEx hdc x y 0

 DAH_GDIP hdc 0 ;;does not apply CP

CLogicalCoord c.Init(hdc x y 300 30)
DAH_Uniscribe hdc c

y+60
MoveToEx hdc x y 0
r.bottom=y+10
R=DrawTextExW(hdc L"D " -1 &r 0 0)

 SetTextAlign hdc TA_LEFT
SetRect &r 0 y+50 200 y+100
R=DrawTextExW(hdc L"multi[]line " -1 &r DT_NOCLIP 0) ;;does not draw multiline, but DT_CALCRECT gives as multiline
R=DrawTextExW(hdc L"multi[]line " -1 &r DT_NOCLIP 0)

y+30
MoveToEx hdc x y 0
R=DrawTextExW(hdc L"center " -1 &r DT_NOCLIP|DT_CENTER 0) ;;does not draw center etc
