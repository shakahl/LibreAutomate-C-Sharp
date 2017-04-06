 /DDE_server
function# wType wFmt hConv hsz1 hsz2 hData dwData1 dwData2

 out "%i %i %i" hsz1 hsz2 hData

str s1 s2 s3
if(hsz1) GetStr(hsz1 s1)
if(hsz2) GetStr(hsz2 s2)
if(hData and !__DdeGetData(wFmt hData s3)) ret
int _r=1

sel wType
	case XTYP_CONNECT
	 out "XTYP_CONNECT: topic='%s'" s1
	
	case XTYP_CONNECT_CONFIRM
	 out "XTYP_CONNECT_CONFIRM: hConv=%i, topic='%s'" hConv s1
	
	case XTYP_DISCONNECT
	 out "XTYP_DISCONNECT: hConv=%i" hConv
	
	case XTYP_EXECUTE
	 out "XTYP_EXECUTE: hConv=%i, topic='%s', data='%s'" hConv s1 s3
	_r=DDE_FACK
	
	case XTYP_POKE
	 out "XTYP_POKE: hConv=%i, topic='%s', item='%s', data='%s'" hConv s1 s2 s3
	_r=DDE_FACK
	
	case XTYP_REQUEST
	 out "XTYP_REQUEST: hConv=%i, topic='%s', item='%s'" hConv s1 s2
	
	case else
	 outx wType
	ret

if(!call(m_cbFunc wType s1 s2 &s3 m_idinst hConv m_cbParam 0)) ret

if(wType!XTYP_REQUEST) ret _r

int n
sel wFmt
	case CF_UNICODETEXT s3.unicode; n=s3.len+2
	case CF_TEXT n=s3.len+1
	case else ret
ret DdeCreateDataHandle(m_idinst s3 n 0 hsz2 wFmt 0)
