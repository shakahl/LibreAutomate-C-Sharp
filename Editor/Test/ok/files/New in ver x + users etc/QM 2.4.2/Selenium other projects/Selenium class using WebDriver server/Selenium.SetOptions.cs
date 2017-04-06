function [$seleniumURL] [WinHttp.WinHttpRequest&httpRequest]

 You can call this function to set some parameters before starting Selenium session with Start(). Optional.

 seleniumURL - URL for connecting to the Selenium Server. Should be like "http://server:port/wd/hub". Default is "http://localhost:4444/wd/hub".
   If empty, the function does not change the previously set (or default) value.
 httpRequest - a variable that receives a pointer to a COM object that will be used to send commands to the server.
   You can call its functions to configure HTTP request parameters - timeouts, proxy, etc. Example: httpRequest.SetProxy(...).


opt noerrorshere 1

if(!empty(seleniumURL)) m_seleniumURL=seleniumURL

if &httpRequest
	if(!m_req) m_req._create
	httpRequest=m_req
