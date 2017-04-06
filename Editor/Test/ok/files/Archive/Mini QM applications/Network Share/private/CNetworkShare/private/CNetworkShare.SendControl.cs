function# !c [flags] ;;flags: 1 no msgbox

if(!m_cu.len)
	str uname cname
	GetUserComputer uname cname
	m_cu.format("ns  %s\%s" cname uname)

_s=m_cu
ret SendData("NS_ReceiveFiles" _s c flags)
