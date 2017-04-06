 out
type PRECT *r
type LRECT $u
type IRECT IUnknown'u
type ARECT ARRAY(str)a
type ERECT
 ARECT r
 ARRAY(str) r
 BSTR r
 VARIANT r="sss"
 RECT* r ;;error
 IUnknown r ;;error
out _s.getstruct(r 1)

 PRECT rr
 _s.setstruct(rr 1) ;;error
 out rr.psa
