 /
function! hParent $keyName IXmlNode&xn

RegKey k
if(RegCreateKeyExW(hParent @keyName 0 0 0 KEY_WRITE 0 &k.hkey &_i)) ret

ARRAY(IXmlNode) a
xn.GetAll(2 a)

int i j t
str ss; lpstr v

for i 0 a.len
	IXmlNode& x=a[i]
	lpstr tag=x.Name
	lpstr name=x.AttributeValue("n")
	sel tag
		case "k"
		if(!RIX_Key(k name x)) ret
		
		case else
		if(tag[0]!='v' or !isdigit(tag[1])) continue
		t=val(tag+1)
		v=x.Value
		sel t
			case [REG_SZ,REG_EXPAND_SZ,REG_MULTI_SZ]
				ss=v
				ss.escape(0)
				ss.unicode(ss _unicode ss.len)
				if(RegSetValueExW(k @name 0 t ss ss.len+2)) ret
			case REG_DWORD
				j=val(v)
				if(RegSetValueExW(k @name 0 t &j 4)) ret
			case else
				ss.decrypt(8 v)
				if(RegSetValueExW(k @name 0 t ss ss.len)) ret

ret 1
