type ___INTSETTINGS !useproxy str'proxy_name str'proxy_bypass str'user_agent
___INTSETTINGS+ ___intsett

int p; lpstr pn pb ua
sel(___intsett.useproxy)
	case 1
	p=INTERNET_OPEN_TYPE_PROXY
	pn=___intsett.proxy_name
	if(___intsett.proxy_bypass.len) pb=___intsett.proxy_bypass
	case 2 p=INTERNET_OPEN_TYPE_DIRECT
ua=iif(___intsett.user_agent.len ___intsett.user_agent "Quick Macros")	

m_hitop=InternetOpen(ua p pn pb INTERNET_FLAG_ASYNC)
if(!m_hitop) end "cannot initialize Internet functions"
