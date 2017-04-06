dll "qm.exe"
	F_args !a @b c ^d %e FLOAT'f BSTR'g VARIANT'h CURRENCY'i DATE'j DECIMAL'k DRAWITEMSTRUCT'm POINT'p RECT*s IUnknown't
	!F_byte x
	@F_word x
	#F_int x
	$F_lpstr x
	^F_double x
	%F_large x
	FLOAT'F_float x
	BSTR'F_bstr x
	VARIANT'F_variant x
	CURRENCY'F_currency x
	DATE'F_date x
	DECIMAL'F_decimal x
	ARRAY'F_array x
	POINT'F_point x
	F_DRAWITEMSTRUCT DRAWITEMSTRUCT*r x
	RECT*F_rect_ptr x

out
DECIMAL k=4000
DRAWITEMSTRUCT m.CtlType=11; m.itemData=21
POINT p.x=11; p.y=21
RECT s.left=11; s.bottom=21
IXml t=CreateXml

 F_args 100 1000 1000000 5.5 10000000001000 6.9 "bstr" "variant" 50000 "2009.06.17" k m p &s t
 ret

out F_byte(5)
out F_word(5)
out F_int(5)
 long gi=F_int(300000000); out gi
long gl=F_lpstr(5); out gl
out F_double(5)
out F_large(5)
out F_float(5)
out F_bstr(5)
out F_variant(5)
out F_currency(5)
out F_date(5)
out F_decimal(5)
long kk=F_array(5); out kk
POINT pr=F_point(5); out "%i %i" pr.x pr.y
DRAWITEMSTRUCT mr; F_DRAWITEMSTRUCT(&mr 5); out "%i %i" mr.CtlType mr.itemData
RECT* rr=F_rect_ptr(5); out "%i %i" rr.left rr.bottom
