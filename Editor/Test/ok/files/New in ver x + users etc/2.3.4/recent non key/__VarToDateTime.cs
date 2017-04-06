 /Macro1712
function! `&v DateTime&d

sel v.vt
	case VT_DATE
	d.FromDATE(v.date)
	
	case VT_BSTR
	_s=v
	d.FromStr(_s)
	
	case VT_DECIMAL ;;VARIANT=long or DateTime
	d.t=v
	
	case [VT_FILETIME,VT_I8,VT_UI8]
	memcpy &d &v.date 8
	
	case else
	ret

err+ ret
ret 1
