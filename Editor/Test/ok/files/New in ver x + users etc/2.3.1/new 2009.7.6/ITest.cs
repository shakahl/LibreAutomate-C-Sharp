type ONE x
type TWO @x @y
interface ITest :IUnknown
	F_args(!a @b c ^d %e FLOAT'f BSTR'g VARIANT'h CURRENCY'i DATE'j DECIMAL'k DRAWITEMSTRUCT'm POINT'p RECT*s IUnknown't last1 last2)
	One(i)
	!F_byte()
	@F_word()
	#F_int(x)
	^F_double()
	%F_large(x)
	FLOAT'F_float()
	BSTR'F_bstr()
	VARIANT'F_variant(x)
	CURRENCY'F_currency(x)
	DATE'F_date()
	DECIMAL'F_decimal()
	POINT'F_point(x)
	F_DRAWITEMSTRUCT(DRAWITEMSTRUCT*r)
	RECT*F_rect_ptr()
	SIZE'F_size(x)
	F_one(ONE*rv x)
	TWO'F_two(x)

	 ONE'F_one(x)

dll "qm.exe" ITest'GetITest
ITest i=GetITest
 out

DRAWITEMSTRUCT m.CtlType=11; m.itemData=21
POINT p.x=11; p.y=21
RECT s.left=11; s.bottom=21
IXml t=CreateXml

 i.One(3)
 i.F_args(100 1000 1000000 5.5 10000000001000 6.9 "bstr" "variant" 40000 "2009.06.17" 50000 m p &s t -1 -2)

 out i.F_byte
 out i.F_word
out i.F_int(1)
 out i.F_double
out i.F_large(2)
 out i.F_float
 out i.F_bstr
out i.F_variant(3)
out i.F_currency(4)
out i.F_date
 out i.F_decimal
POINT pr=i.F_point(5); out "%i %i" pr.x pr.y
 DRAWITEMSTRUCT mr; i.F_DRAWITEMSTRUCT(mr); out "%i %i" mr.CtlType mr.itemData
 RECT* rr=i.F_rect_ptr; out "%i %i" rr.left rr.bottom
SIZE sr=i.F_size(6); out "%i %i" sr.cx sr.cy
 ONE o=i.F_one(7); out o.x
ONE o; i.F_one(o 7); out o.x
TWO tr=i.F_two(8); out "%i %i" tr.x tr.y
