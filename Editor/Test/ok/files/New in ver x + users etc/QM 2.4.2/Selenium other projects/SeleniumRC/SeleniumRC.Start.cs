function $browser $baseURL [flags] ;;flags: 1 start Selenium server if not running, 2 activate browser window, 4 call End when destroying variable

opt noerrorshere 1
if(m_sessionId.len) end "Call End() with this variable."

m_flags=flags
if(flags&3) Selenium_FindServer(flags&3)

str sr
_Post(F"getNewBrowserSession&1={browser}&2={baseURL}" sr)
m_sessionId.gett(sr 1)
if(!StrCompare(browser "*firefox")) m_ffNeedWorkaround=1
