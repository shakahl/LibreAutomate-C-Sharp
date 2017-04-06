 /dlg_apihook
function hdc CLogicalCoord&c

 SetBkMode hdc TRANSPARENT
int LH=30*c.ey

 ExtTextOutW(hdc c.x c.y 0 0 L"CALL ExtTextOut" 15 0)
 DrawTextExW(hdc L"CALL DrawTextExW" -1 +&c DT_NOCLIP 0)
c.Offset(100 LH)
SetBkMode hdc TRANSPARENT

 ExtTextOutW(hdc c.x c.y 0 0 L"CALL ExtTextOut" 15 0)

 c.Offset(0 LH)
 int L=SetLayout(hdc LAYOUT_RTL)
  out L
 ExtTextOutW hdc c.x c.y 0 0 L"Text" 4 0 ;;ok
 SetLayout(hdc L)

SetTextAlign hdc TA_UPDATECP
 rep(2) ExtTextOutW(hdc c.x c.y 0 0 L" " 1 0)
rep 2
	ExtTextOutW(hdc c.x c.y 0 0 L"txt" 3 0)
	 ExtTextOutW(hdc c.x c.y 0 0 L" " 1 0)
	 ExtTextOutW(hdc c.x c.y 0 0 L" " 1 0)
	POINT p; GetCurrentPositionEx hdc &p; MoveToEx hdc p.x+2 p.y 0
	 ExtTextOutW(hdc c.x c.y 0 0 L" " 1 0)
	 ExtTextOutW(hdc c.x c.y 0 0 L" " 1 0)

 ExtTextOutW(hdc c.x c.y 0 0 L"txt" 3 0)
