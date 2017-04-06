interface# ITestH :IUnknown
	F_args(!a @b c ^d %e FLOAT'f BSTR'g VARIANT'h CURRENCY'i DATE'j DECIMAL'k DRAWITEMSTRUCT'm POINT'p RECT*s IUnknown't last1 last2)
	One(i)
	!F_byte(x)
	@F_word(x)
	#F_int(x)
	$F_lpstr
	^F_double()
	%F_large()
	FLOAT'F_float()
	BSTR'F_bstr()
	VARIANT'F_variant()
	CURRENCY'F_currency()
	DATE'F_date()
	DECIMAL'F_decimal()
	POINT'F_point()
	F_DRAWITEMSTRUCT(DRAWITEMSTRUCT*r)
	RECT*F_rect_ptr()

dll "qm.exe" ITestH'GetITestH
ITestH i=GetITestH
out

DRAWITEMSTRUCT m.CtlType=11; m.itemData=21
POINT p.x=11; p.y=21
RECT s.left=11; s.bottom=21
IXml t=CreateXml

 i.One(3)
 i.F_args(100 1000 1000000 5.5 10000000001000 6.9 "bstr" "variant" 40000 "2009.06.17" 50000 m p &s t -1 -2)

out i.F_byte(300)
out i.F_word(65537)
out i.F_int(5)
long gi=i.F_int(300000000); out gi
long gl=i.F_lpstr; out gl
out i.F_double
out i.F_large
out i.F_float
out i.F_bstr
out i.F_variant
out i.F_currency
out i.F_date
out i.F_decimal
POINT pr=i.F_point; out "%i %i" pr.x pr.y
DRAWITEMSTRUCT mr; i.F_DRAWITEMSTRUCT(mr); out "%i %i" mr.CtlType mr.itemData
RECT* rr=i.F_rect_ptr; out "%i %i" rr.left rr.bottom
