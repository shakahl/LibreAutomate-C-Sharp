 /
function useproxy [$proxy_name] [$proxy_bypass] [proxy_bypass_local] [$user_agent]

 Changes default settings used by QM Internet functions.
 Call before any other http/ftp functions.
 Settings are stored in a global variable, and are used until QM exits or file is reloaded or this function is called again with different arguments.

 useproxy - 0 default (use IE settings), 1 use specified settings, 2 don't use
 proxy_name - proxy address optionally followed by :port; May be specified different proxies for specific protocols.
  examples:
   "proxy"
   "proxy:80"
   "http=http://proxy1 https=https://proxy2:90 ftp=ftp://proxy3"
   "ftp=ftp://proxy3:21 proxyforotherprotocols"
 proxy_bypass - optional list (; delimited) of host names that should not be routed through proxy.
  example: "server1.com;123.45.67.8"
 proxy_bypass_local - if nonzero, bypass local addresses.
 user_agent - application accessing the Internet. Default: "Quick Macros".


sel useproxy
	case 1
	__intsett.proxy_name=proxy_name; if(!__intsett.proxy_name.len) end ERR_BADARG
	__intsett.proxy_bypass=proxy_bypass
	if(proxy_bypass_local) __intsett.proxy_bypass+iif(__intsett.proxy_bypass.len ";<local>" "<local>") 
	case [0,2] __intsett.proxy_name.all; __intsett.proxy_bypass.all
	case else end ERR_BADARG
	
__intsett.useproxy=useproxy
__intsett.user_agent=user_agent
