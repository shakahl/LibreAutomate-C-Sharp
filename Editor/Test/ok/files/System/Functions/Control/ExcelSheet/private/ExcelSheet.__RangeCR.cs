function'Excel.Range `&v

int& row=&v+16
int i

sel v.vt
	case VT_BSTR
	BSTR& s=v.bstrVal; i=s.len
	if i and !isdigit(s[i-1])
		if(s[0]='<') _s=v; ret __Range(_s) ;;eg "<active>". note: error if ""
		ret ws.Cells.Item(row v) ;;"A"
	ret ws.Range(v) ;;"A1"
	
	case else
	i=v
	if(i<1 or row<1) end F"{ERR_ARR_INDEX}, must be >=1"
	ret ws.Cells.Item(row i)

err+ end _error
