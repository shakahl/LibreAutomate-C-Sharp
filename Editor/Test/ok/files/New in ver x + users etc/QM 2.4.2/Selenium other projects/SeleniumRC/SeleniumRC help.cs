out

class SeleniumRC -~m_sessionId -m_flags -!m_ffNeedWorkaround

SeleniumRC x
x.Start("*firefox" "http://www.quickmacros.com" 7)
 x.Start("*googlechrome" "http://www.quickmacros.com" 7)
 x.Start("*iexplore" "http://www.quickmacros.com" 7)

x.Action("open" "/index.html")
x.Action("clickAndWait" "id=m_download"); err out _error.description ;;fails in IE
mes 1
 x.End

  *firefoxchrome
  *konqueror
  *opera
  *chrome
  *safari
  *piiexplore
  *googlechrome
  *firefox
  *pifirefox
  *iexploreproxy
  *iehta
  *firefoxproxy
  *firefox2
  *firefox3
  *safariproxy
  *iexplore
  *mock
  *webdriver
  *custom
