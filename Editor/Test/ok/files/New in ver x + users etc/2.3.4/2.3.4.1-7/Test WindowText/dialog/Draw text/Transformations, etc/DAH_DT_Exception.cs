 /dlg_apihook
function hdc CLogicalCoord&c

SetBkMode hdc TRANSPARENT
int R

R=ExtTextOutW(hdc c.x c.y 0 0 L"A" 1 0); c.Offset(0 30)
R=ExtTextOutW(hdc c.x c.y 0 0 0 1 0); err ;;exception
c.Offset(0 30)
R=ExtTextOutW(hdc c.x c.y 0 0 L"C" 1 0); c.Offset(0 30)

R=DrawTextExW(hdc L"D" -1 +&c 0 0); c.Offset(0 30)
R=DrawTextExW(hdc L"E" -1 0 0 0); err ;;exception
c.Offset(0 40)
R=DrawTextExW(hdc L"F" -1 +&c 0 0); c.Offset(0 30)
