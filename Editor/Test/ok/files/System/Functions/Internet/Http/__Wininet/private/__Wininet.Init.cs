function!

int p; lpstr pn pb ua
sel __intsett.useproxy
	case 1
	p=INTERNET_OPEN_TYPE_PROXY
	pn=__intsett.proxy_name
	if(__intsett.proxy_bypass.len) pb=__intsett.proxy_bypass
	case 2 p=INTERNET_OPEN_TYPE_DIRECT
ua=iif(__intsett.user_agent.len __intsett.user_agent "Quick Macros")	

m_hitop=InternetOpen(ua p pn pb 0)
if(m_hitop) ret 1

lasterror="failed to initialize Internet functions."
if(__intsett.useproxy) lasterror+" Possibly used IntSettings with incorrect values."
