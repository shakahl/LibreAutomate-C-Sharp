 /dlg_apihook
function hdc CLogicalCoord&c

SetBkMode hdc TRANSPARENT
int LH=30*c.ey
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=10; dt.iRightMargin=10
int R showR
str s

R=ExtTextOut(hdc c.x c.y ETO_CLIPPED +&c "CALL ExtTextOutA" 16 0); c.Offset(0 LH)
R=DrawTextEx(hdc "CALL DrawTextExA" 16 +&c 0 &dt); c.Offset(0 LH)
R=TextOut(hdc c.x c.y "CALL TextOutA" 13); c.Offset(0 LH)
R=DrawText(hdc "CALL DrawTextA" 14 +&c 0); c.Offset(0 LH)

R=TabbedTextOut(hdc c.x c.y "CALL TabbedTextOutA" 19 1 &_i 0); c.Offset(0 LH)
if c.ex=1 and c.ey=1 ;;because draws rect using device coord. Also bad with world transform.
	DrawStatusText(hdc +&c "CALL DrawStatusTextA" 0); c.Offset(0 LH)

POLYTEXT ta.lpstr="CALL PolyTextOutA"; ta.n=17; ta.x=c.x; ta.y=c.y
POLYTEXT tta.lpstr="CALL PolyTextOutA2"; tta.n=18; tta.x=c.x+(170*c.ex); tta.y=c.y
R=PolyTextOut(hdc &ta 2); c.Offset(0 LH)

s="ETO pDx"
ARRAY(int) a.create(s.len); for(_i 0 a.len) a[_i]=20*c.ex
R=ExtTextOut(hdc c.x c.y 0 0 s s.len &a[0]); c.Offset(0 LH)
