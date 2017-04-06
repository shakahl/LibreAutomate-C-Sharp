 /dlg_apihook
function hdc CLogicalCoord&c

 SetBkMode hdc TRANSPARENT
int R showR

__Font f1 f2 f3 f4
f1.Create("Tahoma" 10*c.ex 0 270)
f2.Create("Tahoma" 10*c.ex 0 90)
f3.Create("Tahoma" 10*c.ex 0 180)
f4.Create("Tahoma" 10*c.ex 0 -45)

BSTR s="Text Ԏݭῼ"

SelectObject hdc f1
c.Offset(50 10)
R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0)

SelectObject hdc f2
c.Offset(100 70)
R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0)

SelectObject hdc f3
c.Offset(50 50)
R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0)

SelectObject hdc f4
c.Offset(-150 0)
R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0)

SelectObject hdc f1
c.Offset(0 50)
R=DrawTextExW(hdc s s.len +&c DT_NOCLIP 0)
 s="LINE1[]line2"; R=DrawTextExW(hdc s s.len +&c DT_NOCLIP 0) ;;does not support vertical font when multiline


 SelectObject hdc f1
 c.Offset(100 0)
  R=ExtTextOutW(hdc c.x c.y 0 0 s s.len 0)
 R=ExtTextOutW(hdc c.x c.y ETO_CLIPPED +&c s s.len 0) ;;just nned to give correct clip rect, to match the final text rect; here I didn't, but tested in other app

c.Offset(0 100)
DAH_Uniscribe hdc c

 DAH_GDIP hdc c



 R=ExtTextOutW(hdc c.x c.y ETO_CLIPPED 0 s s.len 0)
