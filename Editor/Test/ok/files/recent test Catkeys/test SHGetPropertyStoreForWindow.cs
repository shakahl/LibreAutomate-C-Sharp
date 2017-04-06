int w
w=win("Calculator" "ApplicationFrameWindow")
 w=child("Calculator" "Windows.UI.Core.CoreWindow" w) ;;0 props
 w=win("Calculator" "Windows.UI.Core.CoreWindow") ;;0 props
outw w
if(w=0) ret

WINAPI2.IPropertyStore p
if(WINAPI2.SHGetPropertyStoreForWindow(w WINAPI2.IID_IPropertyStore &p)) end "failed"
int i n
p.GetCount(n); out n
for i 0 n
	WINAPI2.PROPERTYKEY k
	p.GetAt(i &k)
	WINAPI2.PROPVARIANT v
	p.GetValue(&k +&v)
	out v.vt
	sel v.vt
		case VT_LPWSTR
		word* u=+v.lVal
		out "%S" u
	WINAPI2.PropVariantClear &v
