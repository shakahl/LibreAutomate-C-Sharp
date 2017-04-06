 /
function! hParent $keyName IXmlNode&x

RegKey k
if(!k.Open(keyName hParent KEY_READ)) ret
x.SetAttribute("n" keyName)

 VALUES
int n nd i t
str s sd ss
BSTR b.alloc(300) ;;300 enough for key name
sd.all(3000)

for i 0 100000000
	 g1
	n=b.len; nd=sd.nc
	sel RegEnumValueW(k i b &n 0 &t sd &nd)
		case 0
		case ERROR_NO_MORE_ITEMS break
		case ERROR_MORE_DATA
		if(sd.nc<nd) sd.all(nd); else if(b.len=n) b.alloc(n*2); else ret
		goto g1
		case else ret
	s.ansi(b)
	sel t
		case [REG_SZ,REG_EXPAND_SZ,REG_MULTI_SZ]
			ss.ansi(sd _unicode nd/2-1)
			ss.escape(1)
		case REG_DWORD
			int* p=sd
			ss=*p
		case else
			sd.len=nd
			ss.encrypt(8 sd)
	IXmlNode xv=x.Add(_s.from("v" t) ss)
	xv.SetAttribute("n" s)

 KEYS
for i 0 1000000000
	n=b.len
	sel RegEnumKeyExW(k i b &n 0 0 0 0)
		case 0
		case ERROR_NO_MORE_ITEMS break
		case else ret
	s.ansi(b)
	IXmlNode xk=x.Add("k")
	if(!REX_Key(k s xk)) ret

ret 1
